using Lpp.Dns.Data;
using Lpp.Dns.DTO.Security;
using Lpp.Workflow.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.Dns.Workflow.DistributedRegression.Activities
{
    /// <summary>
    /// The Second Activity for the Distributed Regression Workflow.  It allows for Altering DataPartners Responding to the Request, Altering Request MetaData, Completing The Request itself,
    /// and Terminating the Request
    /// </summary>
    public class CompleteDistribution : ActivityBase<Request>
    {
        /// <summary>
        /// The Result ID passed by the User to indicate to Redistribute the Request.  This sets the Request status back to Submitted
        /// </summary>
        private static readonly Guid RedistributeResultID = new Guid("5C5E0001-10A6-4992-A8BE-A3F4012D5FEB");
        /// <summary>
        /// The Result ID passed by the User to Indicate that the Response Time has been altered.  This does not alter the Request Status or Task in any way
        /// </summary>
        private static readonly Guid BulkEditResultID = new Guid("4F7E1762-E453-4D12-8037-BAE8A95523F7");
        /// <summary>
        /// The Result ID passed by the User to Indicate that DataPartners have been added to the Request to Respond to.  This does not alter the Request Status or Task if all DataPartners 
        /// havent responded
        /// </summary>
        private static readonly Guid AddDatamartsResultID = new Guid("15BDEF13-6E86-4E0F-8790-C07AE5B798A8");
        /// <summary>
        /// The Result ID passed by the User to Indicate that DataPartners have been Removed from the Request.  This does not alter the Request Status or Task if all DataPartners 
        /// havent responded
        /// </summary>
        private static readonly Guid RemoveDatamartsResultID = new Guid("5E010001-1353-44E9-9204-A3B600E263E9");
        /// <summary>
        /// The Result ID passed by the User to Indicate that the Routing Status has been altered.  This does not alter the Request Status or Task if all the DataPartners
        /// havent responded
        /// </summary>
        private static readonly Guid CompleteRoutingResultID = new Guid("8A68399F-D562-4A98-87C9-195D3D83A103");
        /// <summary>
        /// The Result ID passed by the User to Indicate that only the Request Metadata has been altered.  This does not alter the Request Status or Task.
        /// </summary>
        private static readonly Guid SaveResultID = new Guid("DFF3000B-B076-4D07-8D83-05EDE3636F4D");
        /// <summary>
        /// The Result ID passed by the User to indicate that the Request has been Terminated.  This closes the current Task and Sets the Request Status to cancelled
        /// </summary>
        private static readonly Guid TerminateResultID = new Guid("53579F36-9D20-47D9-AC33-643D9130080B");

        /// <summary>
        /// Gets the Name of the Current Activity
        /// </summary>
        public override string ActivityName
        {
            get
            {
                return "Complete Distribution";
            }
        }

        /// <summary>
        /// The ID of the Current Activity
        /// </summary>
        public override Guid ID
        {
            get
            {
                return new Guid("D0E659B8-1155-4F44-9728-B4B6EA4D4D55");
            }
        }

        /// <summary>
        /// The URL that should be passed back to the User
        /// </summary>
        public override string Uri
        {
            get
            {
                return "requests/details?ID=" + _entity.ID;
            }
        }

        /// <summary>
        /// The String that shows in the Task Subject Window
        /// </summary>
        public override string CustomTaskSubject
        {
            get
            {
                return "Complete Distribution";
            }
        }

        /// <summary>
        /// The Method to Validate the User Permissions and to Validate the Request Information before being passed to the Complete Method
        /// </summary>
        /// <param name="activityResultID">The Result ID passed by the User to indicate which step to proceed to.</param>
        /// <returns></returns>
        public override async Task<ValidationResult> Validate(Guid? activityResultID)
        {
            if (!activityResultID.HasValue)
                activityResultID = AddDatamartsResultID;

            var permissions = await db.GetGrantedWorkflowActivityPermissionsForRequestAsync(_workflow.Identity, _entity, PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.EditTask, PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.CloseTask);

            if (!permissions.Contains(PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.EditTask) 
                && (activityResultID.Value == SaveResultID 
                || activityResultID.Value == RedistributeResultID
                || activityResultID.Value == BulkEditResultID
                || activityResultID.Value == AddDatamartsResultID
                || activityResultID.Value == RemoveDatamartsResultID
                || activityResultID.Value == CompleteRoutingResultID
                || activityResultID.Value == TerminateResultID))
            {
                return new ValidationResult { Success = false, Errors = CommonMessages.RequirePermissionToEditTask };
            }

            if (!permissions.Contains(PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.CloseTask) 
                && (activityResultID.Value == CompleteRoutingResultID
                || activityResultID.Value == TerminateResultID))
            {
                return new ValidationResult { Success = false, Errors = CommonMessages.RequirePermissionToCloseTask };
            }

            return new ValidationResult
            {
                Success = true
            };
        }

        /// <summary>
        /// The Method to do what the User decieded
        /// </summary>
        /// <param name="data">The Data payload passed by the User</param>
        /// <param name="activityResultID">The Result ID Passed by the User to indicate which step to proceed to.</param>
        /// <returns></returns>
        public override async Task<CompletionResult> Complete(string data, Guid? activityResultID)
        {
            if (!activityResultID.HasValue)
                activityResultID = SaveResultID;

            var task = PmnTask.GetActiveTaskForRequestActivity(_entity.ID, ID, db);

            if (activityResultID == SaveResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = SaveResultID
                };
            }
            else if (activityResultID == RedistributeResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = RedistributeResultID
                };
            }
            else if (activityResultID == BulkEditResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = BulkEditResultID
                };
            }
            else if (activityResultID == AddDatamartsResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = AddDatamartsResultID
                };
            }
            else if (activityResultID == RemoveDatamartsResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = RemoveDatamartsResultID
                };
            }
            else if (activityResultID == CompleteRoutingResultID)
            {
                await task.LogAsModifiedAsync(_workflow.Identity, db);
                await db.SaveChangesAsync();
                return new CompletionResult
                {
                    ResultID = CompleteRoutingResultID
                };
            }
            else if (activityResultID == TerminateResultID)
            {
                db.Requests.Remove(_entity);

                if (task != null)
                {
                    db.Actions.Remove(task);
                }

                await db.SaveChangesAsync();

                return new CompletionResult
                {
                    ResultID = TerminateResultID
                };
            }
            else
            {
                throw new NotSupportedException(CommonMessages.ActivityResultNotSupported);
            }
        }
    }
}
