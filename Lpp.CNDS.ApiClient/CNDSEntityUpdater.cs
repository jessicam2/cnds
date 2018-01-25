using System;
using System.Collections.Generic;
using System.Linq;
using Lpp.Utilities;
using Lpp.Dns.Data;
using Lpp.CNDS.DTO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Web;
using Lpp.Dns.DTO;

namespace Lpp.CNDS.ApiClient
{
    public class CNDSEntityUpdater : IDisposable
    {
        public static CNDSClient CNDS;
        static Guid NetworkID;
        public CNDSEntityUpdater(Guid networkID) : this(networkID, System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"])
        {
        }

        public CNDSEntityUpdater(Guid networkID, string url)
        {
            NetworkID = networkID;
            CNDS = new CNDSClient(url);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CNDSEntityUpdater()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CNDS.Dispose();
            }
        }

        /// <summary>
        /// Gets a collection of CNDSEntityIdentifiers which link the CNDS entity ID with the specified PMN ID's that have been registered with CNDS.
        /// </summary>
        /// <param name="ids">The collection of PMN entity ID's to get the reciprocal CNDS entity ID's.</param>
        /// <remarks>Note: The collection returned from CNDS will only contain registered entities.</remarks>
        /// <returns></returns>
        public static async Task<IEnumerable<CNDSEntityIdentifier>> GetCNDSEntityIdentifiers(IEnumerable<Guid> ids)
        {
            using (var api = new CNDSClient(System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"]))
            {
                HttpResponseMessage responseMessage = await api.Networks.LookupEntities(NetworkID, ids);
                string json = await responseMessage.GetMessage();
                var response = JsonConvert.DeserializeObject<Lpp.Utilities.BaseResponse<CNDSEntityIdentifier>>(json);
                return response.results;
            }
        }

        public class CNDSEntityIdentifier
        {
            /// <summary>
            /// The ID of the entity in CNDS.
            /// </summary>
            public Guid EntityID { get; set; }
            /// <summary>
            /// The ID of the entity in PMN.
            /// </summary>
            public Guid NetworkEntityID { get; set; }
        }

        /// <summary>
        /// Gets the ID of the portal's Network.
        /// </summary>
        /// <param name="db">The datacontext to use for the query.</param>
        /// <returns>The ID of the first Network found.</returns>
        public static async Task<Guid> GetNetworkID(DataContext db)
        {
            object networkID = null;
            
            return await db.Networks.Where(n => n.Name != "Aqueduct").Select(n => n.ID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets if the base CNDS url has been configured for this site.
        /// </summary>
        public static bool CanUpdateCNDS
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"].IsNullOrWhiteSpace() == false;
            }
        }
        /// <summary>
        /// Used to Register or Update PMN Users in CNDS
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static async Task RegisterOrUpdateUsers(Dns.DTO.UserDTO user)
        {
            IList<Guid> ids = new List<Guid>();
            ids.Add(user.ID.Value);
            if (user.OrganizationID.HasValue)
                ids.Add(user.OrganizationID.Value);
            var networkEntity = await GetCNDSEntityIdentifiers(ids);
            var cndsOrgID = networkEntity.Where(x => x.NetworkEntityID == user.OrganizationID.Value).Select(x => x.EntityID).FirstOrDefault();
            if (networkEntity.Select(x => x.NetworkEntityID).Contains(user.ID.Value))
            {
                var cndsUserID = networkEntity.Where(x => x.NetworkEntityID == user.ID).Select(x => x.EntityID).FirstOrDefault();
                await Helpers.Users.EditCNDSUser(CNDS, user, NetworkID, cndsUserID, cndsOrgID);
            }
            else
                await Helpers.Users.NewCNDSUser(CNDS, user, NetworkID, cndsOrgID);
        }
        /// <summary>
        /// Used to Register or Update PMN Organizations in CNDS
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        public static async Task RegisterOrUpdateOrganizations(Dns.DTO.OrganizationDTO org)
        {
            IList<Guid> ids = new List<Guid>();
            ids.Add(org.ID.Value);
            if (org.ParentOrganizationID.HasValue)
                ids.Add(org.ParentOrganizationID.Value);
            var networkEntity = await GetCNDSEntityIdentifiers(ids);
            Guid? cndsParentOrgID = null;
            if (org.ParentOrganizationID.HasValue)
                cndsParentOrgID = networkEntity.Where(x => x.NetworkEntityID == org.ParentOrganizationID.Value).Select(x => x.EntityID).FirstOrDefault();
            if (networkEntity.Select(x => x.NetworkEntityID).Contains(org.ID.Value))
            {
                var cndsOrganizationID = networkEntity.Where(x  => x.NetworkEntityID == org.ID.Value).Select(o => o.EntityID).FirstOrDefault();
                await Helpers.Organizations.UpdateCNDSOrg(CNDS, org, NetworkID, cndsOrganizationID, cndsParentOrgID);
            }
            else
            {
                await Helpers.Organizations.RegisterCNDSOrg(CNDS, org, NetworkID, cndsParentOrgID);
            }
        }
        /// <summary>
        /// Used to Register or Update  PMN DataMarts in CNDS
        /// </summary>
        /// <param name="datamart"></param>
        /// <returns></returns>
        public static async Task RegisterOrUpdateDataSources(Dns.DTO.DataMartDTO datamart)
        {
            IList<Guid> ids = new List<Guid>();
            ids.Add(datamart.ID.Value);

            if (datamart.OrganizationID.HasValue)
                ids.Add(datamart.OrganizationID.Value);

            var networkEntity = await GetCNDSEntityIdentifiers(ids);

            Guid? cndsOrganizationID = null;
            if (datamart.OrganizationID.HasValue)
                cndsOrganizationID = networkEntity.Where(x => x.NetworkEntityID == datamart.OrganizationID.Value).Select(x => x.EntityID).FirstOrDefault();

            if (networkEntity.Select(x => x.NetworkEntityID).Contains(datamart.ID.Value))
            {
                var cndsDataSourceID = networkEntity.Where(x => x.NetworkEntityID == datamart.ID.Value).Select(o => o.EntityID).FirstOrDefault();
                await Helpers.DataSources.UpdateCNDSDataSource(CNDS, datamart, NetworkID, cndsDataSourceID, cndsOrganizationID.Value);
            }
            else
            {
                await Helpers.DataSources.RegisterCNDSDataSource(CNDS, datamart, NetworkID, cndsOrganizationID.Value);
            }
        }

        /// <summary>
        /// Used to Register or Update Domains in CNDS
        /// </summary>
        /// <param name="datamart"></param>
        /// <returns></returns>
        public static async Task RegisterOrUpdateDomains(IEnumerable<Dns.DTO.MetadataDTO> domains)
        {
             await Helpers.Domains.UpdateCNDSDomains(domains, CNDS);
        }
        /// <summary>
        /// Used to Register or Update DomainsUses in CNDS
        /// </summary>
        /// <param name="datamart"></param>
        /// <returns></returns>
        public static async Task RegisterOrUpdateDomainsUses(IEnumerable<Lpp.Dns.DTO.CNDSMetadata.DomainUseReturnDTO> domains)
        {
            List<KeyValuePair<Guid, Lpp.CNDS.DTO.Enums.EntityType>> Added = new List<KeyValuePair<Guid, DTO.Enums.EntityType>>();
            List<KeyValuePair<Guid, Lpp.CNDS.DTO.Enums.EntityType>> Remove = new List<KeyValuePair<Guid, DTO.Enums.EntityType>>();

            foreach(var domain in domains)
            {
                if(domain.Checked && domain.DomainUseID == null || domain.Checked && domain.DomainUseID == Guid.Empty)
                    Added.Add(new KeyValuePair<Guid, DTO.Enums.EntityType>(domain.ID,domain.EntityType));
                if (!domain.Checked && domain.DomainUseID != null && domain.DomainUseID != Guid.Empty)
                    Remove.Add(new KeyValuePair<Guid, DTO.Enums.EntityType>(domain.DomainUseID.Value, domain.EntityType));
            }

            var sendCNDS = new DTO.AddRemoveDomainUseDTO()
            {
                AddDomainUse = Added,
                RemoveDomainUse = Remove,
            };
            await CNDS.Domain.InsertOrUpdateDomainUses(sendCNDS);
        }

        public static async Task RegisterOrUpdateOrganizationDomainVisibility(IEnumerable<MetadataDTO> domains)
        {
            var domainUses = new List<DomainDataDTO>();
            foreach(var domain in domains)
            {
                domainUses.AddRange(RecusiveDomainVisibility(domain));
            }

            await CNDS.Organizations.UpdateDomainVisibility(domainUses);
        }
        public static async Task RegisterOrUpdateDatamartDomainVisibility(IEnumerable<MetadataDTO> domains)
        {
            var domainUses = new List<DomainDataDTO>();
            foreach (var domain in domains)
            {
                domainUses.AddRange(RecusiveDomainVisibility(domain));
            }

            await CNDS.DataSources.UpdateDomainVisibility(domainUses);
        }
        public static async Task RegisterOrUpdateUserDomainVisibility(IEnumerable<MetadataDTO> domains)
        {
            var domainUses = new List<DomainDataDTO>();
            foreach (var domain in domains)
            {
                domainUses.AddRange(RecusiveDomainVisibility(domain));
            }

            await CNDS.Users.UpdateDomainVisibility(domainUses);
        }

        public static async Task CreateSecurityGroups(IEnumerable<CNDSSecurityGroupDTO> dtos)
        {
            IList<DTO.SecurityGroupDTO> send = new List<DTO.SecurityGroupDTO>();
            foreach(var dto in dtos)
            {
                send.Add(new DTO.SecurityGroupDTO { Name = dto.Name});
            }
            await CNDS.SecurityGroups.Create(send);
        }
        public static async Task UpdateSecurityGroups(IEnumerable<CNDSSecurityGroupDTO> dtos)
        {
            IList<DTO.SecurityGroupDTO> send = new List<DTO.SecurityGroupDTO>();
            foreach (var dto in dtos)
            {
                send.Add(new DTO.SecurityGroupDTO { ID = dto.ID, Name = dto.Name });
            }
            await CNDS.SecurityGroups.Update(send);
        }
        public static async Task DeleteSecurityGroup(Guid id)
        {
            await CNDS.SecurityGroups.Delete(id);
        }

        public static async Task UpdateSecurityGroupUsers(CNDSSecurityGroupUserDTO dto)
        {
            var cndsUserID = Guid.Empty;
            var cndsDTOS = new DTO.SecurityGroupUserDTO();

            var response = await CNDSEntityUpdater.GetCNDSEntityIdentifiers(new[] { dto.UserID });
            cndsUserID = response.Select(user => user.EntityID).FirstOrDefault();

            await CNDS.SecurityGroupUsers.Update(new SecurityGroupUserDTO { UserID = cndsUserID, SecurityGroups = dto.SecurityGroups.Select(x => new DTO.SecurityGroupDTO { ID = x.ID, Name = x.Name }) });
        }

        public static async Task SetPermissions(IEnumerable<CNDSUpdateAssignedPermissionDTO> dtos)
        {
            IList<DTO.UpdateAssignedPermissionDTO> cndsDTO = new List<DTO.UpdateAssignedPermissionDTO>();
            foreach(var dto in dtos)
            {
                cndsDTO.Add(new UpdateAssignedPermissionDTO { Delete = dto.Delete, Allowed = dto.Allowed, PermissionID = dto.PermissionID, SecurityGroupID = dto.SecurityGroupID});
            }
            await CNDS.Permissions.SetPermissions(cndsDTO);
        }

        public static async Task<IEnumerable<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>> GetDataSourceDomains()
        {
            var domains = await CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'2'");
            List<Dns.DTO.CNDS.CNDSSearchMetaDataDTO> meta = new List<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>();
            foreach (var metadata in domains.Where(x => x.ParentDomainID == null))
            {
                meta.Add(GetSearchMetaData(metadata.ID, domains, ""));
            }
            return meta;
        }

        public static async Task<IEnumerable<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>> GetOrganizationDomains()
        {
            var domains = await CNDS.Domain.List("$filter=EntityType eq Lpp.CNDS.DTO.Enums.EntityType'0'");
            List<Dns.DTO.CNDS.CNDSSearchMetaDataDTO> meta = new List<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>();
            foreach (var metadata in domains.Where(x => x.ParentDomainID == null))
            {
                meta.Add(GetSearchMetaData(metadata.ID, domains, ""));
            }
            return meta;
        }

        public static Dns.DTO.CNDS.CNDSSearchMetaDataDTO GetSearchMetaData(Guid metadataID, IEnumerable<DomainDTO> availableMetadata, string previousName)
        {
            var currentMeta = availableMetadata.Where(x => x.ID == metadataID).FirstOrDefault();
            var metaData = new Dns.DTO.CNDS.CNDSSearchMetaDataDTO
            {
                ID = currentMeta.ID,
                Title = currentMeta.Title,
                SelectedDisplay = previousName != "" ? previousName + " - " + currentMeta.Title : currentMeta.Title
            };

            if (currentMeta.ParentDomainID.HasValue)
                metaData.ParentDomainID = currentMeta.ParentDomainID.Value;

            IList<Dns.DTO.CNDS.CNDSSearchMetaDataDTO> childMetadata = new List<Dns.DTO.CNDS.CNDSSearchMetaDataDTO>();
            IList<Dns.DTO.CNDS.DomainReferenceDTO> domainreferences = new List<Dns.DTO.CNDS.DomainReferenceDTO>();

            // Fills the Child Metadata where the Parent has Domain References
            if (currentMeta.References.Count() > 0)
            {
                foreach (var reference in currentMeta.References)
                {
                    domainreferences.Add(GetSearchMetadataReferences(currentMeta, reference));
                }
                metaData.References = domainreferences.OrderBy(p => p.Title);
            }

            //Fills the Child Metadata where a Parent Metadata Exists
            if (string.Equals(currentMeta.DataType, "container", StringComparison.OrdinalIgnoreCase) || string.Equals(currentMeta.DataType, "booleanGroup", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var child in availableMetadata.Where(x => x.ParentDomainID == currentMeta.ID))
                {
                    childMetadata.Add(GetSearchMetaData(child.ID, availableMetadata, metaData.SelectedDisplay));
                }
                metaData.ChildMetadata = childMetadata.OrderBy(p => p.Title);
            }
            return metaData;
        }

        public MetadataDTO GetMetadataChildren(Guid metadataID, IEnumerable<DomainDTO> availableMetadata, IEnumerable<DomainDataDTO> presetMetadata, IEnumerable<DomainDataDTO> visibility, Guid? entityID)
        {
            var currentMeta = availableMetadata.Where(x => x.ID == metadataID).FirstOrDefault();
            var metaData = new MetadataDTO
            {
                ID = currentMeta.ID,
                DataType = currentMeta.DataType,
                EntityID = entityID,
                EnumValue = currentMeta.EnumValue,
                EntityType = currentMeta.EntityType,
                IsMultiValue = currentMeta.IsMultiValue,
                Title = currentMeta.Title,
                DomainUseID = currentMeta.DomainUseID,
            };

            if(visibility != null)
            {
                metaData.Visibility = visibility.Where(x => x.ID == metadataID).Select(x => x.Visibility).FirstOrDefault();
            }

            IList<MetadataDTO> childMetadata = new List<MetadataDTO>();
            IList<Dns.DTO.CNDS.DomainReferenceDTO> domainreferences = new List<Dns.DTO.CNDS.DomainReferenceDTO>();
            //Filter for the Previously saved parent Metadata
            var current = presetMetadata.Where(x => x.DomainUseID == currentMeta.DomainUseID && x.DomainReferenceID == null);

            // Setting the Value if exists from the CNDS Database
            if (current.Count() > 0)
            {
                foreach (var cur in current)
                {
                    metaData.Value = cur.Value;
                }
            }

            if (metaData.DataType == "boolean" && current.Count() > 0)
                metaData.Value = "true";

            // Fills the Child Metadata where the Parent has Domain References
            if (currentMeta.References.Count() > 0)
            {
                if (!currentMeta.IsMultiValue)
                {
                    var test = (from f in presetMetadata
                                where f.DomainUseID == currentMeta.DomainUseID
                                select f);
                    metaData.Value = presetMetadata.Where(x => x.DomainUseID == currentMeta.DomainUseID).Select(x => x.DomainReferenceID.Value.ToString("D")).FirstOrDefault();
                }
                    
                foreach (var reference in currentMeta.References)
                {
                    domainreferences.Add(GetMetadataReferences(currentMeta, reference, presetMetadata));
                }
                metaData.References = domainreferences;
            }

            //Fills the Child Metadata where a Parent Metadata Exists
            if (string.Equals(metaData.DataType, "container", StringComparison.OrdinalIgnoreCase) || string.Equals(metaData.DataType, "booleanGroup", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var child in availableMetadata.Where(x => x.ParentDomainID == currentMeta.ID))
                {
                    childMetadata.Add(GetMetadataChildren(child.ID, availableMetadata, presetMetadata, visibility, entityID));
                }
                metaData.ChildMetadata = childMetadata;
            }
            return metaData;
        }

        public static Lpp.Dns.DTO.CNDS.DomainDTO GetDomainList(Guid metadataID, IEnumerable<Lpp.CNDS.DTO.DomainDTO> presetMetadata)
        {
            var currentMeta = presetMetadata.Where(x => x.ID == metadataID).FirstOrDefault();
            var metaData = new Lpp.Dns.DTO.CNDS.DomainDTO
            {
                ID = currentMeta.ID,
                DataType = currentMeta.DataType,
                EnumValue = currentMeta.EnumValue,
                EntityType = currentMeta.EntityType.Value,
                IsMultiValue = currentMeta.IsMultiValue,
                Title = currentMeta.Title,
                DomainUseID = currentMeta.DomainUseID.Value,
            };
            IList<Lpp.Dns.DTO.CNDS.DomainDTO> childMetadata = new List<Lpp.Dns.DTO.CNDS.DomainDTO>();
            IList<Dns.DTO.CNDS.DomainReferenceDTO> domainreferences = new List<Dns.DTO.CNDS.DomainReferenceDTO>();

            var children = presetMetadata.Where(x => x.ParentDomainID == metadataID).ToList();
            foreach (var child in children)
            {
                childMetadata.Add(GetDomainList(child.ID, presetMetadata));
            }
            foreach (var refMetadata in currentMeta.References)
            {
                domainreferences.Add(new Dns.DTO.CNDS.DomainReferenceDTO {
                    ID = refMetadata.ID,
                    DomainID = refMetadata.DomainID,
                    ParentDomainReferenceID = refMetadata.ParentDomainReferenceID,
                    Title = refMetadata.Title,
                    Description = refMetadata.Description
                });
            }


            metaData.Children = childMetadata;
            metaData.DomainReferences = domainreferences;

       
            return metaData;
        }

        private Dns.DTO.CNDS.DomainReferenceDTO GetMetadataReferences(DomainDTO currentMetadata, DomainReferenceDTO refMetadata, IEnumerable<DomainDataDTO> presetMetadata)
        {
            var reference = new Dns.DTO.CNDS.DomainReferenceDTO
            {
                ID = refMetadata.ID,
                DomainID = refMetadata.DomainID,
                ParentDomainReferenceID = refMetadata.ParentDomainReferenceID,
                Title = refMetadata.Title,
                Description = refMetadata.Description
            };
            var current = presetMetadata.Where(om => om.DomainUseID == currentMetadata.DomainUseID && om.DomainReferenceID == refMetadata.ID).OrderBy(om => om.SequenceNumber);
            if (current.Count() > 0 && currentMetadata.IsMultiValue)
            {
                foreach (var cur in current)
                {
                    // IF the Title is Other fill the saved Data otherwise set the Value to True to indicate Checked
                    if (string.Equals(refMetadata.Title, "Other", StringComparison.OrdinalIgnoreCase))
                    {
                        reference.Value = cur.Value;
                    }
                    else
                    {
                        reference.Value = "true";
                    }

                }
            }
            return reference;
        }

        private static Dns.DTO.CNDS.DomainReferenceDTO GetSearchMetadataReferences(DomainDTO currentMetadata, DomainReferenceDTO refMetadata)
        {
            var reference = new Dns.DTO.CNDS.DomainReferenceDTO
            {
                ID = refMetadata.ID,
                DomainID = refMetadata.DomainID,
                ParentDomainReferenceID = refMetadata.ParentDomainReferenceID,
                Title = refMetadata.Title,
                Description = refMetadata.Description
            };
            
            return reference;
        }
        private static IEnumerable<DomainDataDTO> RecusiveDomainVisibility(MetadataDTO parent)
        {
            var metadatas = new List<DomainDataDTO>();
            metadatas.Add(new DomainDataDTO {
                EntityID = parent.EntityID,
                DomainUseID = parent.DomainUseID.Value,
                ID = parent.ID,
                Visibility = parent.Visibility
            });
            foreach(var child in parent.ChildMetadata)
            {
                metadatas.AddRange(RecusiveDomainVisibility(child));
            }
            return metadatas;
        } 

    }
}