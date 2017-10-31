using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Lpp.CNDS.Data;
using Lpp.Utilities;
using Lpp.Utilities.WebSites.Controllers;
using System.Text;
using Newtonsoft.Json;

namespace Lpp.CNDS.Api.Requests
{
    public class RequestsController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(RequestsController));

        /// <summary>
        /// Registers a network request with CNDS, and distributes to the participating networks.
        /// </summary>
        /// <param name="dto">The details of the request.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Submit(DTO.Requests.SubmitRequestDTO dto)
        {
            //register the request with CNDS
            var request = DataContext.NetworkRequests.Add(new Data.Requests.NetworkRequest
            {
                ID = dto.SourceRequestID,
                NetworkID = dto.SourceNetworkID
            });

            await DataContext.SaveChangesAsync();

            var routeDefinitionIDs = dto.Routes.Select(rt => rt.NetworkRouteDefinitionID).ToArray();
            var routeDefinitions = await DataContext.NetworkRequestTypeDefinitions.Where(rt => routeDefinitionIDs.Contains(rt.ID)).ToArrayAsync();

            var networkIDs = (new[] { dto.SourceNetworkID }).Union(routeDefinitions.Select(rt => rt.NetworkID).Distinct());
            var networkDetails = (await DataContext.Networks.Where(n => networkIDs.Contains(n.ID)).Select(n => new { n.ID, n.Name, n.ServiceUrl, n.ServiceUserName, n.ServicePassword }).ToArrayAsync()).ToDictionary(n => n.ID);

            var datasourceIDs = routeDefinitions.Select(rt => rt.DataSourceID).Distinct();
            var dataSourceEntities = (await DataContext.NetworkEntities.Where(ne => datasourceIDs.Contains(ne.ID)).ToArrayAsync()).ToDictionary(ne => ne.ID);
                       

            //TODO: make this an asynchronous parallel loop for the registering
            foreach (var participatingRoute in routeDefinitions.GroupBy(k => new { k.NetworkID, k.ProjectID, k.RequestTypeID }))
            {
                var participant = new Data.Requests.NetworkRequestParticipant
                {
                    NetworkRequestID = request.ID,
                    NetworkID = participatingRoute.Key.NetworkID,
                    ProjectID = participatingRoute.Key.ProjectID,
                    RequestTypeID = participatingRoute.Key.RequestTypeID
                };                

                request.Participants.Add(participant);

                var requestDocuments = dto.Documents.Select(d => {
                    Guid docID = DatabaseEx.NewGuid();
                    return new
                    {
                        SourceDocumentID = d.DocumentID,
                        SourceRequestDataMartID = d.SourceRequestDataSourceID,
                        DTO = new Dns.DTO.CNDSRegisterDocumentDTO
                        {
                            SourceDocumentID = d.DocumentID,
                            SourceRevisionSetID = d.RevisionSetID,
                            DocumentID = docID,
                            RevisionSetID = docID,
                            Name = d.Name,
                            FileName = d.FileName,
                            Kind = d.Kind,
                            MimeType = d.MimeType,
                            Length = d.Length,
                            IsViewable = d.IsViewable,
                            Description = d.Description
                        }
                    };
                }).ToArray();                

                var network = networkDetails[participatingRoute.Key.NetworkID];
                using (var pmnAPI = new Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Lpp.Utilities.Crypto.DecryptString(network.ServicePassword)))
                {

                    var requestDto = new Dns.DTO.CNDSRegisterRequestDTO
                    {
                        ParticipantID = participant.ID,
                        ProjectID = participatingRoute.Key.ProjectID,
                        RequestTypeID = participatingRoute.Key.RequestTypeID,
                        SourceNetworkID = request.NetworkID,
                        SourceNetworkName = networkDetails[request.NetworkID].Name,
                        RequestDetails = dto.SerializedSourceRequest,
                        Documents = requestDocuments.Select(d => d.DTO).ToArray()
                    };

                    HashSet<Dns.DTO.CNDSRegisterRouteDTO> routeDTOs = new HashSet<Dns.DTO.CNDSRegisterRouteDTO>();
                    foreach (var rt in participatingRoute)
                    {
                        var rtDTO = dto.Routes.First(x => x.NetworkRouteDefinitionID == rt.ID);

                        var route = new Data.Requests.NetworkRequestRoute
                        {
                            DataSourceID = rt.DataSourceID,
                            ParticipantID = participant.ID,
                            SourceRequestDataMartID = rtDTO.SourceRequestDataMartID                             
                        };

                        var response = new Data.Requests.NetworkRequestResponse { IterationIndex = 1, NetworkRequestRouteID = route.ID, SourceResponseID = rtDTO.SourceResponseID };                        

                        var responseDocuments = new HashSet<Data.Requests.NetworkRequestDocument>();
                        foreach(var requestDocument in requestDocuments.Where(rd => rtDTO.RequestDocumentIDs.Contains(rd.SourceDocumentID) && rd.SourceRequestDataMartID == route.SourceRequestDataMartID))
                        {
                            responseDocuments.Add(new Data.Requests.NetworkRequestDocument {
                                //the ID of the request document in CNDS will be the ID of the document in the destination network
                                ID = requestDocument.DTO.DocumentID,
                                DestinationRevisionSetID = requestDocument.DTO.RevisionSetID,
                                ResponseID = response.ID,
                                SourceDocumentID = requestDocument.SourceDocumentID
                            }); 
                        }
                        response.Documents = responseDocuments;

                        route.Responses.Add(response);
                        participant.Routes.Add(route);

                        routeDTOs.Add(new Dns.DTO.CNDSRegisterRouteDTO
                        {
                            RouteID = route.ID,
                            //Specify the destination networks DataMart ID, not the CNDS DataSource ID
                            DataMartID = dataSourceEntities[rt.DataSourceID].NetworkEntityID,
                            DueDate = rtDTO.DueDate,
                            Priority = (Dns.DTO.Enums.Priorities)rtDTO.Priority,
                            ResponseID = response.ID,
                            //The ID will be the ID of the document in the destination network
                            DocumentIDs = response.Documents.Select(d => d.ID).ToArray()
                        });

                    }

                    requestDto.Routes = routeDTOs;

                    try
                    {
                        await DataContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                    }

                    //has to be saved prior to registering in the participating network to allow for the documents to exist when the participant tries to get the document content.
                    var resp = await pmnAPI.CNDSRequests.Register(requestDto);

                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);                        
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Returns the bytes for the specified document.
        /// </summary>
        /// <param name="id">The ID of the destination document.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> ReadDocument(Guid id)
        {
            var document = await DataContext.NetworkRequestDocuments.FindAsync(id);
            if(document == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Document not found.");
            }

            var networkDetails = await (from rsp in DataContext.NetworkRequestResponses
                                        let network = rsp.NetworkRequestRoute.Participant.NetworkRequest.Network
                                        where rsp.ID == document.ResponseID
                                        select new
                                        {
                                            NetworkID = network.ID,
                                            network.ServiceUrl,
                                            network.ServiceUserName,
                                            network.ServicePassword
                                        }).FirstOrDefaultAsync();

            using(var pmnAPI = new Lpp.Dns.ApiClient.DnsClient(networkDetails.ServiceUrl, networkDetails.ServiceUserName, Lpp.Utilities.Crypto.DecryptString(networkDetails.ServicePassword)))
            {
                return await pmnAPI.Documents.Read(document.SourceDocumentID);
            }
        }

        /// <summary>
        /// Updates the status of an external route on the source request.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateSourceRouting(IEnumerable<DTO.Requests.SetRoutingStatusDTO> dtos)
        {
            var responseIDs = dtos.Select(d => d.ResponseID).Distinct();
            //the response ID specified will be from the participating network request
            var details = await(from rsp in DataContext.NetworkRequestResponses
                          let network = rsp.NetworkRequestRoute.Participant.NetworkRequest.Network
                          where responseIDs.Contains(rsp.ID)
                          select new {
                              ParticipantResponseID = rsp.ID,
                              rsp.SourceResponseID,
                              rsp.NetworkRequestRoute.SourceRequestDataMartID,
                              NetworkID = network.ID,
                              NetworkName = network.Name,
                              network.ServiceUrl,
                              network.ServiceUserName,
                              network.ServicePassword
                          }).ToArrayAsync();

            if(details == null || details.Length < responseIDs.Count())
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to determine the source request details for one or more of the specified responses.");
            }

            foreach (var detail in details.GroupBy(k => new { k.NetworkID, k.NetworkName, k.ServiceUrl, k.ServiceUserName, k.ServicePassword }))
            {
                using (var pmnApi = new Dns.ApiClient.DnsClient(detail.Key.ServiceUrl, detail.Key.ServiceUserName, Lpp.Utilities.Crypto.DecryptString(detail.Key.ServicePassword)))
                {
                    var updateDTOs = detail.Select(k => {
                        var dto = dtos.First(d => d.ResponseID == k.ParticipantResponseID);
                        return new Dns.DTO.CNDSUpdateRoutingStatusDTO
                        {
                            NetworkID = k.NetworkID,
                            Network = k.NetworkName,
                            ResponseID = k.SourceResponseID,
                            Message = dto.Message, 
                            RoutingStatus = (Dns.DTO.Enums.RoutingStatus)dto.RoutingStatus
                        };
                    }).ToArray();

                    var resp = await pmnApi.CNDSRequests.UpdateRoutingStatus(updateDTOs);
                    if(!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, await resp.GetMessage());
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ResubmitParticipateRouting(IEnumerable<DTO.Requests.ResubmitRouteDTO> dtos)
        {
            foreach (var dm in dtos)
            {
                var details = await (from route in DataContext.NetworkRequestRoutes
                                     let network = route.Participant.Network
                                     where route.SourceRequestDataMartID == dm.RequestDatamartID
                                     select new
                                     {
                                         ID = network.ID,
                                         network.Name,
                                         network.ServiceUrl,
                                         network.ServiceUserName,
                                         network.ServicePassword,
                                         NetworkRouteID = route.ID
                                     }).FirstOrDefaultAsync();

                using (var participantNetworkAPI = new Dns.ApiClient.DnsClient(details.ServiceUrl, details.ServiceUserName, Utilities.Crypto.DecryptString(details.ServicePassword)))
                {

                    var response = new Data.Requests.NetworkRequestResponse { IterationIndex = 1, NetworkRequestRouteID = details.NetworkRouteID, SourceResponseID = dm.ResponseID };
                    DataContext.NetworkRequestResponses.Add(response);
                    var resp = await participantNetworkAPI.CNDSRequests.ResubmitRoute(new Dns.DTO.CNDSResubmitRouteDTO { RequestDatamartID = details.NetworkRouteID, ResponseID = response.ID, Message = dm.Message });
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);

                    await DataContext.SaveChangesAsync();
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> ResubmitSourceRouting(IEnumerable<DTO.Requests.ResubmitRouteDTO> dtos)
        {
            foreach (var dm in dtos)
            {
                var details = await (from route in DataContext.NetworkRequestRoutes
                                     let network = route.Participant.NetworkRequest.Network
                                     where route.ID == dm.RequestDatamartID
                                     select new
                                     {
                                         ID = network.ID,
                                         network.Name,
                                         network.ServiceUrl,
                                         network.ServiceUserName,
                                         network.ServicePassword,
                                         NetworkRouteID = route.ID,
                                         route.SourceRequestDataMartID
                                     }).FirstOrDefaultAsync();

                using (var participantNetworkAPI = new Dns.ApiClient.DnsClient(details.ServiceUrl, details.ServiceUserName, Utilities.Crypto.DecryptString(details.ServicePassword)))
                {

                    var response = new Data.Requests.NetworkRequestResponse { ID= dm.ResponseID, IterationIndex = 1, NetworkRequestRouteID = details.NetworkRouteID, SourceResponseID = DatabaseEx.NewGuid() };
                    DataContext.NetworkRequestResponses.Add(response);
                    var resp = await participantNetworkAPI.CNDSRequests.ResubmitRoute(new Dns.DTO.CNDSResubmitRouteDTO { RequestDatamartID = details.SourceRequestDataMartID, ResponseID = response.SourceResponseID, Message = dm.Message });
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);

                    await DataContext.SaveChangesAsync();
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Updates the status of an external route on a participant request.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateParticipantRoutingStatus(IEnumerable<DTO.Requests.SetRoutingStatusDTO> dtos)
        {
            var responseIDs = dtos.Select(d => d.ResponseID).Distinct();
            //the response ID specified will be from the participating network request
            var details = await (from rsp in DataContext.NetworkRequestResponses
                                 let network = rsp.NetworkRequestRoute.Participant.Network
                                 where responseIDs.Contains(rsp.SourceResponseID)
                                 select new
                                 {
                                     ParticipantResponseID = rsp.ID,
                                     rsp.SourceResponseID,
                                     rsp.NetworkRequestRoute.SourceRequestDataMartID,
                                     NetworkID = network.ID,
                                     NetworkName = network.Name,
                                     network.ServiceUrl,
                                     network.ServiceUserName,
                                     network.ServicePassword
                                 }).ToArrayAsync();

            if (details == null || details.Length < responseIDs.Count())
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to determine the source request details for one or more of the specified responses."));
            }

            foreach (var detail in details.GroupBy(k => new { k.NetworkID, k.NetworkName, k.ServiceUrl, k.ServiceUserName, k.ServicePassword }))
            {
                using (var pmnApi = new Dns.ApiClient.DnsClient(detail.Key.ServiceUrl, detail.Key.ServiceUserName, Lpp.Utilities.Crypto.DecryptString(detail.Key.ServicePassword)))
                {
                    var updateDTOs = detail.Select(k => {
                        var dto = dtos.First(d => d.ResponseID == k.SourceResponseID);
                        return new Dns.DTO.CNDSUpdateRoutingStatusDTO
                        {
                            NetworkID = k.NetworkID,
                            Network = k.NetworkName,
                            ResponseID = k.ParticipantResponseID,
                            Message = dto.Message,
                            RoutingStatus = (Dns.DTO.Enums.RoutingStatus)dto.RoutingStatus
                        };
                    }).ToArray();

                    var resp = await pmnApi.CNDSRequests.UpdateRoutingStatus(updateDTOs);
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, await resp.GetMessage());
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);

        }

        /// <summary>
        /// Copies the output response documents for a participant response to the corresponding source response.
        /// </summary>
        /// <param name="responseID">The ID of the participant response.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task PostParticipantResponseDocuments([FromBody]Guid responseID)
        {
            var details = await (from rsp in DataContext.NetworkRequestResponses
                          let pNetwork = rsp.NetworkRequestRoute.Participant.Network
                          let sNetwork = rsp.NetworkRequestRoute.Participant.NetworkRequest.Network
                          where rsp.ID == responseID
                          select new
                          {
                              ParticipantNetwork = new
                              {
                                  ID = pNetwork.ID,
                                  pNetwork.Name,
                                  pNetwork.ServiceUrl,
                                  pNetwork.ServiceUserName,
                                  pNetwork.ServicePassword
                              },
                              SourceNetwork = new
                              {
                                 ID = sNetwork.ID,
                                  sNetwork.Name,
                                  sNetwork.ServiceUrl,
                                  sNetwork.ServiceUserName,
                                  sNetwork.ServicePassword
                              }                          
                          }).FirstOrDefaultAsync();

            if(details == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to determine the response details."));
            }

            using (var participantNetworkAPI = new Dns.ApiClient.DnsClient(details.ParticipantNetwork.ServiceUrl, details.ParticipantNetwork.ServiceUserName, Utilities.Crypto.DecryptString(details.ParticipantNetwork.ServicePassword)))
            {
                try
                {
                    var participantDocuments = await participantNetworkAPI.Documents.ByResponse(new[] { responseID });

                    var cndsDocuments = participantDocuments.Select(d =>
                    {
                        Guid docID = DatabaseEx.NewGuid();
                        return new Data.Requests.NetworkRequestDocument
                        {
                            //create the ID of the document for the source network document
                            ID = docID,
                            DestinationRevisionSetID = docID,
                            ResponseID = responseID,
                            //the source document will the the document in the participant network
                            SourceDocumentID = d.ID.Value
                        };
                    }).ToArray();

                    foreach (var cndsDocument in cndsDocuments)
                    {
                        var sourceResponseID = DataContext.NetworkRequestResponses.Where(x => x.ID == responseID).Select(x => x.SourceResponseID).FirstOrDefault();
                        Dns.DTO.ExtendedDocumentDTO participantDocument = participantDocuments.FirstOrDefault(d => d.ID.Value == cndsDocument.SourceDocumentID);
                        if (participantDocument == null)
                        {
                            throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Participant document information not found."));
                        }
                        using (MultipartFormDataContent docData = new MultipartFormDataContent())
                        using (var web = new System.Net.Http.HttpClient())
                        {
                            var creds = Convert.ToBase64String(Encoding.UTF8.GetBytes(details.SourceNetwork.ServiceUserName + ":" + Utilities.Crypto.DecryptString(details.SourceNetwork.ServicePassword)));
                            web.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", creds);
                            HttpResponseMessage response = new HttpResponseMessage();
                           
                            {
                                docData.Add(new StringContent(sourceResponseID.ToString("D")), "responseID");
                                docData.Add(new StringContent(cndsDocument.ID.ToString("D")), "id");
                                docData.Add(new StringContent(details.ParticipantNetwork.ID.ToString("D")), "uploadedByID");
                                docData.Add(new StringContent(details.ParticipantNetwork.Name), "uploadedBy");
                                docData.Add(new StringContent(cndsDocument.DestinationRevisionSetID.ToString("D")), "revisionSetID");
                                if (participantDocument.ParentDocumentID.HasValue)
                                {
                                    Guid? parentCNDSDocumentID = await DataContext.NetworkRequestDocuments.Where(d => d.SourceDocumentID == participantDocument.ParentDocumentID.Value).Select(d => d.ID).FirstOrDefaultAsync();
                                    if (parentCNDSDocumentID.HasValue)
                                    {
                                        docData.Add(new StringContent(parentCNDSDocumentID.Value.ToString("D")), "parentDocumentID");
                                    }
                                }

                                if (!string.IsNullOrEmpty(participantDocument.Description))
                                {
                                    docData.Add(new StringContent(participantDocument.Description), "description");
                                }

                                docData.Add(new StringContent(participantDocument.Name), "documentName");
                                if (!string.IsNullOrEmpty(participantDocument.Kind))
                                {
                                    docData.Add(new StringContent(participantDocument.Kind), "kind");
                                }

                                //docData.Headers.ContentDisposition.FileName = participantDocument.FileName;
                                
                                
                                var sourceReadResponse = await participantNetworkAPI.Documents.Download(participantDocument.ID.Value);

                                var sourceResponse = await sourceReadResponse.Content.ReadAsStreamAsync();

                                docData.Add(new StreamContent(sourceResponse), "files");

                                docData.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                                {
                                    FileName = participantDocument.FileName,
                                    Size = participantDocument.Length
                                };

                                response = await web.PostAsync(details.SourceNetwork.ServiceUrl + "/Documents/UploadResponseOutput", docData);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    Logger.Error(ex.Message, ex);
                    throw;
                }


            }



        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdateSourceDataMartsPriorityAndDueDates(IEnumerable<DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
        {
            foreach(var dm in dtos)
            {
                var details = await (from route in DataContext.NetworkRequestRoutes
                                     let network = route.Participant.NetworkRequest.Network
                                     where route.ID == dm.RequestDataMartID
                                     select new
                                     {
                                         ID = network.ID,
                                         network.Name,
                                         network.ServiceUrl,
                                         network.ServiceUserName,
                                         network.ServicePassword,
                                         route.SourceRequestDataMartID
                                     }).FirstOrDefaultAsync();

                using (var sourceNetworkAPI = new Dns.ApiClient.DnsClient(details.ServiceUrl, details.ServiceUserName, Utilities.Crypto.DecryptString(details.ServicePassword)))
                {
                    var resp = await sourceNetworkAPI.CNDSRequests.UpdatePriorityAndDueDate(new Dns.DTO.CNDS.CNDSUpdateDataMartPriorityAndDueDateDTO { RequestDataMartID = details.SourceRequestDataMartID, DueDate = dm.DueDate, Priority = dm.Priority });
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdatParticipantDataMartsPriorityAndDueDates(IEnumerable<DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
        {
            foreach (var dm in dtos)
            {
                var details = await (from route in DataContext.NetworkRequestRoutes
                                     let network = route.Participant.Network
                                     where route.SourceRequestDataMartID == dm.RequestDataMartID
                                     select new
                                     {
                                         ID = network.ID,
                                         network.Name,
                                         network.ServiceUrl,
                                         network.ServiceUserName,
                                         network.ServicePassword,
                                         RequestDataMartID = route.ID
                                     }).FirstOrDefaultAsync();

                using (var participantNetworkAPI = new Dns.ApiClient.DnsClient(details.ServiceUrl, details.ServiceUserName, Utilities.Crypto.DecryptString(details.ServicePassword)))
                {
                    var resp = await participantNetworkAPI.CNDSRequests.UpdatePriorityAndDueDate(new Dns.DTO.CNDS.CNDSUpdateDataMartPriorityAndDueDateDTO { RequestDataMartID = details.RequestDataMartID, DueDate = dm.DueDate, Priority = dm.Priority });
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> TerminateParticipantRoute(IEnumerable<Guid> ids)
        {
            var details = (from route in DataContext.NetworkRequestRoutes
                                 let network = route.Participant.Network
                                 where ids.Contains(route.SourceRequestDataMartID)
                                 select new
                                 {
                                     RequestID = route.ParticipantID,
                                     NetworkID = network.ID,
                                     NetworkName = network.Name,
                                     network.ServiceUrl,
                                     network.ServiceUserName,
                                     network.ServicePassword
                                 }).DistinctBy(c => c.RequestID).ToArray();

            if (details == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Unable to determine the source request details for one or more of the specified responses.");
            }

            foreach (var detail in details)
            {
                using (var pmnApi = new Dns.ApiClient.DnsClient(detail.ServiceUrl, detail.ServiceUserName, Lpp.Utilities.Crypto.DecryptString(detail.ServicePassword)))
                {
                    var resp = await pmnApi.CNDSRequests.TerminateRequest(detail.RequestID);
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, await resp.GetMessage());
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> TerminateSourceRoute(IEnumerable<Guid> ids)
        {
            foreach (var dm in ids)
            {
                var details = await (from route in DataContext.NetworkRequestRoutes
                                     let network = route.Participant.NetworkRequest.Network
                                     where route.ID == dm
                                     select new
                                     {
                                         ID = network.ID,
                                         network.Name,
                                         network.ServiceUrl,
                                         network.ServiceUserName,
                                         network.ServicePassword,
                                         route.SourceRequestDataMartID
                                     }).FirstOrDefaultAsync();

                using (var participantNetworkAPI = new Dns.ApiClient.DnsClient(details.ServiceUrl, details.ServiceUserName, Utilities.Crypto.DecryptString(details.ServicePassword)))
                {
                    var resp = await participantNetworkAPI.CNDSRequests.TerminateRoute(details.SourceRequestDataMartID);
                    if (!resp.IsSuccessStatusCode)
                        return Request.CreateErrorResponse(resp.StatusCode, resp.ReasonPhrase);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }    
}