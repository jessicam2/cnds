//disable the missing comment rule warning until auto comments are completed
#pragma warning disable 1591

using Lpp.CNDS.DTO;
using Lpp.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient
{
	 public class CNDSClient : HttpClientEx
	 {
	 	 public CNDSClient(string host) : base(host) { }
	 	 public CNDSClient(string host, string userName, string password) : base(host, userName, password) { }

	 	Users _Users = null;
        public Users Users
                            {
                                get {
                                    if (_Users == null)
                                        _Users = new Users(this);

                                    return _Users;
                                }
                            }
	 	Permissions _Permissions = null;
        public Permissions Permissions
                            {
                                get {
                                    if (_Permissions == null)
                                        _Permissions = new Permissions(this);

                                    return _Permissions;
                                }
                            }
	 	SecurityGroups _SecurityGroups = null;
        public SecurityGroups SecurityGroups
                            {
                                get {
                                    if (_SecurityGroups == null)
                                        _SecurityGroups = new SecurityGroups(this);

                                    return _SecurityGroups;
                                }
                            }
	 	SecurityGroupUsers _SecurityGroupUsers = null;
        public SecurityGroupUsers SecurityGroupUsers
                            {
                                get {
                                    if (_SecurityGroupUsers == null)
                                        _SecurityGroupUsers = new SecurityGroupUsers(this);

                                    return _SecurityGroupUsers;
                                }
                            }
	 	Requests _Requests = null;
        public Requests Requests
                            {
                                get {
                                    if (_Requests == null)
                                        _Requests = new Requests(this);

                                    return _Requests;
                                }
                            }
	 	RequestTypeMapping _RequestTypeMapping = null;
        public RequestTypeMapping RequestTypeMapping
                            {
                                get {
                                    if (_RequestTypeMapping == null)
                                        _RequestTypeMapping = new RequestTypeMapping(this);

                                    return _RequestTypeMapping;
                                }
                            }
	 	QlikData _QlikData = null;
        public QlikData QlikData
                            {
                                get {
                                    if (_QlikData == null)
                                        _QlikData = new QlikData(this);

                                    return _QlikData;
                                }
                            }
	 	Organizations _Organizations = null;
        public Organizations Organizations
                            {
                                get {
                                    if (_Organizations == null)
                                        _Organizations = new Organizations(this);

                                    return _Organizations;
                                }
                            }
	 	Networks _Networks = null;
        public Networks Networks
                            {
                                get {
                                    if (_Networks == null)
                                        _Networks = new Networks(this);

                                    return _Networks;
                                }
                            }
	 	Domain _Domain = null;
        public Domain Domain
                            {
                                get {
                                    if (_Domain == null)
                                        _Domain = new Domain(this);

                                    return _Domain;
                                }
                            }
	 	DataSources _DataSources = null;
        public DataSources DataSources
                            {
                                get {
                                    if (_DataSources == null)
                                        _DataSources = new DataSources(this);

                                    return _DataSources;
                                }
                            }
	 }
	 public class Users
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Users(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Users";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.UserDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.UserDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.UserDTO>> Get(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.UserDTO>(Path + "/Get?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> ListUserDomains(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/ListUserDomains?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.UserDTO> Register(Lpp.CNDS.DTO.UserTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.UserTransferDTO, Lpp.CNDS.DTO.UserDTO>(Path + "/Register", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.UserDTO> Update(Lpp.CNDS.DTO.UserTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.UserTransferDTO, Lpp.CNDS.DTO.UserDTO>(Path + "/Update", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task Delete(System.Guid id)
	 	 {
	 	 	 await Client.Delete(Path + "/Delete", id);
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> GetDomainVisibility(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/GetDomainVisibility?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task UpdateDomainVisibility(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.DomainDataDTO> domains)
	 	 {
	 	 	 var result = await Client.Post(Path + "/UpdateDomainVisibility", domains);
	 	 	 return;
	 	 }
	 }
	 public class Permissions
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Permissions(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Permissions";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.PermissionDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.PermissionDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Boolean> HasPermission(System.Guid permissionID, System.Guid userID)
	 	 {

	 	 	 var result = await Client.Get<System.Boolean>(Path + "/HasPermission?permissionID=" + System.Net.WebUtility.UrlEncode(permissionID.ToString()) + "&userID=" + System.Net.WebUtility.UrlEncode(userID.ToString()) + "&");
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.AssignedPermissionDTO>> GetUserPermissions(System.Guid userID, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.AssignedPermissionDTO>(Path + "/GetUserPermissions?userID=" + System.Net.WebUtility.UrlEncode(userID.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.AssignedPermissionDTO>> GetPermissions(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.AssignedPermissionDTO>(Path + "/GetPermissions", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task SetPermissions(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.UpdateAssignedPermissionDTO> permissions)
	 	 {
	 	 	 var result = await Client.Post(Path + "/SetPermissions", permissions);
	 	 	 return;
	 	 }
	 }
	 public class SecurityGroups
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public SecurityGroups(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/SecurityGroups";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.SecurityGroupDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.SecurityGroupDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.SecurityGroupDTO> Get(System.Guid securityGroupID)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.SecurityGroupDTO>(Path + "/Get?securityGroupID=" + System.Net.WebUtility.UrlEncode(securityGroupID.ToString()) + "&");
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task Create(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.SecurityGroupDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post(Path + "/Create", dtos);
	 	 	 return;
	 	 }
	 	 public async Task Update(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.SecurityGroupDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post(Path + "/Update", dtos);
	 	 	 return;
	 	 }
	 	 public async Task Delete(System.Guid id)
	 	 {
	 	 	 await Client.Delete(Path + "/Delete", id);
	 	 }
	 }
	 public class SecurityGroupUsers
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public SecurityGroupUsers(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/SecurityGroupUsers";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.SecurityGroupUserDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.SecurityGroupUserDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.SecurityGroupUserDTO> Get(System.Guid userID)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.SecurityGroupUserDTO>(Path + "/Get?userID=" + System.Net.WebUtility.UrlEncode(userID.ToString()) + "&");
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task Update(Lpp.CNDS.DTO.SecurityGroupUserDTO dto)
	 	 {
	 	 	 var result = await Client.Post(Path + "/Update", dto);
	 	 	 return;
	 	 }
	 }
	 public class Requests
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Requests(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Requests";
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> Submit(Lpp.CNDS.DTO.Requests.SubmitRequestDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.Requests.SubmitRequestDTO>(Path + "/Submit", dto);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> ReadDocument(System.Guid id)
	 	 {

	 	 	 return await Client._Client.GetAsync(Client._Host + Path + "/ReadDocument?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&");
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> UpdateSourceRouting(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.SetRoutingStatusDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.SetRoutingStatusDTO>>(Path + "/UpdateSourceRouting", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> ResubmitParticipateRouting(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.ResubmitRouteDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.ResubmitRouteDTO>>(Path + "/ResubmitParticipateRouting", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> ResubmitSourceRouting(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.ResubmitRouteDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.ResubmitRouteDTO>>(Path + "/ResubmitSourceRouting", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> UpdateParticipantRoutingStatus(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.SetRoutingStatusDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.SetRoutingStatusDTO>>(Path + "/UpdateParticipantRoutingStatus", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task PostParticipantResponseDocuments(System.Guid responseID)
	 	 {
	 	 	 var result = await Client.Post(Path + "/PostParticipantResponseDocuments", responseID);
	 	 	 return;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> UpdateSourceDataMartsPriorityAndDueDates(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.UpdateDataMartPriorityAndDueDateDTO>>(Path + "/UpdateSourceDataMartsPriorityAndDueDates", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> UpdatParticipantDataMartsPriorityAndDueDates(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.UpdateDataMartPriorityAndDueDateDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.Requests.UpdateDataMartPriorityAndDueDateDTO>>(Path + "/UpdatParticipantDataMartsPriorityAndDueDates", dtos);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> TerminateParticipantRoute(System.Collections.Generic.IEnumerable<System.Guid> ids)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<System.Guid>>(Path + "/TerminateParticipantRoute", ids);
	 	 	 return result;
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> TerminateSourceRoute(System.Collections.Generic.IEnumerable<System.Guid> ids)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<System.Guid>>(Path + "/TerminateSourceRoute", ids);
	 	 	 return result;
	 	 }
	 }
	 public class RequestTypeMapping
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public RequestTypeMapping(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/RequestTypeMapping";
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>> ListMappings(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>(Path + "/ListMappings", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO> GetRequestTypeMapping(System.Guid id)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>(Path + "/GetRequestTypeMapping?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&");
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>> FindMappings(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO> requestTypes)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO>, Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>(Path + "/FindMappings", requestTypes);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>> ListRequestTypeDefinitions(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>(Path + "/ListRequestTypeDefinitions", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO> GetRequestTypeDefinition(System.Guid id)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>(Path + "/GetRequestTypeDefinition?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&");
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>> ValidateNetworkRequestTypeDefinitions(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>, Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>(Path + "/ValidateNetworkRequestTypeDefinitions", dtos);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>> CreateOrUpdateNetworkRequestTypeDefinition(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>, Lpp.CNDS.DTO.NetworkRequestTypeDefinitionDTO>(Path + "/CreateOrUpdateNetworkRequestTypeDefinition", dtos);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task DeleteNetworkRequestTypeDefinitions(System.Collections.Generic.IEnumerable<System.Guid> ID)
	 	 {
	 	 	 await Client.Delete(Path + "/DeleteNetworkRequestTypeDefinitions", ID);
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO>> ListMappingItems(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkProjectRequestTypeDataMartDTO>(Path + "/ListMappingItems", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>> ValidateMappings(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO> dtos)
	 	 {
	 	 	 var result = await Client.Post<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>, Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>(Path + "/ValidateMappings", dtos);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO> CreateOrUpdateMapping(Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO, Lpp.CNDS.DTO.NetworkRequestTypeMappingDTO>(Path + "/CreateOrUpdateMapping", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task DeleteMapping(System.Collections.Generic.IEnumerable<System.Guid> ID)
	 	 {
	 	 	 await Client.Delete(Path + "/DeleteMapping", ID);
	 	 }
	 }
	 public class QlikData
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public QlikData(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/QlikData";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.QlikData.DataSourceWithDomainDataItemDTO>> DataSourcesWithDomainData(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.QlikData.DataSourceWithDomainDataItemDTO>(Path + "/DataSourcesWithDomainData", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.QlikData.UserWithDomainDataItemDTO>> UsersWithDomainData(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.QlikData.UserWithDomainDataItemDTO>(Path + "/UsersWithDomainData", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.QlikData.OrganizationWithDomainDataItemDTO>> OrganizationsWithDomainData(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.QlikData.OrganizationWithDomainDataItemDTO>(Path + "/OrganizationsWithDomainData", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.QlikData.ActiveUserDTO>> ActiveUsers(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.QlikData.ActiveUserDTO>(Path + "/ActiveUsers", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 }
	 public class Organizations
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Organizations(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Organizations";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.OrganizationDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.OrganizationDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.OrganizationDTO>> Get(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.OrganizationDTO>(Path + "/Get?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> ListOrganizationDomains(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/ListOrganizationDomains?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.OrganizationDTO> Register(Lpp.CNDS.DTO.OrganizationTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.OrganizationTransferDTO, Lpp.CNDS.DTO.OrganizationDTO>(Path + "/Register", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.OrganizationDTO> Update(Lpp.CNDS.DTO.OrganizationTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.OrganizationTransferDTO, Lpp.CNDS.DTO.OrganizationDTO>(Path + "/Update", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task Delete(System.Guid id)
	 	 {
	 	 	 await Client.Delete(Path + "/Delete", id);
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> GetDomainVisibility(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/GetDomainVisibility?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task UpdateDomainVisibility(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.DomainDataDTO> domains)
	 	 {
	 	 	 var result = await Client.Post(Path + "/UpdateDomainVisibility", domains);
	 	 	 return;
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.OrganizationSearchDTO>> OrganizationSearch(Lpp.CNDS.DTO.SearchDTO ids)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.SearchDTO, Lpp.CNDS.DTO.OrganizationSearchDTO>(Path + "/OrganizationSearch", ids);
	 	 	 return result.ReturnList();
	 	 }
	 }
	 public class Networks
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Networks(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Networks";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.NetworkDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.NetworkDTO>> Get(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.NetworkDTO>(Path + "/Get?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.NetworkDTO> Register(Lpp.CNDS.DTO.NetworkTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.NetworkTransferDTO, Lpp.CNDS.DTO.NetworkDTO>(Path + "/Register", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> Update(Lpp.CNDS.DTO.NetworkTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.NetworkTransferDTO>(Path + "/Update", dto);
	 	 	 return result;
	 	 }
	 	 public async Task Delete(System.Guid id)
	 	 {
	 	 	 await Client.Delete(Path + "/Delete", id);
	 	 }
	 	 public async Task<System.Net.Http.HttpResponseMessage> LookupEntities(System.Guid networkID, System.Collections.Generic.IEnumerable<System.Guid> entityIDs)
	 	 {
	 	 	 var entityIDsQueryString = string.Join("&", entityIDs.Select(i => string.Format("{0}={1}", "entityIDs", System.Net.WebUtility.UrlEncode(i.ToString()))));

	 	 	 return await Client._Client.GetAsync(Client._Host + Path + "/LookupEntities?networkID=" + System.Net.WebUtility.UrlEncode(networkID.ToString()) + "&" + entityIDsQueryString + "&");
	 	 }
	 }
	 public class Domain
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public Domain(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/Domain";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDTO>> ListDomains(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDTO>(Path + "/ListDomains", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task InsertOrUpdateDomains(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.DomainDTO> domainDatas)
	 	 {
	 	 	 var result = await Client.Post(Path + "/InsertOrUpdateDomains", domainDatas);
	 	 	 return;
	 	 }
	 	 public async Task InsertOrUpdateDomainUses(Lpp.CNDS.DTO.AddRemoveDomainUseDTO changes)
	 	 {
	 	 	 var result = await Client.Post(Path + "/InsertOrUpdateDomainUses", changes);
	 	 	 return;
	 	 }
	 }
	 public class DataSources
	 {
	 	 readonly CNDSClient Client;
	 	 readonly string Path;
	 	 public DataSources(CNDSClient client)
	 	 {
	 	 	 this.Client = client;
	 	 	 this.Path = "/DataSources";
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DataSourceDTO>> List(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DataSourceDTO>(Path + "/List", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DataSourceExtendedDTO>> ListExtended(string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DataSourceExtendedDTO>(Path + "/ListExtended", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DataSourceDTO>> Get(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DataSourceDTO>(Path + "/Get?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> ListDataSourceDomains(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/ListDataSourceDomains?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.DataSourceDTO> Register(Lpp.CNDS.DTO.DataSourceTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.DataSourceTransferDTO, Lpp.CNDS.DTO.DataSourceDTO>(Path + "/Register", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task<Lpp.CNDS.DTO.DataSourceDTO> Update(Lpp.CNDS.DTO.DataSourceTransferDTO dto)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.DataSourceTransferDTO, Lpp.CNDS.DTO.DataSourceDTO>(Path + "/Update", dto);
	 	 	 return result.ReturnSingleItem();
	 	 }
	 	 public async Task Delete(System.Guid id)
	 	 {
	 	 	 await Client.Delete(Path + "/Delete", id);
	 	 }
	 	 public async Task<System.Linq.IQueryable<Lpp.CNDS.DTO.DomainDataDTO>> GetDomainVisibility(System.Guid id, string oDataQuery = null)
	 	 {

	 	 	 var result = await Client.Get<Lpp.CNDS.DTO.DomainDataDTO>(Path + "/GetDomainVisibility?id=" + System.Net.WebUtility.UrlEncode(id.ToString()) + "&", oDataQuery);
	 	 	 return result.ReturnList();
	 	 }
	 	 public async Task UpdateDomainVisibility(System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.DomainDataDTO> domains)
	 	 {
	 	 	 var result = await Client.Post(Path + "/UpdateDomainVisibility", domains);
	 	 	 return;
	 	 }
	 	 public async Task<System.Collections.Generic.IEnumerable<Lpp.CNDS.DTO.DataSourceSearchDTO>> DataSourceSearch(Lpp.CNDS.DTO.SearchDTO ids)
	 	 {
	 	 	 var result = await Client.Post<Lpp.CNDS.DTO.SearchDTO, Lpp.CNDS.DTO.DataSourceSearchDTO>(Path + "/DataSourceSearch", ids);
	 	 	 return result.ReturnList();
	 	 }
	 }

}

#pragma warning restore 1591
