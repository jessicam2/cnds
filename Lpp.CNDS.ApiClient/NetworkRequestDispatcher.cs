using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient
{
    public class NetworkRequestDispatcher : IDisposable
    {
        CNDSClient _client = null;

        CNDSClient CNDSApi { get {
                if(_client == null)
                {
                    _client = new CNDSClient(System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"]);
                }
                return _client;
            } }

        public NetworkRequestDispatcher()
        {
        }

        /// <summary>
        /// Registers a request with CNDS for distribution.
        /// </summary>
        /// <returns></returns>
        public async Task RegisterRequest(Dns.DTO.RequestDTO request, Guid networkID, Dns.Data.DataContext db)
        {
            var documents = await (from rd in db.RequestDocuments
                                   join doc in db.Documents on rd.RevisionSetID equals doc.RevisionSetID
                                   where rd.Response.RequestDataMart.RequestID == request.ID.Value
                                   && doc.ID == db.Documents.Where(dd => dd.RevisionSetID == rd.RevisionSetID).OrderByDescending(dd => dd.MajorVersion).ThenByDescending(dd => dd.MinorVersion).ThenByDescending(dd => dd.BuildVersion).ThenByDescending(dd => dd.RevisionVersion).Select(dd => dd.ID).FirstOrDefault()
                                   select new
                                   {
                                       SourceRequestDataSourceID = rd.Response.RequestDataMart.ID,
                                       ResponseID = rd.ResponseID,
                                       RevisionSetID = rd.RevisionSetID,
                                       DocumentID = doc.ID,
                                       Name = doc.Name,
                                       FileName = doc.FileName,
                                       IsViewable = doc.Viewable,
                                       Kind = doc.Kind,
                                       MimeType = doc.MimeType,
                                       Length = doc.Length,
                                       Description = doc.Description
                                   }).ToArrayAsync();

            DTO.Requests.SubmitRequestDTO dto = new DTO.Requests.SubmitRequestDTO {
                SourceNetworkID = networkID,
                SourceRequestID = request.ID.Value,
                SerializedSourceRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                Documents = documents.Select(d => new DTO.Requests.SubmitRequestDocumentDetailsDTO
                {
                    SourceRequestDataSourceID = d.SourceRequestDataSourceID,
                    DocumentID = d.DocumentID,
                    RevisionSetID = d.RevisionSetID,
                    Description = d.Description,
                    FileName = d.FileName,
                    IsViewable = d.IsViewable,
                    Kind = d.Kind,
                    Length = d.Length,
                    MimeType = d.MimeType,
                    Name = d.Name
                }).ToArray()
            };

            dto.Routes = (await (db.RequestDataMarts.Where(rdm => rdm.RequestID == dto.SourceRequestID)
                .Select(rdm => new {
                    rdm.DataMartID,
                    rdm.DueDate,
                    rdm.Priority,
                    rdm.ID,
                    Responses = rdm.Responses.Select(rsp => new { rsp.ID, rsp.Count })
                }).ToArrayAsync()))
                .Select(rdm => new DTO.Requests.SubmitRouteDTO {
                                    NetworkRouteDefinitionID = rdm.DataMartID,
                                    DueDate = rdm.DueDate,
                                    Priority = (int)rdm.Priority,
                                    SourceRequestDataMartID = rdm.ID,
                                    SourceResponseID = rdm.Responses.OrderByDescending(rsp => rsp.Count).Select(rsp => rsp.ID).FirstOrDefault(),
                                    RequestDocumentIDs = documents.Where(d => d.ResponseID == rdm.Responses.OrderByDescending(rsp => rsp.Count).Select(rsp => rsp.ID).First()).Select(d => d.DocumentID)
                                }).ToArray();

            await CNDSApi.Requests.Submit(dto);
        }

        /// <summary>
        /// Updates the routing status on the source request for the specified participant routes.
        /// </summary>
        /// <param name="responseDetails"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateSourceRoutingStatus(IEnumerable<Dns.DTO.CNDSUpdateRoutingStatusDTO> responseDetails)
        {
            var dtos = responseDetails.Select(r => new DTO.Requests.SetRoutingStatusDTO { ResponseID = r.ResponseID, Message = r.Message, RoutingStatus = (int)r.RoutingStatus });
            return await CNDSApi.Requests.UpdateSourceRouting(dtos);
        }

        /// <summary>
        /// Updates the routing status on the source request for the specified participant routes.
        /// </summary>
        /// <param name="responseDetails"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateParticipateRoutingStatus(IEnumerable<Dns.DTO.CNDSUpdateRoutingStatusDTO> responseDetails)
        {
            var dtos = responseDetails.Select(r => new DTO.Requests.SetRoutingStatusDTO { ResponseID = r.ResponseID, Message = r.Message, RoutingStatus = (int)r.RoutingStatus });
            return await CNDSApi.Requests.UpdateParticipantRoutingStatus(dtos);
        }

        /// <summary>
        /// Resubmit route on the source request for the specified participant routes.
        /// </summary>
        /// <param name="responseDetails"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ResubmitRouteToSource(IEnumerable<Dns.DTO.CNDSResubmitRouteDTO> responseDetails)
        {
            var dtos = responseDetails.Select(r => new DTO.Requests.ResubmitRouteDTO { ResponseID = r.ResponseID, Message = r.Message, RequestDatamartID = r.RequestDatamartID });
            return await CNDSApi.Requests.ResubmitSourceRouting(dtos);
        }

        /// <summary>
        /// Resubmit route on the source request for the specified participant routes.
        /// </summary>
        /// <param name="responseDetails"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ResubmitRouteToParticipant(IEnumerable<Dns.DTO.CNDSResubmitRouteDTO> responseDetails)
        {
            var dtos = responseDetails.Select(r => new DTO.Requests.ResubmitRouteDTO { ResponseID = r.ResponseID, Message = r.Message, RequestDatamartID = r.RequestDatamartID });
            return await CNDSApi.Requests.ResubmitParticipateRouting(dtos);
        }

        /// <summary>
        /// Posts the response documents to the source request for the participant route.
        /// </summary>
        /// <param name="responseID"></param>
        /// <returns></returns>
        public async Task PostResponseDocuments(Guid responseID)
        {
            await CNDSApi.Requests.PostParticipantResponseDocuments(responseID);
        }

        /// <summary>
        /// Updates the Priority and Due Date on the source request for the specified participant routes.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateSourceDataMartsPriorityAndDueDate(IEnumerable<DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
        {
            return await CNDSApi.Requests.UpdateSourceDataMartsPriorityAndDueDates(dtos);
        }

        /// <summary>
        /// Updates the Priority and Due Date on the participant request for the specified source routes.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateParticipateDataMartsPriorityAndDueDate(IEnumerable<DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
        {
            return await CNDSApi.Requests.UpdatParticipantDataMartsPriorityAndDueDates(dtos);
        }

        /// <summary>
        /// Terminates the Request for the following RequestDataMartID's
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> TerminateParticipantRoutes(IEnumerable<Guid> ids)
        {
            return await CNDSApi.Requests.TerminateParticipantRoute(ids);
        }

        /// <summary>
        /// Terminates the Routes for the Following RequestDataMartID'ss
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> TerminateSourceRoutes(IEnumerable<Guid> ids)
        {
            return await CNDSApi.Requests.TerminateSourceRoute(ids);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_client != null)
                    {
                        _client.Dispose();
                        _client = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~NetworkRequestDispatcher() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
