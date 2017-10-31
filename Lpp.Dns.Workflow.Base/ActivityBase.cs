using Lpp.Dns.Data;
using Lpp.Dns.DTO.Security;
using Lpp.Utilities.Logging;
using Lpp.Workflow.Engine;
using Lpp.Workflow.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Objects;
using System.Data.Entity;
using Lpp.Utilities;
using System.Net.Http;

namespace Lpp.Dns.Workflow
{
    /// <summary>
    /// Base workflow activity implementation.
    /// </summary>
    /// <typeparam name="TEntity">The root entity the activity is working from.</typeparam>
    public abstract class ActivityBase<TEntity> : IActivity<DataContext, TEntity>
        where TEntity : Request
    {
        /// <summary>
        /// The workflow instance.
        /// </summary>
        protected Workflow<DataContext, TEntity> _workflow = null;
        /// <summary>
        /// The current datacontext instance.
        /// </summary>
        protected DataContext db = null;
        /// <summary>
        /// The current root entity instance.
        /// </summary>
        protected TEntity _entity = null;
        /// <summary>
        /// A workflow activity logger instance.
        /// </summary>
        protected readonly RequestLogConfiguration _requestLogger = new Lpp.Dns.Data.RequestLogConfiguration();
        /// <summary>
        /// Gets the ID of the workflow activity.
        /// </summary>
        public abstract Guid ID { get; }
        /// <summary>
        /// Gets the display name of the current activity.
        /// </summary>
        public abstract string ActivityName { get; }
        /// <summary>
        /// Gets the uri of the edit/view page for the entity.
        /// </summary>
        public abstract string Uri { get; }
        /// <summary>
        /// Gets subject to set for the activities task, by default is null which will set the subject to the Activity Name. If not null or empty the specified value will be used instead.
        /// </summary>
        public virtual string CustomTaskSubject
        {
            get { return null; }
        }
        /// <summary>
        /// Initializes the activity implementation instance.
        /// </summary>
        /// <param name="workflow"></param>
        public virtual void Initialize(Workflow<DataContext, TEntity> workflow)
        {
            this._workflow = workflow;
            this.db = (DataContext)workflow.DataContext;
            this._entity = workflow.Entity;
            
        }
        /// <summary>
        /// Performs tasks associated to the start of the activity. By default confirms that an active PmnTask has been created for the current activity, and also records any comment entered when leaving the previous activity.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public virtual async Task Start(string comment)
        {
            var task = await PmnTask.GetActiveTaskForRequestActivityAsync(_entity.ID, ID, db);
            if (task == null)
            {
                task = db.Actions.Add(PmnTask.CreateForWorkflowActivity(_entity.ID, ID, _workflow.ID, db, CustomTaskSubject));
                await db.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(comment))
            {
                var cmt = db.Comments.Add(new Comment
                {
                    CreatedByID = _workflow.Identity.ID,
                    ItemID = _entity.ID,
                    Text = comment
                });

                db.CommentReferences.Add(new CommentReference
                {
                    CommentID = cmt.ID,
                    Type = DTO.Enums.CommentItemTypes.Task,
                    ItemTitle = task.Subject,
                    ItemID = task.ID
                });

                await db.SaveChangesAsync();
            }
        }
     
        /// <summary>
        /// Returns a boolean based on user's Approve/Reject Submission rights
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        protected virtual async Task<bool> ApproveRejectSubmission()
        {
            //Locations: Global, Organizations, Projects, Users, Project Organizations
            var globalAcls = db.GlobalAcls.FilterAcl(_workflow.Identity, PermissionIdentifiers.Request.ApproveRejectSubmission);
            var organizationAcls = db.OrganizationAcls.FilterAcl(_workflow.Identity, PermissionIdentifiers.Request.ApproveRejectSubmission);
            var projectAcls = db.ProjectAcls.FilterAcl(_workflow.Identity, PermissionIdentifiers.Request.ApproveRejectSubmission);
            var userAcls = db.UserAcls.FilterAcl(_workflow.Identity, PermissionIdentifiers.Request.ApproveRejectSubmission);
            var projectOrgAcls = db.ProjectOrganizationAcls.FilterAcl(_workflow.Identity, PermissionIdentifiers.Request.ApproveRejectSubmission);


            var results = await (from r in db.Secure<Request>(_workflow.Identity)
                                 where r.ID == _entity.ID
                                 let gAcls = globalAcls
                                 let oAcls = organizationAcls.Where(a => a.OrganizationID == r.OrganizationID)
                                 let pAcls = projectAcls.Where(a => a.ProjectID == r.ProjectID)
                                 let uAcls = userAcls.Where(a => a.UserID == r.SubmittedByID)
                                 let poAcls = projectOrgAcls.Where(a => a.ProjectID == r.ProjectID && a.OrganizationID == r.OrganizationID)
                                 where (
                                   (gAcls.Any() || oAcls.Any() || pAcls.Any() || uAcls.Any() || poAcls.Any())
                                   &&
                                   (gAcls.All(a => a.Allowed) && oAcls.All(a => a.Allowed) && pAcls.All(a => a.Allowed) && uAcls.All(a => a.Allowed) && poAcls.All(a => a.Allowed))
                                 )
                                 select r.ID).AnyAsync();

            return results;
        }

        /// <summary>
        /// Performs validation prior to executing Complete.
        /// </summary>
        /// <param name="activityResultID"></param>
        /// <returns></returns>
        public abstract Task<ValidationResult> Validate(Guid? activityResultID);

        /// <summary>
        /// Executes actions based on the specified activity result.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="activityResultID"></param>
        /// <returns></returns>
        public abstract Task<CompletionResult> Complete(string data, Guid? activityResultID);

        /// <summary>
        /// Creates immediate notifications notifications and sends.
        /// </summary>
        /// <param name="logger">The workflow activity logger.</param>
        /// <param name="logs"></param>
        /// <returns></returns>
        protected async Task ProcessNotifications(IEnumerable<AuditLog> logs)
        {
            await Task.Run(() => {

                List<Notification> notifications = new List<Notification>();

                foreach (Lpp.Dns.Data.Audit.RequestStatusChangedLog logitem in logs)
                {
                    var items = _requestLogger.CreateNotifications(logitem, db, true);
                    if (items != null && items.Any())
                        notifications.AddRange(items);
                }

                if (notifications.Any())
                    _requestLogger.SendNotification(notifications);
            });
        }

        /// <summary>
        /// Marks the specified task as complete and saves.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected async Task MarkTaskComplete(PmnTask task)
        {
            task.Status = DTO.Enums.TaskStatuses.Complete;
            task.EndOn = DateTime.UtcNow;
            task.PercentComplete = 100d;

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Manually set the status of the request using a direct sql command, the status on the entity is also set and a save on the datacontext called. By default the entity is refreshed after the actions have been completed.
        /// </summary>
        /// <param name="newStatus">The status to set.</param>
        /// <param name="refreshEntity">If true the entity will be reloaded.</param>
        /// <returns></returns>
        protected async Task SetRequestStatus(DTO.Enums.RequestStatuses newStatus, bool refreshEntity = true)
        {
            //if we do not set and and save the status the entity logger will not fire based on status changes correctly
            _entity.Status = newStatus;
            await db.SaveChangesAsync();

            //manually override the request status using sql direct, EF does not allow update of computed
            await db.Database.ExecuteSqlCommandAsync("UPDATE Requests SET Status = @status WHERE ID = @ID", new System.Data.SqlClient.SqlParameter("@status", (int)newStatus), new System.Data.SqlClient.SqlParameter("@ID", _entity.ID));

            if (refreshEntity)
                await db.Entry(_entity).ReloadAsync();
        }

        /// <summary>
        /// Create and send request status change notifications.
        /// </summary>
        /// <param name="currentStatus">The original status of the request.</param>
        /// <param name="newStatus">The new status of the request.</param>
        /// <returns></returns>
        protected async Task NotifyRequestStatusChanged(DTO.Enums.RequestStatuses currentStatus, DTO.Enums.RequestStatuses newStatus)
        {
            string[] emailText = await _requestLogger.GenerateRequestStatusChangedEmailContent(db, _entity.ID, _workflow.Identity.ID, currentStatus, newStatus);

            var logItems = _requestLogger.GenerateRequestStatusEvents(db, _workflow.Identity, false, currentStatus, newStatus, _entity.ID, emailText[1], emailText[0], "Request Status Changed");
            await db.SaveChangesAsync();

            await ProcessNotifications(logItems);
        }

        /// <summary>
        /// Registers a request and any external routes with CNDS for distribution.
        /// </summary>
        /// <returns></returns>
        protected async Task SubmitCrossNetworkRequest()
        {
            Guid networkID = db.Networks.Where(n => n.Name != "Aqueduct").Select(n => n.ID).FirstOrDefault();
            using (var cnds = new Lpp.CNDS.ApiClient.NetworkRequestDispatcher())
            {
                await cnds.RegisterRequest((await db.Requests.Where(r => r.ID == _entity.ID).Map<Dns.Data.Request, Dns.DTO.RequestDTO>().ToArrayAsync()).First(), networkID, db);
            }
        }

        protected async Task<HttpResponseMessage> UpdateCrossNetworkRoutingStatusForSourceRequest(IEnumerable<DTO.CNDSUpdateRoutingStatusDTO> responseDetails)
        {
            using(var cnds = new CNDS.ApiClient.NetworkRequestDispatcher())
            {
                return await cnds.UpdateSourceRoutingStatus(responseDetails);
            }
        }

        protected async Task<HttpResponseMessage> UpdateParticipantRoutingStatusForSourceRequest(IEnumerable<DTO.CNDSUpdateRoutingStatusDTO> responseDetails)
        {
            using (var cnds = new CNDS.ApiClient.NetworkRequestDispatcher())
            {
                return await cnds.UpdateParticipateRoutingStatus(responseDetails);
            }
        }

        protected async Task<HttpResponseMessage> ResubmitRouteToSource(IEnumerable<DTO.CNDSResubmitRouteDTO> responseDetails)
        {
            using (var cnds = new CNDS.ApiClient.NetworkRequestDispatcher())
            {
                return await cnds.ResubmitRouteToSource(responseDetails);
            }
        }

        protected async Task<HttpResponseMessage> ResubmitRouteToParticipant(IEnumerable<DTO.CNDSResubmitRouteDTO> responseDetails)
        {
            using (var cnds = new CNDS.ApiClient.NetworkRequestDispatcher())
            {
                return await cnds.ResubmitRouteToParticipant(responseDetails);
            }
        }

        protected async Task PostResponseDocuments(Guid responseID)
        {
            using (var cnds = new CNDS.ApiClient.NetworkRequestDispatcher())
            {
                await cnds.PostResponseDocuments(responseID);
            }
        }
    }
}
