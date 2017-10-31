using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Lpp.CNDS.ApiClient;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using Lpp.Utilities.WebSites.Controllers;
using cndsDTO = Lpp.CNDS.DTO;

namespace Lpp.Dns.Api.CNDS
{
    /// <summary>
    /// Controller for interactiving with CNDS metadata definitions.
    /// </summary>
    public class CNDSMetadataController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static readonly string CNDSurl;

        static CNDSMetadataController()
        {
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        /// <summary>
		/// Returns the List of All Metadata in CNDS
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        public async System.Threading.Tasks.Task<IEnumerable<MetadataDTO>> ListDomains()
        {
            List<MetadataDTO> meta = new List<MetadataDTO>();
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    var availOrgMetdata = await CNDSEntityUpdater.CNDS.Domain.ListDomains();
                    foreach (var metadata in availOrgMetdata.Where(x => x.ParentDomainID == null))
                    {
                        meta.Add(cnds.GetMetadataChildren(metadata.ID, availOrgMetdata, new List<cndsDTO.DomainDataDTO>(), null, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
            return meta;
        }

        /// <summary>
		/// Insert Or Update Metadata in CNDS
		/// </summary>
		/// <returns></returns>
		[HttpPost]
        public async System.Threading.Tasks.Task<IEnumerable<MetadataDTO>> InsertOrUpdateDomains(IEnumerable<MetadataDTO> metaData)
        {
            List<MetadataDTO> meta = new List<MetadataDTO>();
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            try
            {
                using (var cnds = new CNDSEntityUpdater(networkID))
                {
                    await CNDSEntityUpdater.RegisterOrUpdateDomains(metaData);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
            return meta;
        }

        /// <summary>
        /// Gets the metadata definitions for the Organization entity type.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DTO.CNDS.DomainDTO>> GetForOrganization()
        {
            List<DTO.CNDS.DomainDTO> meta = new List<DTO.CNDS.DomainDTO>();
            using (var cnds = new CNDSClient(CNDSurl))
            {

                var cndsMetadata = await cnds.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'0'");
                foreach (var metadata in cndsMetadata.Where(x => x.ParentDomainID == null))
                {
                    meta.Add(CNDSEntityUpdater.GetDomainList(metadata.ID, cndsMetadata));
                }
            }
            return meta;
        }

        /// <summary>
        /// Inserts or Updates Data Use in CNDS
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task InsertorUpdateDataDomains(IEnumerable<DTO.CNDSMetadata.DomainUseReturnDTO> domainUses)
        {
            Guid networkID = await CNDSEntityUpdater.GetNetworkID(DataContext);
            using (var cnds = new CNDSEntityUpdater(networkID))
            {
                try
                {
                    await CNDSEntityUpdater.RegisterOrUpdateDomainsUses(domainUses);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex.InnerException);
                    throw;
                }

            }
        }


        /// <summary>
        /// Gets the metadata definitions for the DataMarts entity type.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DTO.CNDS.DomainDTO>> GetForDataMarts()
        {
            List<DTO.CNDS.DomainDTO> meta = new List<DTO.CNDS.DomainDTO>();
            using (var cnds = new CNDSClient(CNDSurl))
            {

                var cndsMetadata = await cnds.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'2'");
                foreach (var metadata in cndsMetadata.Where(x => x.ParentDomainID == null))
                {
                    meta.Add(CNDSEntityUpdater.GetDomainList(metadata.ID, cndsMetadata));
                }
            }
            return meta;
        }

        /// <summary>
        /// Gets the metadata definitions for the Users entity type.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<DTO.CNDS.DomainDTO>> GetForUsers()
        {
            List<DTO.CNDS.DomainDTO> meta = new List<DTO.CNDS.DomainDTO>();
            using (var cnds = new CNDSClient(CNDSurl))
            {

                var cndsMetadata = await cnds.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'1'");
                foreach (var metadata in cndsMetadata.Where(x => x.ParentDomainID == null))
                {
                    meta.Add(CNDSEntityUpdater.GetDomainList(metadata.ID, cndsMetadata));
                }
            }
            return meta;
        }
    }
}