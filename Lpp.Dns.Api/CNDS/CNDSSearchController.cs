using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Lpp.CNDS.ApiClient;
using Lpp.CNDS.DTO;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Utilities;
using Lpp.Utilities.WebSites.Controllers;

namespace Lpp.Dns.Api.CNDS
{
    public class CNDSSearchController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(CNDSNetworksController));
        static readonly string CNDSurl;
        readonly Lpp.CNDS.ApiClient.CNDSClient CNDSApi;

        static CNDSSearchController()
        {
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public CNDSSearchController()
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IQueryable<Lpp.Dns.DTO.CNDSDataSourceExtendedDTO>> DataSources()
        {

            var odata = HttpUtility.UrlDecode(Request.RequestUri.Query.TrimStart('?'));

            var result = await CNDSApi.DataSources.ListExtended(odata);

            return result.Map<Lpp.CNDS.DTO.DataSourceExtendedDTO, CNDSDataSourceExtendedDTO>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>> DataSourcesDomains()
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var entity = new CNDSEntityUpdater(networkID))
            {
                return (await CNDSEntityUpdater.GetDataSourceDomains());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<Lpp.Dns.DTO.CNDS.CNDSDataSourceSearchDTO>> DataSourcesSearch(DTO.CNDS.CNDSDomainSearchDTO ids)
        {
            var ds = new List<DTO.CNDS.CNDSDataSourceSearchDTO>();
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var entity = new CNDSEntityUpdater(networkID))
            {
                var result = await CNDSApi.DataSources.DataSourceSearch(new SearchDTO { DomainIDs = ids.DomainIDs, DomainReferencesIDs = ids.DomainReferences, NetworkID = networkID});
                if (result.Count() > 0)
                    ds.AddRange((from r in result
                                 select new Dns.DTO.CNDS.CNDSDataSourceSearchDTO
                                 {
                                     ID = r.ID,
                                     Name = r.Name,
                                     Network = r.Network,
                                     NetworkID = r.NetworkID,
                                     Organization = r.Organization,
                                     OrganizationID = r.OrganizationID,
                                     ContactInformation = r.ContactInformation
                                 }));
            }

            return ds;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>> OrganizationsDomains()
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var entity = new CNDSEntityUpdater(networkID))
            {
                return (await CNDSEntityUpdater.GetOrganizationDomains());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<Lpp.Dns.DTO.CNDS.CNDSOrganizationSearchDTO>> OrganizationsSearch(DTO.CNDS.CNDSDomainSearchDTO ids)
        {
            var ds = new List<DTO.CNDS.CNDSOrganizationSearchDTO>();
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var entity = new CNDSEntityUpdater(networkID))
            {
                var result = await CNDSApi.Organizations.OrganizationSearch(new SearchDTO { DomainIDs = ids.DomainIDs, DomainReferencesIDs = ids.DomainReferences, NetworkID = networkID });
                if (result.Count() > 0)
                    ds.AddRange((from r in result
                                 select new Dns.DTO.CNDS.CNDSOrganizationSearchDTO
                                 {
                                     ID = r.ID,
                                     Name = r.Name,
                                     Network = r.Network,
                                     NetworkID = r.NetworkID,
                                     ContactInformation = r.ContactInformation
                                 }));
            }

            return ds;
        }
    }

    internal class CNDSDataSourceSearchResultDTOMappingConfiguration : Lpp.Utilities.EntityMappingConfiguration<Lpp.CNDS.DTO.DataSourceExtendedDTO, DTO.CNDSDataSourceExtendedDTO>
    {
        public override Expression<Func<DataSourceExtendedDTO, CNDSDataSourceExtendedDTO>> MapExpression
        {
            get
            {
                return (ds) => new CNDSDataSourceExtendedDTO {
                    ID = ds.ID,
                    Name = ds.Name,
                    Acronym = ds.Acronym,
                    AdapterSupportedID = ds.AdapterSupportedID,
                    AdapterSupported = ds.AdapterSupported,
                    OrganizationID = ds.OrganizationID,
                    Organization = ds.Organization,
                    NetworkID = ds.NetworkID,
                    Network = ds.Network
                };
            }
        }
    }
}