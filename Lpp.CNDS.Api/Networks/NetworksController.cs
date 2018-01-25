using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Lpp.Utilities.WebSites.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace Lpp.CNDS.Api.Networks
{
    /// <summary>
    /// WebAPI Controller for all Network Actions
    /// </summary>
    [AllowAnonymous]
    public class NetworksController : LppApiController<DataContext>
    {
        /// <summary>
        /// An OData enpoint for Listing all Networks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<NetworkDTO> List()
        {
            return DataContext.Networks.AsQueryable().Map<Network, NetworkDTO>();
        }
        /// <summary>
        /// Endpoint for Details about a specific Network
        /// </summary>
        /// <param name="id">The Guid ID of the Network</param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<NetworkDTO> Get(Guid id)
        {
            return DataContext.Networks.Where(n => n.ID == id).Map<Network, NetworkDTO>();
        }
        /// <summary>
        /// Endpoint for Registering a Network
        /// </summary>
        /// <param name="dto">The DTO of the Network</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<NetworkDTO> Register(NetworkTransferDTO dto)
        {
            Data.Network newNetwork = DataContext.Networks.Add(new Data.Network()
            {
                ID = dto.ID,
                Name = dto.Name,
                Url = dto.Url,
                ServiceUrl = dto.ServiceUrl,
                ServiceUserName = dto.ServiceUserName,
                ServicePassword = Lpp.Utilities.Crypto.EncryptString(dto.ServicePassword)
            });

            await DataContext.SaveChangesAsync();

            return newNetwork.Map<Network, NetworkDTO>();
        }
        /// <summary>
        /// Updates the specified network details.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Update(NetworkTransferDTO dto)
        {
            var network = DataContext.Networks.Find(dto.ID);
            if (network == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified Network was not found.");
            }

            network.Name = dto.Name;
            network.Url = dto.Url;
            network.ServiceUrl = dto.ServiceUrl;
            network.ServiceUserName = dto.ServiceUserName;
            network.ServicePassword = Crypto.EncryptString(dto.ServicePassword);

            var validationErrors = DataContext.GetValidationErrors();
            if(validationErrors != null && validationErrors.Any())
            {
                var messages = validationErrors.Select(v => string.Join(Environment.NewLine, v.ValidationErrors.Select(ex => string.Format("{0}: {1}", ex.PropertyName, ex.ErrorMessage)))).ToArray();
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(Environment.NewLine, messages));
            }

            await DataContext.SaveChangesAsync();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Endpoint for Deleting a Network
        /// </summary>
        /// <param name="id">The Guid ID of the Network</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var network = DataContext.Networks.Find(id);
            if (network == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified Network was not found.");
            }
            else
            {
                DataContext.Networks.Remove(network);
                await DataContext.SaveChangesAsync();

                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }

        [HttpGet]
        public HttpResponseMessage LookupEntities(Guid networkID, [FromUri]IEnumerable<Guid> entityIDs)
        {
            var et = DataContext.NetworkEntities.Where(ne => ne.NetworkID == networkID && (entityIDs.Contains(ne.NetworkEntityID) || entityIDs.Contains(ne.ID))).Select(ne => new { EntityID = ne.ID, NetworkEntityID = ne.NetworkEntityID }).ToArray();
            return Request.CreateResponse(et);
        }

        public IQueryable<NetworkEntityDTO> ListNetworkEntities()
        {
            var result = DataContext.NetworkEntities.Where(ne =>
                    DataContext.DataSources.Where(ds => ne.ID == ds.ID && ds.Deleted == false).Any() ||
                    DataContext.Organizations.Where(org => ne.ID == org.ID && org.Deleted == false).Any() ||
                    DataContext.Users.Where(user => user.ID == ne.ID && user.Deleted == false).Any()
                );

            return result.Map<NetworkEntity, NetworkEntityDTO>();
        }
    }
}
