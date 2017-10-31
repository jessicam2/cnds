using Lpp.Utilities;
using Lpp.Dns.Data;
using Lpp.Utilities.Security;
using Lpp.Workflow.Engine;
using Lpp.Workflow.Engine.Interfaces;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Dns.DTO.Security;
using Lpp.Utilities.Logging;
using Newtonsoft.Json;
//using Lpp.Dns.DTO.Enums;

namespace Lpp.Dns.Workflow.Default.Activities
{
    public class NewRequest : ActivityBase<Request>
    {
        private Guid SubmitResultID = new Guid("48B20001-BD0B-425D-8D49-A3B5015A2258");
        private Guid ReviewResultID = new Guid("C4FB25F8-8521-427E-8FB1-78A84311BF1C");
        private Guid DeleteResultID = new Guid("61110001-1708-4869-BDCF-A3B600E24AA3");
        private Guid SaveResultID = new Guid("DFF3000B-B076-4D07-8D83-05EDE3636F4D");

        private const string DocumentKind = "Lpp.Dns.Workflow.Default.Activities.Request";
        
        public override Guid ID
        {
            get
            {
                return DefaultWorkflowConfiguration.NewRequestActivityID;
            }

        }

        public override string ActivityName
        {
            get
            {
                return "Draft";
            }
        }

        public override string Uri
        {
            get { return "/requests/details?ID=" + _entity.ID; }
        }

        public override string CustomTaskSubject
        {
            get
            {
                return "Compose Request";
            }
        }

        public override async Task<ValidationResult> Validate(Guid? activityResultID)
        {
            var permissions = await db.GetGrantedWorkflowActivityPermissionsForRequestAsync(_workflow.Identity, _entity);

            //default to save result ID if not specified
            if (!activityResultID.HasValue)
                activityResultID = SaveResultID;

            if (activityResultID.Value == SaveResultID && !permissions.Contains(PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.EditTask))
            {
                return new ValidationResult
                {
                    Success = false,
                    Errors = CommonMessages.RequirePermissionToEditTask
                };
            }
            else if (activityResultID.Value == SubmitResultID && !permissions.Contains(PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.CloseTask))
            {
                return new ValidationResult
                {
                    Success = false,
                    Errors = CommonMessages.RequirePermissionToCloseTask
                };
            }
            else if (activityResultID.Value == DeleteResultID && !permissions.Contains(PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.TerminateWorkflow))
            {
                return new ValidationResult
                {
                    Success = false,
                    Errors = CommonMessages.RequirePermissionToTerminateWorkflow
                };
            }

            var errors = new StringBuilder();
            if (activityResultID.Value == SubmitResultID) {
                if (_entity.ProjectID == null)
                    errors.AppendHtmlLine("Please ensure that you have selected a project for the request.");

                if (_entity.DueDate.HasValue && _entity.DueDate.Value < DateTime.UtcNow)
                    errors.AppendHtmlLine("The Request Due Date must be set in the future.");

                var dataMartsDueDate = false;
                foreach (var dm in _entity.DataMarts)
                {
                    if (dm.DueDate.HasValue && dm.DueDate.Value < DateTime.UtcNow)
                        dataMartsDueDate = true;
                }
                if (dataMartsDueDate)
                    errors.AppendHtmlLine("The Request's DataMart Due Dates must be set in the future.");

                if (_entity.SubmittedOn.HasValue)
                    errors.AppendHtmlLine("Cannot submit a request that has already been submitted");

                if (_entity.Template)
                    errors.AppendHtmlLine("Cannot submit a request template");

                if (_entity.Scheduled)
                    errors.AppendHtmlLine("Cannot submit a scheduled request");
               
                await db.LoadReference(_entity, (r) => r.Project);
                await db.LoadCollection(_entity, (r) => r.DataMarts);

                //If a project loaded successfully check it.
                if (_entity.Project != null)
                {
                    if (!_entity.Project.Active || _entity.Project.Deleted)
                        errors.AppendHtmlLine("Cannot submit a request for an inactive or deleted project.");

                    if (_entity.Project.EndDate < DateTime.UtcNow)
                        errors.AppendHtmlLine("Cannot submit a request for a project that has ended.");

                    await db.LoadCollection(_entity.Project, (p) => p.DataMarts);

                    if (_entity.DataMarts.Where(dm => dm.RoutingType == null || (dm.RoutingType != DTO.Enums.RoutingType.SourceCNDS && dm.RoutingType != DTO.Enums.RoutingType.ExternalCNDS)).Any(dm => !_entity.Project.DataMarts.Any(pdm => pdm.DataMartID == dm.DataMartID)))
                        errors.AppendHtmlLine("The request contains datamarts that are not part of the project specified and thus cannot be processed. Please remove these datamarts and try again.");
                }

                
                var dataMarts = _entity.GetGrantedDataMarts(db, _workflow.Identity);

                if (_entity.DataMarts.Where(dm => dm.RoutingType == null || (dm.RoutingType != DTO.Enums.RoutingType.SourceCNDS && dm.RoutingType != DTO.Enums.RoutingType.ExternalCNDS)).Any(dm => !dataMarts.Any(gdm => gdm.ID == dm.DataMartID)))
                    errors.AppendHtmlLine("This request contains datamarts you are not permitted to submit to. Please remove them and try again.");


                var filters = new ExtendedQuery
                {
                    Projects = (a) => a.ProjectID == _entity.ProjectID,
                    ProjectOrganizations = a => a.ProjectID == _entity.ProjectID && a.OrganizationID == _entity.OrganizationID,
                    Organizations = a => a.OrganizationID == _entity.OrganizationID,
                    Users = a => a.UserID == _entity.CreatedByID
                };

                if (_entity.DataMarts.Count < 2)
                {
                    var skip2DataMartRulePerms = await db.HasGrantedPermissions<Request>(_workflow.Identity, _entity, filters, PermissionIdentifiers.Portal.SkipTwoDataMartRule);

                    if (!skip2DataMartRulePerms.Contains(PermissionIdentifiers.Portal.SkipTwoDataMartRule))
                        errors.AppendHtmlLine("Cannot submit a request with less than 2 datamarts");
                }
           }


            if (errors.Length > 0)
            {
                return new ValidationResult
                {
                    Success = false,
                    Errors = errors.ToString()
                };
            }
            else
            {
                return new ValidationResult
                {
                    Success = true
                };
            }
        }

