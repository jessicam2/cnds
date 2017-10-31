using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Lpp.CNDS.ApiClient;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Utilities.WebSites.Controllers;
using cndsDTO = Lpp.CNDS.DTO;
using System.Web.Http;
using System.Net.Http;

namespace Lpp.Dns.Api.CNDS
{
    public class CNDSNetworksController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(CNDSNetworksController));
        static readonly string CNDSurl;
        readonly Lpp.CNDS.ApiClient.CNDSClient CNDSApi;

        static CNDSNetworksController()
        {
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        public CNDSNetworksController()
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
        /// Gets all networks registered with CNDS.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IQueryable<CNDSNetworkDTO>> List()
        {
            var cndsResponse = await CNDSApi.Networks.List(Request.RequestUri.Query);

            return MapFromCNDSNetwork(cndsResponse);
        }

        [HttpPost]
        public async Task<CNDSNetworkDTO> Register(CNDSNetworkDTO dto)
        {
            var network = await CNDSApi.Networks.Register(new Lpp.CNDS.DTO.NetworkTransferDTO { ID = dto.ID, Name = dto.Name, Url = dto.Url, ServiceUrl = dto.ServiceUrl, ServiceUserName = dto.ServiceUserName, ServicePassword = dto.ServicePassword });

            return new CNDSNetworkDTO {
                ID = network.ID.Value,
                Name = network.Name,
                Url = network.Url,
                ServiceUrl = network.ServiceUrl,
                ServiceUserName = network.ServiceUserName,
                ServicePassword =  network.ServicePassword
            };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update(CNDSNetworkDTO dto)
        {
            var rsp = await CNDSApi.Networks.Update(new cndsDTO.NetworkTransferDTO { ID = dto.ID, Name = dto.Name, Url = dto.Url, ServiceUrl = dto.ServiceUrl, ServiceUserName = dto.ServiceUserName, ServicePassword = dto.ServicePassword });
            if (rsp.IsSuccessStatusCode)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateErrorResponse(rsp.StatusCode, rsp.ReasonPhrase);
        }

        [HttpDelete]
        public async Task Delete(Guid id)
        {
            await CNDSApi.Networks.Delete(id);
        }

        static IQueryable<CNDSNetworkDTO> MapFromCNDSNetwork(IQueryable<cndsDTO.NetworkDTO> cnds)
        {
            return cnds.Select(n => new CNDSNetworkDTO { ID = n.ID.Value, Name = n.Name, Url = n.Url, ServiceUrl = n.ServiceUrl, ServiceUserName = n.ServiceUserName, ServicePassword = n.ServicePassword });
        }
    }
}
