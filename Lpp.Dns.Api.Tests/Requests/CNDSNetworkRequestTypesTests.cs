using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lpp.Dns.Data;
using Lpp.Dns.DTO;
using System.Data.Entity;
using Lpp.CNDS.ApiClient;

namespace Lpp.Dns.Api.Tests.Requests
{
    [TestClass]
    public class CNDSNetworkRequestTypesTests : IDisposable
    {
        readonly log4net.ILog Logger;
        static readonly string CNDSurl;
        readonly Lpp.CNDS.ApiClient.CNDSClient CNDSApi;

        static CNDSNetworkRequestTypesTests()
        {
            log4net.Config.XmlConfigurator.Configure();
            CNDSurl = System.Configuration.ConfigurationManager.AppSettings["CNDS.URL"] ?? string.Empty;
        }

        public CNDSNetworkRequestTypesTests()
        {
            Logger = log4net.LogManager.GetLogger(typeof(CNDSNetworkRequestTypesTests));
            CNDSApi = new CNDSClient(CNDSurl);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                CNDSApi.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public static readonly Guid CNDSImportedDataMartTypeID = new Guid("5B060001-2A5F-4243-980F-A70E011E7D5F");

        [TestMethod]
        public async Task GetAllRequestTypesAvailable()
        {
            //going to know the datamart's involved in the request


            //get all the requesttypes the user can create, get the datamarts for the request that are included in the specified datamarts

            //get all the network requesttype mappings for the request types

            //only return requesttypes that have at least one mapping or one local datasource
            Guid IdentityID = new Guid("96DC0001-94F1-47CC-BFE6-A22201424AD0");
            var Identity = new Utilities.Security.ApiIdentity(IdentityID, "SystemAdministrator", "System Administrator");

            IEnumerable<Guid> cndsDatamartIDs = new[] { new Guid("efdb60d1-1501-4598-88ba-a6880096e838"),
                                                        new Guid("6b388358-31e1-4ea4-a6b9-a68a00b721c0"),
                                                        new Guid("adaffde9-2a7a-442d-951a-a70f00be7591"),
                                                        new Guid("47a822af-52b7-47e2-ae15-a79200ab9431"),
                                                        new Guid("410f8140-1d63-4b09-860c-a78600f08a31"),
                                                        new Guid("3ee787a1-56b5-46e5-82ac-a78600f49607"),//CNDS ID, for local
                                                        new Guid("5bb1a5df-8786-4b18-b7f3-a68a00b8602d"),//CNDS ID for local
                                                        new Guid("1ae5139d-7ac9-41c3-8552-a68300ae8381")//CNDS ID, for local
                                                      };

            

            using (var DataContext = new DataContext())
            {
                DataContext.Configuration.AutoDetectChangesEnabled = false;
                DataContext.Configuration.LazyLoadingEnabled = false;
                //DataContext.Database.Log = (sql) => {
                //    Logger.Debug(sql);
                //};

                var externalDataSourceInformation = await CNDSApi.DataSources.ListExtended("$filter=" + string.Join(" or ", cndsDatamartIDs.Select(i => string.Format("ID eq {0}", i.ToString("D")))));

                var localNetwork = DataContext.Networks.Where(n => n.Name != "Aqueduct").Select(n => new { n.ID, n.Name }).FirstOrDefault();

                var networkResponse = await CNDSApi.Networks.LookupEntities(localNetwork.ID, cndsDatamartIDs);
                var content = await networkResponse.Content.ReadAsStringAsync();
                
                var localDataMartIdentifiers = Newtonsoft.Json.JsonConvert.DeserializeObject<Lpp.Utilities.BaseResponse<NetworkEntityIdentifier>>(content);
                var localDataMartIds = localDataMartIdentifiers.results.Select(i => i.NetworkEntityID).ToArray();

                foreach(var dm in localDataMartIdentifiers.results)
                {
                    Logger.Debug(string.Format("CNDS ID: {1}        Local ID:{0}", dm.NetworkEntityID, dm.EntityID));
                }

                var projects = await (from p in DataContext.Secure<Project>(Identity, Lpp.Dns.DTO.Security.PermissionIdentifiers.Request.Edit)
                                      where p.Active && !p.Deleted && (!p.EndDate.HasValue || p.EndDate.Value > DateTime.UtcNow) && (p.StartDate <= DateTime.UtcNow)
                                      select p.ID).ToArrayAsync();

                var requestTypes = await (from rt in DataContext.ProjectRequestTypes
                                          join project in DataContext.Projects on rt.ProjectID equals project.ID
                                          let userID = IdentityID
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
                                            select new SourceRequestType
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
                                                                            select new Routing {
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

                foreach(var item in requestTypes)
                {
                    var sourceMapping = networkMappings.Where(m => m.NetworkID == localNetwork.ID && m.ProjectID == item.ProjectID && m.RequestTypeID == item.RequestTypeID).FirstOrDefault();
                    if(sourceMapping != null)
                    {
                        item.ExternalRoutes = sourceMapping.Routes
                                                    .Select(rt => new Routing
                                                    {
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
                        item.ExternalRoutes = Array.Empty<Routing>();
                    }

                    item.InvalidRoutes = externalDataSourceInformation
                                            .Where(ds => !item.ExternalRoutes.Any(d => d.NetworkID == ds.NetworkID && ds.ID == d.DataMartID)
                                            && !item.LocalRoutes.Any(d => d.NetworkID == localNetwork.ID && localDataMartIdentifiers.results.Any(i => i.EntityID == ds.ID && i.NetworkEntityID == d.DataMartID)))
                                            .Select(ds => new Routing { NetworkID = ds.NetworkID, Network = ds.Network, DataMartID = ds.ID.Value, DataMart = ds.Name, IsLocal = false })
                                            .Distinct().OrderBy(ds => ds.Network).ThenBy(ds => ds.DataMart);

                }

                //filter out any that do not have any local or external routes
                requestTypes = requestTypes.Where(rt => rt.LocalRoutes.Any() || rt.ExternalRoutes.Any()).ToArray();

                foreach (var requestType in requestTypes)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Project - RequestType: " + requestType.Project + " - " + requestType.RequestType);
                    sb.AppendLine("Local DataMarts:");
                    foreach (var dm in requestType.LocalRoutes)
                    {
                        sb.AppendLine("\tDataMart: " + dm.DataMart);
                    }
                    if (!requestType.LocalRoutes.Any())
                    {
                        sb.AppendLine("\tNo local datamarts.");
                    }
                    sb.AppendLine("External Routes:");
                    foreach(var dm in requestType.ExternalRoutes)
                    {
                        sb.AppendLine(string.Format("\tNetwork:{0}  Project:{1}  RequestType:{2}  Data Source:{3}", dm.Network, dm.Project, dm.RequestType, dm.DataMart));
                    }
                    if (!requestType.ExternalRoutes.Any())
                    {
                        sb.AppendLine("\tNo external routes.");
                    }

                    if (requestType.InvalidRoutes.Any())
                    {
                        sb.AppendLine("Invalid Data Sources:");
                        foreach(var ds in requestType.InvalidRoutes)
                        {
                            sb.AppendLine(string.Format("Network: {0}     Data Source:{1}", ds.Network, ds.DataMart));
                        }
                    }

                    Logger.Debug(sb.ToString());
                }




            }
        }
    }


    /// <summary>
    /// The CNDS and PMN ID link.
    /// </summary>
    public class NetworkEntityIdentifier
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

    public class SourceRequestType
    {
        public Guid ProjectID { get; set; }
        public string Project { get; set; }

        public Guid RequestTypeID { get; set; }

        public string RequestType { get; set; }
        /// <summary>
        /// The datamart that are part of the project the requesttype belongs to. The DataMart ID is local.
        /// </summary>
        public IEnumerable<Routing> LocalRoutes { get; set; }
        /// <summary>
        /// The external data source routing information, the DataMart ID is CNDS.
        /// </summary>
        public IEnumerable<Routing> ExternalRoutes { get; set; }
        /// <summary>
        /// The data source information for invalid selected data sources, the DataMart ID is CNDS.
        /// </summary>
        public IEnumerable<Routing> InvalidRoutes { get; set; }
    }

    public class Routing
    {
        public Guid NetworkID { get; set; }

        public string Network { get; set; }

        public Guid ProjectID { get; set; }

        public string Project { get; set; }

        public Guid RequestTypeID { get; set; }

        public string RequestType { get; set; }

        public Guid DataMartID { get; set; }

        public string DataMart { get; set; }

        public bool IsLocal { get; set; }
    }
}