        public override async Task<CompletionResult> Complete(string data, Guid? activityResultID)
        {
            //default to SaveResultID if resultID not specified
            if(!activityResultID.HasValue)
                activityResultID = SaveResultID;            

            var task = await PmnTask.GetActiveTaskForRequestActivityAsync(_entity.ID, ID, db);

            if (activityResultID.Value == SubmitResultID) //Submit
            {

                var filters = new ExtendedQuery
                {
                    Projects = (a) => a.ProjectID == _entity.ProjectID,
                    ProjectOrganizations = a => a.ProjectID == _entity.ProjectID && a.OrganizationID == _entity.OrganizationID,
                    Organizations = a => a.OrganizationID == _entity.OrganizationID,
                    Users = a => a.UserID == _entity.CreatedByID
                };

                var permissions = await db.HasGrantedPermissions<Request>(_workflow.Identity, _entity, filters, PermissionIdentifiers.Request.SkipSubmissionApproval);
                await db.Entry(_entity).ReloadAsync();

                if (Newtonsoft.Json.JsonConvert.DeserializeObject<Lpp.Dns.DTO.QueryComposer.QueryComposerRequestDTO>(_entity.Query).Where.Criteria.Any(c => c.Terms.Any(t => t.Type.ToString().ToUpper() == "2F60504D-9B2F-4DB1-A961-6390117D3CAC") || c.Criteria.Any(ic => ic.Terms.Any(t => t.Type.ToString().ToUpper() == "2F60504D-9B2F-4DB1-A961-6390117D3CAC"))))
                {
                    await db.LoadCollection(_entity, (r) => r.DataMarts);

                    if (!_entity.DataMarts.Any())
                        throw new Exception("At least one routing needs to be specified when submitting a requests.");

                    //prepare the request documents, save created documents same as legacy
                    IList<Guid> documentRevisionSets = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<Guid>>(data);

                    IEnumerable<Document> documents = await (from d in db.Documents.AsNoTracking()
                                                             join x in (
                                                                 db.Documents.Where(dd => documentRevisionSets.Contains(dd.RevisionSetID.Value))
                                                                 .GroupBy(k => k.RevisionSetID)
                                                                 .Select(k => k.OrderByDescending(d => d.MajorVersion).ThenByDescending(d => d.MinorVersion).ThenByDescending(d => d.BuildVersion).ThenByDescending(d => d.RevisionVersion).Select(y => y.ID).Distinct().FirstOrDefault())
                                                             ) on d.ID equals x
                                                             orderby d.ItemID descending, d.RevisionSetID descending, d.CreatedOn descending
                                                             select d).ToArrayAsync();

                    await db.Entry(_entity).Reference(r => r.Activity).LoadAsync();
                    await db.Entry(_entity).Reference(r => r.RequestType).LoadAsync();
                    string submitterEmail = await db.Users.Where(u => u.ID == _workflow.Identity.ID).Select(u => u.Email).SingleAsync();                    

                    //update the request
                    _entity.SubmittedByID = _workflow.Identity.ID;
                    _entity.SubmittedOn = DateTime.UtcNow;
                    _entity.AdapterPackageVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).FileVersion;
                    _entity.RejectedByID = null;
                    _entity.RejectedOn = null;
                    _entity.Private = false;

                    DTO.QueryComposer.QueryComposerRequestDTO qcRequestDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.QueryComposer.QueryComposerRequestDTO>(_entity.Query);
                    var fileUploadTerm = qcRequestDTO.Where.Criteria.SelectMany(c => c.Terms.Where(t => t.Type == FileUploadTermID)).FirstOrDefault();
                    var termValues = Newtonsoft.Json.JsonConvert.DeserializeObject<FileUploadValues>(fileUploadTerm.Values["Values"].ToString());

                    //update the request.json term value to include system generated documents revisionsetIDs
                    termValues.Documents.Clear();

                    for (int i = 0; i < documentRevisionSets.Count; i++)
                    {
                        termValues.Documents.Add(new FileUploadValues.Document { RevisionSetID = documentRevisionSets[i] });
                    }

                    fileUploadTerm.Values["Values"] = termValues;
                    _entity.Query = Newtonsoft.Json.JsonConvert.SerializeObject(qcRequestDTO);

                    //save the changes to the request now since the trigger for routings will change the status invalidating the object before save
                    await db.SaveChangesAsync();
                    await db.Entry(_entity).ReloadAsync();

                    var originalStatus = _entity.Status;

                    foreach (var dm in _entity.DataMarts.Where(dm => dm.Status == 0 || dm.Status == DTO.Enums.RoutingStatus.AwaitingRequestApproval || dm.Status == DTO.Enums.RoutingStatus.Draft))
                    {
                        dm.Status = DTO.Enums.RoutingStatus.Submitted;

                        var currentResponse = db.Responses.FirstOrDefault(r => r.RequestDataMartID == dm.ID && r.Count == r.RequestDataMart.Responses.Max(rr => rr.Count));
                        if (currentResponse == null)
                        {
                            currentResponse = db.Responses.Add(new Response { RequestDataMartID = dm.ID });
                        }
                        currentResponse.SubmittedByID = _workflow.Identity.ID;
                        currentResponse.SubmittedOn = DateTime.UtcNow;

                        //add the request document associations
                        for (int i = 0; i < documentRevisionSets.Count; i++)
                        {
                            db.RequestDocuments.Add(new RequestDocument { RevisionSetID = documentRevisionSets[i], ResponseID = currentResponse.ID, DocumentType = DTO.Enums.RequestDocumentType.Input });
                        }
                    }

                    foreach (var dm in _entity.DataMarts.Where(d => d.RoutingType == DTO.Enums.RoutingType.SourceCNDS || d.RoutingType == DTO.Enums.RoutingType.ExternalCNDS))
                    {
                        //default submit to CNDS will be awaiting request approval
                        dm.Status = DTO.Enums.RoutingStatus.AwaitingRequestApproval;
                    }

                    _entity.Status = DTO.Enums.RequestStatuses.Submitted;

                    await db.SaveChangesAsync();

                    //reload the request since altering the routings triggers a change of the request status in the db by a trigger.
                    await db.Entry(_entity).ReloadAsync();

                    await SetRequestStatus(DTO.Enums.RequestStatuses.Submitted, false);

                    await NotifyRequestStatusChanged(originalStatus, DTO.Enums.RequestStatuses.Submitted);
                }
                else
                {

                    //var parentDocument = db.Documents.FirstOrDefault(d => d.ItemID == task.ID && d.Kind == DocumentKind && d.ParentDocumentID == null);

                    Document parentDocument = await (from d in db.Documents.AsNoTracking()
                                                     join x in (
                                                         db.Documents.Where(dd => dd.ItemID == task.ID && dd.FileName == "request.json")
                                                         .GroupBy(k => k.RevisionSetID)
                                                         .Select(k => k.OrderByDescending(d => d.MajorVersion).ThenByDescending(d => d.MinorVersion).ThenByDescending(d => d.BuildVersion).ThenByDescending(d => d.RevisionVersion).Select(y => y.ID).Distinct().FirstOrDefault())
                                                     ) on d.ID equals x
                                                     orderby d.ItemID descending, d.RevisionSetID descending, d.CreatedOn descending
                                                     select d).FirstOrDefaultAsync();





                    byte[] documentContent = System.Text.UTF8Encoding.UTF8.GetBytes(_entity.Query ?? string.Empty);
                    var document = new Document
                    {
                        Name = "Request Criteria",
                        MajorVersion = parentDocument == null ? 1 : parentDocument.MajorVersion,
                        MinorVersion = parentDocument == null ? 0 : parentDocument.MinorVersion,
                        RevisionVersion = parentDocument == null ? 0 : parentDocument.RevisionVersion,
                        MimeType = "application/json",
                        Viewable = false,
                        UploadedByID = _workflow.Identity.ID,
                        FileName = "request.json",
                        CreatedOn = DateTime.UtcNow,
                        BuildVersion = parentDocument == null ? 0 : parentDocument.BuildVersion,
                        ParentDocumentID = parentDocument == null ? (Guid?)null : parentDocument.ID,
                        ItemID = task.ID,
                        Length = documentContent.LongLength,
                        Kind = Dns.DTO.Enums.DocumentKind.Request
                    };

                    db.Documents.Add(document);
                    document.RevisionSetID = parentDocument == null ? document.ID : parentDocument.RevisionSetID;
                    await db.SaveChangesAsync();

                    document.SetData(db, documentContent);


                    //await db.SaveChangesAsync();

                    await db.Entry(_entity).ReloadAsync();

                    _entity.SubmittedByID = _workflow.Identity.ID;
                    _entity.SubmittedOn = DateTime.UtcNow;
                    _entity.AdapterPackageVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(this.GetType().Assembly.Location).FileVersion;
                    //Reset reject for resubmit.
                    _entity.RejectedByID = null;
                    _entity.RejectedOn = null;
                    _entity.Private = false;

                    await db.SaveChangesAsync();

                    DTO.Enums.RequestStatuses newRequestStatus = DTO.Enums.RequestStatuses.AwaitingRequestApproval;

                    await db.LoadCollection(_entity, (r) => r.DataMarts);

                    if (permissions.Contains(PermissionIdentifiers.Request.SkipSubmissionApproval))
                    {
                        newRequestStatus = DTO.Enums.RequestStatuses.Submitted;

                        foreach (var dm in _entity.DataMarts)
                        {
                            dm.Status = DTO.Enums.RoutingStatus.Submitted;

                            var currentResponse = db.Responses.FirstOrDefault(r => r.RequestDataMartID == dm.ID && r.Count == r.RequestDataMart.Responses.Max(rr => rr.Count));
                            if (currentResponse == null)
                            {
                                currentResponse = db.Responses.Add(new Response { RequestDataMartID = dm.ID });
                            }
                            currentResponse.SubmittedByID = _workflow.Identity.ID;
                            currentResponse.SubmittedOn = DateTime.UtcNow;
                            db.RequestDocuments.Add(new RequestDocument { RevisionSetID = document.RevisionSetID.Value, ResponseID = currentResponse.ID, DocumentType = DTO.Enums.RequestDocumentType.Input });
                        }

                        foreach(var dm in _entity.DataMarts.Where(d => d.RoutingType == DTO.Enums.RoutingType.SourceCNDS || d.RoutingType == DTO.Enums.RoutingType.ExternalCNDS))
                        {
                            //default submit to CNDS will be awaiting request approval
                            dm.Status = DTO.Enums.RoutingStatus.AwaitingRequestApproval;
                        }
                    }
                    else
                    {
                        foreach (var dm in _entity.DataMarts)
                        {
                            dm.Status = DTO.Enums.RoutingStatus.AwaitingRequestApproval;
                        }
                    }

                    _entity.Status = newRequestStatus;

                    await db.SaveChangesAsync();

                    await db.Entry(_entity).ReloadAsync();
                    await SetRequestStatus(newRequestStatus);
                    
                }

                if (_entity.DataMarts.Any(dm => dm.RoutingType == DTO.Enums.RoutingType.SourceCNDS) && permissions.Contains(PermissionIdentifiers.Request.SkipSubmissionApproval))
                {
                    //register the request with CNDS for distribution
                    await SubmitCrossNetworkRequest();
                }

                await MarkTaskComplete(task);

                return new CompletionResult
                {
                    ResultID = permissions.Contains(PermissionIdentifiers.Request.SkipSubmissionApproval) ? SubmitResultID : ReviewResultID
                };

            }
            else if (activityResultID.Value == SaveResultID) //Save
            {

                if (!string.IsNullOrEmpty(_entity.Query) && Newtonsoft.Json.JsonConvert.DeserializeObject<Lpp.Dns.DTO.QueryComposer.QueryComposerRequestDTO>(_entity.Query).Where.Criteria.Any(c => c.Terms.Any(t => t.Type.ToString().ToUpper() == "2F60504D-9B2F-4DB1-A961-6390117D3CAC") || c.Criteria.Any(ic => ic.Terms.Any(t => t.Type.ToString().ToUpper() == "2F60504D-9B2F-4DB1-A961-6390117D3CAC"))))
                {
                    await db.Entry(_entity).ReloadAsync();
                    _entity.Private = false;
                    _entity.SubmittedByID = _workflow.Identity.ID;

                    //Reset reject for resubmit.
                    _entity.RejectedByID = null;
                    _entity.RejectedOn = null;

                    var originalStatus = _entity.Status;
                    await SetRequestStatus(DTO.Enums.RequestStatuses.DraftReview);

                    await NotifyRequestStatusChanged(originalStatus, DTO.Enums.RequestStatuses.DraftReview);

                    await MarkTaskComplete(task);
                }
                else
                {

                    if (_entity.Private)
                    {
                        await db.Entry(_entity).ReloadAsync();

                        _entity.Private = false;

                        await task.LogAsModifiedAsync(_workflow.Identity, db);
                        await db.SaveChangesAsync();
                    }
                }

                return new CompletionResult
                {
                    ResultID = SaveResultID
                };
                
            }
            else if (activityResultID.Value == DeleteResultID) //Delete
            {
                db.Requests.Remove(_entity);
                
                if (task != null)
                {
                    db.Actions.Remove(task);
                }

                await db.SaveChangesAsync();

                return null;
            }
            else
            {
                throw new ArgumentOutOfRangeException(CommonMessages.ActivityResultNotSupported);
            }
        }

        public static readonly Guid FileUploadTermID = new Guid("2F60504D-9B2F-4DB1-A961-6390117D3CAC");
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
