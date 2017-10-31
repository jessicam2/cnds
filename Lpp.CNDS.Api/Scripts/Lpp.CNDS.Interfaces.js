var CNDS;
(function (CNDS) {
    var Enums;
    (function (Enums) {
        var AccessType;
        (function (AccessType) {
            AccessType[AccessType["NoOne"] = 0] = "NoOne";
            AccessType[AccessType["MyNetwork"] = 100] = "MyNetwork";
            AccessType[AccessType["AllPMNNetworks"] = 1000] = "AllPMNNetworks";
            AccessType[AccessType["AllNetworks"] = 10000] = "AllNetworks";
            AccessType[AccessType["Anyone"] = 100000] = "Anyone";
        })(AccessType = Enums.AccessType || (Enums.AccessType = {}));
        Enums.AccessTypeTranslation = [
            { value: AccessType.NoOne, text: 'No One' },
            { value: AccessType.MyNetwork, text: 'My Network Members' },
            { value: AccessType.AllPMNNetworks, text: 'All PMN Members' },
            { value: AccessType.AllNetworks, text: 'All PMN and CNDS Members' },
            { value: AccessType.Anyone, text: 'Anyone' },
        ];
        var EntityType;
        (function (EntityType) {
            EntityType[EntityType["Organization"] = 0] = "Organization";
            EntityType[EntityType["User"] = 1] = "User";
            EntityType[EntityType["DataSource"] = 2] = "DataSource";
        })(EntityType = Enums.EntityType || (Enums.EntityType = {}));
        Enums.EntityTypeTranslation = [
            { value: EntityType.Organization, text: 'Organization' },
            { value: EntityType.User, text: 'User' },
            { value: EntityType.DataSource, text: 'DataSource' },
        ];
    })(Enums = CNDS.Enums || (CNDS.Enums = {}));
})(CNDS || (CNDS = {}));
(function (CNDS) {
    var Interfaces;
    (function (Interfaces) {
        Interfaces.KendoModelDataSourceDTO = {
            fields: {
                'ID': { type: 'any', nullable: true },
                'Name': { type: 'string', nullable: false },
                'Acronym': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'AdapterSupportedID': { type: 'any', nullable: true },
                'AdapterSupported': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelOrganizationSearchDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'Name': { type: 'string', nullable: false },
                'ContactInformation': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelDataSourceSearchDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'Organization': { type: 'string', nullable: false },
                'Name': { type: 'string', nullable: false },
                'ContactInformation': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelDataSourceTransferDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
                'Acronym': { type: 'string', nullable: false },
                'AdapterSupportedID': { type: 'any', nullable: true },
                'NetworkID': { type: 'any', nullable: true },
                'OrganizationID': { type: 'any', nullable: false },
                'Metadata': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelAddRemoveDomainUseDTO = {
            fields: {
                'AddDomainUse': { type: 'any[]', nullable: false },
                'RemoveDomainUse': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelDomainDataDTO = {
            fields: {
                'ID': { type: 'any', nullable: true },
                'EntityID': { type: 'any', nullable: true },
                'DomainUseID': { type: 'any', nullable: false },
                'Value': { type: 'string', nullable: false },
                'DomainReferenceID': { type: 'any', nullable: true },
                'SequenceNumber': { type: 'number', nullable: false },
                'Visibility': { type: 'cnds.enums.accesstype', nullable: false },
            }
        };
        Interfaces.KendoModelDomainDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'DomainUseID': { type: 'any', nullable: true },
                'ParentDomainID': { type: 'any', nullable: true },
                'Title': { type: 'string', nullable: false },
                'IsMultiValue': { type: 'boolean', nullable: false },
                'EnumValue': { type: 'number', nullable: true },
                'DataType': { type: 'string', nullable: false },
                'EntityType': { type: 'cnds.enums.entitytype', nullable: true },
                'ChildMetadata': { type: 'any[]', nullable: false },
                'References': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelDomainReferenceDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'DomainID': { type: 'any', nullable: false },
                'ParentDomainReferenceID': { type: 'any', nullable: true },
                'Title': { type: 'string', nullable: false },
                'Description': { type: 'string', nullable: false },
                'Value': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelNetworkTransferDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
                'Url': { type: 'string', nullable: false },
                'ServiceUrl': { type: 'string', nullable: false },
                'ServiceUserName': { type: 'string', nullable: false },
                'ServicePassword': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelOrganizationTransferDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
                'Acronym': { type: 'string', nullable: false },
                'ParentOrganizationID': { type: 'any', nullable: true },
                'NetworkID': { type: 'any', nullable: true },
                'Metadata': { type: 'any[]', nullable: false },
                'ContactEmail': { type: 'string', nullable: false },
                'ContactFirstName': { type: 'string', nullable: false },
                'ContactLastName': { type: 'string', nullable: false },
                'ContactPhone': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelNetworkProjectRequestTypeDataMartDTO = {
            fields: {
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'ProjectID': { type: 'any', nullable: false },
                'Project': { type: 'string', nullable: false },
                'DataMartID': { type: 'any', nullable: false },
                'DataMart': { type: 'string', nullable: false },
                'RequestTypeID': { type: 'any', nullable: false },
                'RequestType': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelSearchDTO = {
            fields: {
                'DomainIDs': { type: 'any[]', nullable: false },
                'DomainReferencesIDs': { type: 'any[]', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
            }
        };
        Interfaces.KendoModelAssignedPermissionDTO = {
            fields: {
                'SecurityGroupID': { type: 'any', nullable: false },
                'PermissionID': { type: 'any', nullable: false },
                'Allowed': { type: 'boolean', nullable: false },
            }
        };
        Interfaces.KendoModelPermissionDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
                'Description': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelSecurityGroupDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelSecurityGroupUserDTO = {
            fields: {
                'UserID': { type: 'any', nullable: false },
                'SecurityGroups': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelUserTransferDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'UserName': { type: 'string', nullable: false },
                'Salutation': { type: 'string', nullable: false },
                'FirstName': { type: 'string', nullable: false },
                'MiddleName': { type: 'string', nullable: false },
                'LastName': { type: 'string', nullable: false },
                'EmailAddress': { type: 'string', nullable: false },
                'PhoneNumber': { type: 'string', nullable: false },
                'FaxNumber': { type: 'string', nullable: false },
                'Active': { type: 'boolean', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'Metadata': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelResubmitRouteDTO = {
            fields: {
                'ResponseID': { type: 'any', nullable: false },
                'RequestDatamartID': { type: 'any', nullable: false },
                'Message': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelUpdateDataMartPriorityAndDueDateDTO = {
            fields: {
                'RequestDataMartID': { type: 'any', nullable: true },
                'Priority': { type: 'number', nullable: false },
                'DueDate': { type: 'date', nullable: true },
            }
        };
        Interfaces.KendoModelSubmitRequestDTO = {
            fields: {
                'SourceNetworkID': { type: 'any', nullable: false },
                'SourceRequestID': { type: 'any', nullable: false },
                'SerializedSourceRequest': { type: 'string', nullable: false },
                'Routes': { type: 'any[]', nullable: false },
                'Documents': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelSubmitRouteDTO = {
            fields: {
                'NetworkRouteDefinitionID': { type: 'any', nullable: false },
                'DueDate': { type: 'date', nullable: true },
                'Priority': { type: 'number', nullable: false },
                'SourceRequestDataMartID': { type: 'any', nullable: false },
                'SourceResponseID': { type: 'any', nullable: false },
                'RequestDocumentIDs': { type: 'any[]', nullable: false },
            }
        };
        Interfaces.KendoModelSubmitRequestDocumentDetailsDTO = {
            fields: {
                'SourceRequestDataSourceID': { type: 'any', nullable: false },
                'RevisionSetID': { type: 'any', nullable: false },
                'DocumentID': { type: 'any', nullable: false },
                'Name': { type: 'string', nullable: false },
                'IsViewable': { type: 'boolean', nullable: false },
                'Kind': { type: 'string', nullable: false },
                'MimeType': { type: 'string', nullable: false },
                'FileName': { type: 'string', nullable: false },
                'Length': { type: 'number', nullable: false },
                'Description': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelSetRoutingStatusDTO = {
            fields: {
                'ResponseID': { type: 'any', nullable: false },
                'RoutingStatus': { type: 'number', nullable: false },
                'Message': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelActiveUserDTO = {
            fields: {
                'ID': { type: 'any', nullable: false },
                'PmnID': { type: 'any', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'PmnOrganizationID': { type: 'any', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'UserName': { type: 'string', nullable: false },
                'Network': { type: 'string', nullable: false },
                'Organization': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelEntityWithDomainDataItemDTO = {
            fields: {
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'NetworkUrl': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'Organization': { type: 'string', nullable: false },
                'OrganizationAcronym': { type: 'string', nullable: false },
                'ParentOrganizationID': { type: 'any', nullable: true },
                'DomainUseID': { type: 'any', nullable: false },
                'DomainID': { type: 'any', nullable: false },
                'ParentDomainID': { type: 'any', nullable: true },
                'DomainTitle': { type: 'string', nullable: false },
                'DomainIsMultiValueSelect': { type: 'boolean', nullable: false },
                'DomainDataType': { type: 'string', nullable: false },
                'DomainReferenceID': { type: 'any', nullable: true },
                'DomainReferenceTitle': { type: 'string', nullable: false },
                'DomainReferenceDescription': { type: 'string', nullable: false },
                'DomainReferenceValue': { type: 'string', nullable: false },
                'DomainDataValue': { type: 'string', nullable: false },
                'DomainDataDomainReferenceID': { type: 'any', nullable: true },
                'DomainAccessValue': { type: 'number', nullable: false },
            }
        };
        Interfaces.KendoModelDataSourceExtendedDTO = {
            fields: {
                'Organization': { type: 'string', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Name': { type: 'string', nullable: false },
                'Acronym': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'AdapterSupportedID': { type: 'any', nullable: true },
                'AdapterSupported': { type: 'string', nullable: false },
            }
        };
        Interfaces.KendoModelUpdateAssignedPermissionDTO = {
            fields: {
                'Delete': { type: 'boolean', nullable: false },
                'SecurityGroupID': { type: 'any', nullable: false },
                'PermissionID': { type: 'any', nullable: false },
                'Allowed': { type: 'boolean', nullable: false },
            }
        };
        Interfaces.KendoModelDataSourceWithDomainDataItemDTO = {
            fields: {
                'DataSourceID': { type: 'any', nullable: false },
                'DataSource': { type: 'string', nullable: false },
                'DataSourceAcronym': { type: 'string', nullable: false },
                'DataSourceAdapterSupportedID': { type: 'any', nullable: true },
                'DataSourceAdapterSupported': { type: 'string', nullable: false },
                'SupportsCrossNetworkRequests': { type: 'boolean', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'NetworkUrl': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'Organization': { type: 'string', nullable: false },
                'OrganizationAcronym': { type: 'string', nullable: false },
                'ParentOrganizationID': { type: 'any', nullable: true },
                'DomainUseID': { type: 'any', nullable: false },
                'DomainID': { type: 'any', nullable: false },
                'ParentDomainID': { type: 'any', nullable: true },
                'DomainTitle': { type: 'string', nullable: false },
                'DomainIsMultiValueSelect': { type: 'boolean', nullable: false },
                'DomainDataType': { type: 'string', nullable: false },
                'DomainReferenceID': { type: 'any', nullable: true },
                'DomainReferenceTitle': { type: 'string', nullable: false },
                'DomainReferenceDescription': { type: 'string', nullable: false },
                'DomainReferenceValue': { type: 'string', nullable: false },
                'DomainDataValue': { type: 'string', nullable: false },
                'DomainDataDomainReferenceID': { type: 'any', nullable: true },
                'DomainAccessValue': { type: 'number', nullable: false },
            }
        };
        Interfaces.KendoModelUserWithDomainDataItemDTO = {
            fields: {
                'UserID': { type: 'any', nullable: false },
                'UserName': { type: 'string', nullable: false },
                'UserSalutation': { type: 'string', nullable: false },
                'UserFirstName': { type: 'string', nullable: false },
                'UserMiddleName': { type: 'string', nullable: false },
                'UserLastName': { type: 'string', nullable: false },
                'UserEmailAddress': { type: 'string', nullable: false },
                'UserPhoneNumber': { type: 'string', nullable: false },
                'UserFaxNumber': { type: 'string', nullable: false },
                'UserIsActive': { type: 'boolean', nullable: false },
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'NetworkUrl': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: false },
                'Organization': { type: 'string', nullable: false },
                'OrganizationAcronym': { type: 'string', nullable: false },
                'ParentOrganizationID': { type: 'any', nullable: true },
                'DomainUseID': { type: 'any', nullable: false },
                'DomainID': { type: 'any', nullable: false },
                'ParentDomainID': { type: 'any', nullable: true },
                'DomainTitle': { type: 'string', nullable: false },
                'DomainIsMultiValueSelect': { type: 'boolean', nullable: false },
                'DomainDataType': { type: 'string', nullable: false },
                'DomainReferenceID': { type: 'any', nullable: true },
                'DomainReferenceTitle': { type: 'string', nullable: false },
                'DomainReferenceDescription': { type: 'string', nullable: false },
                'DomainReferenceValue': { type: 'string', nullable: false },
                'DomainDataValue': { type: 'string', nullable: false },
                'DomainDataDomainReferenceID': { type: 'any', nullable: true },
                'DomainAccessValue': { type: 'number', nullable: false },
            }
        };
        Interfaces.KendoModelNetworkDTO = {
            fields: {
                'Name': { type: 'string', nullable: false },
                'Url': { type: 'string', nullable: false },
                'ServiceUrl': { type: 'string', nullable: false },
                'ServiceUserName': { type: 'string', nullable: false },
                'ServicePassword': { type: 'string', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Timestamp': { type: 'any', nullable: false },
            }
        };
        Interfaces.KendoModelOrganizationDTO = {
            fields: {
                'Name': { type: 'string', nullable: false },
                'Acronym': { type: 'string', nullable: false },
                'ParentOrganizationID': { type: 'any', nullable: true },
                'NetworkID': { type: 'any', nullable: false },
                'ContactEmail': { type: 'string', nullable: false },
                'ContactFirstName': { type: 'string', nullable: false },
                'ContactLastName': { type: 'string', nullable: false },
                'ContactPhone': { type: 'string', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Timestamp': { type: 'any', nullable: false },
            }
        };
        Interfaces.KendoModelNetworkRequestTypeDefinitionDTO = {
            fields: {
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'ProjectID': { type: 'any', nullable: false },
                'Project': { type: 'string', nullable: false },
                'RequestTypeID': { type: 'any', nullable: false },
                'RequestType': { type: 'string', nullable: false },
                'DataSourceID': { type: 'any', nullable: false },
                'DataSource': { type: 'string', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Timestamp': { type: 'any', nullable: false },
            }
        };
        Interfaces.KendoModelNetworkRequestTypeMappingDTO = {
            fields: {
                'NetworkID': { type: 'any', nullable: false },
                'Network': { type: 'string', nullable: false },
                'ProjectID': { type: 'any', nullable: false },
                'Project': { type: 'string', nullable: false },
                'RequestTypeID': { type: 'any', nullable: false },
                'RequestType': { type: 'string', nullable: false },
                'Routes': { type: 'any[]', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Timestamp': { type: 'any', nullable: false },
            }
        };
        Interfaces.KendoModelUserDTO = {
            fields: {
                'NetworkID': { type: 'any', nullable: false },
                'UserName': { type: 'string', nullable: false },
                'Salutation': { type: 'string', nullable: false },
                'FirstName': { type: 'string', nullable: false },
                'MiddleName': { type: 'string', nullable: false },
                'LastName': { type: 'string', nullable: false },
                'EmailAddress': { type: 'string', nullable: false },
                'PhoneNumber': { type: 'string', nullable: false },
                'FaxNumber': { type: 'string', nullable: false },
                'OrganizationID': { type: 'any', nullable: true },
                'Active': { type: 'boolean', nullable: false },
                'ID': { type: 'any', nullable: true },
                'Timestamp': { type: 'any', nullable: false },
            }
        };
    })(Interfaces = CNDS.Interfaces || (CNDS.Interfaces = {}));
})(CNDS || (CNDS = {}));
