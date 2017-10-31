declare module CNDS.Structures {
	 export interface KeyValuePair
	 {
	 	 text: string;
	 	 value: any;
	 }
}
module CNDS.Enums
{
	 export enum AccessType{
	 	NoOne = 0,
	 	MyNetwork = 100,
	 	AllPMNNetworks = 1000,
	 	AllNetworks = 10000,
	 	Anyone = 100000,
	 }
	 export var AccessTypeTranslation: CNDS.Structures.KeyValuePair[] = [
	 	 {value:AccessType.NoOne , text: 'No One'},
	 	 {value:AccessType.MyNetwork , text: 'My Network Members'},
	 	 {value:AccessType.AllPMNNetworks , text: 'All PMN Members'},
	 	 {value:AccessType.AllNetworks , text: 'All PMN and CNDS Members'},
	 	 {value:AccessType.Anyone , text: 'Anyone'},
	 ]
	 export enum EntityType{
	 	Organization = 0,
	 	User = 1,
	 	DataSource = 2,
	 }
	 export var EntityTypeTranslation: CNDS.Structures.KeyValuePair[] = [
	 	 {value:EntityType.Organization , text: 'Organization'},
	 	 {value:EntityType.User , text: 'User'},
	 	 {value:EntityType.DataSource , text: 'DataSource'},
	 ]
}
module CNDS.Interfaces
{
	 export interface IEntityDtoWithID extends IEntityDto {
	 	 ID?: any;
	 	 Timestamp?: any;
	 }
	 export interface IEntityDto {
	 }
	 export interface IDataSourceDTO{
	 	 ID?: any;
	 	 Name: string;
	 	 Acronym: string;
	 	 OrganizationID: any;
	 	 AdapterSupportedID?: any;
	 	 AdapterSupported: string;
	 }
	 export var KendoModelDataSourceDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Acronym': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'AdapterSupportedID': { type:'any', nullable: true},
	 	 	'AdapterSupported': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IOrganizationSearchDTO{
	 	 ID: any;
	 	 NetworkID: any;
	 	 Network: string;
	 	 Name: string;
	 	 ContactInformation: string;
	 }
	 export var KendoModelOrganizationSearchDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'ContactInformation': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IDataSourceSearchDTO{
	 	 ID: any;
	 	 NetworkID: any;
	 	 Network: string;
	 	 OrganizationID: any;
	 	 Organization: string;
	 	 Name: string;
	 	 ContactInformation: string;
	 }
	 export var KendoModelDataSourceSearchDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Organization': { type:'string', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'ContactInformation': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IDataSourceTransferDTO{
	 	 ID: any;
	 	 Name: string;
	 	 Acronym: string;
	 	 AdapterSupportedID?: any;
	 	 NetworkID?: any;
	 	 OrganizationID: any;
	 	 Metadata: IDomainDataDTO[];
	 }
	 export var KendoModelDataSourceTransferDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Acronym': { type:'string', nullable: false},
	 	 	'AdapterSupportedID': { type:'any', nullable: true},
	 	 	'NetworkID': { type:'any', nullable: true},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Metadata': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface IAddRemoveDomainUseDTO{
	 	 AddDomainUse: any[];
	 	 RemoveDomainUse: any[];
	 }
	 export var KendoModelAddRemoveDomainUseDTO: any = {
	 	 fields: {
	 	 	'AddDomainUse': { type:'any[]', nullable: false},
	 	 	'RemoveDomainUse': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface IDomainDataDTO{
	 	 ID?: any;
	 	 EntityID?: any;
	 	 DomainUseID: any;
	 	 Value: string;
	 	 DomainReferenceID?: any;
	 	 SequenceNumber: number;
	 	 Visibility: CNDS.Enums.AccessType;
	 }
	 export var KendoModelDomainDataDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: true},
	 	 	'EntityID': { type:'any', nullable: true},
	 	 	'DomainUseID': { type:'any', nullable: false},
	 	 	'Value': { type:'string', nullable: false},
	 	 	'DomainReferenceID': { type:'any', nullable: true},
	 	 	'SequenceNumber': { type:'number', nullable: false},
	 	 	'Visibility': { type:'cnds.enums.accesstype', nullable: false},
	 	 }
	 }
	 export interface IDomainDTO{
	 	 ID: any;
	 	 DomainUseID?: any;
	 	 ParentDomainID?: any;
	 	 Title: string;
	 	 IsMultiValue: boolean;
	 	 EnumValue?: number;
	 	 DataType: string;
	 	 EntityType?: CNDS.Enums.EntityType;
	 	 ChildMetadata: IDomainDTO[];
	 	 References: IDomainReferenceDTO[];
	 }
	 export var KendoModelDomainDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'DomainUseID': { type:'any', nullable: true},
	 	 	'ParentDomainID': { type:'any', nullable: true},
	 	 	'Title': { type:'string', nullable: false},
	 	 	'IsMultiValue': { type:'boolean', nullable: false},
	 	 	'EnumValue': { type:'number', nullable: true},
	 	 	'DataType': { type:'string', nullable: false},
	 	 	'EntityType': { type:'cnds.enums.entitytype', nullable: true},
	 	 	'ChildMetadata': { type:'any[]', nullable: false},
	 	 	'References': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface IDomainReferenceDTO{
	 	 ID: any;
	 	 DomainID: any;
	 	 ParentDomainReferenceID?: any;
	 	 Title: string;
	 	 Description: string;
	 	 Value: string;
	 }
	 export var KendoModelDomainReferenceDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'DomainID': { type:'any', nullable: false},
	 	 	'ParentDomainReferenceID': { type:'any', nullable: true},
	 	 	'Title': { type:'string', nullable: false},
	 	 	'Description': { type:'string', nullable: false},
	 	 	'Value': { type:'string', nullable: false},
	 	 }
	 }
	 export interface INetworkTransferDTO{
	 	 ID: any;
	 	 Name: string;
	 	 Url: string;
	 	 ServiceUrl: string;
	 	 ServiceUserName: string;
	 	 ServicePassword: string;
	 }
	 export var KendoModelNetworkTransferDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Url': { type:'string', nullable: false},
	 	 	'ServiceUrl': { type:'string', nullable: false},
	 	 	'ServiceUserName': { type:'string', nullable: false},
	 	 	'ServicePassword': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IOrganizationTransferDTO{
	 	 ID: any;
	 	 Name: string;
	 	 Acronym: string;
	 	 ParentOrganizationID?: any;
	 	 NetworkID?: any;
	 	 Metadata: IDomainDataDTO[];
	 	 ContactEmail: string;
	 	 ContactFirstName: string;
	 	 ContactLastName: string;
	 	 ContactPhone: string;
	 }
	 export var KendoModelOrganizationTransferDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Acronym': { type:'string', nullable: false},
	 	 	'ParentOrganizationID': { type:'any', nullable: true},
	 	 	'NetworkID': { type:'any', nullable: true},
	 	 	'Metadata': { type:'any[]', nullable: false},
	 	 	'ContactEmail': { type:'string', nullable: false},
	 	 	'ContactFirstName': { type:'string', nullable: false},
	 	 	'ContactLastName': { type:'string', nullable: false},
	 	 	'ContactPhone': { type:'string', nullable: false},
	 	 }
	 }
	 export interface INetworkProjectRequestTypeDataMartDTO{
	 	 NetworkID: any;
	 	 Network: string;
	 	 ProjectID: any;
	 	 Project: string;
	 	 DataMartID: any;
	 	 DataMart: string;
	 	 RequestTypeID: any;
	 	 RequestType: string;
	 }
	 export var KendoModelNetworkProjectRequestTypeDataMartDTO: any = {
	 	 fields: {
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'ProjectID': { type:'any', nullable: false},
	 	 	'Project': { type:'string', nullable: false},
	 	 	'DataMartID': { type:'any', nullable: false},
	 	 	'DataMart': { type:'string', nullable: false},
	 	 	'RequestTypeID': { type:'any', nullable: false},
	 	 	'RequestType': { type:'string', nullable: false},
	 	 }
	 }
	 export interface ISearchDTO{
	 	 DomainIDs: any[];
	 	 DomainReferencesIDs: any[];
	 	 NetworkID: any;
	 }
	 export var KendoModelSearchDTO: any = {
	 	 fields: {
	 	 	'DomainIDs': { type:'any[]', nullable: false},
	 	 	'DomainReferencesIDs': { type:'any[]', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 }
	 }
	 export interface IAssignedPermissionDTO{
	 	 SecurityGroupID: any;
	 	 PermissionID: any;
	 	 Allowed: boolean;
	 }
	 export var KendoModelAssignedPermissionDTO: any = {
	 	 fields: {
	 	 	'SecurityGroupID': { type:'any', nullable: false},
	 	 	'PermissionID': { type:'any', nullable: false},
	 	 	'Allowed': { type:'boolean', nullable: false},
	 	 }
	 }
	 export interface IPermissionDTO{
	 	 ID: any;
	 	 Name: string;
	 	 Description: string;
	 }
	 export var KendoModelPermissionDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Description': { type:'string', nullable: false},
	 	 }
	 }
	 export interface ISecurityGroupDTO{
	 	 ID: any;
	 	 Name: string;
	 }
	 export var KendoModelSecurityGroupDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 }
	 }
	 export interface ISecurityGroupUserDTO{
	 	 UserID: any;
	 	 SecurityGroups: ISecurityGroupDTO[];
	 }
	 export var KendoModelSecurityGroupUserDTO: any = {
	 	 fields: {
	 	 	'UserID': { type:'any', nullable: false},
	 	 	'SecurityGroups': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface IUserTransferDTO{
	 	 ID: any;
	 	 NetworkID: any;
	 	 UserName: string;
	 	 Salutation: string;
	 	 FirstName: string;
	 	 MiddleName: string;
	 	 LastName: string;
	 	 EmailAddress: string;
	 	 PhoneNumber: string;
	 	 FaxNumber: string;
	 	 Active: boolean;
	 	 OrganizationID: any;
	 	 Metadata: IDomainDataDTO[];
	 }
	 export var KendoModelUserTransferDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'UserName': { type:'string', nullable: false},
	 	 	'Salutation': { type:'string', nullable: false},
	 	 	'FirstName': { type:'string', nullable: false},
	 	 	'MiddleName': { type:'string', nullable: false},
	 	 	'LastName': { type:'string', nullable: false},
	 	 	'EmailAddress': { type:'string', nullable: false},
	 	 	'PhoneNumber': { type:'string', nullable: false},
	 	 	'FaxNumber': { type:'string', nullable: false},
	 	 	'Active': { type:'boolean', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Metadata': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface IResubmitRouteDTO{
	 	 ResponseID: any;
	 	 RequestDatamartID: any;
	 	 Message: string;
	 }
	 export var KendoModelResubmitRouteDTO: any = {
	 	 fields: {
	 	 	'ResponseID': { type:'any', nullable: false},
	 	 	'RequestDatamartID': { type:'any', nullable: false},
	 	 	'Message': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IUpdateDataMartPriorityAndDueDateDTO{
	 	 RequestDataMartID?: any;
	 	 Priority: number;
	 	 DueDate?: Date;
	 }
	 export var KendoModelUpdateDataMartPriorityAndDueDateDTO: any = {
	 	 fields: {
	 	 	'RequestDataMartID': { type:'any', nullable: true},
	 	 	'Priority': { type:'number', nullable: false},
	 	 	'DueDate': { type:'date', nullable: true},
	 	 }
	 }
	 export interface ISubmitRequestDTO{
	 	 SourceNetworkID: any;
	 	 SourceRequestID: any;
	 	 SerializedSourceRequest: string;
	 	 Routes: ISubmitRouteDTO[];
	 	 Documents: ISubmitRequestDocumentDetailsDTO[];
	 }
	 export var KendoModelSubmitRequestDTO: any = {
	 	 fields: {
	 	 	'SourceNetworkID': { type:'any', nullable: false},
	 	 	'SourceRequestID': { type:'any', nullable: false},
	 	 	'SerializedSourceRequest': { type:'string', nullable: false},
	 	 	'Routes': { type:'any[]', nullable: false},
	 	 	'Documents': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface ISubmitRouteDTO{
	 	 NetworkRouteDefinitionID: any;
	 	 DueDate?: Date;
	 	 Priority: number;
	 	 SourceRequestDataMartID: any;
	 	 SourceResponseID: any;
	 	 RequestDocumentIDs: any[];
	 }
	 export var KendoModelSubmitRouteDTO: any = {
	 	 fields: {
	 	 	'NetworkRouteDefinitionID': { type:'any', nullable: false},
	 	 	'DueDate': { type:'date', nullable: true},
	 	 	'Priority': { type:'number', nullable: false},
	 	 	'SourceRequestDataMartID': { type:'any', nullable: false},
	 	 	'SourceResponseID': { type:'any', nullable: false},
	 	 	'RequestDocumentIDs': { type:'any[]', nullable: false},
	 	 }
	 }
	 export interface ISubmitRequestDocumentDetailsDTO{
	 	 SourceRequestDataSourceID: any;
	 	 RevisionSetID: any;
	 	 DocumentID: any;
	 	 Name: string;
	 	 IsViewable: boolean;
	 	 Kind: string;
	 	 MimeType: string;
	 	 FileName: string;
	 	 Length: number;
	 	 Description: string;
	 }
	 export var KendoModelSubmitRequestDocumentDetailsDTO: any = {
	 	 fields: {
	 	 	'SourceRequestDataSourceID': { type:'any', nullable: false},
	 	 	'RevisionSetID': { type:'any', nullable: false},
	 	 	'DocumentID': { type:'any', nullable: false},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'IsViewable': { type:'boolean', nullable: false},
	 	 	'Kind': { type:'string', nullable: false},
	 	 	'MimeType': { type:'string', nullable: false},
	 	 	'FileName': { type:'string', nullable: false},
	 	 	'Length': { type:'number', nullable: false},
	 	 	'Description': { type:'string', nullable: false},
	 	 }
	 }
	 export interface ISetRoutingStatusDTO{
	 	 ResponseID: any;
	 	 RoutingStatus: number;
	 	 Message: string;
	 }
	 export var KendoModelSetRoutingStatusDTO: any = {
	 	 fields: {
	 	 	'ResponseID': { type:'any', nullable: false},
	 	 	'RoutingStatus': { type:'number', nullable: false},
	 	 	'Message': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IActiveUserDTO{
	 	 ID: any;
	 	 PmnID: any;
	 	 OrganizationID: any;
	 	 PmnOrganizationID: any;
	 	 NetworkID: any;
	 	 UserName: string;
	 	 Network: string;
	 	 Organization: string;
	 }
	 export var KendoModelActiveUserDTO: any = {
	 	 fields: {
	 	 	'ID': { type:'any', nullable: false},
	 	 	'PmnID': { type:'any', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'PmnOrganizationID': { type:'any', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'UserName': { type:'string', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'Organization': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IEntityWithDomainDataItemDTO{
	 	 NetworkID: any;
	 	 Network: string;
	 	 NetworkUrl: string;
	 	 OrganizationID: any;
	 	 Organization: string;
	 	 OrganizationAcronym: string;
	 	 ParentOrganizationID?: any;
	 	 DomainUseID: any;
	 	 DomainID: any;
	 	 ParentDomainID?: any;
	 	 DomainTitle: string;
	 	 DomainIsMultiValueSelect: boolean;
	 	 DomainDataType: string;
	 	 DomainReferenceID?: any;
	 	 DomainReferenceTitle: string;
	 	 DomainReferenceDescription: string;
	 	 DomainReferenceValue: string;
	 	 DomainDataValue: string;
	 	 DomainDataDomainReferenceID?: any;
	 	 DomainAccessValue: number;
	 }
	 export var KendoModelEntityWithDomainDataItemDTO: any = {
	 	 fields: {
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'NetworkUrl': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Organization': { type:'string', nullable: false},
	 	 	'OrganizationAcronym': { type:'string', nullable: false},
	 	 	'ParentOrganizationID': { type:'any', nullable: true},
	 	 	'DomainUseID': { type:'any', nullable: false},
	 	 	'DomainID': { type:'any', nullable: false},
	 	 	'ParentDomainID': { type:'any', nullable: true},
	 	 	'DomainTitle': { type:'string', nullable: false},
	 	 	'DomainIsMultiValueSelect': { type:'boolean', nullable: false},
	 	 	'DomainDataType': { type:'string', nullable: false},
	 	 	'DomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainReferenceTitle': { type:'string', nullable: false},
	 	 	'DomainReferenceDescription': { type:'string', nullable: false},
	 	 	'DomainReferenceValue': { type:'string', nullable: false},
	 	 	'DomainDataValue': { type:'string', nullable: false},
	 	 	'DomainDataDomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainAccessValue': { type:'number', nullable: false},
	 	 }
	 }
	 export interface IDataSourceExtendedDTO extends IDataSourceDTO{
	 	 Organization: string;
	 	 NetworkID: any;
	 	 Network: string;
	 }
	 export var KendoModelDataSourceExtendedDTO: any = {
	 	 fields: {
	 	 	'Organization': { type:'string', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Acronym': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'AdapterSupportedID': { type:'any', nullable: true},
	 	 	'AdapterSupported': { type:'string', nullable: false},
	 	 }
	 }
	 export interface IUpdateAssignedPermissionDTO extends IAssignedPermissionDTO{
	 	 Delete: boolean;
	 }
	 export var KendoModelUpdateAssignedPermissionDTO: any = {
	 	 fields: {
	 	 	'Delete': { type:'boolean', nullable: false},
	 	 	'SecurityGroupID': { type:'any', nullable: false},
	 	 	'PermissionID': { type:'any', nullable: false},
	 	 	'Allowed': { type:'boolean', nullable: false},
	 	 }
	 }
	 export interface IDataSourceWithDomainDataItemDTO extends IEntityWithDomainDataItemDTO{
	 	 DataSourceID: any;
	 	 DataSource: string;
	 	 DataSourceAcronym: string;
	 	 DataSourceAdapterSupportedID?: any;
	 	 DataSourceAdapterSupported: string;
	 	 SupportsCrossNetworkRequests: boolean;
	 }
	 export var KendoModelDataSourceWithDomainDataItemDTO: any = {
	 	 fields: {
	 	 	'DataSourceID': { type:'any', nullable: false},
	 	 	'DataSource': { type:'string', nullable: false},
	 	 	'DataSourceAcronym': { type:'string', nullable: false},
	 	 	'DataSourceAdapterSupportedID': { type:'any', nullable: true},
	 	 	'DataSourceAdapterSupported': { type:'string', nullable: false},
	 	 	'SupportsCrossNetworkRequests': { type:'boolean', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'NetworkUrl': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Organization': { type:'string', nullable: false},
	 	 	'OrganizationAcronym': { type:'string', nullable: false},
	 	 	'ParentOrganizationID': { type:'any', nullable: true},
	 	 	'DomainUseID': { type:'any', nullable: false},
	 	 	'DomainID': { type:'any', nullable: false},
	 	 	'ParentDomainID': { type:'any', nullable: true},
	 	 	'DomainTitle': { type:'string', nullable: false},
	 	 	'DomainIsMultiValueSelect': { type:'boolean', nullable: false},
	 	 	'DomainDataType': { type:'string', nullable: false},
	 	 	'DomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainReferenceTitle': { type:'string', nullable: false},
	 	 	'DomainReferenceDescription': { type:'string', nullable: false},
	 	 	'DomainReferenceValue': { type:'string', nullable: false},
	 	 	'DomainDataValue': { type:'string', nullable: false},
	 	 	'DomainDataDomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainAccessValue': { type:'number', nullable: false},
	 	 }
	 }
	 export interface IUserWithDomainDataItemDTO extends IEntityWithDomainDataItemDTO{
	 	 UserID: any;
	 	 UserName: string;
	 	 UserSalutation: string;
	 	 UserFirstName: string;
	 	 UserMiddleName: string;
	 	 UserLastName: string;
	 	 UserEmailAddress: string;
	 	 UserPhoneNumber: string;
	 	 UserFaxNumber: string;
	 	 UserIsActive: boolean;
	 }
	 export var KendoModelUserWithDomainDataItemDTO: any = {
	 	 fields: {
	 	 	'UserID': { type:'any', nullable: false},
	 	 	'UserName': { type:'string', nullable: false},
	 	 	'UserSalutation': { type:'string', nullable: false},
	 	 	'UserFirstName': { type:'string', nullable: false},
	 	 	'UserMiddleName': { type:'string', nullable: false},
	 	 	'UserLastName': { type:'string', nullable: false},
	 	 	'UserEmailAddress': { type:'string', nullable: false},
	 	 	'UserPhoneNumber': { type:'string', nullable: false},
	 	 	'UserFaxNumber': { type:'string', nullable: false},
	 	 	'UserIsActive': { type:'boolean', nullable: false},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'NetworkUrl': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: false},
	 	 	'Organization': { type:'string', nullable: false},
	 	 	'OrganizationAcronym': { type:'string', nullable: false},
	 	 	'ParentOrganizationID': { type:'any', nullable: true},
	 	 	'DomainUseID': { type:'any', nullable: false},
	 	 	'DomainID': { type:'any', nullable: false},
	 	 	'ParentDomainID': { type:'any', nullable: true},
	 	 	'DomainTitle': { type:'string', nullable: false},
	 	 	'DomainIsMultiValueSelect': { type:'boolean', nullable: false},
	 	 	'DomainDataType': { type:'string', nullable: false},
	 	 	'DomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainReferenceTitle': { type:'string', nullable: false},
	 	 	'DomainReferenceDescription': { type:'string', nullable: false},
	 	 	'DomainReferenceValue': { type:'string', nullable: false},
	 	 	'DomainDataValue': { type:'string', nullable: false},
	 	 	'DomainDataDomainReferenceID': { type:'any', nullable: true},
	 	 	'DomainAccessValue': { type:'number', nullable: false},
	 	 }
	 }
	 export interface INetworkDTO extends IEntityDtoWithID{
	 	 Name: string;
	 	 Url: string;
	 	 ServiceUrl: string;
	 	 ServiceUserName: string;
	 	 ServicePassword: string;
	 }
	 export var KendoModelNetworkDTO: any = {
	 	 fields: {
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Url': { type:'string', nullable: false},
	 	 	'ServiceUrl': { type:'string', nullable: false},
	 	 	'ServiceUserName': { type:'string', nullable: false},
	 	 	'ServicePassword': { type:'string', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Timestamp': { type:'any', nullable: false},
	 	 }
	 }
	 export interface IOrganizationDTO extends IEntityDtoWithID{
	 	 Name: string;
	 	 Acronym: string;
	 	 ParentOrganizationID?: any;
	 	 NetworkID: any;
	 	 ContactEmail: string;
	 	 ContactFirstName: string;
	 	 ContactLastName: string;
	 	 ContactPhone: string;
	 }
	 export var KendoModelOrganizationDTO: any = {
	 	 fields: {
	 	 	'Name': { type:'string', nullable: false},
	 	 	'Acronym': { type:'string', nullable: false},
	 	 	'ParentOrganizationID': { type:'any', nullable: true},
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'ContactEmail': { type:'string', nullable: false},
	 	 	'ContactFirstName': { type:'string', nullable: false},
	 	 	'ContactLastName': { type:'string', nullable: false},
	 	 	'ContactPhone': { type:'string', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Timestamp': { type:'any', nullable: false},
	 	 }
	 }
	 export interface INetworkRequestTypeDefinitionDTO extends IEntityDtoWithID{
	 	 NetworkID: any;
	 	 Network: string;
	 	 ProjectID: any;
	 	 Project: string;
	 	 RequestTypeID: any;
	 	 RequestType: string;
	 	 DataSourceID: any;
	 	 DataSource: string;
	 }
	 export var KendoModelNetworkRequestTypeDefinitionDTO: any = {
	 	 fields: {
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'ProjectID': { type:'any', nullable: false},
	 	 	'Project': { type:'string', nullable: false},
	 	 	'RequestTypeID': { type:'any', nullable: false},
	 	 	'RequestType': { type:'string', nullable: false},
	 	 	'DataSourceID': { type:'any', nullable: false},
	 	 	'DataSource': { type:'string', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Timestamp': { type:'any', nullable: false},
	 	 }
	 }
	 export interface INetworkRequestTypeMappingDTO extends IEntityDtoWithID{
	 	 NetworkID: any;
	 	 Network: string;
	 	 ProjectID: any;
	 	 Project: string;
	 	 RequestTypeID: any;
	 	 RequestType: string;
	 	 Routes: INetworkRequestTypeDefinitionDTO[];
	 }
	 export var KendoModelNetworkRequestTypeMappingDTO: any = {
	 	 fields: {
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'Network': { type:'string', nullable: false},
	 	 	'ProjectID': { type:'any', nullable: false},
	 	 	'Project': { type:'string', nullable: false},
	 	 	'RequestTypeID': { type:'any', nullable: false},
	 	 	'RequestType': { type:'string', nullable: false},
	 	 	'Routes': { type:'any[]', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Timestamp': { type:'any', nullable: false},
	 	 }
	 }
	 export interface IUserDTO extends IEntityDtoWithID{
	 	 NetworkID: any;
	 	 UserName: string;
	 	 Salutation: string;
	 	 FirstName: string;
	 	 MiddleName: string;
	 	 LastName: string;
	 	 EmailAddress: string;
	 	 PhoneNumber: string;
	 	 FaxNumber: string;
	 	 OrganizationID?: any;
	 	 Active: boolean;
	 }
	 export var KendoModelUserDTO: any = {
	 	 fields: {
	 	 	'NetworkID': { type:'any', nullable: false},
	 	 	'UserName': { type:'string', nullable: false},
	 	 	'Salutation': { type:'string', nullable: false},
	 	 	'FirstName': { type:'string', nullable: false},
	 	 	'MiddleName': { type:'string', nullable: false},
	 	 	'LastName': { type:'string', nullable: false},
	 	 	'EmailAddress': { type:'string', nullable: false},
	 	 	'PhoneNumber': { type:'string', nullable: false},
	 	 	'FaxNumber': { type:'string', nullable: false},
	 	 	'OrganizationID': { type:'any', nullable: true},
	 	 	'Active': { type:'boolean', nullable: false},
	 	 	'ID': { type:'any', nullable: true},
	 	 	'Timestamp': { type:'any', nullable: false},
	 	 }
	 }
}
