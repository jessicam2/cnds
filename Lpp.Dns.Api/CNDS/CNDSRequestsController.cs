using Lpp.CNDS.ApiClient;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Dns.DTO.Enums;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Lpp.Dns.Api.CNDS
{
    /// <summary>
    /// 
    /// </summary>
    public class CNDSRequestsController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(CNDSNetworksController));
        static readonly string CNDSurl;
        readonly CNDSClient CNDSApi;

        static CNDSRequestsController()
        {
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public CNDSRequestsController()
        {
            CNDSApi = new CNDSClient(CNDSurl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CNDSApi.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Registers a request within the network.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Register(CNDSRegisterRequestDTO dto)
        {
            await ConfirmProxyUser(dto);

            var requestTypeDetails = await DataContext.RequestTypes.Where(rt => rt.ID == dto.RequestTypeID)
                .Select(rt => new
                {
                    rt.ID,
                    rt.WorkflowID,
                    StartActivityID = rt.Workflow.RequestReviewWorkflowActivityID ?? DataContext.WorkflowActivityCompletionMaps.Where(wa => wa.WorkflowID == rt.WorkflowID && wa.SourceWorkflowActivity.Start == true).Select(wa => wa.SourceWorkflowActivityID).FirstOrDefault(),
                    SubmittedActivityID = rt.Workflow.CompleteDistributionWorkflowActivityID,
                    WorkflowRoleID = rt.Workflow.Roles.Where(wr => wr.IsRequestCreator == true).Select(wr => wr.ID).FirstOrDefault()
                }
                ).FirstOrDefaultAsync();

            DTO.RequestDTO sourceRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.RequestDTO>(dto.RequestDetails);

            //register the request, routes, responses, documents
            var request = DataContext.Requests.Add(new Data.Request
            {
                ID = dto.ParticipantID,
                ProjectID = dto.ProjectID,
                RequestTypeID = dto.RequestTypeID,
                WorkflowID = requestTypeDetails.WorkflowID,
                WorkFlowActivityID = requestTypeDetails.StartActivityID,
                CreatedByID = dto.SourceNetworkID,
                UpdatedByID = dto.SourceNetworkID,
                SubmittedOn = DateTime.UtcNow,
                SubmittedByID = dto.SourceNetworkID,
                Description = sourceRequest.Description,
                //TODO: figure out the naming format of the request
                Name = sourceRequest.Project + ": " + sourceRequest.Name,
                OrganizationID = Requests.RequestsController.CNDSOrganizationRouteProxyID,
                Priority = sourceRequest.Priority,
                Query = sourceRequest.Query,
                DataMarts = new List<RequestDataMart>(),
                AdapterPackageVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).FileVersion,
                RejectedByID = null,
                RejectedOn = null,
                Private = false
            });

            if (sourceRequest.DueDate.HasValue)
            {
                request.DueDate = sourceRequest.DueDate.Value.DateTime;
            }

            request.Users.Add(new RequestUser
            {
                RequestID = request.ID,
                UserID = dto.SourceNetworkID,
                WorkflowRoleID = requestTypeDetails.WorkflowRoleID
            });

            var workflowTask = DataContext.Actions.Add(PmnTask.CreateForWorkflowActivity(request.ID, request.WorkFlowActivityID.Value, request.WorkflowID.Value, DataContext));

            //register the documents
            List<Document> documents = new List<Document>();
            foreach (var doc in dto.Documents)
            {
                var document = DataContext.Documents.Add(new Document
                {
                    ID = doc.DocumentID,
                    RevisionSetID = doc.RevisionSetID,
                    ItemID = workflowTask.ID,
                    Name = doc.Name,
                    FileName = doc.FileName,
                    Kind = doc.Kind,
                    Length = doc.Length,
                    Description = doc.Description,
                    MimeType = doc.MimeType,
                    UploadedByID = dto.SourceNetworkID
                });

                DataContext.ActionReferences.Add(new TaskReference { TaskID = workflowTask.ID, ItemID = document.ID, Type = DTO.Enums.TaskItemTypes.ActivityDataDocument });
                documents.Add(document);
            }

            foreach (var datamart in dto.Routes)
            {
                var route = new RequestDataMart
                {
                    ID = datamart.RouteID,
                    DataMartID = datamart.DataMartID,
                    DueDate = datamart.DueDate,
                    Priority = datamart.Priority,
                    RequestID = request.ID,
                    RoutingType = DTO.Enums.RoutingType.ExternalCNDS,
                    //default status is awaiting request approval, if the proxy user has permission to skip will be updated and notification sent to source request
                    Status = DTO.Enums.RoutingStatus.AwaitingRequestApproval,
                    Responses = new HashSet<Response>()
                };

                var response = route.AddResponse(request.CreatedByID);
                response.ID = datamart.ResponseID;

                var responseDocuments = datamart.DocumentIDs.Select(docID => {
                    var dd = documents.First(d => d.ID == docID);
                    return new RequestDocument { DocumentType = DTO.Enums.RequestDocumentType.Input, ResponseID = response.ID, RevisionSetID = dd.RevisionSetID.Value };
                });

                DataContext.RequestDocuments.AddRange(responseDocuments);

                request.DataMarts.Add(DataContext.RequestDataMarts.Add(route));
            }

            //Parse the query and fix the document references in the fileupload and modular program terms
            DTO.QueryComposer.QueryComposerRequestDTO queryDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.QueryComposer.QueryComposerRequestDTO>(request.Query);
            var fileUploadTerms = queryDTO.Where.Criteria.SelectMany(c => c.Terms.Where(t => t.Type == Lpp.QueryComposer.ModelTermsFactory.FileUploadID || t.Type == QueryComposer.ModelTermsFactory.ModularProgramID)).ToArray();
            if (fileUploadTerms.Length > 0)
            {
                foreach (var term in fileUploadTerms)
                {
                    var termValue = Newtonsoft.Json.JsonConvert.DeserializeObject<FileUploadValues>(term.Values["Values"].ToString());
                    foreach (var termDoc in termValue.Documents)
                    {
                        var docDTO = dto.Documents.Where(d => d.SourceRevisionSetID == termDoc.RevisionSetID).FirstOrDefault();
                        if (docDTO != null)
                        {
                            termDoc.RevisionSetID = docDTO.RevisionSetID;
                        }
                    }

                    term.Values["Values"] = termValue;
                }

                request.Query = Newtonsoft.Json.JsonConvert.SerializeObject(queryDTO);
            }

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error("Error saving request: " + request.ID, ex);
                System.Diagnostics.Debugger.Break();
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            if (documents.Count > 0)
            {
                //var tasks = dto.Documents.Select(async doc => await GetDocumentContent(doc.DocumentID));
                //await Task.WhenAll(tasks);

                foreach (var doc in documents)
                {
                    await GetDocumentContent(doc.ID);
                }

            }

            //Submit the request if the proxy user has permission to skip request approval
            var permissionFilter = new ExtendedQuery
            {
                Projects = (a) => a.ProjectID == request.ProjectID,
                ProjectOrganizations = (a) => a.ProjectID == request.ProjectID && a.OrganizationID == request.OrganizationID,
                Organizations = (a) => a.OrganizationID == request.OrganizationID,
                Users = (a) => a.UserID == request.CreatedByID
            };

            var apiIdentity = new Utilities.Security.ApiIdentity(dto.SourceNetworkID, dto.SourceNetworkName.Replace(" ", "_"), dto.SourceNetworkName, Requests.RequestsController.CNDSOrganizationRouteProxyID);
            var permissions = await DataContext.HasGrantedPermissions<Request>(apiIdentity, request.ID, permissionFilter, Dns.DTO.Security.PermissionIdentifiers.Request.SkipSubmissionApproval);
            if (permissions.Contains(DTO.Security.PermissionIdentifiers.Request.SkipSubmissionApproval))
            {
                await DataContext.Entry(request).ReloadAsync();
                await DataContext.Entry(request).Collection(r => r.DataMarts).LoadAsync();
                
                var responses = (await DataContext.RequestDataMarts.Where(rdm => rdm.RequestID == request.ID && rdm.Status == DTO.Enums.RoutingStatus.AwaitingRequestApproval).SelectMany(rdm => rdm.Responses.Where(rsp => rsp.Count == rdm.Responses.Max(rr => rr.Count))).ToArrayAsync()).ToDictionary(r => r.RequestDataMartID);

                //update the routings to Submitted
                foreach (var dm in request.DataMarts)
                {
                    await DataContext.Entry(dm).ReloadAsync();

                    Response response = null;
                    if (responses.TryGetValue(dm.ID, out response))
                    {
                        response.SubmittedByID = dto.SourceNetworkID;
                        response.SubmittedOn = DateTime.UtcNow;
                    }

                    dm.Status = DTO.Enums.RoutingStatus.Submitted;
                }
                try
                {
                    await DataContext.SaveChangesAsync();
                }
                catch (Exception ex) {
                    Logger.Error(ex.Message, ex);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }

                //update the request to submitted
                //manually override the request status using sql direct, EF does not allow update of computed
                await DataContext.Database.ExecuteSqlCommandAsync("UPDATE Requests SET Status = @status WHERE ID = @ID", new System.Data.SqlClient.SqlParameter("@status", (int)DTO.Enums.RequestStatuses.Submitted), new System.Data.SqlClient.SqlParameter("@ID", request.ID));
                await DataContext.Entry(request).ReloadAsync();

                //close the review task
                workflowTask.Status = DTO.Enums.TaskStatuses.Complete;
                workflowTask.EndOn = DateTime.UtcNow;
                workflowTask.PercentComplete = 100d;

                //create a new task for the next activity, and set the activity on the request
                request.WorkFlowActivityID = requestTypeDetails.SubmittedActivityID;
                var submitTask = DataContext.Actions.Add(PmnTask.CreateForWorkflowActivity(request.ID, request.WorkFlowActivityID.Value, request.WorkflowID.Value, DataContext));
                try
                {
                    await DataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }

                //notify CNDS of request status change, which will in turn notify the source network to update the status of the route
                System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    using (var helper = new Lpp.CNDS.ApiClient.NetworkRequestDispatcher())
                    {
                        foreach (var responseID in dto.Routes.Select(rt => rt.ResponseID))
                        {
                            await helper.UpdateSourceRoutingStatus(new[] { new DTO.CNDSUpdateRoutingStatusDTO { ResponseID = responseID, Message = string.Empty, RoutingStatus = DTO.Enums.RoutingStatus.Submitted } });
                        }
                    }
                });
            }


            return Request.CreateResponse(HttpStatusCode.OK); ;

        }

        /// <summary>
        /// Updates the status of a route.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateRoutingStatus(IEnumerable<CNDSUpdateRoutingStatusDTO> dtos)
        {
            var responseIDs = dtos.Select(r => r.ResponseID).Distinct();
            var responses = (await DataContext.Responses.Include(rsp => rsp.RequestDataMart).Where(rsp => responseIDs.Contains(rsp.ID)).ToArrayAsync()).ToDictionary(k => k.ID);

            if (responses.Count < responseIDs.Count())
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "One or more of the specified responses were not found.");
            }

            foreach (var dto in dtos)
            {
                Response response;
                if (!responses.TryGetValue(dto.ResponseID, out response))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified response was not found.");
                }

                await ConfirmProxyUser(dto.NetworkID, dto.Network);

                var routing = response.RequestDataMart;
                await DataContext.Entry(routing).Reference(r => r.Request).LoadAsync();

                var originalRequestStatus = routing.Request.Status;
                var originalStatus = routing.Status;

                if (originalStatus == DTO.Enums.RoutingStatus.Completed)
                    routing.Status = DTO.Enums.RoutingStatus.ResultsModified;

                if (originalStatus != DTO.Enums.RoutingStatus.Completed && originalStatus != DTO.Enums.RoutingStatus.ResultsModified)
                    routing.Status = dto.RoutingStatus;

                routing.UpdatedOn = DateTime.UtcNow;

                response.ResponseMessage = dto.Message;

                //only set the response time and ID if the response is completed
                var completeStatuses = new[] {
                    Lpp.Dns.DTO.Enums.RoutingStatus.Completed,
                    Lpp.Dns.DTO.Enums.RoutingStatus.ResultsModified,
                    Lpp.Dns.DTO.Enums.RoutingStatus.RequestRejected,
                    Lpp.Dns.DTO.Enums.RoutingStatus.ResponseRejectedBeforeUpload,
                    Lpp.Dns.DTO.Enums.RoutingStatus.ResponseRejectedAfterUpload,
                    Lpp.Dns.DTO.Enums.RoutingStatus.AwaitingResponseApproval
                };

                if (completeStatuses.Contains(routing.Status))
                {
                    response.ResponseTime = DateTime.UtcNow;
                    response.RespondedByID = dto.NetworkID;
                }
                Logger.Error("Updating status to : " + dto.RoutingStatus + " from " + originalStatus);

                await DataContext.SaveChangesAsync();

                if (completeStatuses.Contains(routing.Status))
                {
                    await DataContext.Entry(routing.Request).ReloadAsync();
                    if (routing.Request.Status == DTO.Enums.RequestStatuses.Complete)
                    {
                        var apiIdentity = new Utilities.Security.ApiIdentity(dto.NetworkID, dto.Network.Replace(" ", "_"), dto.Network, Requests.RequestsController.CNDSOrganizationRouteProxyID);

                        //send the request status complete notification
                        var request = routing.Request;

                        var requestStatusLogger = new Dns.Data.RequestLogConfiguration();
                        string[] emailText = await requestStatusLogger.GenerateRequestStatusChangedEmailContent(DataContext, request.ID, apiIdentity.ID, originalRequestStatus, request.Status);
                        var logItems = requestStatusLogger.GenerateRequestStatusEvents(DataContext, apiIdentity, false, originalRequestStatus, request.Status, request.ID, emailText[1], emailText[0], "Request Status Changed");

                        try
                        {
                            await DataContext.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message, ex);
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                        }

                        await Task.Run(() =>
                        {

                            List<Utilities.Logging.Notification> notifications = new List<Utilities.Logging.Notification>();

                            foreach (Lpp.Dns.Data.Audit.RequestStatusChangedLog logitem in logItems)
                            {
                                var items = requestStatusLogger.CreateNotifications(logitem, DataContext, true);
                                if (items != null && items.Any())
                                    notifications.AddRange(items);
                            }

                            if (notifications.Any())
                                requestStatusLogger.SendNotification(notifications);
                        });
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Updates the DueDate and Priority of a route
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UpdatePriorityAndDueDate(DTO.CNDS.CNDSUpdateDataMartPriorityAndDueDateDTO dto)
        {
            var requestDM = await DataContext.RequestDataMarts.Where(x => x.ID == dto.RequestDataMartID).FirstOrDefaultAsync();
            requestDM.Priority = (Lpp.Dns.DTO.Enums.Priorities)dto.Priority;
            requestDM.DueDate = dto.DueDate;

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ResubmitRoute(DTO.CNDSResubmitRouteDTO dto)
        {
            var requestDM = await DataContext.RequestDataMarts.Where(x => x.ID == dto.RequestDatamartID).FirstOrDefaultAsync();
            var requestUserID = await (from r in DataContext.Requests
                                       join rdm in DataContext.RequestDataMarts on r.ID equals rdm.RequestID
                                       where rdm.ID == dto.RequestDatamartID
                                       select r.CreatedByID).FirstOrDefaultAsync();
            var response = requestDM.AddResponse(requestUserID);
            response.ID = dto.ResponseID;
            response.SubmitMessage = dto.Message;
            requestDM.Status = DTO.Enums.RoutingStatus.Resubmitted;

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> TerminateRoute([FromBody]Guid id)
        {
            var requestDatamart = await DataContext.RequestDataMarts.Where(x => x.ID == id).FirstOrDefaultAsync();
            requestDatamart.Status = DTO.Enums.RoutingStatus.Canceled;

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }


            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> TerminateRequest([FromBody]Guid id)
        {
            var request = await DataContext.Requests.FindAsync(id);

            request.CancelledByID = Identity.ID;
            request.CancelledOn = DateTime.UtcNow;

            var newStatus = (int)request.Status < 400 ? RequestStatuses.TerminatedPriorToDistribution : DTO.Enums.RequestStatuses.Cancelled;


            var completedRoutingStatuses = new[] { RoutingStatus.RequestRejected, RoutingStatus.Canceled, RoutingStatus.ResponseRejectedAfterUpload, RoutingStatus.ResponseRejectedBeforeUpload, RoutingStatus.Failed, RoutingStatus.Completed };
            var rdms = DataContext.RequestDataMarts.Where(s => s.RequestID == id);
            foreach (var rdm in rdms)
            {
                if (completedRoutingStatuses.Contains(rdm.Status) == false)
                    rdm.Status = DTO.Enums.RoutingStatus.Canceled;
            }

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            await DataContext.Entry(request).ReloadAsync();

            request.Status = newStatus;
            await DataContext.SaveChangesAsync();

            //have to explicitly change the request status since the property is marked as computed for EF.
            await DataContext.Database.ExecuteSqlCommandAsync("UPDATE Requests SET Status = @status WHERE ID = @ID", new System.Data.SqlClient.SqlParameter("@status", (int)newStatus), new System.Data.SqlClient.SqlParameter("@ID", request.ID));

            //cancel any outstanding tasks associated with the request.
            var incompleteTaskStatuses = new[] { TaskStatuses.Complete, TaskStatuses.Cancelled };
            IEnumerable<PmnTask> incompleteTasks = await DataContext.Actions.Where(t => t.References.Any(r => r.ItemID == id) && t.Status != TaskStatuses.Complete && t.Status != TaskStatuses.Cancelled).ToArrayAsync();
            foreach (var incompleteTask in incompleteTasks)
            {
                incompleteTask.Status = DTO.Enums.TaskStatuses.Cancelled;
                incompleteTask.EndOn = DateTime.UtcNow;
                incompleteTask.PercentComplete = 100d;
            }
            await DataContext.SaveChangesAsync();

            await DataContext.Entry(request).ReloadAsync();
            //This has to be down here or the task can't be found to update.
            request.WorkFlowActivityID = Guid.Parse("CC2E0001-9B99-4C67-8DED-A3B600E1C696");

            var ta = new PmnTask
            {
                CreatedOn = DateTime.UtcNow,
                PercentComplete = 100,
                Priority = DTO.Enums.Priorities.High,
                StartOn = DateTime.UtcNow,
                EndOn = DateTime.UtcNow,
                Status = DTO.Enums.TaskStatuses.Complete,
                Subject = "Request Terminated",
                Type = DTO.Enums.TaskTypes.Task,
                WorkflowActivityID = request.WorkFlowActivityID
            };

            DataContext.Actions.Add(ta);

            var reference = new TaskReference
            {
                ItemID = request.ID,
                TaskID = ta.ID,
                Type = DTO.Enums.TaskItemTypes.Request
            };

            DataContext.ActionReferences.Add(reference);

            await DataContext.SaveChangesAsync();

            try
            {
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }


            return Request.CreateResponse(HttpStatusCode.OK);
        }

        async Task GetDocumentContent(Guid documentID)
        {
            using (var db = new DataContext())
            using (var cndsAPI = new Lpp.CNDS.ApiClient.CNDSClient(CNDSurl))
            {
                using (var docStream = new Data.Documents.DocumentStream(db, documentID))
                {
                    var httpResponse = await cndsAPI.Requests.ReadDocument(documentID);

                    await httpResponse.Content.CopyToAsync(docStream);

                    await docStream.FlushAsync();
                }
            }
        }

        async Task ConfirmProxyUser(CNDSRegisterRequestDTO dto)
        {
            await ConfirmProxyUser(dto.SourceNetworkID, dto.SourceNetworkName);
        }

        async Task ConfirmProxyUser(Guid networkID, string networkName)
        {

            if (await DataContext.Users.AnyAsync(u => u.ID == networkID))
            {
                return;
            }

            DataContext.Users.Add(new Data.User
            {
                ID = networkID,
                UserType = DTO.Enums.UserTypes.CNDSNetworkProxy,
                FirstName = networkName,
                LastName = "NetworkProxyUser",
                Email = "support@popmednet.org",
                UserName = networkName.Replace(" ", "") + "_proxyuser",
                OrganizationID = Requests.RequestsController.CNDSOrganizationRouteProxyID
            });

            await DataContext.SaveChangesAsync();
        }

        internal class FileUploadValues
        {
            public IList<Document> Documents { get; set; }

            public class Document
            {
                public Guid RevisionSetID { get; set; }
            }
        }
    }
}
 