using Lpp.Dns.Data;
using Lpp.Utilities;
using System.Data.Entity;
using Lpp.Dns.DTO;
using Lpp.Dns.DTO.Security;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Lpp.Dns.DTO.Enums;

namespace Lpp.Dns.Api.Requests
{
    /// <summary>
    /// Controller for servicing Response actions.
    /// </summary>
    public class ResponseController : LppApiDataController<Response, ResponseDTO, DataContext, PermissionDefinition>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(ResponseController));

        Lpp.CNDS.ApiClient.CNDSClient _cndsClient = null;
        Lpp.CNDS.ApiClient.CNDSClient CNDSApi
        {
            get
            {
                if(_cndsClient == null)
                {
                    _cndsClient = new Lpp.CNDS.ApiClient.CNDSClient(System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"]);
                }
                return _cndsClient;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(_cndsClient != null)
            {
                _cndsClient.Dispose();
                _cndsClient = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the details of a response
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet]
        public override async Task<ResponseDTO> Get(Guid ID)
        {
            var result = await base.Get(ID);

            //Record the log of the view of the result
            await DataContext.LogRead(await DataContext.Requests.FindAsync(ID));

            await DataContext.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Approves a response in the system
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> ApproveResponses(ApproveResponseDTO responses)
        {
            var globalAclFilter = DataContext.GlobalAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var datamartsAclFilter = DataContext.DataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectAclFilter = DataContext.ProjectAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectDataMartsAclFilter = DataContext.ProjectDataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var organizationAclFilter = DataContext.OrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);

            var hasPermission = (from r in DataContext.Responses
                                 let globalAcls = globalAclFilter
                                 let datamartAcls = datamartsAclFilter.Where(a => a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                 let projectAcls = projectAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID))
                                 let orgAcls = organizationAclFilter.Where(a => a.Organization.Requests.Any(rq => rq.ID == r.RequestDataMart.RequestID))
                                 let projectDataMartsAcls = projectDataMartsAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID) && a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                 where responses.ResponseIDs.Contains(r.ID)
                                 && (globalAcls.Any() || datamartAcls.Any() || projectAcls.Any() || projectDataMartsAcls.Any() || orgAcls.Any())
                                 && (globalAcls.All(a => a.Allowed) && datamartAcls.All(a => a.Allowed) && projectAcls.All(a => a.Allowed) && projectDataMartsAcls.All(a => a.Allowed) && orgAcls.All(a => a.Allowed))
                                 select r.ID).ToArray();

            if (responses.ResponseIDs.Count() != hasPermission.Length)
            {
                var deniedInstances = responses.ResponseIDs.Except(hasPermission);
                var deniedDataMarts = DataContext.RequestDataMarts.Where(dm => dm.Responses.Any(r => deniedInstances.Contains(r.ID)))
                                                            .GroupBy(dm => dm.DataMart)
                                                            .Select(dm => dm.Key.Organization.Name + "\\" + dm.Key.Name)
                                                            .ToArray();

                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Access Denied to 'Approve/Reject Response' for the following DataMarts: " + string.Join(", ", deniedDataMarts));
            }

            var requests = GetRequests(responses.ResponseIDs.ToArray());

            Logger.Error("Attepmting to loop Response");

            var routes = DataContext.RequestDataMarts.Include(dm => dm.Responses).Where(dm => dm.Responses.Any(r => responses.ResponseIDs.Contains(r.ID)));
            foreach (var route in routes)
            {
                route.Status = DTO.Enums.RoutingStatus.Completed;
                foreach (var r in route.Responses.Where(r => r.Count == r.RequestDataMart.Responses.Max(x => x.Count)))
                {
                    Logger.Error("route is: " + route.RoutingType);
                    r.ResponseMessage = responses.Message ?? r.ResponseMessage;
                    if (route.RoutingType == RoutingType.ExternalCNDS)
                    {
                        using (var helper = new Lpp.CNDS.ApiClient.NetworkRequestDispatcher())
                        {
                            try
                            {
                                Logger.Error("Attepmting to Send Response");
                                await helper.PostResponseDocuments(r.ID);
                                Logger.Error("Attepmting to update status");
                                //update the status of the route in the source request
                                await helper.UpdateSourceRoutingStatus(new[] { new DTO.CNDSUpdateRoutingStatusDTO { ResponseID = r.ID, Message = responses.Message ?? r.ResponseMessage, RoutingStatus = route.Status } });
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                                throw;
                            }
                        }
                    }
                }
            }

            foreach (var req in requests)
            {
                req.Item1.UpdatedOn = DateTime.UtcNow;
                req.Item1.UpdatedByID = Identity.ID;
            }

            await DataContext.SaveChangesAsync();

            await SendRequestCompleteNotifications(requests);         

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// rejects a response in the system
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> RejectResponses(RejectResponseDTO responses)
        {
            var globalAclFilter = DataContext.GlobalAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var datamartsAclFilter = DataContext.DataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectAclFilter = DataContext.ProjectAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectDataMartsAclFilter = DataContext.ProjectDataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var organizationAclFilter = DataContext.OrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);

            var hasPermission = (from r in DataContext.Responses
                                 let globalAcls = globalAclFilter
                                 let datamartAcls = datamartsAclFilter.Where(a => a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                 let projectAcls = projectAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID))
                                 let orgAcls = organizationAclFilter.Where(a => a.Organization.Requests.Any(rq => rq.ID == r.RequestDataMart.RequestID))
                                 let projectDataMartsAcls = projectDataMartsAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID) && a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                 where responses.ResponseIDs.Contains(r.ID)
                                 && (globalAcls.Any() || datamartAcls.Any() || projectAcls.Any() || projectDataMartsAcls.Any() || orgAcls.Any())
                                 && (globalAcls.All(a => a.Allowed) && datamartAcls.All(a => a.Allowed) && projectAcls.All(a => a.Allowed) && projectDataMartsAcls.All(a => a.Allowed) && orgAcls.All(a => a.Allowed))
                                 select r.ID).ToArray();



            if (responses.ResponseIDs.Count() != hasPermission.Length)
            {
                var deniedInstances = responses.ResponseIDs.Except(hasPermission);
                var deniedDataMarts = DataContext.RequestDataMarts.Where(dm => dm.Responses.Any(r => deniedInstances.Contains(r.ID)))
                                                            .GroupBy(dm => dm.DataMart)
                                                            .Select(dm => dm.Key.Organization.Name + "\\" + dm.Key.Name)
                                                            .ToArray();

                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Access Denied to 'Approve/Reject Response' for the following DataMarts: " + string.Join(", ", deniedDataMarts));
            }

            var requests = GetRequests(responses.ResponseIDs.ToArray());

            var routes = DataContext.RequestDataMarts.Include(dm => dm.Responses).Where(dm => dm.Responses.Any(r => responses.ResponseIDs.Contains(r.ID)));
            foreach (var route in routes)
            {
                route.Status = DTO.Enums.RoutingStatus.ResponseRejectedAfterUpload;
                foreach (var r in route.Responses.Where(r => r.Count == r.RequestDataMart.Responses.Max(x => x.Count)))
                {
                    r.ResponseMessage = responses.Message ?? r.ResponseMessage;
                }
            }

            foreach(var req in requests)
            {
                req.Item1.UpdatedOn = DateTime.UtcNow;
                req.Item1.UpdatedByID = Identity.ID;
            }

            await DataContext.SaveChangesAsync();

            await SendRequestCompleteNotifications(requests);

            return Request.CreateResponse(HttpStatusCode.Accepted);
        }

        IEnumerable<Tuple<Request,RequestStatuses>> GetRequests(Guid[] responseID)
        {
            var requests = new List<Tuple<Request, RequestStatuses>>();
            foreach (var req in DataContext.Responses.Where(r => responseID.Contains(r.ID)).Select(r => r.RequestDataMart.Request).DistinctBy(r => r.ID))
            {
                req.UpdatedOn = DateTime.UtcNow;
                req.UpdatedByID = Identity.ID;

                requests.Add(new Tuple<Request, RequestStatuses>(req, req.Status));
            }

            return requests;
        }

        async Task SendRequestCompleteNotifications(IEnumerable<Tuple<Request,RequestStatuses>> requests)
        {
            var requestStatusLogger = new Dns.Data.RequestLogConfiguration();
            List<Utilities.Logging.Notification> notifications = new List<Utilities.Logging.Notification>();
            //refresh the request statuses and send notifications if needed.
            foreach (var req in requests)
            {
                await DataContext.Entry(req.Item1).ReloadAsync();

                if (req.Item1.Status == RequestStatuses.Complete && req.Item1.Status != req.Item2)
                {
                    //request status was updated to complete, send notication                    
                    string[] emailText = await requestStatusLogger.GenerateRequestStatusChangedEmailContent(DataContext, req.Item1.ID, Identity.ID, req.Item2, req.Item1.Status);
                    var logItems = requestStatusLogger.GenerateRequestStatusEvents(DataContext, Identity, false, req.Item2, req.Item1.Status, req.Item1.ID, emailText[1], emailText[0], "Request Status Changed");

                    await DataContext.SaveChangesAsync();

                    foreach (Lpp.Dns.Data.Audit.RequestStatusChangedLog logitem in logItems)
                    {
                        var items = requestStatusLogger.CreateNotifications(logitem, DataContext, true);
                        if (items != null && items.Any())
                            notifications.AddRange(items);
                    }

                }

            }

            if (notifications.Count > 0)
            {
                await Task.Run(() => {
                    if (notifications.Any())
                        requestStatusLogger.SendNotification(notifications);
                });
            }
        }


        /// <summary>
        /// Gets the responses associated to the active task for the specified workflowactivity and request.
        /// </summary>
        /// <param name="requestID">The ID of the request.</param>
        /// <param name="workflowActivityID">The workflowactivityID</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<ResponseDTO> GetByWorkflowActivity(Guid requestID, Guid workflowActivityID)
        {
            var results = from r in DataContext.Responses.AsNoTracking()
                          join t in
                              (
                                  from tr in DataContext.ActionReferences
                                  where tr.Task.WorkflowActivityID == workflowActivityID
                                  && tr.Task.EndOn == null
                                  && tr.Task.References.Any(x => x.Type == DTO.Enums.TaskItemTypes.Request && x.ItemID == requestID)
                                  && (tr.Type == DTO.Enums.TaskItemTypes.Response || tr.Type == DTO.Enums.TaskItemTypes.AggregateResponse)
                                  select tr.ItemID
                                  ) on r.ID equals t
                          select new ResponseDTO
                          {
                              Count = r.Count,
                              ID = r.ID,
                              RequestDataMartID = r.RequestDataMartID,
                              RespondedByID = r.RespondedByID,
                              ResponseGroupID = r.ResponseGroupID,
                              ResponseMessage = r.ResponseMessage,
                              ResponseTime = r.ResponseTime,
                              SubmitMessage = r.SubmitMessage,
                              SubmittedByID = r.SubmittedByID,
                              SubmittedOn = r.SubmittedOn,
                              Timestamp = r.Timestamp
                          };

            return results.ToArray();
        }

        /// <summary>
        /// Gets if a user can view responses for a specific request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> CanViewResponses(Guid requestID)
        {
            var permissionIDs = new PermissionDefinition[] { PermissionIdentifiers.DataMartInProject.ApproveResponses, PermissionIdentifiers.DataMartInProject.GroupResponses, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus, PermissionIdentifiers.DataMartInProject.SeeRequests };

            var globalAcls = DataContext.GlobalAcls.FilterAcl(Identity, permissionIDs);
            var projectAcls = DataContext.ProjectAcls.FilterAcl(Identity, permissionIDs);
            var projectDataMartAcls = DataContext.ProjectDataMartAcls.FilterAcl(Identity, permissionIDs);
            var datamartAcls = DataContext.DataMartAcls.FilterAcl(Identity, permissionIDs);
            var organizationAcls = DataContext.OrganizationAcls.FilterAcl(Identity, permissionIDs);
            var userAcls = DataContext.UserAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus);
            var projectOrgAcls = DataContext.ProjectOrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewStatus);

            #region Orignal Query
            //NOTE: the original query took a very long time due to the number of concat's required. Since we are only interested if at least one of the permissions is applicable splitting into separate queries that are only run as needed.
            //var responsePermissions = await (from rri in DataContext.Responses.AsNoTracking()
            //                                 join t in
            //                                     (
            //                                         from rdm in DataContext.RequestDataMarts
            //                                         where rdm.RequestID == requestID
            //                                         select rdm.ID
            //                                         ) on rri.RequestDataMartID equals t
            //                                 let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
            //                                                         .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
            //                                                         .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
            //                                                         .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
            //                                                         .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                         .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
            //                                                         .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
            //                                                         .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                         .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                         .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

            //                                 let canViewStatus = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID).Select(a => a.Allowed)
            //                                                               .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
            //                                                               .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
            //                                                               .Concat(projectOrgAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Organization.Requests.Any(r => r.ID == requestID) && a.Organization.DataMarts.Any(dm => dm.ID == rri.RequestDataMart.DataMartID) && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
            //                                                               .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.UserID == Identity.ID).Select(a => a.Allowed))

            //                                 let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
            //                                                            .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
            //                                                            .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                            .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                            .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))

            //                                 let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
            //                                                            .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
            //                                                            .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                            .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
            //                                                            .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
            //                                 where (
            //                                     //the user can group
            //                                     (canGroup.Any() && canGroup.All(a => a)) ||
            //                                     //the user can view status
            //                                     //If they created or submitted the request, then they can view the status.
            //                                     rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
            //                                     rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
            //                                     (canViewStatus.Any() && canViewStatus.All(a => a)) ||
            //                                     (canViewResults.Any() && canViewResults.All(a => a)) ||
            //                                     //the user can approve
            //                                     (canApprove.Any() && canApprove.All(a => a))
            //                                  )
            //                                  || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
            //                                 select new
            //                                 {
            //                                     CanApprove = (canApprove.Any() && canApprove.All(a => a)),
            //                                     CanGroup = (canGroup.Any() && canGroup.All(a => a)),
            //                                     CanView = (canViewResults.Any() && canViewResults.All(a => a)) || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified)),
            //                                     ID = rri.ID,
            //                                     RequestDataMartID = rri.RequestDataMartID
            //                                 }).ToArrayAsync();

            //return responsePermissions.Any(r => r.CanView || r.CanApprove || r.CanGroup);
            #endregion
            
            var canView = await (from rri in DataContext.Responses.AsNoTracking()
                                             join t in
                                                 (
                                                     from rdm in DataContext.RequestDataMarts
                                                     where rdm.RequestID == requestID
                                                     select rdm.ID
                                                     ) on rri.RequestDataMartID equals t
                                             let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
                                                                     .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                     .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                     .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
                                                                     .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                     .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
                                                                     .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                     .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                     .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                     .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

                                 where (
                                                 //the user can view status
                                                 //If they created or submitted the request, then they can view the status.
                                                 (rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                                 rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                                 (canViewResults.Any() && canViewResults.All(a => a)))
                                                 && (rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload)
                                              )
                                              || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                             select new
                                             {
                                                 CanView = (canViewResults.Any() && canViewResults.All(a => a)) || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                             }).Where(a => a.CanView == true).AnyAsync();

            if (canView)
            {
                return true;
            }

            var canApproveResponses = await (from rri in DataContext.Responses.AsNoTracking()
                                             join t in
                                                 (
                                                     from rdm in DataContext.RequestDataMarts
                                                     where rdm.RequestID == requestID
                                                     select rdm.ID
                                                     ) on rri.RequestDataMartID equals t

                                             let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
                                                                        .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                        .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                        .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                        .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                             where (
                                                 //the user can view status
                                                 //If they created or submitted the request, then they can view the status.
                                                 rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                                 rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                                 //the user can approve
                                                 (canApprove.Any() && canApprove.All(a => a))
                                              )
                                              || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                             select new
                                             {
                                                 CanApprove = (canApprove.Any() && canApprove.All(a => a)),
                                             }).Where(a => a.CanApprove == true).AnyAsync();

            if (canApproveResponses)
            {
                return true;
            }

            var canGroupResponses = await (from rri in DataContext.Responses.AsNoTracking()
                                             join t in
                                                 (
                                                     from rdm in DataContext.RequestDataMarts
                                                     where rdm.RequestID == requestID
                                                     select rdm.ID
                                                     ) on rri.RequestDataMartID equals t
                                             let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
                                                                        .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                        .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                        .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                        .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                             where (
                                                 //the user can group
                                                 (canGroup.Any() && canGroup.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload) ||
                                                 //the user can view status
                                                 //If they created or submitted the request, then they can view the status.
                                                 rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                                 rri.RequestDataMart.Request.SubmittedByID == Identity.ID
                                              )
                                              || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                             select new
                                             {
                                                 CanGroup = (canGroup.Any() && canGroup.All(a => a))
                                             }).Where(a => a.CanGroup == true).AnyAsync();

            return canGroupResponses;
        }

        /// <summary>
        /// Verify if the user is capable of viewing responses when the selected responses are in Awaiting Approval status or have had the response rejected.
        /// </summary>
        /// <param name="responses"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> CanViewPendingApprovalResponses(ApproveResponseDTO responses)
        {
            var globalAclFilter = DataContext.GlobalAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var datamartsAclFilter = DataContext.DataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectAclFilter = DataContext.ProjectAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var projectDataMartsAclFilter = DataContext.ProjectDataMartAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            var organizationAclFilter = DataContext.OrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.DataMartInProject.ApproveResponses);
            
            if (DataContext.Responses.Any(r => responses.ResponseIDs.Contains(r.ID) && (r.RequestDataMart.Status == RoutingStatus.AwaitingResponseApproval || r.RequestDataMart.Status == RoutingStatus.ResponseRejectedAfterUpload)))
            {
                var hasPermission = await (from r in DataContext.Responses
                                           let datamartAcls = datamartsAclFilter.Where(a => a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                           let projectAcls = projectAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID))
                                           let orgAcls = organizationAclFilter.Where(a => a.Organization.Requests.Any(rq => rq.ID == r.RequestDataMart.RequestID))
                                           let projectDataMartsAcls = projectDataMartsAclFilter.Where(a => a.Project.Requests.Any(rd => rd.ID == r.RequestDataMart.RequestID) && a.DataMart.Requests.Any(rd => rd.ID == r.RequestDataMartID))
                                           where responses.ResponseIDs.Contains(r.ID)
                                           && (r.RequestDataMart.Status == RoutingStatus.AwaitingResponseApproval || r.RequestDataMart.Status == RoutingStatus.ResponseRejectedAfterUpload)
                                           && (datamartAcls.Any() || projectAcls.Any() || projectDataMartsAcls.Any() || orgAcls.Any())
                                           && (datamartAcls.All(a => a.Allowed) && projectAcls.All(a => a.Allowed) && projectDataMartsAcls.All(a => a.Allowed) && orgAcls.All(a => a.Allowed))
                                           select r.ID).ToArrayAsync();

                if (DataContext.Responses.Count(r => responses.ResponseIDs.Contains(r.ID) && (r.RequestDataMart.Status == RoutingStatus.AwaitingResponseApproval || r.RequestDataMart.Status == RoutingStatus.ResponseRejectedAfterUpload)) != hasPermission.Length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets all the details for the responses associated to the specified requests current workflow activity.
        /// </summary>
        /// <param name="requestID">The ID of the request.</param>
        /// <param name="viewDocuments">Optional flag if one wants the documents included.  Defaults to false</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CommonResponseDetailDTO> GetForWorkflowRequest(Guid requestID, bool? viewDocuments = false)
        {
            var permissionIDs = new PermissionDefinition[] { PermissionIdentifiers.DataMartInProject.ApproveResponses, PermissionIdentifiers.DataMartInProject.GroupResponses, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus, PermissionIdentifiers.DataMartInProject.SeeRequests };

            var globalAcls = DataContext.GlobalAcls.FilterAcl(Identity, permissionIDs);
            var projectAcls = DataContext.ProjectAcls.FilterAcl(Identity, permissionIDs);
            var projectDataMartAcls = DataContext.ProjectDataMartAcls.FilterAcl(Identity, permissionIDs);
            var datamartAcls = DataContext.DataMartAcls.FilterAcl(Identity, permissionIDs);
            var organizationAcls = DataContext.OrganizationAcls.FilterAcl(Identity, permissionIDs);
            var userAcls = DataContext.UserAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus);
            var projectOrgAcls = DataContext.ProjectOrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewStatus);

            CommonResponseDetailDTO response = new CommonResponseDetailDTO();


            response.Responses = await (from rri in DataContext.Responses.AsNoTracking()
                                        join rdmr in DataContext.RequestDataMarts on rri.RequestDataMartID equals rdmr.ID
                                        where rdmr.RequestID == requestID && rri.ResponseTime != null && rri.RespondedByID != null
                                        let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
                                                                          .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                          .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                          .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
                                                                          .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                          .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
                                                                          .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                          .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                          .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                          .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

                                        let canViewStatus = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID).Select(a => a.Allowed)
                                                                      .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                      .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                      .Concat(projectOrgAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Organization.Requests.Any(r => r.ID == requestID) && a.Organization.DataMarts.Any(dm => dm.ID == rri.RequestDataMart.DataMartID) && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                      .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.UserID == Identity.ID).Select(a => a.Allowed))

                                        let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
                                                                   .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                   .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))

                                        let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
                                                                   .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                   .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                        where (
                                            //the user can group
                                            (canGroup.Any() && canGroup.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            //the user can view status
                                            //If they created or submitted the request, then they can view the status.
                                            rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                            rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                            (canViewStatus.Any() && canViewStatus.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            (canViewResults.Any() && canViewResults.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            //the user can approve
                                            (canApprove.Any() && canApprove.All(a => a))
                                         )
                                         || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                        select new ResponseDTO
                                        {
                                            Count = rri.Count,
                                            ID = rri.ID,
                                            RequestDataMartID = rri.RequestDataMartID,
                                            RespondedByID = rri.RespondedByID,
                                            ResponseGroupID = rri.ResponseGroupID,
                                            ResponseMessage = rri.ResponseMessage,
                                            ResponseTime = rri.ResponseTime,
                                            SubmitMessage = rri.SubmitMessage,
                                            SubmittedByID = rri.SubmittedByID,
                                            SubmittedOn = rri.SubmittedOn,
                                            Timestamp = rri.Timestamp
                                        }).ToArrayAsync();

            response.RequestDataMarts = await DataContext.RequestDataMarts.Where(dm => dm.RequestID == requestID).Map<RequestDataMart, RequestDataMartDTO>().ToArrayAsync();

            var externalRequestDataMarts = await DataContext.RequestDataMarts.Where(rdm => rdm.RequestID == requestID && rdm.DataMart.DataMartTypeID == RequestsController.CNDSDataMartTypeID).Select(rdm => new { ID = rdm.ID, DataMartID = rdm.DataMartID }).ToArrayAsync();
            if (externalRequestDataMarts.Length > 0)
            {
                var routeDefinitions = (await CNDSApi.RequestTypeMapping.ListRequestTypeDefinitions("$filter=" + string.Join(" OR ", externalRequestDataMarts.Select(rdm => string.Format("ID eq {0:D}", rdm.DataMartID))))).ToDictionary(k => k.ID);
                if (routeDefinitions.Count > 0)
                {
                    for(int i = 0; i < externalRequestDataMarts.Length; i++)
                    {
                        var datamartDTO = response.RequestDataMarts.First(rdm => rdm.ID == externalRequestDataMarts[i].ID);
                        Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO definition;
                        if(routeDefinitions.TryGetValue(datamartDTO.DataMartID, out definition))
                        {
                            datamartDTO.DataMart = string.Format("{0}/{1}/{2}/{3}", definition.Network, definition.Project, definition.RequestType, definition.DataSource);
                        }
                    }
                }
            }

            if (viewDocuments.Value)
            {
                var responseIDs = response.Responses.Where(r => r.ResponseGroupID.IsNull()).Select(r => r.ID).Distinct();
                response.Documents = await DataContext.Documents
                                                      .Where(d => responseIDs.Contains(d.ItemID))
                                                      .Select(d => new ExtendedDocumentDTO
                                                      {
                                                          ID = d.ID,
                                                          Name = d.Name,
                                                          FileName = d.FileName,
                                                          MimeType = d.MimeType,
                                                          Description = d.Description,
                                                          Viewable = d.Viewable,
                                                          ItemID = d.ItemID,
                                                      //set the item title to the datamart name that responded
                                                      ItemTitle = DataContext.Responses.Where(r => r.ID == d.ItemID).Select(r => r.RequestDataMart.DataMart.Name).FirstOrDefault(),
                                                          Kind = d.Kind,
                                                          Length = d.Length,
                                                          CreatedOn = d.CreatedOn,
                                                          ParentDocumentID = d.ParentDocumentID,
                                                          RevisionDescription = d.RevisionDescription,
                                                          RevisionSetID = d.RevisionSetID,
                                                          MajorVersion = d.MajorVersion,
                                                          MinorVersion = d.MinorVersion,
                                                          BuildVersion = d.BuildVersion,
                                                          RevisionVersion = d.RevisionVersion,
                                                          Timestamp = d.Timestamp,
                                                          UploadedByID = d.UploadedByID,
                                                          UploadedBy = DataContext.Users.Where(u => u.ID == d.UploadedByID).Select(u => u.UserName).FirstOrDefault()
                                                      }).ToArrayAsync(); 
            }


            return response;
        }

        /// <summary>
        /// Return the response details for the specified responses.
        /// Note: All responses must belong the same Request.
        /// </summary>
        /// <param name="id">The collection if response id's to return the details for.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CommonResponseDetailDTO> GetDetails([FromUri]IEnumerable<Guid> id)
        {
            Guid[] IDs = id.ToArray();

            var responseIDs = await DataContext.Responses.Where(rsp => IDs.Contains(rsp.ID) || IDs.Contains(rsp.ResponseGroupID.HasValue ? rsp.ResponseGroupID.Value : new Guid())).Select(rsp => rsp.ID).ToArrayAsync();

            //make sure the responses all belong to the same request
            var requestIDs = await DataContext.Responses.Where(rsp => responseIDs.Contains(rsp.ID)).Select(rsp => rsp.RequestDataMart.RequestID).ToArrayAsync();
            Guid requestID = requestIDs[0];
            if (requestIDs.Length > 1 && !requestIDs.All(i => i == requestID))
            {
                throw new ArgumentOutOfRangeException("id", "All the responses must belong to the same request!");
            }

            var permissionIDs = new PermissionDefinition[] { PermissionIdentifiers.DataMartInProject.ApproveResponses, PermissionIdentifiers.DataMartInProject.GroupResponses, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus, PermissionIdentifiers.DataMartInProject.SeeRequests };

            var globalAcls = DataContext.GlobalAcls.FilterAcl(Identity, permissionIDs);
            var projectAcls = DataContext.ProjectAcls.FilterAcl(Identity, permissionIDs);
            var projectDataMartAcls = DataContext.ProjectDataMartAcls.FilterAcl(Identity, permissionIDs);
            var datamartAcls = DataContext.DataMartAcls.FilterAcl(Identity, permissionIDs);
            var organizationAcls = DataContext.OrganizationAcls.FilterAcl(Identity, permissionIDs);
            var userAcls = DataContext.UserAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus);
            var projectOrgAcls = DataContext.ProjectOrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewStatus);

            CommonResponseDetailDTO response = new CommonResponseDetailDTO();

            response.Responses = await (from rri in DataContext.Responses.Where(rsp => responseIDs.Contains(rsp.ID)).AsNoTracking()
                                        let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

                                        let canViewStatus = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID).Select(a => a.Allowed)
                                                                      .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                      .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                      .Concat(projectOrgAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Organization.Requests.Any(r => r.ID == requestID) && a.Organization.DataMarts.Any(dm => dm.ID == rri.RequestDataMart.DataMartID) && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                      .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.UserID == Identity.ID).Select(a => a.Allowed))

                                        let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
                                                                   .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                   .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))

                                        let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
                                                                   .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                   .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                   .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                        where (
                                            //the user can group
                                            (canGroup.Any() && canGroup.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            //the user can view status
                                            //If they created or submitted the request, then they can view the status.
                                            rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                            rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                            (canViewStatus.Any() && canViewStatus.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            (canViewResults.Any() && canViewResults.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                            //the user can approve
                                            (canApprove.Any() && canApprove.All(a => a))
                                         )
                                         || 
                                         ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                        select new ResponseDTO
                                        {
                                            Count = rri.Count,
                                            ID = rri.ID,
                                            RequestDataMartID = rri.RequestDataMartID,
                                            RespondedByID = rri.RespondedByID,
                                            ResponseGroupID = rri.ResponseGroupID,
                                            ResponseMessage = rri.ResponseMessage,
                                            ResponseTime = rri.ResponseTime,
                                            SubmitMessage = rri.SubmitMessage,
                                            SubmittedByID = rri.SubmittedByID,
                                            SubmittedOn = rri.SubmittedOn,
                                            Timestamp = rri.Timestamp
                                        }).ToArrayAsync();

            var requestDataMarts = response.Responses.Select(r => r.RequestDataMartID).Distinct();
            response.RequestDataMarts = await DataContext.RequestDataMarts.Where(rdm => requestDataMarts.Contains(rdm.ID)).Map<RequestDataMart, RequestDataMartDTO>().ToArrayAsync();

            var nonGroupedResponseIDs = response.Responses.Where(r => r.ResponseGroupID.IsNull()).Select(r => r.ID).Distinct();
            response.Documents = (from doc in DataContext.Documents
                                        join reqDoc in DataContext.RequestDocuments on doc.RevisionSetID equals reqDoc.RevisionSetID
                                        where nonGroupedResponseIDs.Contains(reqDoc.ResponseID)
                                        select new ExtendedDocumentDTO
                                        {
                                            ID = doc.ID,
                                            Name = doc.Name,
                                            FileName = doc.FileName,
                                            MimeType = doc.MimeType,
                                            Description = doc.Description,
                                            Viewable = doc.Viewable,
                                            ItemID = doc.ItemID,
                                            //set the item title to the datamart name that responded
                                            ItemTitle = DataContext.Responses.Where(r => r.ID == doc.ItemID).Select(r => r.RequestDataMart.DataMart.Name).FirstOrDefault(),
                                            Kind = doc.Kind,
                                            Length = doc.Length,
                                            CreatedOn = doc.CreatedOn,
                                            ParentDocumentID = doc.ParentDocumentID,
                                            RevisionDescription = doc.RevisionDescription,
                                            RevisionSetID = doc.RevisionSetID,
                                            MajorVersion = doc.MajorVersion,
                                            MinorVersion = doc.MinorVersion,
                                            BuildVersion = doc.BuildVersion,
                                            RevisionVersion = doc.RevisionVersion,
                                            Timestamp = doc.Timestamp,
                                            UploadedByID = doc.UploadedByID,
                                            UploadedBy = DataContext.Users.Where(u => u.ID == doc.UploadedByID).Select(u => u.UserName).FirstOrDefault(),
                                            DocumentType = reqDoc.DocumentType
                                        }).Concat(DataContext.Documents
                                                  .Where(d => nonGroupedResponseIDs.Contains(d.ItemID))
                                                  .Select(d => new ExtendedDocumentDTO
                                                  {
                                                      ID = d.ID,
                                                      Name = d.Name,
                                                      FileName = d.FileName,
                                                      MimeType = d.MimeType,
                                                      Description = d.Description,
                                                      Viewable = d.Viewable,
                                                      ItemID = d.ItemID,
                                                      //set the item title to the datamart name that responded
                                                      ItemTitle = DataContext.Responses.Where(r => r.ID == d.ItemID).Select(r => r.RequestDataMart.DataMart.Name).FirstOrDefault(),
                                                      Kind = d.Kind,
                                                      Length = d.Length,
                                                      CreatedOn = d.CreatedOn,
                                                      ParentDocumentID = d.ParentDocumentID,
                                                      RevisionDescription = d.RevisionDescription,
                                                      RevisionSetID = d.RevisionSetID,
                                                      MajorVersion = d.MajorVersion,
                                                      MinorVersion = d.MinorVersion,
                                                      BuildVersion = d.BuildVersion,
                                                      RevisionVersion = d.RevisionVersion,
                                                      Timestamp = d.Timestamp,
                                                      UploadedByID = d.UploadedByID,
                                                      UploadedBy = DataContext.Users.Where(u => u.ID == d.UploadedByID).Select(u => u.UserName).FirstOrDefault(),
                                                      DocumentType = null
                                                  })).GroupBy(doc => doc.ID).Select(grp => grp.FirstOrDefault()).ToArray();

            response.CanViewPendingApprovalResponses = await CanViewPendingApprovalResponses(new ApproveResponseDTO { Message = "", ResponseIDs = response.Responses.Where(rsp => rsp.ID.HasValue).Select(rsp => rsp.ID.Value).ToArray() });

            return response;
        }

        /// <summary>
        /// Aggregates content of respones
        /// </summary>
        /// <param name="responsesToAggregate">List for parsing</param>
        /// <param name="requestID">The ID of the request.</param>
        /// <returns></returns>
        public DTO.QueryComposer.QueryComposerResponseDTO AggregateResults(List<DTO.QueryComposer.QueryComposerResponseDTO> responsesToAggregate, Guid requestID)
        {
            DTO.QueryComposer.QueryComposerResponseDTO combinedResponse = new DTO.QueryComposer.QueryComposerResponseDTO();
            combinedResponse.RequestID = requestID;
            combinedResponse.ResponseDateTime = DateTime.UtcNow;//TODO: should this be based on a response date? the most recent or earlies?

            //merge results
            if (responsesToAggregate.Count == 1)
            {
                combinedResponse = responsesToAggregate[0];
            }
            else
            {
                IEnumerable<Dictionary<string, object>> combined = Enumerable.Empty<Dictionary<string, object>>();
                foreach (var r in responsesToAggregate.SelectMany(rr => rr.Results))
                {
                    combined = combined.Concat(r);
                }
                combinedResponse.Results = new[] { combined };
            }

            IEnumerable<Objects.Dynamic.IPropertyDefinition> propertyDefinitions = responsesToAggregate.Where(r => r.Properties.Any()).Select(r => r.Properties).FirstOrDefault();

            //add a default groupingkey, this is needed for when there is only a single property in the response and it is getting aggregated
            propertyDefinitions = propertyDefinitions.Union(new[] { new DTO.QueryComposer.QueryComposerResponsePropertyDefinitionDTO { Name = "__DefaultGroupingKey", Type = typeof(string).FullName } });

            //convert to typed objects so that we can work with the results using reflection, all responses must have the same property and aggregation definition.
            Type resultType = Lpp.Objects.Dynamic.TypeBuilderHelper.CreateType("ResponseItem", propertyDefinitions);

            //create a typed list to hold the converted response items
            Type listType = typeof(List<>).MakeGenericType(resultType);
            //System.Collections.IList items = Activator.CreateInstance(listType) as System.Collections.IList;
            var items = Activator.CreateInstance(listType);

            //build a map of the property info to the dictionary key values
            IDictionary<string, System.Reflection.PropertyInfo> propertyInfoMap = Lpp.Objects.Dynamic.TypeBuilderHelper.CreatePropertyInfoMap(resultType, propertyDefinitions);

            foreach (var dic in combinedResponse.Results.First())
            {
                //add the default grouping value to the existing result item
                dic.Add("__DefaultGroupingKey", "__DefaultGroupingKeyValue");

                //create and add the populated object to the collection
                var obj = Lpp.Objects.Dynamic.TypeBuilderHelper.FlattenDictionaryToType(resultType, dic, propertyInfoMap);
                ((System.Collections.IList)items).Add(obj);
            }

            if (((System.Collections.IList)items).Count == 0)
            {
                combinedResponse.Results = new[] { new Dictionary<string, object>[0] };
            }
            else
            {
                var aggregate = responsesToAggregate.Where(r => r.Aggregation != null).Select(r => r.Aggregation).FirstOrDefault();

                List<string> selectBy = new List<string>(aggregate.Select.Count() + 10);
                foreach (Lpp.Dns.DTO.QueryComposer.QueryComposerResponsePropertyDefinitionDTO prop in aggregate.Select)
                {
                    string s = (aggregate.GroupBy.Contains(prop.Name, StringComparer.OrdinalIgnoreCase) ? "Key." : "") + prop.As;
                    if (!string.IsNullOrWhiteSpace(prop.Aggregate))
                    {
                        s = prop.Aggregate + "(" + Lpp.Objects.Dynamic.TypeBuilderHelper.CleanString(s) + ")";
                    }

                    if (!string.IsNullOrWhiteSpace(prop.Aggregate))
                    {
                        s += " as " + Lpp.Objects.Dynamic.TypeBuilderHelper.CleanString(prop.As);
                    }
                    if (s != "LowThreshold") //dont add LowThreshold to the select
                        selectBy.Add(s);
                }

                var q = ((System.Collections.IList)items).AsQueryable();

                if (aggregate.GroupBy != null && aggregate.GroupBy.Any())
                {
                    string groupingStatement = "new (" + string.Join(",", aggregate.GroupBy) + ")";
                    q = q.GroupBy(groupingStatement);
                }
                else
                {
                    //since no fields were specified for grouping use the default grouping key
                    q = q.GroupBy("new (__DefaultGroupingKey)");
                }

                string selectStatement = "new (" + string.Join(",", selectBy) + ")";
                q = q.Select(selectStatement);

                //convert results back to IEnumerable<Dictionary<string,object>>, and add to the results being returned
                //dont include LowThreshold as an aggregation
                IEnumerable<Dictionary<string, object>> aggregatedResults = Lpp.Objects.Dynamic.TypeBuilderHelper.ConvertToDictionary(((IQueryable)q).AsEnumerable(), aggregate.Select.Where(s => s.Name != "LowThreshold"));
                combinedResponse.Results = new[] { aggregatedResults.ToArray() };
                combinedResponse.Aggregation = aggregate;

            }

            return combinedResponse;
        }


        /// <summary>
        /// Gets the content for the respones for the specified requests current workflow activity.
        /// </summary>
        /// <param name="requestID">The ID of the request.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DTO.QueryComposer.QueryComposerResponseDTO>> GetResponseContentForWorkflowRequest(Guid requestID)
        {
            var permissionIDs = new PermissionDefinition[] { PermissionIdentifiers.DataMartInProject.ApproveResponses, PermissionIdentifiers.DataMartInProject.GroupResponses, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus, PermissionIdentifiers.DataMartInProject.SeeRequests };

            var globalAcls = DataContext.GlobalAcls.FilterAcl(Identity, permissionIDs);
            var projectAcls = DataContext.ProjectAcls.FilterAcl(Identity, permissionIDs);
            var projectDataMartAcls = DataContext.ProjectDataMartAcls.FilterAcl(Identity, permissionIDs);
            var datamartAcls = DataContext.DataMartAcls.FilterAcl(Identity, permissionIDs);
            var organizationAcls = DataContext.OrganizationAcls.FilterAcl(Identity, permissionIDs);
            var userAcls = DataContext.UserAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus);
            var projectOrgAcls = DataContext.ProjectOrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewStatus);


            var responseReferences = await (from rqst in DataContext.Requests
                                            where rqst.ID == requestID && rqst.WorkFlowActivityID != null
                                            from tr in DataContext.ActionReferences
                                            join rri in DataContext.Responses on tr.ItemID equals rri.ID
                                            let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

                                            let canViewStatus = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID).Select(a => a.Allowed)
                                                                          .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                          .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                          .Concat(projectOrgAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Organization.Requests.Any(r => r.ID == requestID) && a.Organization.DataMarts.Any(dm => dm.ID == rri.RequestDataMart.DataMartID) && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                          .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.UserID == Identity.ID).Select(a => a.Allowed))

                                            let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
                                                                       .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                       .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))

                                            let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
                                                                       .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                       .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == requestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == requestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                            where tr.Task.WorkflowActivityID == rqst.WorkFlowActivityID
                                            && tr.Task.EndOn == null
                                            && tr.Task.References.Any(x => x.Type == DTO.Enums.TaskItemTypes.Request && x.ItemID == requestID)
                                            && (tr.Type == DTO.Enums.TaskItemTypes.Response || tr.Type == DTO.Enums.TaskItemTypes.AggregateResponse)
                                            &&(
                                                (
                                                        //the user can group
                                                    (canGroup.Any() && canGroup.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                        //the user can view status
                                                        //If they created or submitted the request, then they can view the status.
                                                    rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                                    rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                                    (canViewStatus.Any() && canViewStatus.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                    (canViewResults.Any() && canViewResults.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                        //the user can approve
                                                    (canApprove.Any() && canApprove.All(a => a))
                                                 )
                                                 || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                            )
                                            select new
                                            {
                                                ResponseID = tr.ItemID,
                                                ReferenceType = tr.Type,
                                                ResponseGroupName = rri.ResponseGroup.Name,
                                                ResponseGroupID = rri.ResponseGroupID,
                                                Documents = DataContext.Documents.Where(d => d.ItemID == tr.ItemID && d.Name == "response.json").Select(d => d.ID)
                                            }).ToArrayAsync();

            var serializationSettings = new Newtonsoft.Json.JsonSerializerSettings();
            serializationSettings.Converters.Add(new DTO.QueryComposer.QueryComposerResponsePropertyDefinitionConverter());
            var deserializer = Newtonsoft.Json.JsonSerializer.Create(serializationSettings);

            Type queryComposerResponseDTOType = typeof(DTO.QueryComposer.QueryComposerResponseDTO);
            List<DTO.QueryComposer.QueryComposerResponseDTO> results = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

            //for any marked as aggregate response, merge and then apply aggregation
            var resultsToAggregate = responseReferences.Where(r => r.ReferenceType == DTO.Enums.TaskItemTypes.AggregateResponse)
                                                        .SelectMany(r =>
                                                        {

                                                            List<DTO.QueryComposer.QueryComposerResponseDTO> l = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

                                                            foreach (var documentID in r.Documents)
                                                            {
                                                                using (var documentStream = new Data.Documents.DocumentStream(DataContext, documentID))
                                                                using (var streamReader = new System.IO.StreamReader(documentStream))
                                                                {
                                                                    DTO.QueryComposer.QueryComposerResponseDTO rsp = (DTO.QueryComposer.QueryComposerResponseDTO)deserializer.Deserialize(streamReader, queryComposerResponseDTOType);
                                                                    rsp.ID = r.ResponseID;
                                                                    rsp.RequestID = requestID;
                                                                    l.Add(rsp);
                                                                }
                                                            }

                                                            return l;
                                                        }).ToList();

            //Called it here
            if (resultsToAggregate.Count > 0)
                results.Add(AggregateResults(resultsToAggregate, requestID));

            #region Old_Aggregate_Code

            //if (resultsToAggregate.Count > 0) 
            //{
            //    DTO.QueryComposer.QueryComposerResponseDTO combinedResponse = new DTO.QueryComposer.QueryComposerResponseDTO();
            //    combinedResponse.RequestID = requestID;
            //    combinedResponse.ResponseDateTime = DateTime.UtcNow;//TODO: should this be based on a response date? the most recent or earlies?

            //    //merge results
            //    if (resultsToAggregate.Count == 1)
            //    {
            //        combinedResponse = resultsToAggregate[0];
            //    }
            //    else
            //    {
            //        IEnumerable<Dictionary<string, object>> combined = Enumerable.Empty<Dictionary<string, object>>();
            //        foreach (var r in resultsToAggregate.SelectMany(rr => rr.Results))
            //        {
            //            combined = combined.Concat(r);
            //        }
            //        combinedResponse.Results = new []{ combined };
            //    }

            //    IEnumerable<Objects.Dynamic.IPropertyDefinition> propertyDefinitions = resultsToAggregate.Where(r => r.Properties.Any()).Select(r => r.Properties).FirstOrDefault();

            //    //add a default groupingkey, this is needed for when there is only a single property in the response and it is getting aggregated
            //    propertyDefinitions = propertyDefinitions.Union(new[] { new DTO.QueryComposer.QueryComposerResponsePropertyDefinitionDTO { Name = "__DefaultGroupingKey", Type = typeof(string).FullName } });

            //    //convert to typed objects so that we can work with the results using reflection, all responses must have the same property and aggregation definition.
            //    Type resultType = Lpp.Objects.Dynamic.TypeBuilderHelper.CreateType("ResponseItem", propertyDefinitions);

            //    //create a typed list to hold the converted response items
            //    Type listType = typeof(List<>).MakeGenericType(resultType);
            //    //System.Collections.IList items = Activator.CreateInstance(listType) as System.Collections.IList;
            //    var items = Activator.CreateInstance(listType);

            //    //build a map of the property info to the dictionary key values
            //    IDictionary<string, System.Reflection.PropertyInfo> propertyInfoMap = Lpp.Objects.Dynamic.TypeBuilderHelper.CreatePropertyInfoMap(resultType, propertyDefinitions);

            //    foreach (var dic in combinedResponse.Results.First())
            //    {
            //        //add the default grouping value to the existing result item
            //        dic.Add("__DefaultGroupingKey", "__DefaultGroupingKeyValue");

            //        //create and add the populated object to the collection
            //        var obj = Lpp.Objects.Dynamic.TypeBuilderHelper.FlattenDictionaryToType(resultType, dic, propertyInfoMap);
            //        ((System.Collections.IList)items).Add(obj);
            //    }

            //    if (((System.Collections.IList)items).Count == 0)
            //    {
            //        combinedResponse.Results = new[] { new Dictionary<string,object>[0] };
            //    }
            //    else
            //    {   
            //        var aggregate = resultsToAggregate.Where(r => r.Aggregation != null).Select(r => r.Aggregation).FirstOrDefault();

            //        List<string> selectBy = new List<string>(aggregate.Select.Count() + 10);
            //        foreach (Lpp.Dns.DTO.QueryComposer.QueryComposerResponsePropertyDefinitionDTO prop in aggregate.Select)
            //        {
            //            string s = (aggregate.GroupBy.Contains(prop.Name, StringComparer.OrdinalIgnoreCase) ? "Key." : "") + prop.As;
            //            if (!string.IsNullOrWhiteSpace(prop.Aggregate))
            //            {
            //                s = prop.Aggregate + "(" + Lpp.Objects.Dynamic.TypeBuilderHelper.CleanString(s) + ")";
            //            }

            //            if (!string.IsNullOrWhiteSpace(prop.Aggregate))
            //            {
            //                s += " as " + Lpp.Objects.Dynamic.TypeBuilderHelper.CleanString(prop.As);
            //            }
            //            selectBy.Add(s);
            //        }

            //        var q = ((System.Collections.IList)items).AsQueryable();

            //        if (aggregate.GroupBy != null && aggregate.GroupBy.Any())
            //        {
            //            string groupingStatement = "new (" + string.Join(",", aggregate.GroupBy) + ")";
            //            q = q.GroupBy(groupingStatement);
            //        }
            //        else
            //        {
            //            //since no fields were specified for grouping use the default grouping key
            //            q = q.GroupBy("new (__DefaultGroupingKey)");
            //        }

            //        string selectStatement = "new (" + string.Join(",", selectBy) + ")";
            //        q = q.Select(selectStatement);

            //        //convert results back to IEnumerable<Dictionary<string,object>>, and add to the results being returned
            //        IEnumerable<Dictionary<string, object>> aggregatedResults = Lpp.Objects.Dynamic.TypeBuilderHelper.ConvertToDictionary(((IQueryable)q).AsEnumerable(), aggregate.Select);
            //        combinedResponse.Results = new[] { aggregatedResults.ToArray() };

            //    }

            //    results.Add(combinedResponse);

            //}
            #endregion

            var virtualResponses = responseReferences.Where(r => r.ReferenceType == DTO.Enums.TaskItemTypes.Response).GroupBy(g => g.ResponseGroupID).ToArray();
            var individualResults = responseReferences.Where(r => r.ReferenceType == DTO.Enums.TaskItemTypes.Response).ToArray();
            //for any marked as individual just load and add to return content

            foreach (var vr in virtualResponses)
            {
                List<DTO.QueryComposer.QueryComposerResponseDTO> vresults = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

                foreach (var taskReference in vr)
                {
                    foreach (Guid documentID in taskReference.Documents)
                    {
                        using (var documentStream = new Data.Documents.DocumentStream(DataContext, documentID))
                        using (var streamReader = new System.IO.StreamReader(documentStream))
                        {
                            DTO.QueryComposer.QueryComposerResponseDTO rsp = (DTO.QueryComposer.QueryComposerResponseDTO)deserializer.Deserialize(streamReader, queryComposerResponseDTOType);
                            rsp.ID = taskReference.ResponseID;
                            rsp.RequestID = requestID;
                            if (!rsp.Aggregation.IsNull())
                                rsp.Aggregation.Name = vr.Select(r => r.ResponseGroupName).FirstOrDefault();
                            vresults.Add(rsp);
                        }
                    }

                }

                bool isGrouped = vr.Where(v => !v.ResponseGroupID.IsNull()).Count() > 0;
                if (!isGrouped)
                    results.AddRange(vresults);
                else
                    results.Add(AggregateResults(vresults, requestID));
            }


            return results;
        }

        /// <summary>
        /// Gets the content for the specified responses of a workflow request.
        /// </summary>
        /// <param name="id">The collection of response ids to get the content for.</param>
        /// <param name="view">The type of processing of the responses: Individual, Aggregate, etc..</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DTO.QueryComposer.QueryComposerResponseDTO>> GetWorkflowResponseContent([FromUri]IEnumerable<Guid> id, TaskItemTypes view)
        {
            var responseID = id.ToArray();

            //make sure the responses all belong to the same request
            var requestIDs = await DataContext.Responses.Where(rsp => responseID.Contains(rsp.ID)).Select(rsp => rsp.RequestDataMart.RequestID).ToArrayAsync();
            Guid requestID = requestIDs[0];
            if (requestIDs.Length > 1 && !requestIDs.All(i => i == requestID))
            {
                throw new ArgumentOutOfRangeException("id", "All the responses must belong to the same request!");
            }

            var permissionIDs = new PermissionDefinition[] { PermissionIdentifiers.DataMartInProject.ApproveResponses, PermissionIdentifiers.DataMartInProject.GroupResponses, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus, PermissionIdentifiers.DataMartInProject.SeeRequests };

            var globalAcls = DataContext.GlobalAcls.FilterAcl(Identity, permissionIDs);
            var projectAcls = DataContext.ProjectAcls.FilterAcl(Identity, permissionIDs);
            var projectDataMartAcls = DataContext.ProjectDataMartAcls.FilterAcl(Identity, permissionIDs);
            var datamartAcls = DataContext.DataMartAcls.FilterAcl(Identity, permissionIDs);
            var organizationAcls = DataContext.OrganizationAcls.FilterAcl(Identity, permissionIDs);
            var userAcls = DataContext.UserAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewResults, PermissionIdentifiers.Request.ViewStatus);
            var projectOrgAcls = DataContext.ProjectOrganizationAcls.FilterAcl(Identity, PermissionIdentifiers.Request.ViewStatus);

            var responseReferences = await (from rri in DataContext.Responses
                                            let canViewResults = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID).Select(a => a.Allowed)
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewResults.ID && a.UserID == Identity.ID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID).Select(a => a.Allowed))
                                                                .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.ProjectID == rri.RequestDataMart.Request.ProjectID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                .Concat(globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.SeeRequests.ID).Select(a => a.Allowed))

                                            let canViewStatus = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID).Select(a => a.Allowed)
                                                                          .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                          .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                                                          .Concat(projectOrgAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.Organization.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Organization.DataMarts.Any(dm => dm.ID == rri.RequestDataMart.DataMartID) && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                          .Concat(userAcls.Where(a => a.PermissionID == PermissionIdentifiers.Request.ViewStatus.ID && a.UserID == Identity.ID).Select(a => a.Allowed))

                                            let canApprove = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID).Select(a => a.Allowed)
                                                                       .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                       .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == rri.RequestDataMart.RequestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == rri.RequestDataMart.RequestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.ApproveResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))

                                            let canGroup = globalAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID).Select(a => a.Allowed)
                                                                       .Concat(projectAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID)).Select(a => a.Allowed))
                                                                       .Concat(projectDataMartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.Project.Requests.Any(r => r.ID == rri.RequestDataMart.RequestID) && a.Project.DataMarts.Any(dm => dm.DataMartID == rri.RequestDataMart.DataMartID) && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID && r.RequestID == rri.RequestDataMart.RequestID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(datamartAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.DataMart.Requests.Any(r => r.ID == rri.RequestDataMartID) && a.DataMartID == rri.RequestDataMart.DataMartID).Select(a => a.Allowed))
                                                                       .Concat(organizationAcls.Where(a => a.PermissionID == PermissionIdentifiers.DataMartInProject.GroupResponses.ID && a.OrganizationID == rri.RequestDataMart.Request.OrganizationID).Select(a => a.Allowed))
                                            where responseID.Contains(rri.ID)
                                            && (
                                                (
                                                    //the user can group
                                                    (canGroup.Any() && canGroup.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                    //the user can view status
                                                    //If they created or submitted the request, then they can view the status.
                                                    rri.RequestDataMart.Request.CreatedByID == Identity.ID ||
                                                    rri.RequestDataMart.Request.SubmittedByID == Identity.ID ||
                                                    (canViewStatus.Any() && canViewStatus.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                    (canViewResults.Any() && canViewResults.All(a => a) && rri.RequestDataMart.Status != RoutingStatus.ResponseRejectedAfterUpload && rri.RequestDataMart.Status != RoutingStatus.AwaitingResponseApproval) ||
                                                    //the user can approve
                                                    (canApprove.Any() && canApprove.All(a => a))
                                                 )
                                                 || ((rri.RequestDataMart.Request.CreatedByID == Identity.ID || rri.RequestDataMart.Request.SubmittedByID == Identity.ID) && (rri.RequestDataMart.Status == DTO.Enums.RoutingStatus.Completed || rri.RequestDataMart.Status == RoutingStatus.ResultsModified))
                                            )
                                            select new
                                            {
                                                ResponseID = rri.ID,
                                                ResponseGroupName = rri.ResponseGroup.Name,
                                                ResponseGroupID = rri.ResponseGroupID,
                                                Documents = DataContext.Documents.Where(d => d.ItemID == rri.ID && d.Name == "response.json").Select(d => d.ID)
                                            }).ToArrayAsync();

            var serializationSettings = new Newtonsoft.Json.JsonSerializerSettings();
            serializationSettings.Converters.Add(new DTO.QueryComposer.QueryComposerResponsePropertyDefinitionConverter());
            var deserializer = Newtonsoft.Json.JsonSerializer.Create(serializationSettings);

            Type queryComposerResponseDTOType = typeof(DTO.QueryComposer.QueryComposerResponseDTO);
            List<DTO.QueryComposer.QueryComposerResponseDTO> results = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

            if (view == TaskItemTypes.AggregateResponse)
            {
                var resultsToAggregate = responseReferences.SelectMany(r =>
                                                            {

                                                                List<DTO.QueryComposer.QueryComposerResponseDTO> l = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

                                                                foreach (var documentID in r.Documents)
                                                                {
                                                                    using (var documentStream = new Data.Documents.DocumentStream(DataContext, documentID))
                                                                    using (var streamReader = new System.IO.StreamReader(documentStream))
                                                                    {
                                                                        DTO.QueryComposer.QueryComposerResponseDTO rsp = (DTO.QueryComposer.QueryComposerResponseDTO)deserializer.Deserialize(streamReader, queryComposerResponseDTOType);
                                                                        rsp.ID = r.ResponseID;
                                                                        rsp.RequestID = requestID;
                                                                        l.Add(rsp);
                                                                    }
                                                                }

                                                                return l;
                                                            }).ToList();

                if (resultsToAggregate.Count > 0)
                    results.Add(AggregateResults(resultsToAggregate, requestID));
            }
            else
            {
                var virtualResponses = responseReferences.GroupBy(g => g.ResponseGroupID).ToArray();
                //non-grouped results will be be grouped into key 'null'

                foreach (var vr in virtualResponses)
                {
                    List<DTO.QueryComposer.QueryComposerResponseDTO> vresults = new List<DTO.QueryComposer.QueryComposerResponseDTO>();

                    foreach (var vrsp in vr)
                    {
                        foreach (Guid documentID in vrsp.Documents)
                        {
                            using (var documentStream = new Data.Documents.DocumentStream(DataContext, documentID))
                            using (var streamReader = new System.IO.StreamReader(documentStream))
                            {
                                DTO.QueryComposer.QueryComposerResponseDTO rsp = (DTO.QueryComposer.QueryComposerResponseDTO)deserializer.Deserialize(streamReader, queryComposerResponseDTOType);
                                rsp.ID = vrsp.ResponseID;
                                rsp.RequestID = requestID;
                                if (!rsp.Aggregation.IsNull())
                                    rsp.Aggregation.Name = vr.Select(r => r.ResponseGroupName).FirstOrDefault();
                                vresults.Add(rsp);
                            }
                        }

                    }

                    bool isGrouped = vr.Where(v => !v.ResponseGroupID.IsNull()).Count() > 0;
                    if (!isGrouped)
                        results.AddRange(vresults);
                    else
                        results.Add(AggregateResults(vresults, requestID));
                }

            }

            return results;
        }



        /// <summary>
        /// Gets the response groups for the specified responses.
        /// </summary>
        /// <param name="responseIDs">The IDs of the responses to get the response groups for.</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<ResponseGroupDTO> GetResponseGroups(IEnumerable<Guid> responseIDs)
        {
            var results = from rg in DataContext.ResponseGroups
                          where responseIDs.Any(r => rg.Responses.Any(rr => rr.ID == r))
                          select rg;

            return results.Map<ResponseGroup, ResponseGroupDTO>();
        }
        /// <summary>
        /// Get the response groups for the specified request.
        /// </summary>
        /// <param name="requestID">The ID of the request to get the response groups for.</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<ResponseGroupDTO> GetResponseGroupsByRequestID(Guid requestID)
        {
            var results = from rg in DataContext.ResponseGroups
                          where DataContext.Responses.Any(r => r.RequestDataMart.RequestID == requestID && r.Count == r.RequestDataMart.Responses.Max(rsp => rsp.Count) && r.ResponseGroupID == rg.ID)
                          select rg;

            return results.Map<ResponseGroup, ResponseGroupDTO>();
        }

        /// <summary>
        /// Exports the specified responses in the indicated format.
        /// </summary>
        /// <param name="id">The collection of responses ids.</param>
        /// <param name="view">The response view type: Individual, Aggregate, etc..</param>
        /// <param name="format">The format to export: csv, or xlsx.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Export([FromUri]IEnumerable<Guid> id, TaskItemTypes view, string format)
        {
            DTO.QueryComposer.QueryComposerResponseDTO[] requestResponses = (await GetWorkflowResponseContent(id, view)).ToArray();

            //all the view response permissions have been applied when getting the response content, lets not duplicate when getting the response details and document details.
            var responseIDs = requestResponses.Where(r => r.ID.HasValue).Select(r => r.ID.Value).ToArray();
            Guid requestID = requestResponses[0].RequestID;

            var dataSourceName = await (from rsp in DataContext.Responses
                                        let groups = rsp.ResponseGroup
                                        let datamart = rsp.RequestDataMart.DataMart
                                        where responseIDs.Contains(rsp.ID)
                                        select new { ResponseID = rsp.ID, Title = groups != null ? groups.Name : datamart.Name }).ToArrayAsync();


            string filename = (DataContext.Requests.Where(r => r.ID == requestID).Select(r => r.Name).FirstOrDefault() ?? "response");
            char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            filename = string.Join("", filename.Select(c => invalidFileNameChars.Contains(c) ? '_' : c).ToArray());

            HttpResponseMessage result2 = new HttpResponseMessage(HttpStatusCode.OK);

            #region csv
            if (format.ToLower() == "csv")
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter writer = new StreamWriter(ms);

                for (int i = 0; i < requestResponses.Length; i++)
                {
                    DTO.QueryComposer.QueryComposerResponseDTO response = requestResponses[i];

                    var datamartName = dataSourceName.Where(ds => ds.ResponseID == response.ID.Value).Select(ds => ds.Title).FirstOrDefault();
                    var tableName = datamartName;
                    if (response.Aggregation != null)
                    {
                        if (response.Aggregation.Name != null)
                        {
                            tableName = response.Aggregation.Name;
                        }
                    }

                    List<string> rowValues = new List<string>();
                    if (i == 0)
                    {
                        if (!tableName.IsNullOrEmpty() && !tableName.IsNullOrWhiteSpace() && view != TaskItemTypes.AggregateResponse)
                        {
                            rowValues.Add("DataMart");
                        }


                        foreach (var table in response.Results)
                        {
                            if (table.Any())
                            {
                                rowValues.AddRange(table.First().Keys.Select(k => EscapeForCsv(k)));
                                if (rowValues.Contains("LowThreshold"))
                                {
                                    rowValues.Remove("LowThreshold");
                                }
                            }
                        }

                        writer.WriteLine(string.Join(",", rowValues.ToArray()));
                    }

                    foreach (var table in response.Results)
                    {

                        foreach (var row in table)
                        {
                            rowValues.Clear();

                            if (!tableName.IsNullOrEmpty() && !tableName.IsNullOrWhiteSpace() && view != TaskItemTypes.AggregateResponse)
                            {
                                rowValues.Add(tableName);
                            }
                            if (row.ContainsKey("LowThreshold"))
                            {
                                row.Remove("LowThreshold");
                            }
                            rowValues.AddRange(row.Select(k => EscapeForCsv(k.Value.ToStringEx())).ToArray());

                            writer.WriteLine(string.Join(",", rowValues.ToArray()));
                        }

                    }

                }

                writer.Flush();
                ms.Position = 0;

                result2.Content = new StreamContent(ms);
                result2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result2.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.{1}", filename, format.ToLower())
                };

            }
            #endregion csv

            else if (format.ToLower() == "xlsx")
            {               

                MemoryStream stream = new MemoryStream();
                using (SpreadsheetDocument s = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    //Each response will be on a separate worksheet, default name of the worksheet will be the datamart/group falling back to indexed "Sheet {X}" format
                    WorkbookPart workbookPart = s.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    s.WorkbookPart.Workbook.Sheets = new Sheets();
                    Sheets sheets = s.WorkbookPart.Workbook.GetFirstChild<Sheets>();

                    for (uint sheetID = 1; sheetID <= requestResponses.Length; sheetID++)
                    {
                        var response = requestResponses[sheetID - 1];

                        string responseSourceName = dataSourceName.Where(t => t.ResponseID == response.ID).Select(t => t.Title).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(responseSourceName))
                        {
                            var aggregationDefinition = requestResponses[sheetID - 1].Aggregation;
                            if (aggregationDefinition != null && !string.IsNullOrWhiteSpace(aggregationDefinition.Name))
                                responseSourceName = aggregationDefinition.Name;
                        }

                        //responseSourceName = string.Empty;
                        //Max length for a worksheet name is 31 characters.
                        responseSourceName = (string.IsNullOrWhiteSpace(responseSourceName) ? "Sheet " + sheetID : responseSourceName).Trim().MaxLength(30);

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        
                        Sheet sheet = new Sheet() { Id = s.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = sheetID, Name = responseSourceName };
                        sheets.Append(sheet);

                        SheetData sheetData = new SheetData();
                        worksheetPart.Worksheet = new Worksheet(sheetData);

                        int totalResultSets = response.Results.Count();
                        int resultSetIndex = 0;
                        foreach (var table in response.Results)
                        {
                            //foreach resultset create a header row, each set of results for a datamart/grouping will be on the same sheet

                            Row headerRow = new Row();
                            if (requestResponses.Length == 1 && !string.IsNullOrWhiteSpace(responseSourceName) && view != TaskItemTypes.AggregateResponse)
                            {
                                headerRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue("DataMart") });
                            }

                            var firstRow = table.ElementAt(0);
                            foreach (var columnName in firstRow.Keys)
                            {
                                if (columnName != "LowThreshold")
                                {
                                    headerRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(columnName) });
                                }
                            }
                            sheetData.AppendChild(headerRow);

                            Row dataRow;
                            foreach (var row in table)
                            {
                                dataRow = new Row();
                                if (requestResponses.Length == 1 && !string.IsNullOrWhiteSpace(responseSourceName) && view != TaskItemTypes.AggregateResponse)
                                {
                                    dataRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(responseSourceName) });
                                }

                                foreach (var column in row)
                                {
                                    if (column.Key != "LowThreshold")
                                    {
                                        dataRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(column.Value.ToStringEx()) });
                                    }
                                }

                                sheetData.AppendChild(dataRow);
                            }

                            resultSetIndex++;

                            if (resultSetIndex < totalResultSets)
                            {
                                //add an empty row between resultsets
                                var emptyRow = new Row();
                                emptyRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue("") });
                                sheetData.AppendChild(emptyRow);
                            }
                        }

                        worksheetPart.Worksheet.Save();

                    }

                    workbookPart.Workbook.Save();
                    s.Close();
                }

                stream.Position = 0;
                result2.Content = new StreamContent(stream);
                result2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result2.Content.Headers.ContentLength = stream.Length;
                result2.Content.Headers.Expires = DateTimeOffset.UtcNow;
                result2.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.{1}", filename, format.ToLower())
                };


            }

            return result2;
        }

        /// <summary>
        /// Exports the selected responses for the request.
        /// </summary>
        /// <param name="requestID"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> ExportResponse(Guid requestID, string format)
        {

            DTO.QueryComposer.QueryComposerResponseDTO[] requestResponses = (await GetResponseContentForWorkflowRequest(requestID)).ToArray();

            var responseDataMarts = await GetForWorkflowRequest(requestID);

            string filename = (DataContext.Requests.Where(r => r.ID == requestID).Select(r => r.Name).FirstOrDefault() ?? "response");
            char[] invalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
            filename = string.Join("", filename.Select(c => invalidFileNameChars.Contains(c) ? '_' : c).ToArray());

            HttpResponseMessage result2 = new HttpResponseMessage(HttpStatusCode.OK);

            #region csv
            if (format.ToLower() == "csv")
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter writer = new StreamWriter(ms);

                for (int i = 0; i < requestResponses.Length; i++)
                {
                    DTO.QueryComposer.QueryComposerResponseDTO response = requestResponses[i];
                    var datamartName = (from dm in responseDataMarts.Documents where dm.ItemID == response.ID select dm.ItemTitle).FirstOrDefault().ToStringEx();
                    var tableName = datamartName;
                    if (response.Aggregation != null)
                    {
                        if (response.Aggregation.Name != null)
                        {
                            tableName = response.Aggregation.Name;
                        }
                    }

                    List<string> rowValues = new List<string>();
                    if (i == 0)
                    {
                        if (!tableName.IsNullOrEmpty() && !tableName.IsNullOrWhiteSpace())
                        {
                            rowValues.Add("DataMart");
                        }
                        

                        foreach (var table in response.Results)
                        {
                            if (table.Any())
                            {
                                rowValues.AddRange(table.First().Keys.Select(k => EscapeForCsv(k)));
                                if (rowValues.Contains("LowThreshold")) 
                                { 
                                    rowValues.Remove("LowThreshold");
                                }
                            }
                        }

                        writer.WriteLine(string.Join(",", rowValues.ToArray()));
                    }

                    foreach (var table in response.Results)
                    {

                        foreach (var row in table)
                        {
                            rowValues.Clear();

                            if (!tableName.IsNullOrEmpty() && !tableName.IsNullOrWhiteSpace())
                            {
                                rowValues.Add(tableName);
                            }
                            if (row.ContainsKey("LowThreshold"))
                            {
                                row.Remove("LowThreshold");
                            }
                            rowValues.AddRange(row.Select(k => EscapeForCsv(k.Value.ToStringEx())).ToArray());

                            writer.WriteLine(string.Join(",", rowValues.ToArray()));
                        }

                    }

                }

                writer.Flush();
                ms.Position = 0;

                result2.Content = new StreamContent(ms);
                result2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result2.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.{1}", filename, format.ToLower())
                };

            }
            #endregion csv

            else if (format.ToLower() == "xlsx")
            {
                IEnumerable<Guid> responseIDs = requestResponses.Where(r => r.ID.HasValue).Select(r => r.ID.Value);
                var sheetTitles = await (from rsp in DataContext.Responses
                                         let groups = rsp.ResponseGroup
                                         let datamart = rsp.RequestDataMart.DataMart
                                         where responseIDs.Contains(rsp.ID)
                                         select new { ResponseID = rsp.ID, Title = groups != null ? groups.Name : datamart.Name }).ToArrayAsync();

                MemoryStream stream = new MemoryStream();
                using (SpreadsheetDocument s = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    //Each response will be on a separate worksheet, default name of the worksheet will be the datamart/group falling back to indexed "Sheet {X}" format
                    WorkbookPart workbookPart = s.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    s.WorkbookPart.Workbook.Sheets = new Sheets();
                    Sheets sheets = s.WorkbookPart.Workbook.GetFirstChild<Sheets>();

                    for (uint sheetID = 1; sheetID <= requestResponses.Length; sheetID++)
                    {
                        var response = requestResponses[sheetID - 1];

                        string responseSourceName = sheetTitles.Where(t => t.ResponseID == response.ID).Select(t => t.Title).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(responseSourceName))
                        {
                            var aggregationDefinition = requestResponses[sheetID - 1].Aggregation;
                            if (aggregationDefinition != null && !string.IsNullOrWhiteSpace(aggregationDefinition.Name))
                                responseSourceName = aggregationDefinition.Name;
                        }

                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        Sheet sheet = new Sheet() { Id = s.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = sheetID, Name = string.IsNullOrWhiteSpace(responseSourceName) ? "Sheet " + sheetID : responseSourceName };
                        sheets.Append(sheet);

                        SheetData sheetData = new SheetData();
                        worksheetPart.Worksheet = new Worksheet(sheetData);


                        int totalResultSets = response.Results.Count();
                        int resultSetIndex = 0;
                        foreach (var table in response.Results)
                        {
                            //foreach resultset create a header row, each set of results for a datamart/grouping will be on the same sheet

                            Row headerRow = new Row();
                            if (requestResponses.Length == 1 && !string.IsNullOrWhiteSpace(responseSourceName))
                            {
                                headerRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue("DataMart") });
                            }

                            var firstRow = table.ElementAt(0);
                            foreach (var columnName in firstRow.Keys)
                            {
                                if (columnName != "LowThreshold")
                                {
                                    headerRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(columnName) });
                                }
                            }
                            sheetData.AppendChild(headerRow);

                            Row dataRow;
                            foreach (var row in table)
                            {
                                dataRow = new Row();
                                if (requestResponses.Length == 1 && !string.IsNullOrWhiteSpace(responseSourceName))
                                {
                                    dataRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(responseSourceName) });
                                }

                                foreach (var column in row)
                                {
                                    if (column.Key != "LowThreshold")
                                    {
                                        dataRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue(column.Value.ToStringEx()) });
                                    }
                                }

                                sheetData.AppendChild(dataRow);
                            }

                            resultSetIndex++;

                            if (resultSetIndex < totalResultSets)
                            {
                                //add an empty row between resultsets
                                var emptyRow = new Row();
                                emptyRow.AppendChild(new Cell { DataType = CellValues.String, CellValue = new CellValue("") });
                                sheetData.AppendChild(emptyRow);
                            }
                        }

                        worksheetPart.Worksheet.Save();

                    }

                    workbookPart.Workbook.Save();
                    s.Close();
                }

                stream.Position = 0;
                result2.Content = new StreamContent(stream);
                result2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result2.Content.Headers.ContentLength = stream.Length;
                result2.Content.Headers.Expires = DateTimeOffset.UtcNow;
                result2.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.{1}", filename, format.ToLower())
                };


            }

            return result2;

        }

        static string EscapeForCsv(string value)
        {
            if (value.IsNull())
                value = string.Empty;

            //http://tools.ietf.org/html/rfc4180

            char[] escapeValues = new[] { ',', '"' };
            if (value.Contains(Environment.NewLine) || value.Any(c => escapeValues.Contains(c)))
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
    }
}
