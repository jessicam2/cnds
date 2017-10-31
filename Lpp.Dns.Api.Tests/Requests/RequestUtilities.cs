using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Dns.Data;
using Lpp.Objects;
using Lpp.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lpp.Dns.Api.Tests.Requests
{
    [TestClass]
    public class RequestUtilities
    {
        static readonly log4net.ILog Logger;

        static RequestUtilities()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = log4net.LogManager.GetLogger(typeof(RequestUtilities));
        }

        [TestMethod]
        public void DeleteRequest()
        {
            string[] requests = new[] {
                ""
            };

            foreach (var requestID in requests)
            {
                Guid id = new Guid(requestID);

                Logger.Info("Deleting request ID: " + id.ToString("D"));

                using (var db = new DataContext())
                {
                    db.Database.Log = (s) => Logger.Debug(s);

                    //request logs
                    db.LogsNewRequestDraftSubmitted.RemoveRange(db.LogsNewRequestDraftSubmitted.Where(l => l.RequestID == id));
                    db.LogsNewRequestSubmitted.RemoveRange(db.LogsNewRequestSubmitted.Where(l => l.RequestID == id));
                    db.LogsRequestAssignmentChange.RemoveRange(db.LogsRequestAssignmentChange.Where(l => l.RequestID == id));
                    db.LogsRequestCommentChange.RemoveRange(db.LogsRequestCommentChange.Where(l => l.Comment.ItemID == id));
                    db.LogsRequestDataMartAddedRemoved.RemoveRange(db.LogsRequestDataMartAddedRemoved.Where(l => l.RequestDataMart.RequestID == id));
                    db.LogsRequestDataMartMetadataChange.RemoveRange(db.LogsRequestDataMartMetadataChange.Where(l => l.RequestID == id || l.RequestDataMart.RequestID == id));
                    db.LogsRequestDocumentChange.RemoveRange(db.LogsRequestDocumentChange.Where(l => l.RequestID == id));
                    db.LogsRequestMetadataChange.RemoveRange(db.LogsRequestMetadataChange.Where(l => l.RequestID == id));
                    db.LogsRequestStatusChanged.RemoveRange(db.LogsRequestStatusChanged.Where(l => l.RequestID == id));
                    db.LogsResponseViewed.RemoveRange(db.LogsResponseViewed.Where(l => l.Response.RequestDataMart.RequestID == id));
                    db.LogsResultsReminder.RemoveRange(db.LogsResultsReminder.Where(l => l.RequestID == id));
                    db.LogsRoutingStatusChange.RemoveRange(db.LogsRoutingStatusChange.Where(l => l.RequestDataMart.RequestID == id));
                    db.LogsSubmittedRequestAwaitsResponse.RemoveRange(db.LogsSubmittedRequestAwaitsResponse.Where(l => l.RequestID == id));
                    db.LogsSubmittedrequestNeedsApproval.RemoveRange(db.LogsSubmittedrequestNeedsApproval.Where(l => l.RequestID == id));
                    db.LogsTaskChange.RemoveRange(db.LogsTaskChange.Where(t => t.Task.References.Any(tr => tr.ItemID == id && tr.Type == DTO.Enums.TaskItemTypes.Request)));
                    db.LogsTaskReminder.RemoveRange(db.LogsTaskReminder.Where(t => t.Task.References.Any(tr => tr.ItemID == id && tr.Type == DTO.Enums.TaskItemTypes.Request)));
                    db.LogsUploadedResultNeedsApproval.RemoveRange(db.LogsUploadedResultNeedsApproval.Where(l => l.RequestDataMart.RequestID == id));


                    //response documents
                    db.RequestDocuments.RemoveRange(db.RequestDocuments.Where(d => d.Response.RequestDataMart.RequestID == id));

                    var documentIDs = db.Documents.Where(d => d.ItemID == id ||
                                                            db.Responses.Any(r => r.RequestDataMart.RequestID == id && d.ItemID == r.ID) ||
                                                            db.Actions.Where(t => t.References.Any(tr => tr.ItemID == id && tr.Type == DTO.Enums.TaskItemTypes.Request)).Any()
                                                        ).Select(d => d.ID);

                    db.LogsDocumentChange.RemoveRange(db.LogsDocumentChange.Where(l => documentIDs.Contains(l.DocumentID)));

                    //document comment references
                    db.CommentReferences.RemoveRange(db.CommentReferences.Where(cr => documentIDs.Contains(cr.ItemID)));

                    //documents
                    db.Documents.RemoveRange(db.Documents.Where(d => d.ItemID == id ||
                                                                     db.Responses.Any(r => r.RequestDataMart.RequestID == id && d.ItemID == r.ID) ||
                                                                     db.Actions.Where(t => t.References.Any(tr => tr.ItemID == id && tr.Type == DTO.Enums.TaskItemTypes.Request)).Any()
                                                                 )
                                                             );

                    //comments
                    db.Comments.RemoveRange(db.Comments.Where(c => c.ItemID == id));

                    //tasks
                    db.Actions.RemoveRange(db.Actions.Where(t => t.References.Any(tr => tr.ItemID == id && tr.Type == DTO.Enums.TaskItemTypes.Request)));

                    //request users
                    db.RequestUsers.RemoveRange(db.RequestUsers.Where(ru => ru.RequestID == id));

                    //request observers
                    db.RequestObserverEventSubscriptions.RemoveRange(db.RequestObserverEventSubscriptions.Where(ros => ros.RequestObserver.RequestID == id));
                    db.RequestObservers.RemoveRange(db.RequestObservers.Where(ro => ro.RequestID == id));

                    //responses
                    db.Responses.RemoveRange(db.Responses.Where(rsp => rsp.RequestDataMart.RequestID == id));

                    //routes
                    db.RequestDataMarts.RemoveRange(db.RequestDataMarts.Where(rdm => rdm.RequestID == id));

                    //request
                    db.Requests.RemoveRange(db.Requests.Where(r => r.ID == id));

                    db.SaveChanges();
                }
            }
        }



    }
}
