using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
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
    /// <summary>
    /// 
    /// </summary>
    public class CNDSRequestTypesController : LppApiController<DataContext>
    {
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(CNDSNetworksController));
        static readonly string CNDSurl;
        readonly Lpp.CNDS.ApiClient.CNDSClient CNDSApi;

        static CNDSRequestTypesController()
        {
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public CNDSRequestTypesController()
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
        /// Gets all the available routes that are possible for CNDS.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSProjectRequestTypeDataMartDTO>> ListAvailableRequestTypes()
        {

            var q = await (from pdm in DataContext.ProjectDataMarts
                           join p in DataContext.Projects on pdm.ProjectID equals p.ID
                           join dm in DataContext.DataMarts on pdm.DataMartID equals dm.ID
                           join prt in DataContext.ProjectRequestTypes on p.ID equals prt.ProjectID
                           let dmAcl = DataContext.DataMartRequestTypeAcls.Where(a => a.DataMartID == dm.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                           let prtACL = DataContext.ProjectRequestTypeAcls.Where(a => a.ProjectID == p.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                           let pdmrtACL = DataContext.ProjectDataMartRequestTypeAcls.Where(a => a.ProjectID == p.ID && a.DataMartID == dm.ID && a.RequestTypeID == prt.RequestTypeID).Select(a => a.Permission)
                           where
                           p.Active && !p.Deleted && (!p.EndDate.HasValue || p.EndDate.Value > DateTime.UtcNow) && (p.StartDate <= DateTime.UtcNow)
                           && dm.AdapterID.HasValue && dm.Deleted == false
                           && (
                               (dmAcl.Any() || prtACL.Any() || pdmrtACL.Any()) &&
                               (dmAcl.All(a => a > 0) && prtACL.All(a => a > 0) && pdmrtACL.All(a => a > 0))
                           )
                           orderby p.Name, prt.RequestType.Name, dm.Name
                           select new CNDSProjectRequestTypeDataMartDTO
                           {
                               ProjectID = p.ID,
                               Project = p.Name,
                               DataMartID = pdm.DataMartID,
                               DataMart = dm.Name,
                               RequestTypeID = prt.RequestTypeID,
                               RequestType = prt.RequestType.Name
                           }).ToArrayAsync();

            return q;
        }

        [HttpGet]
        public async Task<IEnumerable<CNDSMappingItemDTO>> ListAvailableNetworkRoutes()
        {
            var response = await CNDSApi.RequestTypeMapping.ListMappingItems();

            //networks
            var items = response.GroupBy(k => new { k.NetworkID, k.Network })
                                .Select(n => new CNDSMappingItemDTO
                                {
                                    ID = n.Key.NetworkID,
                                    Name = n.Key.Network,
                                    //projects
                                    Children = n.GroupBy(k => new { k.ProjectID, k.Project })
                                                .Select(k => new CNDSMappingItemDTO
                                                {
                                                    ID = k.Key.ProjectID,
                                                    Name = k.Key.Project,
                                                    //request types
                                                    Children = k.GroupBy(v => new { v.RequestTypeID, v.RequestType })
                                                                .Select(u => new CNDSMappingItemDTO
                                                                {
                                                                    ID = u.Key.RequestTypeID,
                                                                    Name = u.Key.RequestType,
                                                                    //datamarts
                                                                    Children = u.Select(dm => new CNDSMappingItemDTO { ID = dm.DataMartID, Name = dm.DataMart })
                                                                })
                                                })
                                });

            return items;
        }

        /// <summary>
        /// Validates the specified network requesttype mappings.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<CNDSNetworkRequestTypeMappingDTO>> ValidateNetworkRequestTypeMappings([FromBody]IEnumerable<CNDSNetworkRequestTypeMappingDTO> dtos)
        {
            var mappings = dtos.Select((dto) => new Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO
            {
                ID = dto.ID,
                NetworkID = dto.NetworkID,
                ProjectID = dto.ProjectID,
                RequestTypeID = dto.RequestTypeID,
                Routes = Enumerable.Empty<NetworkRequestTypeDefinitionDTO>()
            });

            var result = (await CNDSApi.RequestTypeMapping.ValidateMappings(mappings)).AsQueryable().Map<NetworkRequestTypeMappingDTO, CNDSNetworkRequestTypeMappingDTO>();
            return result;
        }

        /// <summary>
        /// Creates a new CNDS Network RequestType mapping.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task CreateNetworkRequestTypeMapping([FromBody]CNDSNetworkRequestTypeMappingDTO dto)
        {
            var mapping = new Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO {
                ID = dto.ID,
                NetworkID = dto.NetworkID,
                ProjectID = dto.ProjectID,
                RequestTypeID = dto.RequestTypeID,
                Routes = dto.Routes.Select(r => new Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO { ID = r.ID, NetworkID = r.NetworkID, ProjectID = r.ProjectID, RequestTypeID = r.RequestTypeID, DataSourceID = r.DataSourceID }).ToArray()
            };

            await CNDSApi.RequestTypeMapping.CreateOrUpdateMapping(mapping);
        }

        /// <summary>
        /// Gets all the defined network requesttype mappings.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSNetworkRequestTypeMappingDTO>> ListMappings()
        {
            var result = (await CNDSApi.RequestTypeMapping.ListMappings()).AsQueryable().Map<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeMappingDTO>().ToArray();
            return result;
        }

        /// <summary>
        /// Gets the details for a specific network requesttype mapping.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CNDSNetworkRequestTypeMappingDTO> GetNetworkRequestTypeMapping(Guid id)
        {
            var result = (await CNDSApi.RequestTypeMapping.GetRequestTypeMapping(id)).Map< Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeMappingDTO>();
            return result;
        }

        /// <summary>
        /// Deletes the specified requesttype mapping.
        /// </summary>
        /// <param name="mappingID">The definition IDs that make up the mapping key.</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task DeleteMapping([FromUri]IEnumerable<Guid> mappingID)
        {
            var mappingIDs = mappingID.ToArray();
            if (mappingIDs.Length != 1)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid mapping ID, specify the ID of the network requesttype mapping to delete."));
            }

            try
            {
                await CNDSApi.RequestTypeMapping.DeleteMapping(mappingIDs);
            } catch (HttpResponseException hex)
            {
                Logger.Error("Error deleting requesttype mapping:" + mappingIDs[0] + ", " + mappingIDs[1], hex);
                throw new HttpResponseException(hex.Response);
            }

        }

        /// <summary>
        /// Gets the cnds requesttype mappings that are applicable to the current network and the current user has permission to create a request for.
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<CNDSExternalRequestTypeSelectionItemDTO>> AvailableRequestTypesForNewRequestOriginal()
        {
            //get all the requesttypes the user has permission to create a request for

            var projects = from p in DataContext.Secure<Project>(Identity, Lpp.Dns.DTO.Security.PermissionIdentifiers.Request.Edit)
                           where p.Active && !p.Deleted && (!p.EndDate.HasValue || p.EndDate.Value > DateTime.UtcNow) && (p.StartDate <= DateTime.UtcNow)
                           select p;

            //only WF requests are supported, so the user only needs to have the  "Edit Task" permission on the "New Request" activity for the workflow (see ProjectsController.GetAvailableRequestTypeForNewRequest)
            var requestTypes = await (from rt in DataContext.ProjectRequestTypes
                                      join project in projects on rt.ProjectID equals project.ID
                                      let userID = Identity.ID
                                      let networkID = DataContext.Networks.Where(n => n.Name != "Aqueduct").Select(n => n.ID).FirstOrDefault()
                                      let wAcls = DataContext.ProjectRequestTypeWorkflowActivities
                                                  .Where(a => a.RequestTypeID == rt.RequestTypeID && a.WorkflowActivity.Start == true
                                                  && a.SecurityGroup.Users.Any(sgu => sgu.UserID == userID)
                                                  && a.ProjectID == project.ID
                                                  && a.PermissionID == DTO.Security.PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.EditTask.ID
                                                  )
                                      where (rt.RequestType.WorkflowID.HasValue && wAcls.Any(a => a.Allowed) && wAcls.All(a => a.Allowed))
                                      select new CNDSExternalRequestTypeSelectionItemDTO
                                      {
                                          NetworkID = networkID,
                                          ProjectID = project.ID,
                                          Project = project.Name,
                                          RequestTypeID = rt.RequestTypeID,
                                          RequestType = rt.RequestType.Name
                                      }).ToArrayAsync();

            //now get the CNDS network requesttype mappings that contain the requesttypes
            var networkMappings = await CNDSApi.RequestTypeMapping.FindMappings(requestTypes.Select(rt => new Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO { NetworkID = rt.NetworkID, ProjectID = rt.ProjectID, RequestTypeID = rt.RequestTypeID }).ToArray());
            
            foreach(var item in requestTypes)
            {
                var sourceMapping = networkMappings.Where(m => m.NetworkID == item.NetworkID && m.ProjectID == item.ProjectID && m.RequestTypeID == item.RequestTypeID).FirstOrDefault();
                if(sourceMapping != null)
                {
                    item.MappingDefinitions = sourceMapping.Routes.Select(rt => new CNDSNetworkProjectRequestTypeDataMartDTO { DefinitionID = rt.ID.Value, Network = rt.Network, NetworkID = rt.NetworkID, Project = rt.Project, ProjectID = rt.ProjectID, RequestType = rt.RequestType, RequestTypeID = rt.RequestTypeID, DataMart = rt.DataSource, DataMartID = rt.DataSourceID });
                }
            }

            return requestTypes.OrderBy(rt => rt.Project).ThenByDescending(rt => rt.MappingDefinitions.Count()).ThenBy(rt => rt.RequestType);
        }

        /// <summary>
        /// Gets the valid requesttypes that the user can create for the specified datasources. The returned requesttypes will support at least one local or external datasource.
        /// </summary>
        /// <param name="id">The ids of the datasources that are desired to be routed to.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSSourceRequestTypeDTO>> AvailableRequestTypesForNewRequest([FromUri]IEnumerable<Guid> id)
        {
            var externalDataSourceInformation = await CNDSApi.DataSources.ListExtended("$filter=" + string.Join(" or ", id.Select(i => string.Format("ID eq {0}", i.ToString("D")))));

            var localNetwork = await DataContext.Networks.Where(n => n.Name != "Aqueduct").Select(n => new { n.ID, n.Name }).FirstOrDefaultAsync();

            var networkResponse = await CNDSApi.Networks.LookupEntities(localNetwork.ID, id);
            var content = await networkResponse.Content.ReadAsStringAsync();

            var localDataMartIdentifiers = Newtonsoft.Json.JsonConvert.DeserializeObject<Lpp.Utilities.BaseResponse<NetworkEntityIdentifier>>(content);
            var localDataMartIds = localDataMartIdentifiers.results.Select(i => i.NetworkEntityID).ToArray();

            var projects = await (from p in DataContext.Secure<Project>(Identity, Lpp.Dns.DTO.Security.PermissionIdentifiers.Request.Edit)
                                  where p.Active && !p.Deleted && (!p.EndDate.HasValue || p.EndDate.Value > DateTime.UtcNow) && (p.StartDate <= DateTime.UtcNow)
                                  select p.ID).ToArrayAsync();

            var requestTypes = await (from rt in DataContext.ProjectRequestTypes
                                      join project in DataContext.Projects on rt.ProjectID equals project.ID
                                      let userID = Identity.ID
                                      let netID = localNetwork.ID
                                      let netName = localNetwork.Name
                                      let wAcls = DataContext.ProjectRequestTypeWorkflowActivities
                                              .Where(a => a.RequestTypeID == rt.RequestTypeID && a.WorkflowActivity.Start == true
                                              && a.SecurityGroup.Users.Any(sgu => sgu.UserID == userID)
                                              && a.ProjectID == project.ID
                                              && a.PermissionID == DTO.Security.PermissionIdentifiers.ProjectRequestTypeWorkflowActivities.EditTask.ID
                                              )
                                      where (rt.RequestType.WorkflowID.HasValue && wAcls.Any(a => a.Allowed) && wAcls.All(a => a.Allowed))
                                      && projects.Contains(project.ID)
                                      select new CNDSSourceRequestTypeDTO
                                      {
                                          ProjectID = project.ID,
                                          Project = project.Name,
                                          RequestTypeID = rt.RequestTypeID,
                                          RequestType = rt.RequestType.Name,
                                          LocalRoutes = (
                                                                      from dm in project.DataMarts
                                                                      let rtAcls = DataContext.ProjectRequestTypeAcls.Where(a => a.RequestTypeID == rt.RequestTypeID && a.ProjectID == dm.ProjectID && a.SecurityGroup.Users.Any(u => u.UserID == userID)).Select(a => a.Permission)
                                                                                  .Concat(DataContext.DataMartRequestTypeAcls.Where(a => a.RequestTypeID == rt.RequestTypeID && a.DataMartID == dm.DataMartID && a.SecurityGroup.Users.Any(sgu => sgu.UserID == userID)).Select(a => a.Permission))
                                                                                  .Concat(DataContext.ProjectDataMartRequestTypeAcls.Where(a => a.RequestTypeID == rt.RequestTypeID && a.ProjectID == dm.ProjectID && a.DataMartID == dm.DataMartID && a.SecurityGroup.Users.Any(sgu => sgu.UserID == userID)).Select(a => a.Permission))
                                                                      where localDataMartIds.Contains(dm.DataMartID) && (rtAcls.Any(a => a != DTO.Enums.RequestTypePermissions.Deny) && rtAcls.All(a => a != DTO.Enums.RequestTypePermissions.Deny))
                                                                      orderby dm.DataMart.Name
                                                                      select new CNDSSourceRequestTypeRoutingDTO
                                                                      {
                                                                          IsLocal = true,
                                                                          NetworkID = netID,
                                                                          Network = netName,
                                                                          ProjectID = project.ID,
                                                                          Project = project.Name,
                                                                          RequestTypeID = rt.RequestTypeID,
                                                                          RequestType = rt.RequestType.Name,
                                                                          DataMartID = dm.DataMartID,
                                                                          DataMart = dm.DataMart.Name
                                                                      }
                                                                    )
                                      }
                                          ).ToArrayAsync();




            var networkMappings = await CNDSApi.RequestTypeMapping.FindMappings(requestTypes.Select(rt => new Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO { NetworkID = localNetwork.ID, ProjectID = rt.ProjectID, RequestTypeID = rt.RequestTypeID }).ToArray());

            foreach (var item in requestTypes)
            {
                var sourceMapping = networkMappings.Where(m => m.NetworkID == localNetwork.ID && m.ProjectID == item.ProjectID && m.RequestTypeID == item.RequestTypeID).FirstOrDefault();
                if (sourceMapping != null)
                {
                    item.ExternalRoutes = sourceMapping.Routes.Where(rt => id.Contains(rt.DataSourceID))
                                                .Select(rt => new CNDSSourceRequestTypeRoutingDTO
                                                {
                                                    MappingDefinitionID = rt.ID,
                                                    IsLocal = rt.NetworkID == localNetwork.ID && rt.ProjectID == item.ProjectID,
                                                    NetworkID = rt.NetworkID,
                                                    Network = rt.Network,
                                                    ProjectID = rt.ProjectID,
                                                    Project = rt.Project,
                                                    RequestTypeID = rt.RequestTypeID,
                                                    RequestType = rt.RequestType,
                                                    DataMartID = rt.DataSourceID,
                                                    DataMart = rt.DataSource
                                                }).OrderBy(rt => rt.Network).ThenBy(rt => rt.Project).ThenBy(rt => rt.DataMart).ToArray();
                }
                else
                {
                    item.ExternalRoutes = Array.Empty<CNDSSourceRequestTypeRoutingDTO>();
                }

                item.InvalidRoutes = externalDataSourceInformation
                                        .Where(ds => !item.ExternalRoutes.Any(d => d.NetworkID == ds.NetworkID && ds.ID == d.DataMartID)
                                        && !item.LocalRoutes.Any(d => d.NetworkID == localNetwork.ID && localDataMartIdentifiers.results.Any(i => i.EntityID == ds.ID && i.NetworkEntityID == d.DataMartID)))
                                        .Select(ds => new CNDSSourceRequestTypeRoutingDTO { NetworkID = ds.NetworkID, Network = ds.Network, DataMartID = ds.ID.Value, DataMart = ds.Name, IsLocal = false })
                                        .Distinct().OrderBy(ds => ds.Network).ThenBy(ds => ds.DataMart);

            }

            //filter out any that do not have any local or external routes
            requestTypes = requestTypes.Where(rt => rt.LocalRoutes.Any() || rt.ExternalRoutes.Any()).ToArray();

            return requestTypes;
        }

        /// <summary>
        /// List all Network RequestType Definitions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CNDSNetworkRequestTypeDefinitionDTO>> ListNetworkRequestTypeDefinitions()
        {
            var result = await CNDSApi.RequestTypeMapping.ListRequestTypeDefinitions();

            return result.AsQueryable().Map<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeDefinitionDTO>();
        }

        /// <summary>
        /// Gets the specified CNDS network requestype definition.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CNDSNetworkRequestTypeDefinitionDTO> GetNetworkRequestTypeDefinition(Guid id)
        {
            var result = await CNDSApi.RequestTypeMapping.GetRequestTypeDefinition(id);
            return result.Map<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO, CNDSNetworkRequestTypeDefinitionDTO>();
        }

        /// <summary>
        /// Validates the specified network requesttype definitions, confirms there are no duplicate definitions in the system.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<CNDSNetworkRequestTypeDefinitionDTO>> ValidateNetworkRequestTypeDefinition([FromBody]IEnumerable<CNDSNetworkRequestTypeDefinitionDTO> dtos)
        {
            var cndsDTO = dtos.Select(d => new Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO
            {
                ID = d.ID,
                NetworkID = d.NetworkID,
                ProjectID = d.ProjectID,
                RequestTypeID = d.RequestTypeID,
                DataSourceID = d.DataSourceID
            });

            var result = (await CNDSApi.RequestTypeMapping.ValidateNetworkRequestTypeDefinitions(cndsDTO)).AsQueryable().Map<NetworkRequestTypeDefinitionDTO, CNDSNetworkRequestTypeDefinitionDTO>();

            return result;
        }

        /// <summary>
        /// Creates or updates the specified Network RequestType Definitions.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<CNDSNetworkRequestTypeDefinitionDTO>> CreateOrUpdateNetworkRequestTypeDefinition([FromBody]IEnumerable<CNDSNetworkRequestTypeDefinitionDTO> dtos)
        {
            var cndsDTO = dtos.Select(d => new Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO
            {
                ID = d.ID,
                NetworkID = d.NetworkID,
                ProjectID = d.ProjectID,
                RequestTypeID = d.RequestTypeID,
                DataSourceID = d.DataSourceID
            });

            var result = await CNDSApi.RequestTypeMapping.CreateOrUpdateNetworkRequestTypeDefinition(cndsDTO);

            return result.AsQueryable().Map<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeDefinitionDTO>();
        }

        /// <summary>
        /// Deletes the specified Network RequestType Definitions.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task DeleteNetworkRequestTypeDefinitions([FromUri] IEnumerable<Guid> ID)
        {
            await CNDSApi.RequestTypeMapping.DeleteNetworkRequestTypeDefinitions(ID);
        }


        internal class CNDSNetworkRequestTypeMappingDTOMappingConfiguration : Lpp.Utilities.EntityMappingConfiguration<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeMappingDTO>
        {
            public override Expression<Func<NetworkRequestTypeMappingDTO, CNDSNetworkRequestTypeMappingDTO>> MapExpression
            {
                get
                {
                    return (m) => new CNDSNetworkRequestTypeMappingDTO {
                        ID = m.ID,
                        Timestamp = m.Timestamp,
                        NetworkID = m.NetworkID,
                        Network = m.Network,
                        ProjectID = m.ProjectID,
                        Project = m.Project,
                        RequestTypeID = m.RequestTypeID,
                        RequestType = m.RequestType,
                        Routes = m.Routes != null ? m.Routes.Select(r => new CNDSNetworkRequestTypeDefinitionDTO {
                            ID = r.ID,
                            Timestamp = r.Timestamp,
                            NetworkID = r.NetworkID,
                            Network = r.Network,
                            ProjectID = r.ProjectID,
                            Project = r.Project,
                            RequestTypeID = r.RequestTypeID,
                            RequestType = r.RequestType,
                            DataSourceID = r.DataSourceID,
                            DataSource = r.DataSource
                        }) : Enumerable.Empty<CNDSNetworkRequestTypeDefinitionDTO>()
                    };
                }
            }
        }

        internal class CNDSNetworkRequestTypeDefinitionDTOMappingConfiguration : EntityMappingConfiguration<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO, Lpp.Dns.DTO.CNDSNetworkRequestTypeDefinitionDTO> {
            public override Expression<Func<NetworkRequestTypeDefinitionDTO, CNDSNetworkRequestTypeDefinitionDTO>> MapExpression
            {
                get
                {
                    return (d) => new CNDSNetworkRequestTypeDefinitionDTO {
                        ID = d.ID,
                        NetworkID = d.NetworkID,
                        Network = d.Network,
                        ProjectID = d.ProjectID,
                        Project = d.Project,
                        RequestTypeID = d.RequestTypeID,
                        RequestType = d.RequestType,
                        DataSourceID = d.DataSourceID,
                        DataSource = d.DataSource,
                        Timestamp = d.Timestamp
                    };
                }
            }
        }


        /// <summary>
        /// The CNDS and PMN ID link.
        /// </summary>
        internal class NetworkEntityIdentifier
        {
            /// <summary>
            /// The ID of the entity in the PMN instance.
            /// </summary>
            public Guid EntityID { get; set; }
            /// <summary>
            /// The ID of the entity in CNDS>
            /// </summary>
            public Guid NetworkEntityID { get; set; }
        }



    }
}