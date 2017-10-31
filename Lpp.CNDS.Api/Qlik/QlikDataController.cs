using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Lpp.CNDS.Data;
using Lpp.Utilities.WebSites.Controllers;

namespace Lpp.CNDS.Api.Qlik
{
    public class QlikDataController : LppApiController<DataContext>
    {
        /// <summary>
        /// Returns all DataSources with Domain definitions and data values.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DTO.QlikData.DataSourceWithDomainDataItemDTO> DataSourcesWithDomainData()
        {
            var query = from ds in DataContext.DataSources
                    from def in (
                        from d in DataContext.Domains.Where(x => x.Deleted == false)
                        join du in DataContext.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                        join domainReference in DataContext.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                        from dr in domainReferences.DefaultIfEmpty()
                        where du.EntityType == DTO.Enums.EntityType.DataSource
                        select new
                        {
                            DomainUseID = du.ID,
                            DomainID = du.DomainID,
                            ParentDomainID = d.ParentDomainID,
                            DomainTitle = d.Title,
                            DomainIsMultiValueSelect = d.IsMultiValue,
                            DomainDataType = d.DataType,
                            DomainReferenceID = (Guid?)dr.ID,
                            DomainReferenceTItle = dr.Title,
                            DomainReferenceDescription = dr.Description,
                            DomainReferenceValue = dr.Value
                        }
                    )
                    join org in DataContext.Organizations on ds.OrganizationID equals org.ID
                    let domData = DataContext.DomainDatas.OfType<DataSourceDomainData>().Where(dat => ds.ID == dat.DataSourceID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                    let domVis = ds.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                    where ds.Deleted == false
                    select new DTO.QlikData.DataSourceWithDomainDataItemDTO
                    {
                        NetworkID = org.NetworkID,
                        Network = org.Network.Name,
                        NetworkUrl = org.Network.Url,
                        OrganizationID = ds.OrganizationID,
                        Organization = org.Name,
                        OrganizationAcronym = org.Acronym,
                        ParentOrganizationID = org.ParentOrganizationID,
                        DataSourceID = ds.ID,
                        DataSource = ds.Name,
                        DataSourceAcronym = ds.Acronym,
                        DataSourceAdapterSupportedID = ds.AdapterSupportedID,
                        DataSourceAdapterSupported = ds.AdapterSupported.Name, 
                        SupportsCrossNetworkRequests = ds.AdapterSupportedID.HasValue, 
                        DomainUseID = def.DomainUseID,
                        DomainID = def.DomainID,
                        ParentDomainID = def.ParentDomainID,
                        DomainTitle = def.DomainTitle,
                        DomainIsMultiValueSelect = def.DomainIsMultiValueSelect,
                        DomainDataType = def.DomainDataType,
                        DomainReferenceID = def.DomainReferenceID,
                        DomainReferenceTitle = def.DomainReferenceTItle,
                        DomainReferenceDescription = def.DomainReferenceDescription,
                        DomainReferenceValue = def.DomainReferenceValue,    
                        DomainDataValue = def.DomainDataType == "boolean" ? (domData.ID == null ? "false" : "true") : def.DomainDataType == "booleanGroup" ? (domData.ID == null ? "false" : "true") : domData.Value,
                        DomainDataDomainReferenceID = domData.DomainReferenceID,
                        DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                    };

            return query;
        }

        /// <summary>
        /// Returns all Users with Domain definitions and data values.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DTO.QlikData.UserWithDomainDataItemDTO> UsersWithDomainData()
        {
            var query = from user in DataContext.Users
                        from def in (
                            from d in DataContext.Domains.Where(x => x.Deleted == false)
                            join du in DataContext.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                            join domainReference in DataContext.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                            from dr in domainReferences.DefaultIfEmpty()
                            where du.EntityType == DTO.Enums.EntityType.User
                            select new
                            {
                                DomainUseID = du.ID,
                                DomainID = du.DomainID,
                                ParentDomainID = d.ParentDomainID,
                                DomainTitle = d.Title,
                                DomainIsMultiValueSelect = d.IsMultiValue,
                                DomainDataType = d.DataType,
                                DomainReferenceID = (Guid?)dr.ID,
                                DomainReferenceTItle = dr.Title,
                                DomainReferenceDescription = dr.Description,
                                DomainReferenceValue = dr.Value
                            }
                        )
                        join org in DataContext.Organizations on user.OrganizationID equals org.ID
                        let domData = DataContext.DomainDatas.OfType<UserDomainData>().Where(dat => user.ID == dat.UserID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                        let domVis = user.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                        where user.Deleted == false && user.OrganizationID.HasValue
                        select new DTO.QlikData.UserWithDomainDataItemDTO
                        {
                            NetworkID = org.NetworkID,
                            Network = org.Network.Name,
                            NetworkUrl = org.Network.Url,
                            OrganizationID = org.ID,
                            Organization = org.Name,
                            OrganizationAcronym = org.Acronym,
                            ParentOrganizationID = org.ParentOrganizationID,
                            UserID = user.ID,
                            UserName = user.UserName,
                            UserSalutation = user.Salutation,
                            UserFirstName = user.FirstName,
                            UserMiddleName = user.MiddleName,
                            UserLastName = user.LastName,
                            UserEmailAddress = user.EmailAddress,
                            UserPhoneNumber = user.PhoneNumber,
                            UserFaxNumber = user.FaxNumber,
                            UserIsActive = user.Active,
                            DomainUseID = def.DomainUseID,
                            DomainID = def.DomainID,
                            ParentDomainID = def.ParentDomainID,
                            DomainTitle = def.DomainTitle,
                            DomainIsMultiValueSelect = def.DomainIsMultiValueSelect,
                            DomainDataType = def.DomainDataType,
                            DomainReferenceID = def.DomainReferenceID,
                            DomainReferenceTitle = def.DomainReferenceTItle,
                            DomainReferenceDescription = def.DomainReferenceDescription,
                            DomainReferenceValue = def.DomainReferenceValue,
                            DomainDataValue = def.DomainDataType == "boolean" ? (domData.ID == null ? "false" : "true") : def.DomainDataType == "booleanGroup" ? (domData.ID == null ? "false" : "true") : domData.Value,
                            DomainDataDomainReferenceID = domData.DomainReferenceID,
                            DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                        };

            return query;
        }

        /// <summary>
        /// Returns all Organizations with Domain definitions and data values.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DTO.QlikData.OrganizationWithDomainDataItemDTO> OrganizationsWithDomainData()
        {
            var db = DataContext;
            var query = from org in db.Organizations
                        from def in (
                            from d in db.Domains.Where(x => x.Deleted == false)
                            join du in db.DomainUses.Where(x => x.Deleted == false) on d.ID equals du.DomainID
                            join domainReference in db.DomainReferences.Where(x => x.Deleted == false) on d.ID equals domainReference.DomainID into domainReferences
                            from dr in domainReferences.DefaultIfEmpty()
                            where du.EntityType == DTO.Enums.EntityType.Organization
                            select new
                            {
                                DomainUseID = du.ID,
                                DomainID = du.DomainID,
                                ParentDomainID = d.ParentDomainID,
                                DomainTitle = d.Title,
                                DomainIsMultiValueSelect = d.IsMultiValue,
                                DomainDataType = d.DataType,
                                DomainReferenceID = (Guid?)dr.ID,
                                DomainReferenceTItle = dr.Title,
                                DomainReferenceDescription = dr.Description,
                                DomainReferenceValue = dr.Value
                            }
                        )
                        let domData = db.DomainDatas.OfType<OrganizationDomainData>().Where(dat => org.ID == dat.OrganizationID && def.DomainUseID == dat.DomainUseID && def.DomainReferenceID == dat.DomainReferenceID).FirstOrDefault()
                        let domVis = org.DomainAccess.Where(da => da.DomainUseID == def.DomainUseID).FirstOrDefault()
                        where org.Deleted == false
                        select new DTO.QlikData.OrganizationWithDomainDataItemDTO
                        {
                            NetworkID = org.NetworkID,
                            Network = org.Network.Name,
                            NetworkUrl = org.Network.Url,
                            OrganizationID = org.ID,
                            Organization = org.Name,
                            OrganizationAcronym = org.Acronym,
                            ParentOrganizationID = org.ParentOrganizationID,
                            DomainUseID = def.DomainUseID,
                            DomainID = def.DomainID,
                            ParentDomainID = def.ParentDomainID,
                            DomainTitle = def.DomainTitle,
                            DomainIsMultiValueSelect = def.DomainIsMultiValueSelect,
                            DomainDataType = def.DomainDataType,
                            DomainReferenceID = def.DomainReferenceID,
                            DomainReferenceTitle = def.DomainReferenceTItle,
                            DomainReferenceDescription = def.DomainReferenceDescription,
                            DomainReferenceValue = def.DomainReferenceValue,
                            DomainDataValue = def.DomainDataType == "boolean" ? (domData.ID == null ? "false" : "true") : def.DomainDataType == "booleanGroup" ? (domData.ID == null ? "false" : "true") : domData.Value,
                            DomainDataDomainReferenceID = domData.DomainReferenceID,
                            DomainAccessValue = domVis != null ? (int)domVis.AccessType : 0
                        };
            
            return query;
        }

        /// <summary>
        /// Returns information about the active users in CNDS - they cannot be deleted or inactive.
        /// The user must belong to an organization.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<DTO.QlikData.ActiveUserDTO> ActiveUsers()
        {
            var query = from u in DataContext.Users
                        let uent = DataContext.NetworkEntities.Where(ne => ne.ID == u.ID).FirstOrDefault()
                        let oent = DataContext.NetworkEntities.Where(ne => ne.ID == u.OrganizationID).FirstOrDefault()
                        where u.Deleted == false && u.Active && u.OrganizationID.HasValue
                        select new DTO.QlikData.ActiveUserDTO {
                            ID = u.ID,
                            PmnID = uent.NetworkEntityID,
                            UserName = u.UserName,
                            OrganizationID = u.OrganizationID.Value,
                            Organization = u.Organization.Name,
                            PmnOrganizationID = oent.NetworkEntityID,
                            NetworkID = u.NetworkID,
                            Network = u.Network.Name
                        };

            return query;
        }
    }
}
