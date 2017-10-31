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
using LinqKit;
using Lpp.CNDS.Data;
using Lpp.CNDS.DTO;
using Lpp.Utilities.WebSites.Controllers;

namespace Lpp.CNDS.Api.Requests
{
    [AllowAnonymous]//TODO: security should be implemented and enforced for modifying mappings
    public class RequestTypeMappingController : LppApiController<DataContext>
    {
        const string UnableToDetermineMessage = "<<Unable to Determine>>";
        static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(RequestTypeMappingController));

        /// <summary>
        /// Get all the defined mappings.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<NetworkRequestTypeMappingDTO>> ListMappings()
        {
            var mappings = await (from nm in DataContext.NetworkRequestTypeMappings
                                  select new NetworkRequestTypeMappingDTO {
                                      ID = nm.ID,
                                      NetworkID = nm.NetworkID,
                                      Network = nm.Network.Name,
                                      ProjectID = nm.ProjectID,
                                      RequestTypeID = nm.RequestTypeID,
                                      Timestamp = nm.Timestamp,
                                      Routes = nm.NetworkRoutes.Select(rt => new NetworkRequestTypeDefinitionDTO {
                                          ID = rt.RequestTypeDefinition.ID,
                                          NetworkID = rt.RequestTypeDefinition.NetworkID,
                                          Network = rt.RequestTypeDefinition.Network.Name,
                                          ProjectID = rt.RequestTypeDefinition.ProjectID,
                                          RequestTypeID = rt.RequestTypeDefinition.RequestTypeID,
                                          DataSourceID = rt.RequestTypeDefinition.DataSourceID,
                                          DataSource = rt.RequestTypeDefinition.DataSource.Name,
                                          Timestamp = rt.RequestTypeDefinition.Timestamp
                                      })
                                  }).ToArrayAsync();

            if(mappings.Length == 0)
            {
                return Array.Empty<NetworkRequestTypeMappingDTO>();
            }

            //get the distinct network/project/requesttype definitions
            var networkProjectRequestTypes = mappings.Select(m => new { m.NetworkID, m.ProjectID, m.RequestTypeID })
                                                     .Concat(mappings.SelectMany(m => m.Routes.Select(r => new { r.NetworkID, r.ProjectID, r.RequestTypeID })))
                                                     .GroupBy(k => new { k.NetworkID, k.ProjectID, k.RequestTypeID })
                                                     .Select(g => new NetworkRequestTypeDefinitionDTO {
                                                         NetworkID = g.Key.NetworkID,
                                                         ProjectID = g.Key.ProjectID,
                                                         RequestTypeID = g.Key.RequestTypeID
                                                     })
                                                     .ToArray();

            var networkIDs = networkProjectRequestTypes.Select(n => n.NetworkID).Distinct().ToArray();

            var networks = await DataContext.Networks.AsNoTracking()
                                                .Where(n => networkIDs.Contains(n.ID))
                                                .Select(n => new { n.ID, n.ServiceUrl, n.ServicePassword, n.ServiceUserName })
                                                .ToArrayAsync();

            //get the project and requestType names for each combination
            foreach(var nprt in networkProjectRequestTypes.GroupBy(k => k.NetworkID))
            {
                var network = networks.FirstOrDefault(n => n.ID == nprt.Key);

                using (var pmnAPI = new Lpp.Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Utilities.Crypto.DecryptString(network.ServicePassword)))
                {
                    string odataFilter = "$filter=" + string.Join(" or ", nprt.Select(n => n.ProjectID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnProjects = await pmnAPI.Projects.List(odataFilter);

                    odataFilter = "$filter=" + string.Join(" or ", nprt.Select(n => n.RequestTypeID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnRequestTypes = await pmnAPI.RequestTypes.List(odataFilter);

                    foreach (var definition in nprt)
                    {
                        definition.Project = pmnProjects.Where(p => p.ID == definition.ProjectID).Select(p => p.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                        definition.RequestType = pmnRequestTypes.Where(rt => rt.ID == definition.RequestTypeID).Select(rt => rt.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                    }
                }

            }

            var projectRequestTypeDetailsMap = networkProjectRequestTypes.ToDictionary(k => string.Format("{0:D}-{1:D}-{2:D}", k.NetworkID, k.ProjectID, k.RequestTypeID));
            string key;
            //update the mapping dto details
            foreach(var map in mappings)
            {
                key = string.Format("{0:D}-{1:D}-{2:D}", map.NetworkID, map.ProjectID, map.RequestTypeID);
                NetworkRequestTypeDefinitionDTO detail;
                if (projectRequestTypeDetailsMap.TryGetValue(key, out detail))
                {
                    map.Project = detail.Project ?? UnableToDetermineMessage;
                    map.RequestType = detail.RequestType ?? UnableToDetermineMessage;
                }
                else
                {
                    map.Project = UnableToDetermineMessage;
                    map.RequestType = UnableToDetermineMessage;
                }

                foreach(var def in map.Routes)
                {
                    key = string.Format("{0:D}-{1:D}-{2:D}", def.NetworkID, def.ProjectID, def.RequestTypeID);
                    if (projectRequestTypeDetailsMap.TryGetValue(key, out detail))
                    {
                        def.Project = detail.Project ?? UnableToDetermineMessage;
                        def.RequestType = detail.RequestType ?? UnableToDetermineMessage;
                    }
                    else
                    {
                        def.Project = UnableToDetermineMessage;
                        def.RequestType = UnableToDetermineMessage;
                    }
                }

                map.Routes = map.Routes.OrderBy(r => r.Network).ThenBy(r => r.Project).ThenBy(r => r.RequestType).ThenBy(r => r.DataSource);                
            }

            mappings = mappings.OrderBy(m => m.Network).ThenBy(m => m.Project).ThenBy(m => m.RequestType).ToArray();

            return mappings;
        }

        /// <summary>
        /// Gets the details for a specific Network RequestType mapping.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<NetworkRequestTypeMappingDTO> GetRequestTypeMapping(Guid id)
        {
            var details = await DataContext.NetworkRequestTypeMappings.Where(m => m.ID == id)
                .Select(m => new
                {
                    DTO = new NetworkRequestTypeMappingDTO
                    {
                        ID = m.ID,
                        NetworkID = m.NetworkID,
                        Network = m.Network.Name,
                        ProjectID = m.ProjectID,
                        RequestTypeID = m.RequestTypeID,
                        Routes = m.NetworkRoutes.Select(rt => new NetworkRequestTypeDefinitionDTO { ID = rt.RequestTypeDefinitionID, NetworkID = rt.RequestTypeDefinition.NetworkID, Network = rt.RequestTypeDefinition.Network.Name, ProjectID = rt.RequestTypeDefinition.ProjectID, RequestTypeID = rt.RequestTypeDefinition.RequestTypeID, DataSourceID = rt.RequestTypeDefinition.DataSourceID, DataSource = rt.RequestTypeDefinition.DataSource.Name, Timestamp = rt.RequestTypeDefinition.Timestamp }),
                        Timestamp = m.Timestamp
                    },
                    Networks = (DataContext.NetworkRequestTypeMappings.Where(mm => mm.ID == id).Select(mm => new { mm.Network.ID, mm.Network.ServiceUrl, mm.Network.ServiceUserName, mm.Network.ServicePassword })
                                .Concat(DataContext.NetworkRequestTypeDefinitions.Where(d => d.NetworkRequestTypeMappingRoutes.Any(rt => rt.RequestTypeMappingID == id) && d.NetworkID != m.NetworkID).Select(d => new { d.Network.ID, d.Network.ServiceUrl, d.Network.ServiceUserName, d.Network.ServicePassword }))).Distinct()
                }).FirstOrDefaultAsync();
            
            if (details == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The requested network requesttype was not found."));
            }

            var networkProjectRequestTypes = (new[] { new { details.DTO.NetworkID, details.DTO.ProjectID, details.DTO.RequestTypeID } }.Concat(details.DTO.Routes.Select(r => new { r.NetworkID, r.ProjectID, r.RequestTypeID }))).Distinct();
            var networks = details.Networks.ToDictionary(k => k.ID);

            foreach (var grp in networkProjectRequestTypes.GroupBy(k => k.NetworkID))
            {
                var network = networks[grp.Key];
                using (var pmnAPI = new Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Utilities.Crypto.DecryptString(network.ServicePassword)))
                {
                    string odataFilter = "$filter=" + string.Join(" or ", grp.Select(n => n.ProjectID).Distinct().Select(pid => string.Format("ID eq {0:D}", pid)));
                    var pmnProjects = await pmnAPI.Projects.List(odataFilter);

                    odataFilter = "$filter=" + string.Join(" or ", grp.Select(n => n.RequestTypeID).Distinct().Select(rid => string.Format("ID eq {0:D}", rid)));
                    var pmnRequestTypes = await pmnAPI.RequestTypes.List(odataFilter);

                    if (grp.Key == details.DTO.NetworkID)
                    {
                        details.DTO.Project = pmnProjects.Where(p => p.ID == details.DTO.ProjectID).Select(p => p.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                        details.DTO.RequestType = pmnRequestTypes.Where(rt => rt.ID == details.DTO.RequestTypeID).Select(rt => rt.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                    }

                    foreach (var route in details.DTO.Routes.Where(rt => rt.NetworkID == grp.Key))
                    {
                        route.Project = pmnProjects.Where(p => p.ID == route.ProjectID).Select(p => p.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                        route.RequestType = pmnRequestTypes.Where(rt => rt.ID == route.RequestTypeID).Select(rt => rt.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                    }
                }
            }

            return details.DTO;
        }

        /// <summary>
        /// Gets the mappings for a specific network.
        /// </summary>
        /// <param name="id">The ID of the network.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<NetworkRequestTypeMappingDTO>> FindMappings([FromBody] IEnumerable<NetworkProjectRequestTypeDataMartDTO> requestTypes)
        {
            if (!requestTypes.Any())
                return Enumerable.Empty<NetworkRequestTypeMappingDTO>();

            Expression<Func<NetworkRequestTypeMapping, bool>> predicate = null;

            foreach (var rt in requestTypes)
            {
                if (predicate == null)
                {
                    predicate = (m) => (m.NetworkID == rt.NetworkID && m.ProjectID == rt.ProjectID && m.RequestTypeID == rt.RequestTypeID);
                }
                else
                {
                    predicate = predicate.Or((m) => (m.NetworkID == rt.NetworkID && m.ProjectID == rt.ProjectID && m.RequestTypeID == rt.RequestTypeID));
                }
            }

            var mappings = await DataContext.NetworkRequestTypeMappings.Where(m => m.NetworkRoutes.Any()).AsExpandable()
                            .Where(predicate.Expand())
                            .Select(m => new NetworkRequestTypeMappingDTO {
                                ID  = m.ID,
                                NetworkID = m.NetworkID,
                                Network = m.Network.Name,
                                ProjectID = m.ProjectID,
                                RequestTypeID = m.RequestTypeID,
                                Timestamp = m.Timestamp,
                                Routes = m.NetworkRoutes.Select(rt => new NetworkRequestTypeDefinitionDTO
                                {
                                    ID = rt.RequestTypeDefinition.ID,
                                    NetworkID = rt.RequestTypeDefinition.NetworkID,
                                    Network = rt.RequestTypeDefinition.Network.Name,
                                    ProjectID = rt.RequestTypeDefinition.ProjectID,
                                    RequestTypeID = rt.RequestTypeDefinition.RequestTypeID,
                                    DataSourceID = rt.RequestTypeDefinition.DataSourceID,
                                    DataSource = rt.RequestTypeDefinition.DataSource.Name,
                                    Timestamp = rt.RequestTypeDefinition.Timestamp
                                })
                            }).ToArrayAsync();

            if (!mappings.Any())
                return mappings;

            if (mappings.Length == 0)
            {
                return Array.Empty<NetworkRequestTypeMappingDTO>();
            }

            //get the distinct network/project/requesttype definitions
            var networkProjectRequestTypes = mappings.Select(m => new { m.NetworkID, m.ProjectID, m.RequestTypeID })
                                                     .Concat(mappings.SelectMany(m => m.Routes.Select(r => new { r.NetworkID, r.ProjectID, r.RequestTypeID })))
                                                     .GroupBy(k => new { k.NetworkID, k.ProjectID, k.RequestTypeID })
                                                     .Select(g => new NetworkRequestTypeDefinitionDTO
                                                     {
                                                         NetworkID = g.Key.NetworkID,
                                                         ProjectID = g.Key.ProjectID,
                                                         RequestTypeID = g.Key.RequestTypeID
                                                     })
                                                     .ToArray();

            var networkIDs = networkProjectRequestTypes.Select(n => n.NetworkID).Distinct().ToArray();

            var networks = await DataContext.Networks.AsNoTracking()
                                                .Where(n => networkIDs.Contains(n.ID))
                                                .Select(n => new { n.ID, n.ServiceUrl, n.ServicePassword, n.ServiceUserName })
                                                .ToArrayAsync();

            //get the project and requestType names for each combination
            foreach (var nprt in networkProjectRequestTypes.GroupBy(k => k.NetworkID))
            {
                var network = networks.FirstOrDefault(n => n.ID == nprt.Key);

                using (var pmnAPI = new Lpp.Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Utilities.Crypto.DecryptString(network.ServicePassword)))
                {
                    string odataFilter = "$filter=" + string.Join(" or ", nprt.Select(n => n.ProjectID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnProjects = await pmnAPI.Projects.List(odataFilter);

                    odataFilter = "$filter=" + string.Join(" or ", nprt.Select(n => n.RequestTypeID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnRequestTypes = await pmnAPI.RequestTypes.List(odataFilter);

                    foreach (var definition in nprt)
                    {
                        definition.Project = pmnProjects.Where(p => p.ID == definition.ProjectID).Select(p => p.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                        definition.RequestType = pmnRequestTypes.Where(rt => rt.ID == definition.RequestTypeID).Select(rt => rt.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                    }
                }

            }

            var projectRequestTypeDetailsMap = networkProjectRequestTypes.ToDictionary(k => string.Format("{0:D}-{1:D}-{2:D}", k.NetworkID, k.ProjectID, k.RequestTypeID));
            string key;
            //update the mapping dto details
            foreach (var map in mappings)
            {
                key = string.Format("{0:D}-{1:D}-{2:D}", map.NetworkID, map.ProjectID, map.RequestTypeID);
                NetworkRequestTypeDefinitionDTO detail;
                if (projectRequestTypeDetailsMap.TryGetValue(key, out detail))
                {
                    map.Project = detail.Project ?? UnableToDetermineMessage;
                    map.RequestType = detail.RequestType ?? UnableToDetermineMessage;
                }
                else
                {
                    map.Project = UnableToDetermineMessage;
                    map.RequestType = UnableToDetermineMessage;
                }

                foreach (var def in map.Routes)
                {
                    key = string.Format("{0:D}-{1:D}-{2:D}", def.NetworkID, def.ProjectID, def.RequestTypeID);
                    if (projectRequestTypeDetailsMap.TryGetValue(key, out detail))
                    {
                        def.Project = detail.Project ?? UnableToDetermineMessage;
                        def.RequestType = detail.RequestType ?? UnableToDetermineMessage;
                    }
                    else
                    {
                        def.Project = UnableToDetermineMessage;
                        def.RequestType = UnableToDetermineMessage;
                    }
                }

                map.Routes = map.Routes.OrderBy(r => r.Network).ThenBy(r => r.Project).ThenBy(r => r.RequestType).ThenBy(r => r.DataSource);
            }

            mappings = mappings.OrderBy(m => m.Network).ThenBy(m => m.Project).ThenBy(m => m.RequestType).ToArray();

            return mappings;
        }

        /// <summary>
        /// Gets all the defined routes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<NetworkRequestTypeDefinitionDTO>> ListRequestTypeDefinitions()
        {
            var definitions = await DataContext.NetworkRequestTypeDefinitions.Select(d => new NetworkRequestTypeDefinitionDTO { ID = d.ID, NetworkID = d.NetworkID, Network = d.Network.Name, ProjectID = d.ProjectID, RequestTypeID = d.RequestTypeID, DataSourceID = d.DataSourceID, DataSource = d.DataSource.Name, Timestamp = d.Timestamp }).ToArrayAsync();

            var networkIDs = definitions.Select(n => n.NetworkID).Distinct().ToArray();

            var networks = await DataContext.Networks.AsNoTracking()
                                                .Where(n => networkIDs.Contains(n.ID))
                                                .Select(n => new { n.ID, n.ServiceUrl, n.ServicePassword, n.ServiceUserName })
                                                .ToArrayAsync();

            //get the project and requestType names for each combination
            foreach (var networkGrouping in definitions.GroupBy(k => k.NetworkID))
            {
                var network = networks.FirstOrDefault(n => n.ID == networkGrouping.Key);

                using (var pmnAPI = new Lpp.Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Utilities.Crypto.DecryptString(network.ServicePassword)))
                {
                    string odataFilter = "$filter=" + string.Join(" or ", networkGrouping.Select(n => n.ProjectID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnProjects = await pmnAPI.Projects.List(odataFilter);

                    odataFilter = "$filter=" + string.Join(" or ", networkGrouping.Select(n => n.RequestTypeID).Distinct().Select(id => string.Format("ID eq {0:D}", id)));
                    var pmnRequestTypes = await pmnAPI.RequestTypes.List(odataFilter);

                    foreach (var definition in networkGrouping)
                    {
                        definition.Project = pmnProjects.Where(p => p.ID == definition.ProjectID).Select(p => p.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                        definition.RequestType = pmnRequestTypes.Where(rt => rt.ID == definition.RequestTypeID).Select(rt => rt.Name).FirstOrDefault() ?? UnableToDetermineMessage;
                    }
                }

            }

            return definitions.OrderBy(d => d.Network).ThenBy(d => d.Project).ThenBy(d => d.RequestType).ThenBy(d => d.DataSource);
        }

        /// <summary>
        /// Gets the specified RequestTypeDefinition.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<NetworkRequestTypeDefinitionDTO> GetRequestTypeDefinition(Guid id)
        {
            var definition = await DataContext.NetworkRequestTypeDefinitions.Where(rt => rt.ID == id)
                                    .Select(rt => new
                                    {
                                        DTO = new NetworkRequestTypeDefinitionDTO
                                        {
                                            ID = rt.ID,
                                            NetworkID = rt.NetworkID,
                                            Network = rt.Network.Name,
                                            ProjectID = rt.ProjectID,
                                            RequestTypeID = rt.RequestTypeID,
                                            DataSourceID = rt.DataSourceID,
                                            DataSource = rt.DataSource.Name,
                                            Timestamp = rt.Timestamp
                                        },
                                        NetworkSettings = new { rt.Network.ServiceUrl, rt.Network.ServiceUserName, rt.Network.ServicePassword }
                                    })
                                    .FirstOrDefaultAsync();

            if (definition == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "A requesttype definition was not found for the specified ID."));
            }

            using (var pmnAPI = new Lpp.Dns.ApiClient.DnsClient(definition.NetworkSettings.ServiceUrl, definition.NetworkSettings.ServiceUserName, Utilities.Crypto.DecryptString(definition.NetworkSettings.ServicePassword)))
            {
                var project = await pmnAPI.Projects.Get(definition.DTO.ProjectID);
                var requestType = await pmnAPI.RequestTypes.Get(definition.DTO.RequestTypeID);

                definition.DTO.Project = project == null ? UnableToDetermineMessage : project.Name;
                definition.DTO.RequestType = requestType == null ? UnableToDetermineMessage : requestType.Name;
            }

            return definition.DTO;
        }

        /// <summary>
        /// Validates that there are no existing network requesttype definitions that match the specified dtos.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<NetworkRequestTypeDefinitionDTO>> ValidateNetworkRequestTypeDefinitions([FromBody]IEnumerable<NetworkRequestTypeDefinitionDTO> dtos)
        {
            if (dtos == null || dtos.Any() == false)
            {
                return Enumerable.Empty<NetworkRequestTypeDefinitionDTO>();
            }

            Expression<Func<NetworkRequestTypeDefinition, bool>> predicate = null;
            foreach (var def in dtos)
            {
                if (predicate == null)
                {
                    predicate = (p) => (def.ID == null || def.ID.Value != p.ID) && p.NetworkID == def.NetworkID && p.ProjectID == def.ProjectID && def.RequestTypeID == p.RequestTypeID && def.DataSourceID == p.DataSourceID;
                }
                else
                {
                    predicate = predicate.Or((p) => (def.ID == null || def.ID.Value != p.ID) && p.NetworkID == def.NetworkID && p.ProjectID == def.ProjectID && def.RequestTypeID == p.RequestTypeID && def.DataSourceID == p.DataSourceID);
                }
            }

            var result = await DataContext.NetworkRequestTypeDefinitions.AsExpandable().Where(predicate.Expand()).ToArrayAsync();

            return result.Select(r => new NetworkRequestTypeDefinitionDTO {
                ID = r.ID,
                Timestamp = r.Timestamp,
                NetworkID = r.NetworkID,
                ProjectID = r.ProjectID,
                RequestTypeID = r.RequestTypeID,
                DataSourceID = r.DataSourceID
            });
        }

        /// <summary>
        /// Creates or updates the specified network requesttype definitions.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<NetworkRequestTypeDefinitionDTO>> CreateOrUpdateNetworkRequestTypeDefinition([FromBody]IEnumerable<NetworkRequestTypeDefinitionDTO> dtos)
        {
            if(dtos == null || dtos.Any() == false)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No network requesttype definitions were specified."));
            }

            Expression<Func<NetworkRequestTypeDefinition, bool>> predicate = null;
            foreach(var def in dtos)
            {
                if (predicate == null)
                {
                    predicate = (p) => (def.ID == null || def.ID.Value != p.ID) && p.NetworkID == def.NetworkID && p.ProjectID == def.ProjectID && def.RequestTypeID == p.RequestTypeID && def.DataSourceID == p.DataSourceID;
                }
                else
                {
                    predicate = predicate.Or((p) => (def.ID == null || def.ID.Value != p.ID) && p.NetworkID == def.NetworkID && p.ProjectID == def.ProjectID && def.RequestTypeID == p.RequestTypeID && def.DataSourceID == p.DataSourceID);
                }
            }

            if((await DataContext.NetworkRequestTypeDefinitions.AsExpandable().Where(predicate.Expand()).AnyAsync()))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, "One or more definition duplicates an existing Network RequestType Definition."));
            }

            //update existing
            var existingDefinitionIDs = dtos.Where(d => d.ID.HasValue).Select(d => d.ID.Value);
            if (existingDefinitionIDs.Any()) {
                var existingDefinitions = await DataContext.NetworkRequestTypeDefinitions.Where(d => existingDefinitionIDs.Contains(d.ID)).ToDictionaryAsync(d => d.ID);

                foreach(var def in dtos.Where(d => d.ID.HasValue))
                {
                    var definition = existingDefinitions[def.ID.Value];
                    definition.NetworkID = def.NetworkID;
                    definition.ProjectID = def.ProjectID;
                    definition.RequestTypeID = def.RequestTypeID;
                    definition.DataSourceID = def.DataSourceID;
                }

                await DataContext.SaveChangesAsync();
            }

            //create new
            if (dtos.Any(d => d.ID.HasValue == false))
            {
                foreach (var def in dtos.Where(d => d.ID.HasValue == false))
                {
                    var definition = DataContext.NetworkRequestTypeDefinitions.Add(new NetworkRequestTypeDefinition
                    {
                        NetworkID = def.NetworkID,
                        ProjectID = def.ProjectID,
                        RequestTypeID = def.RequestTypeID,
                        DataSourceID = def.DataSourceID
                    });

                    def.ID = definition.ID;
                }

                await DataContext.SaveChangesAsync();
            }

            return dtos;
        }

        /// <summary>
        /// Deletes the specified network requesttype definitions.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task DeleteNetworkRequestTypeDefinitions([FromUri]IEnumerable<Guid> ID)
        {
            DataContext.NetworkRequestTypeMappingRoutes.RemoveRange(DataContext.NetworkRequestTypeMappingRoutes.Where(r => ID.Contains(r.RequestTypeDefinitionID)));
            DataContext.NetworkRequestTypeDefinitions.RemoveRange(DataContext.NetworkRequestTypeDefinitions.Where(d => ID.Contains(d.ID)));
            await DataContext.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<NetworkProjectRequestTypeDataMartDTO>> ListMappingItems()
        {
            //for each registered network, call to the network and get all the applicable Project/RequestType/DataSource combinations

            List<NetworkProjectRequestTypeDataMartDTO> routes = new List<NetworkProjectRequestTypeDataMartDTO>();
            
            foreach(var network in await DataContext.Networks.AsNoTracking().ToArrayAsync())
            {
                var datasources = await (from ds in DataContext.DataSources.AsNoTracking()
                                   join ne in DataContext.NetworkEntities.AsNoTracking() on ds.ID equals ne.ID
                                   where ds.Deleted == false && ds.Organization.NetworkID == network.ID && ds.Organization.Deleted == false && ne.EntityType == DTO.Enums.EntityType.DataSource
                                   select ne
                                   ).ToDictionaryAsync(ne => ne.NetworkEntityID);

                using (var pmn = new Dns.ApiClient.DnsClient(network.ServiceUrl, network.ServiceUserName, Utilities.Crypto.DecryptString(network.ServicePassword)))
                {
                    try
                    {
                        var rt = await pmn.CNDSRequestTypes.ListAvailableRequestTypes();

                        foreach(var route in rt)
                        {
                            NetworkEntity link;
                            if(datasources.TryGetValue(route.DataMartID, out link))
                            {
                                routes.Add(new NetworkProjectRequestTypeDataMartDTO
                                {
                                    NetworkID = network.ID,
                                    Network = network.Name,
                                    ProjectID = route.ProjectID,
                                    Project = route.Project,
                                    RequestTypeID = route.RequestTypeID,
                                    RequestType = route.RequestType,
                                    //Return the CNDS ID for the datasource, not the networks local ID
                                    DataMartID = link.ID,
                                    DataMart = route.DataMart
                                });
                            }
                        }

                    }catch(Exception ex)
                    {
                        Logger.Error("Error retrieving available requesttypes from Network: " + network.Name + " (" + network.ServiceUrl + ").", ex);
                    }
                }
            }

            return routes;
        }

        /// <summary>
        /// Returns back any of the specified dtos that already exist.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<NetworkRequestTypeMappingDTO>> ValidateMappings([FromBody] IEnumerable<NetworkRequestTypeMappingDTO> dtos)
        {
            if(dtos == null || dtos.Any() == false)
            {
                return Enumerable.Empty<NetworkRequestTypeMappingDTO>();
            }

            Expression<Func<NetworkRequestTypeMapping, bool>> predicate = null;
            foreach(var dto in dtos)
            {
                if (predicate == null)
                {
                    predicate = (p) => (dto.ID == null || dto.ID.Value != p.ID) && p.NetworkID == dto.NetworkID && p.ProjectID == dto.ProjectID && p.RequestTypeID == dto.RequestTypeID;
                }
                else
                {
                    predicate = predicate.Or((p) => (dto.ID == null || dto.ID.Value != p.ID) && p.NetworkID == dto.NetworkID && p.ProjectID == dto.ProjectID && p.RequestTypeID == dto.RequestTypeID);
                }
            }

            var existing = await DataContext.NetworkRequestTypeMappings.AsExpandable().Where(predicate.Expand()).ToArrayAsync();

            return existing.Select(m => new NetworkRequestTypeMappingDTO
            {
                ID = m.ID,
                Timestamp = m.Timestamp,
                NetworkID = m.NetworkID,
                ProjectID = m.ProjectID,
                RequestTypeID = m.RequestTypeID
            });
        }

        [HttpPost]
        public async Task<NetworkRequestTypeMappingDTO> CreateOrUpdateMapping(NetworkRequestTypeMappingDTO dto)
        {
            NetworkRequestTypeMapping mapping;
            
            mapping = await DataContext.NetworkRequestTypeMappings.Where(m => (dto.ID == null || dto.ID.Value != m.ID) && m.NetworkID == dto.NetworkID && m.ProjectID == dto.ProjectID && m.RequestTypeID == dto.RequestTypeID).FirstOrDefaultAsync();

            if (mapping != null && mapping.ID != dto.ID.Value)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "A source mapping for the specified Network, Project, and RequestType combination already exists."));
            }

            if (dto.ID.HasValue == false)
            {
                mapping = DataContext.NetworkRequestTypeMappings.Add(new NetworkRequestTypeMapping
                {
                    NetworkID = dto.NetworkID,
                    ProjectID = dto.ProjectID,
                    RequestTypeID = dto.RequestTypeID
                });
            }
            else
            {
                mapping = await DataContext.NetworkRequestTypeMappings.FindAsync(dto.ID.Value);
                if (mapping == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "The specified network requesttype mapping was not found."));
                }
                else {
                    await DataContext.Entry(mapping).Collection(m => m.NetworkRoutes).LoadAsync();
                }

                mapping.NetworkID = dto.NetworkID;
                mapping.ProjectID = dto.ProjectID;
                mapping.RequestTypeID = dto.RequestTypeID;
            }

            foreach(var route in dto.Routes)
            {
                if(!mapping.NetworkRoutes.Any(nrt => nrt.RequestTypeDefinitionID == route.ID))
                {
                    mapping.NetworkRoutes.Add(new NetworkRequestTypeMappingRoutes { RequestTypeMappingID = mapping.ID, RequestTypeDefinitionID = route.ID.Value });
                }
            }

            var routesToRemove = mapping.NetworkRoutes.Where(nrt => dto.Routes.Any(rt => rt.ID == nrt.RequestTypeDefinitionID) == false).ToArray();
            foreach(var rtd in routesToRemove)
            {
                mapping.NetworkRoutes.Remove(rtd);
            }

            await DataContext.SaveChangesAsync();

            var routeDefinitions = await DataContext.NetworkRequestTypeDefinitions.Where(rt => rt.NetworkRequestTypeMappingRoutes.Any(nmrt => nmrt.RequestTypeMappingID == mapping.ID)).ToDictionaryAsync(k => k.ID);

            var result = new NetworkRequestTypeMappingDTO
            {
                ID = mapping.ID,
                Timestamp = mapping.Timestamp,
                NetworkID = mapping.NetworkID,
                ProjectID = mapping.ProjectID,
                RequestTypeID = mapping.RequestTypeID,
                Routes = mapping.NetworkRoutes.Select(r => {
                    var definition = routeDefinitions[r.RequestTypeDefinitionID];
                    return new NetworkRequestTypeDefinitionDTO { ID = r.RequestTypeDefinitionID, NetworkID = definition.NetworkID, ProjectID = definition.ProjectID, RequestTypeID = definition.RequestTypeID, DataSourceID = definition.DataSourceID };
                })
            };

            return result;
        }

        [HttpDelete]
        public async Task DeleteMapping([FromUri]IEnumerable<Guid> ID)
        {
            var mappingIDs = ID.ToArray();

            DataContext.NetworkRequestTypeMappingRoutes.RemoveRange(DataContext.NetworkRequestTypeMappingRoutes.Where(rt => mappingIDs.Contains(rt.RequestTypeMappingID)));
            DataContext.NetworkRequestTypeMappings.RemoveRange(DataContext.NetworkRequestTypeMappings.Where(m => mappingIDs.Contains(m.ID)));

            await DataContext.SaveChangesAsync();            
        }

    }
}