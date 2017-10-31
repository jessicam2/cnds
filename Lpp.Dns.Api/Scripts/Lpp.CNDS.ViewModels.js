var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var CNDS;
(function (CNDS) {
    var ViewModels;
    (function (ViewModels) {
        var ViewModel = (function () {
            function ViewModel() {
            }
            ViewModel.prototype.update = function (obj) {
                for (var prop in obj) {
                    this[prop](obj[prop]);
                }
            };
            return ViewModel;
        }());
        ViewModels.ViewModel = ViewModel;
        var EntityDtoViewModel = (function (_super) {
            __extends(EntityDtoViewModel, _super);
            function EntityDtoViewModel(BaseDTO) {
                return _super.call(this) || this;
            }
            EntityDtoViewModel.prototype.toData = function () {
                return {};
            };
            return EntityDtoViewModel;
        }(ViewModel));
        ViewModels.EntityDtoViewModel = EntityDtoViewModel;
        var EntityDtoWithIDViewModel = (function (_super) {
            __extends(EntityDtoWithIDViewModel, _super);
            function EntityDtoWithIDViewModel(BaseDTO) {
                var _this = _super.call(this, BaseDTO) || this;
                if (BaseDTO == null) {
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                return _this;
            }
            EntityDtoWithIDViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return EntityDtoWithIDViewModel;
        }(EntityDtoViewModel));
        ViewModels.EntityDtoWithIDViewModel = EntityDtoWithIDViewModel;
        var DataSourceViewModel = (function (_super) {
            __extends(DataSourceViewModel, _super);
            function DataSourceViewModel(DataSourceDTO) {
                var _this = _super.call(this) || this;
                if (DataSourceDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Acronym = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.AdapterSupportedID = ko.observable();
                    _this.AdapterSupported = ko.observable();
                }
                else {
                    _this.ID = ko.observable(DataSourceDTO.ID);
                    _this.Name = ko.observable(DataSourceDTO.Name);
                    _this.Acronym = ko.observable(DataSourceDTO.Acronym);
                    _this.OrganizationID = ko.observable(DataSourceDTO.OrganizationID);
                    _this.AdapterSupportedID = ko.observable(DataSourceDTO.AdapterSupportedID);
                    _this.AdapterSupported = ko.observable(DataSourceDTO.AdapterSupported);
                }
                return _this;
            }
            DataSourceViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                    Acronym: this.Acronym(),
                    OrganizationID: this.OrganizationID(),
                    AdapterSupportedID: this.AdapterSupportedID(),
                    AdapterSupported: this.AdapterSupported(),
                };
            };
            return DataSourceViewModel;
        }(ViewModel));
        ViewModels.DataSourceViewModel = DataSourceViewModel;
        var OrganizationSearchViewModel = (function (_super) {
            __extends(OrganizationSearchViewModel, _super);
            function OrganizationSearchViewModel(OrganizationSearchDTO) {
                var _this = _super.call(this) || this;
                if (OrganizationSearchDTO == null) {
                    _this.ID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.Name = ko.observable();
                    _this.ContactInformation = ko.observable();
                }
                else {
                    _this.ID = ko.observable(OrganizationSearchDTO.ID);
                    _this.NetworkID = ko.observable(OrganizationSearchDTO.NetworkID);
                    _this.Network = ko.observable(OrganizationSearchDTO.Network);
                    _this.Name = ko.observable(OrganizationSearchDTO.Name);
                    _this.ContactInformation = ko.observable(OrganizationSearchDTO.ContactInformation);
                }
                return _this;
            }
            OrganizationSearchViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    Name: this.Name(),
                    ContactInformation: this.ContactInformation(),
                };
            };
            return OrganizationSearchViewModel;
        }(ViewModel));
        ViewModels.OrganizationSearchViewModel = OrganizationSearchViewModel;
        var DataSourceSearchViewModel = (function (_super) {
            __extends(DataSourceSearchViewModel, _super);
            function DataSourceSearchViewModel(DataSourceSearchDTO) {
                var _this = _super.call(this) || this;
                if (DataSourceSearchDTO == null) {
                    _this.ID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Organization = ko.observable();
                    _this.Name = ko.observable();
                    _this.ContactInformation = ko.observable();
                }
                else {
                    _this.ID = ko.observable(DataSourceSearchDTO.ID);
                    _this.NetworkID = ko.observable(DataSourceSearchDTO.NetworkID);
                    _this.Network = ko.observable(DataSourceSearchDTO.Network);
                    _this.OrganizationID = ko.observable(DataSourceSearchDTO.OrganizationID);
                    _this.Organization = ko.observable(DataSourceSearchDTO.Organization);
                    _this.Name = ko.observable(DataSourceSearchDTO.Name);
                    _this.ContactInformation = ko.observable(DataSourceSearchDTO.ContactInformation);
                }
                return _this;
            }
            DataSourceSearchViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    OrganizationID: this.OrganizationID(),
                    Organization: this.Organization(),
                    Name: this.Name(),
                    ContactInformation: this.ContactInformation(),
                };
            };
            return DataSourceSearchViewModel;
        }(ViewModel));
        ViewModels.DataSourceSearchViewModel = DataSourceSearchViewModel;
        var DataSourceTransferViewModel = (function (_super) {
            __extends(DataSourceTransferViewModel, _super);
            function DataSourceTransferViewModel(DataSourceTransferDTO) {
                var _this = _super.call(this) || this;
                if (DataSourceTransferDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Acronym = ko.observable();
                    _this.AdapterSupportedID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Metadata = ko.observableArray();
                }
                else {
                    _this.ID = ko.observable(DataSourceTransferDTO.ID);
                    _this.Name = ko.observable(DataSourceTransferDTO.Name);
                    _this.Acronym = ko.observable(DataSourceTransferDTO.Acronym);
                    _this.AdapterSupportedID = ko.observable(DataSourceTransferDTO.AdapterSupportedID);
                    _this.NetworkID = ko.observable(DataSourceTransferDTO.NetworkID);
                    _this.OrganizationID = ko.observable(DataSourceTransferDTO.OrganizationID);
                    _this.Metadata = ko.observableArray(DataSourceTransferDTO.Metadata == null ? null : DataSourceTransferDTO.Metadata.map(function (item) { return new DomainDataViewModel(item); }));
                }
                return _this;
            }
            DataSourceTransferViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                    Acronym: this.Acronym(),
                    AdapterSupportedID: this.AdapterSupportedID(),
                    NetworkID: this.NetworkID(),
                    OrganizationID: this.OrganizationID(),
                    Metadata: this.Metadata == null ? null : this.Metadata().map(function (item) { return item.toData(); }),
                };
            };
            return DataSourceTransferViewModel;
        }(ViewModel));
        ViewModels.DataSourceTransferViewModel = DataSourceTransferViewModel;
        var AddRemoveDomainUseViewModel = (function (_super) {
            __extends(AddRemoveDomainUseViewModel, _super);
            function AddRemoveDomainUseViewModel(AddRemoveDomainUseDTO) {
                var _this = _super.call(this) || this;
                if (AddRemoveDomainUseDTO == null) {
                    _this.AddDomainUse = ko.observableArray();
                    _this.RemoveDomainUse = ko.observableArray();
                }
                else {
                    _this.AddDomainUse = ko.observableArray(AddRemoveDomainUseDTO.AddDomainUse == null ? null : AddRemoveDomainUseDTO.AddDomainUse.map(function (item) { return item; }));
                    _this.RemoveDomainUse = ko.observableArray(AddRemoveDomainUseDTO.RemoveDomainUse == null ? null : AddRemoveDomainUseDTO.RemoveDomainUse.map(function (item) { return item; }));
                }
                return _this;
            }
            AddRemoveDomainUseViewModel.prototype.toData = function () {
                return {
                    AddDomainUse: this.AddDomainUse(),
                    RemoveDomainUse: this.RemoveDomainUse(),
                };
            };
            return AddRemoveDomainUseViewModel;
        }(ViewModel));
        ViewModels.AddRemoveDomainUseViewModel = AddRemoveDomainUseViewModel;
        var DomainDataViewModel = (function (_super) {
            __extends(DomainDataViewModel, _super);
            function DomainDataViewModel(DomainDataDTO) {
                var _this = _super.call(this) || this;
                if (DomainDataDTO == null) {
                    _this.ID = ko.observable();
                    _this.EntityID = ko.observable();
                    _this.DomainUseID = ko.observable();
                    _this.Value = ko.observable();
                    _this.DomainReferenceID = ko.observable();
                    _this.SequenceNumber = ko.observable();
                    _this.Visibility = ko.observable();
                }
                else {
                    _this.ID = ko.observable(DomainDataDTO.ID);
                    _this.EntityID = ko.observable(DomainDataDTO.EntityID);
                    _this.DomainUseID = ko.observable(DomainDataDTO.DomainUseID);
                    _this.Value = ko.observable(DomainDataDTO.Value);
                    _this.DomainReferenceID = ko.observable(DomainDataDTO.DomainReferenceID);
                    _this.SequenceNumber = ko.observable(DomainDataDTO.SequenceNumber);
                    _this.Visibility = ko.observable(DomainDataDTO.Visibility);
                }
                return _this;
            }
            DomainDataViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    EntityID: this.EntityID(),
                    DomainUseID: this.DomainUseID(),
                    Value: this.Value(),
                    DomainReferenceID: this.DomainReferenceID(),
                    SequenceNumber: this.SequenceNumber(),
                    Visibility: this.Visibility(),
                };
            };
            return DomainDataViewModel;
        }(ViewModel));
        ViewModels.DomainDataViewModel = DomainDataViewModel;
        var DomainViewModel = (function (_super) {
            __extends(DomainViewModel, _super);
            function DomainViewModel(DomainDTO) {
                var _this = _super.call(this) || this;
                if (DomainDTO == null) {
                    _this.ID = ko.observable();
                    _this.DomainUseID = ko.observable();
                    _this.ParentDomainID = ko.observable();
                    _this.Title = ko.observable();
                    _this.IsMultiValue = ko.observable();
                    _this.EnumValue = ko.observable();
                    _this.DataType = ko.observable();
                    _this.EntityType = ko.observable();
                    _this.ChildMetadata = ko.observableArray();
                    _this.References = ko.observableArray();
                }
                else {
                    _this.ID = ko.observable(DomainDTO.ID);
                    _this.DomainUseID = ko.observable(DomainDTO.DomainUseID);
                    _this.ParentDomainID = ko.observable(DomainDTO.ParentDomainID);
                    _this.Title = ko.observable(DomainDTO.Title);
                    _this.IsMultiValue = ko.observable(DomainDTO.IsMultiValue);
                    _this.EnumValue = ko.observable(DomainDTO.EnumValue);
                    _this.DataType = ko.observable(DomainDTO.DataType);
                    _this.EntityType = ko.observable(DomainDTO.EntityType);
                    _this.ChildMetadata = ko.observableArray(DomainDTO.ChildMetadata == null ? null : DomainDTO.ChildMetadata.map(function (item) { return new DomainViewModel(item); }));
                    _this.References = ko.observableArray(DomainDTO.References == null ? null : DomainDTO.References.map(function (item) { return new DomainReferenceViewModel(item); }));
                }
                return _this;
            }
            DomainViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    DomainUseID: this.DomainUseID(),
                    ParentDomainID: this.ParentDomainID(),
                    Title: this.Title(),
                    IsMultiValue: this.IsMultiValue(),
                    EnumValue: this.EnumValue(),
                    DataType: this.DataType(),
                    EntityType: this.EntityType(),
                    ChildMetadata: this.ChildMetadata == null ? null : this.ChildMetadata().map(function (item) { return item.toData(); }),
                    References: this.References == null ? null : this.References().map(function (item) { return item.toData(); }),
                };
            };
            return DomainViewModel;
        }(ViewModel));
        ViewModels.DomainViewModel = DomainViewModel;
        var DomainReferenceViewModel = (function (_super) {
            __extends(DomainReferenceViewModel, _super);
            function DomainReferenceViewModel(DomainReferenceDTO) {
                var _this = _super.call(this) || this;
                if (DomainReferenceDTO == null) {
                    _this.ID = ko.observable();
                    _this.DomainID = ko.observable();
                    _this.ParentDomainReferenceID = ko.observable();
                    _this.Title = ko.observable();
                    _this.Description = ko.observable();
                    _this.Value = ko.observable();
                }
                else {
                    _this.ID = ko.observable(DomainReferenceDTO.ID);
                    _this.DomainID = ko.observable(DomainReferenceDTO.DomainID);
                    _this.ParentDomainReferenceID = ko.observable(DomainReferenceDTO.ParentDomainReferenceID);
                    _this.Title = ko.observable(DomainReferenceDTO.Title);
                    _this.Description = ko.observable(DomainReferenceDTO.Description);
                    _this.Value = ko.observable(DomainReferenceDTO.Value);
                }
                return _this;
            }
            DomainReferenceViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    DomainID: this.DomainID(),
                    ParentDomainReferenceID: this.ParentDomainReferenceID(),
                    Title: this.Title(),
                    Description: this.Description(),
                    Value: this.Value(),
                };
            };
            return DomainReferenceViewModel;
        }(ViewModel));
        ViewModels.DomainReferenceViewModel = DomainReferenceViewModel;
        var NetworkTransferViewModel = (function (_super) {
            __extends(NetworkTransferViewModel, _super);
            function NetworkTransferViewModel(NetworkTransferDTO) {
                var _this = _super.call(this) || this;
                if (NetworkTransferDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Url = ko.observable();
                    _this.ServiceUrl = ko.observable();
                    _this.ServiceUserName = ko.observable();
                    _this.ServicePassword = ko.observable();
                }
                else {
                    _this.ID = ko.observable(NetworkTransferDTO.ID);
                    _this.Name = ko.observable(NetworkTransferDTO.Name);
                    _this.Url = ko.observable(NetworkTransferDTO.Url);
                    _this.ServiceUrl = ko.observable(NetworkTransferDTO.ServiceUrl);
                    _this.ServiceUserName = ko.observable(NetworkTransferDTO.ServiceUserName);
                    _this.ServicePassword = ko.observable(NetworkTransferDTO.ServicePassword);
                }
                return _this;
            }
            NetworkTransferViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                    Url: this.Url(),
                    ServiceUrl: this.ServiceUrl(),
                    ServiceUserName: this.ServiceUserName(),
                    ServicePassword: this.ServicePassword(),
                };
            };
            return NetworkTransferViewModel;
        }(ViewModel));
        ViewModels.NetworkTransferViewModel = NetworkTransferViewModel;
        var OrganizationTransferViewModel = (function (_super) {
            __extends(OrganizationTransferViewModel, _super);
            function OrganizationTransferViewModel(OrganizationTransferDTO) {
                var _this = _super.call(this) || this;
                if (OrganizationTransferDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Acronym = ko.observable();
                    _this.ParentOrganizationID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Metadata = ko.observableArray();
                    _this.ContactEmail = ko.observable();
                    _this.ContactFirstName = ko.observable();
                    _this.ContactLastName = ko.observable();
                    _this.ContactPhone = ko.observable();
                }
                else {
                    _this.ID = ko.observable(OrganizationTransferDTO.ID);
                    _this.Name = ko.observable(OrganizationTransferDTO.Name);
                    _this.Acronym = ko.observable(OrganizationTransferDTO.Acronym);
                    _this.ParentOrganizationID = ko.observable(OrganizationTransferDTO.ParentOrganizationID);
                    _this.NetworkID = ko.observable(OrganizationTransferDTO.NetworkID);
                    _this.Metadata = ko.observableArray(OrganizationTransferDTO.Metadata == null ? null : OrganizationTransferDTO.Metadata.map(function (item) { return new DomainDataViewModel(item); }));
                    _this.ContactEmail = ko.observable(OrganizationTransferDTO.ContactEmail);
                    _this.ContactFirstName = ko.observable(OrganizationTransferDTO.ContactFirstName);
                    _this.ContactLastName = ko.observable(OrganizationTransferDTO.ContactLastName);
                    _this.ContactPhone = ko.observable(OrganizationTransferDTO.ContactPhone);
                }
                return _this;
            }
            OrganizationTransferViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                    Acronym: this.Acronym(),
                    ParentOrganizationID: this.ParentOrganizationID(),
                    NetworkID: this.NetworkID(),
                    Metadata: this.Metadata == null ? null : this.Metadata().map(function (item) { return item.toData(); }),
                    ContactEmail: this.ContactEmail(),
                    ContactFirstName: this.ContactFirstName(),
                    ContactLastName: this.ContactLastName(),
                    ContactPhone: this.ContactPhone(),
                };
            };
            return OrganizationTransferViewModel;
        }(ViewModel));
        ViewModels.OrganizationTransferViewModel = OrganizationTransferViewModel;
        var NetworkProjectRequestTypeDataMartViewModel = (function (_super) {
            __extends(NetworkProjectRequestTypeDataMartViewModel, _super);
            function NetworkProjectRequestTypeDataMartViewModel(NetworkProjectRequestTypeDataMartDTO) {
                var _this = _super.call(this) || this;
                if (NetworkProjectRequestTypeDataMartDTO == null) {
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.ProjectID = ko.observable();
                    _this.Project = ko.observable();
                    _this.DataMartID = ko.observable();
                    _this.DataMart = ko.observable();
                    _this.RequestTypeID = ko.observable();
                    _this.RequestType = ko.observable();
                }
                else {
                    _this.NetworkID = ko.observable(NetworkProjectRequestTypeDataMartDTO.NetworkID);
                    _this.Network = ko.observable(NetworkProjectRequestTypeDataMartDTO.Network);
                    _this.ProjectID = ko.observable(NetworkProjectRequestTypeDataMartDTO.ProjectID);
                    _this.Project = ko.observable(NetworkProjectRequestTypeDataMartDTO.Project);
                    _this.DataMartID = ko.observable(NetworkProjectRequestTypeDataMartDTO.DataMartID);
                    _this.DataMart = ko.observable(NetworkProjectRequestTypeDataMartDTO.DataMart);
                    _this.RequestTypeID = ko.observable(NetworkProjectRequestTypeDataMartDTO.RequestTypeID);
                    _this.RequestType = ko.observable(NetworkProjectRequestTypeDataMartDTO.RequestType);
                }
                return _this;
            }
            NetworkProjectRequestTypeDataMartViewModel.prototype.toData = function () {
                return {
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    ProjectID: this.ProjectID(),
                    Project: this.Project(),
                    DataMartID: this.DataMartID(),
                    DataMart: this.DataMart(),
                    RequestTypeID: this.RequestTypeID(),
                    RequestType: this.RequestType(),
                };
            };
            return NetworkProjectRequestTypeDataMartViewModel;
        }(ViewModel));
        ViewModels.NetworkProjectRequestTypeDataMartViewModel = NetworkProjectRequestTypeDataMartViewModel;
        var SearchViewModel = (function (_super) {
            __extends(SearchViewModel, _super);
            function SearchViewModel(SearchDTO) {
                var _this = _super.call(this) || this;
                if (SearchDTO == null) {
                    _this.DomainIDs = ko.observableArray();
                    _this.DomainReferencesIDs = ko.observableArray();
                    _this.NetworkID = ko.observable();
                }
                else {
                    _this.DomainIDs = ko.observableArray(SearchDTO.DomainIDs == null ? null : SearchDTO.DomainIDs.map(function (item) { return item; }));
                    _this.DomainReferencesIDs = ko.observableArray(SearchDTO.DomainReferencesIDs == null ? null : SearchDTO.DomainReferencesIDs.map(function (item) { return item; }));
                    _this.NetworkID = ko.observable(SearchDTO.NetworkID);
                }
                return _this;
            }
            SearchViewModel.prototype.toData = function () {
                return {
                    DomainIDs: this.DomainIDs(),
                    DomainReferencesIDs: this.DomainReferencesIDs(),
                    NetworkID: this.NetworkID(),
                };
            };
            return SearchViewModel;
        }(ViewModel));
        ViewModels.SearchViewModel = SearchViewModel;
        var AssignedPermissionViewModel = (function (_super) {
            __extends(AssignedPermissionViewModel, _super);
            function AssignedPermissionViewModel(AssignedPermissionDTO) {
                var _this = _super.call(this) || this;
                if (AssignedPermissionDTO == null) {
                    _this.SecurityGroupID = ko.observable();
                    _this.PermissionID = ko.observable();
                    _this.Allowed = ko.observable();
                }
                else {
                    _this.SecurityGroupID = ko.observable(AssignedPermissionDTO.SecurityGroupID);
                    _this.PermissionID = ko.observable(AssignedPermissionDTO.PermissionID);
                    _this.Allowed = ko.observable(AssignedPermissionDTO.Allowed);
                }
                return _this;
            }
            AssignedPermissionViewModel.prototype.toData = function () {
                return {
                    SecurityGroupID: this.SecurityGroupID(),
                    PermissionID: this.PermissionID(),
                    Allowed: this.Allowed(),
                };
            };
            return AssignedPermissionViewModel;
        }(ViewModel));
        ViewModels.AssignedPermissionViewModel = AssignedPermissionViewModel;
        var PermissionViewModel = (function (_super) {
            __extends(PermissionViewModel, _super);
            function PermissionViewModel(PermissionDTO) {
                var _this = _super.call(this) || this;
                if (PermissionDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Description = ko.observable();
                }
                else {
                    _this.ID = ko.observable(PermissionDTO.ID);
                    _this.Name = ko.observable(PermissionDTO.Name);
                    _this.Description = ko.observable(PermissionDTO.Description);
                }
                return _this;
            }
            PermissionViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                    Description: this.Description(),
                };
            };
            return PermissionViewModel;
        }(ViewModel));
        ViewModels.PermissionViewModel = PermissionViewModel;
        var SecurityGroupViewModel = (function (_super) {
            __extends(SecurityGroupViewModel, _super);
            function SecurityGroupViewModel(SecurityGroupDTO) {
                var _this = _super.call(this) || this;
                if (SecurityGroupDTO == null) {
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                }
                else {
                    _this.ID = ko.observable(SecurityGroupDTO.ID);
                    _this.Name = ko.observable(SecurityGroupDTO.Name);
                }
                return _this;
            }
            SecurityGroupViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    Name: this.Name(),
                };
            };
            return SecurityGroupViewModel;
        }(ViewModel));
        ViewModels.SecurityGroupViewModel = SecurityGroupViewModel;
        var SecurityGroupUserViewModel = (function (_super) {
            __extends(SecurityGroupUserViewModel, _super);
            function SecurityGroupUserViewModel(SecurityGroupUserDTO) {
                var _this = _super.call(this) || this;
                if (SecurityGroupUserDTO == null) {
                    _this.UserID = ko.observable();
                    _this.SecurityGroups = ko.observableArray();
                }
                else {
                    _this.UserID = ko.observable(SecurityGroupUserDTO.UserID);
                    _this.SecurityGroups = ko.observableArray(SecurityGroupUserDTO.SecurityGroups == null ? null : SecurityGroupUserDTO.SecurityGroups.map(function (item) { return new SecurityGroupViewModel(item); }));
                }
                return _this;
            }
            SecurityGroupUserViewModel.prototype.toData = function () {
                return {
                    UserID: this.UserID(),
                    SecurityGroups: this.SecurityGroups == null ? null : this.SecurityGroups().map(function (item) { return item.toData(); }),
                };
            };
            return SecurityGroupUserViewModel;
        }(ViewModel));
        ViewModels.SecurityGroupUserViewModel = SecurityGroupUserViewModel;
        var UserTransferViewModel = (function (_super) {
            __extends(UserTransferViewModel, _super);
            function UserTransferViewModel(UserTransferDTO) {
                var _this = _super.call(this) || this;
                if (UserTransferDTO == null) {
                    _this.ID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.UserName = ko.observable();
                    _this.Salutation = ko.observable();
                    _this.FirstName = ko.observable();
                    _this.MiddleName = ko.observable();
                    _this.LastName = ko.observable();
                    _this.EmailAddress = ko.observable();
                    _this.PhoneNumber = ko.observable();
                    _this.FaxNumber = ko.observable();
                    _this.Active = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Metadata = ko.observableArray();
                }
                else {
                    _this.ID = ko.observable(UserTransferDTO.ID);
                    _this.NetworkID = ko.observable(UserTransferDTO.NetworkID);
                    _this.UserName = ko.observable(UserTransferDTO.UserName);
                    _this.Salutation = ko.observable(UserTransferDTO.Salutation);
                    _this.FirstName = ko.observable(UserTransferDTO.FirstName);
                    _this.MiddleName = ko.observable(UserTransferDTO.MiddleName);
                    _this.LastName = ko.observable(UserTransferDTO.LastName);
                    _this.EmailAddress = ko.observable(UserTransferDTO.EmailAddress);
                    _this.PhoneNumber = ko.observable(UserTransferDTO.PhoneNumber);
                    _this.FaxNumber = ko.observable(UserTransferDTO.FaxNumber);
                    _this.Active = ko.observable(UserTransferDTO.Active);
                    _this.OrganizationID = ko.observable(UserTransferDTO.OrganizationID);
                    _this.Metadata = ko.observableArray(UserTransferDTO.Metadata == null ? null : UserTransferDTO.Metadata.map(function (item) { return new DomainDataViewModel(item); }));
                }
                return _this;
            }
            UserTransferViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    NetworkID: this.NetworkID(),
                    UserName: this.UserName(),
                    Salutation: this.Salutation(),
                    FirstName: this.FirstName(),
                    MiddleName: this.MiddleName(),
                    LastName: this.LastName(),
                    EmailAddress: this.EmailAddress(),
                    PhoneNumber: this.PhoneNumber(),
                    FaxNumber: this.FaxNumber(),
                    Active: this.Active(),
                    OrganizationID: this.OrganizationID(),
                    Metadata: this.Metadata == null ? null : this.Metadata().map(function (item) { return item.toData(); }),
                };
            };
            return UserTransferViewModel;
        }(ViewModel));
        ViewModels.UserTransferViewModel = UserTransferViewModel;
        var ResubmitRouteViewModel = (function (_super) {
            __extends(ResubmitRouteViewModel, _super);
            function ResubmitRouteViewModel(ResubmitRouteDTO) {
                var _this = _super.call(this) || this;
                if (ResubmitRouteDTO == null) {
                    _this.ResponseID = ko.observable();
                    _this.RequestDatamartID = ko.observable();
                    _this.Message = ko.observable();
                }
                else {
                    _this.ResponseID = ko.observable(ResubmitRouteDTO.ResponseID);
                    _this.RequestDatamartID = ko.observable(ResubmitRouteDTO.RequestDatamartID);
                    _this.Message = ko.observable(ResubmitRouteDTO.Message);
                }
                return _this;
            }
            ResubmitRouteViewModel.prototype.toData = function () {
                return {
                    ResponseID: this.ResponseID(),
                    RequestDatamartID: this.RequestDatamartID(),
                    Message: this.Message(),
                };
            };
            return ResubmitRouteViewModel;
        }(ViewModel));
        ViewModels.ResubmitRouteViewModel = ResubmitRouteViewModel;
        var UpdateDataMartPriorityAndDueDateViewModel = (function (_super) {
            __extends(UpdateDataMartPriorityAndDueDateViewModel, _super);
            function UpdateDataMartPriorityAndDueDateViewModel(UpdateDataMartPriorityAndDueDateDTO) {
                var _this = _super.call(this) || this;
                if (UpdateDataMartPriorityAndDueDateDTO == null) {
                    _this.RequestDataMartID = ko.observable();
                    _this.Priority = ko.observable();
                    _this.DueDate = ko.observable();
                }
                else {
                    _this.RequestDataMartID = ko.observable(UpdateDataMartPriorityAndDueDateDTO.RequestDataMartID);
                    _this.Priority = ko.observable(UpdateDataMartPriorityAndDueDateDTO.Priority);
                    _this.DueDate = ko.observable(UpdateDataMartPriorityAndDueDateDTO.DueDate);
                }
                return _this;
            }
            UpdateDataMartPriorityAndDueDateViewModel.prototype.toData = function () {
                return {
                    RequestDataMartID: this.RequestDataMartID(),
                    Priority: this.Priority(),
                    DueDate: this.DueDate(),
                };
            };
            return UpdateDataMartPriorityAndDueDateViewModel;
        }(ViewModel));
        ViewModels.UpdateDataMartPriorityAndDueDateViewModel = UpdateDataMartPriorityAndDueDateViewModel;
        var SubmitRequestViewModel = (function (_super) {
            __extends(SubmitRequestViewModel, _super);
            function SubmitRequestViewModel(SubmitRequestDTO) {
                var _this = _super.call(this) || this;
                if (SubmitRequestDTO == null) {
                    _this.SourceNetworkID = ko.observable();
                    _this.SourceRequestID = ko.observable();
                    _this.SerializedSourceRequest = ko.observable();
                    _this.Routes = ko.observableArray();
                    _this.Documents = ko.observableArray();
                }
                else {
                    _this.SourceNetworkID = ko.observable(SubmitRequestDTO.SourceNetworkID);
                    _this.SourceRequestID = ko.observable(SubmitRequestDTO.SourceRequestID);
                    _this.SerializedSourceRequest = ko.observable(SubmitRequestDTO.SerializedSourceRequest);
                    _this.Routes = ko.observableArray(SubmitRequestDTO.Routes == null ? null : SubmitRequestDTO.Routes.map(function (item) { return new SubmitRouteViewModel(item); }));
                    _this.Documents = ko.observableArray(SubmitRequestDTO.Documents == null ? null : SubmitRequestDTO.Documents.map(function (item) { return new SubmitRequestDocumentDetailsViewModel(item); }));
                }
                return _this;
            }
            SubmitRequestViewModel.prototype.toData = function () {
                return {
                    SourceNetworkID: this.SourceNetworkID(),
                    SourceRequestID: this.SourceRequestID(),
                    SerializedSourceRequest: this.SerializedSourceRequest(),
                    Routes: this.Routes == null ? null : this.Routes().map(function (item) { return item.toData(); }),
                    Documents: this.Documents == null ? null : this.Documents().map(function (item) { return item.toData(); }),
                };
            };
            return SubmitRequestViewModel;
        }(ViewModel));
        ViewModels.SubmitRequestViewModel = SubmitRequestViewModel;
        var SubmitRouteViewModel = (function (_super) {
            __extends(SubmitRouteViewModel, _super);
            function SubmitRouteViewModel(SubmitRouteDTO) {
                var _this = _super.call(this) || this;
                if (SubmitRouteDTO == null) {
                    _this.NetworkRouteDefinitionID = ko.observable();
                    _this.DueDate = ko.observable();
                    _this.Priority = ko.observable();
                    _this.SourceRequestDataMartID = ko.observable();
                    _this.SourceResponseID = ko.observable();
                    _this.RequestDocumentIDs = ko.observableArray();
                }
                else {
                    _this.NetworkRouteDefinitionID = ko.observable(SubmitRouteDTO.NetworkRouteDefinitionID);
                    _this.DueDate = ko.observable(SubmitRouteDTO.DueDate);
                    _this.Priority = ko.observable(SubmitRouteDTO.Priority);
                    _this.SourceRequestDataMartID = ko.observable(SubmitRouteDTO.SourceRequestDataMartID);
                    _this.SourceResponseID = ko.observable(SubmitRouteDTO.SourceResponseID);
                    _this.RequestDocumentIDs = ko.observableArray(SubmitRouteDTO.RequestDocumentIDs == null ? null : SubmitRouteDTO.RequestDocumentIDs.map(function (item) { return item; }));
                }
                return _this;
            }
            SubmitRouteViewModel.prototype.toData = function () {
                return {
                    NetworkRouteDefinitionID: this.NetworkRouteDefinitionID(),
                    DueDate: this.DueDate(),
                    Priority: this.Priority(),
                    SourceRequestDataMartID: this.SourceRequestDataMartID(),
                    SourceResponseID: this.SourceResponseID(),
                    RequestDocumentIDs: this.RequestDocumentIDs(),
                };
            };
            return SubmitRouteViewModel;
        }(ViewModel));
        ViewModels.SubmitRouteViewModel = SubmitRouteViewModel;
        var SubmitRequestDocumentDetailsViewModel = (function (_super) {
            __extends(SubmitRequestDocumentDetailsViewModel, _super);
            function SubmitRequestDocumentDetailsViewModel(SubmitRequestDocumentDetailsDTO) {
                var _this = _super.call(this) || this;
                if (SubmitRequestDocumentDetailsDTO == null) {
                    _this.SourceRequestDataSourceID = ko.observable();
                    _this.RevisionSetID = ko.observable();
                    _this.DocumentID = ko.observable();
                    _this.Name = ko.observable();
                    _this.IsViewable = ko.observable();
                    _this.Kind = ko.observable();
                    _this.MimeType = ko.observable();
                    _this.FileName = ko.observable();
                    _this.Length = ko.observable();
                    _this.Description = ko.observable();
                }
                else {
                    _this.SourceRequestDataSourceID = ko.observable(SubmitRequestDocumentDetailsDTO.SourceRequestDataSourceID);
                    _this.RevisionSetID = ko.observable(SubmitRequestDocumentDetailsDTO.RevisionSetID);
                    _this.DocumentID = ko.observable(SubmitRequestDocumentDetailsDTO.DocumentID);
                    _this.Name = ko.observable(SubmitRequestDocumentDetailsDTO.Name);
                    _this.IsViewable = ko.observable(SubmitRequestDocumentDetailsDTO.IsViewable);
                    _this.Kind = ko.observable(SubmitRequestDocumentDetailsDTO.Kind);
                    _this.MimeType = ko.observable(SubmitRequestDocumentDetailsDTO.MimeType);
                    _this.FileName = ko.observable(SubmitRequestDocumentDetailsDTO.FileName);
                    _this.Length = ko.observable(SubmitRequestDocumentDetailsDTO.Length);
                    _this.Description = ko.observable(SubmitRequestDocumentDetailsDTO.Description);
                }
                return _this;
            }
            SubmitRequestDocumentDetailsViewModel.prototype.toData = function () {
                return {
                    SourceRequestDataSourceID: this.SourceRequestDataSourceID(),
                    RevisionSetID: this.RevisionSetID(),
                    DocumentID: this.DocumentID(),
                    Name: this.Name(),
                    IsViewable: this.IsViewable(),
                    Kind: this.Kind(),
                    MimeType: this.MimeType(),
                    FileName: this.FileName(),
                    Length: this.Length(),
                    Description: this.Description(),
                };
            };
            return SubmitRequestDocumentDetailsViewModel;
        }(ViewModel));
        ViewModels.SubmitRequestDocumentDetailsViewModel = SubmitRequestDocumentDetailsViewModel;
        var SetRoutingStatusViewModel = (function (_super) {
            __extends(SetRoutingStatusViewModel, _super);
            function SetRoutingStatusViewModel(SetRoutingStatusDTO) {
                var _this = _super.call(this) || this;
                if (SetRoutingStatusDTO == null) {
                    _this.ResponseID = ko.observable();
                    _this.RoutingStatus = ko.observable();
                    _this.Message = ko.observable();
                }
                else {
                    _this.ResponseID = ko.observable(SetRoutingStatusDTO.ResponseID);
                    _this.RoutingStatus = ko.observable(SetRoutingStatusDTO.RoutingStatus);
                    _this.Message = ko.observable(SetRoutingStatusDTO.Message);
                }
                return _this;
            }
            SetRoutingStatusViewModel.prototype.toData = function () {
                return {
                    ResponseID: this.ResponseID(),
                    RoutingStatus: this.RoutingStatus(),
                    Message: this.Message(),
                };
            };
            return SetRoutingStatusViewModel;
        }(ViewModel));
        ViewModels.SetRoutingStatusViewModel = SetRoutingStatusViewModel;
        var ActiveUserViewModel = (function (_super) {
            __extends(ActiveUserViewModel, _super);
            function ActiveUserViewModel(ActiveUserDTO) {
                var _this = _super.call(this) || this;
                if (ActiveUserDTO == null) {
                    _this.ID = ko.observable();
                    _this.PmnID = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.PmnOrganizationID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.UserName = ko.observable();
                    _this.Network = ko.observable();
                    _this.Organization = ko.observable();
                }
                else {
                    _this.ID = ko.observable(ActiveUserDTO.ID);
                    _this.PmnID = ko.observable(ActiveUserDTO.PmnID);
                    _this.OrganizationID = ko.observable(ActiveUserDTO.OrganizationID);
                    _this.PmnOrganizationID = ko.observable(ActiveUserDTO.PmnOrganizationID);
                    _this.NetworkID = ko.observable(ActiveUserDTO.NetworkID);
                    _this.UserName = ko.observable(ActiveUserDTO.UserName);
                    _this.Network = ko.observable(ActiveUserDTO.Network);
                    _this.Organization = ko.observable(ActiveUserDTO.Organization);
                }
                return _this;
            }
            ActiveUserViewModel.prototype.toData = function () {
                return {
                    ID: this.ID(),
                    PmnID: this.PmnID(),
                    OrganizationID: this.OrganizationID(),
                    PmnOrganizationID: this.PmnOrganizationID(),
                    NetworkID: this.NetworkID(),
                    UserName: this.UserName(),
                    Network: this.Network(),
                    Organization: this.Organization(),
                };
            };
            return ActiveUserViewModel;
        }(ViewModel));
        ViewModels.ActiveUserViewModel = ActiveUserViewModel;
        var EntityWithDomainDataItemViewModel = (function (_super) {
            __extends(EntityWithDomainDataItemViewModel, _super);
            function EntityWithDomainDataItemViewModel(EntityWithDomainDataItemDTO) {
                var _this = _super.call(this) || this;
                if (EntityWithDomainDataItemDTO == null) {
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.NetworkUrl = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Organization = ko.observable();
                    _this.OrganizationAcronym = ko.observable();
                    _this.ParentOrganizationID = ko.observable();
                    _this.DomainUseID = ko.observable();
                    _this.DomainID = ko.observable();
                    _this.ParentDomainID = ko.observable();
                    _this.DomainTitle = ko.observable();
                    _this.DomainIsMultiValueSelect = ko.observable();
                    _this.DomainDataType = ko.observable();
                    _this.DomainReferenceID = ko.observable();
                    _this.DomainReferenceTitle = ko.observable();
                    _this.DomainReferenceDescription = ko.observable();
                    _this.DomainReferenceValue = ko.observable();
                    _this.DomainDataValue = ko.observable();
                    _this.DomainDataDomainReferenceID = ko.observable();
                    _this.DomainAccessValue = ko.observable();
                }
                else {
                    _this.NetworkID = ko.observable(EntityWithDomainDataItemDTO.NetworkID);
                    _this.Network = ko.observable(EntityWithDomainDataItemDTO.Network);
                    _this.NetworkUrl = ko.observable(EntityWithDomainDataItemDTO.NetworkUrl);
                    _this.OrganizationID = ko.observable(EntityWithDomainDataItemDTO.OrganizationID);
                    _this.Organization = ko.observable(EntityWithDomainDataItemDTO.Organization);
                    _this.OrganizationAcronym = ko.observable(EntityWithDomainDataItemDTO.OrganizationAcronym);
                    _this.ParentOrganizationID = ko.observable(EntityWithDomainDataItemDTO.ParentOrganizationID);
                    _this.DomainUseID = ko.observable(EntityWithDomainDataItemDTO.DomainUseID);
                    _this.DomainID = ko.observable(EntityWithDomainDataItemDTO.DomainID);
                    _this.ParentDomainID = ko.observable(EntityWithDomainDataItemDTO.ParentDomainID);
                    _this.DomainTitle = ko.observable(EntityWithDomainDataItemDTO.DomainTitle);
                    _this.DomainIsMultiValueSelect = ko.observable(EntityWithDomainDataItemDTO.DomainIsMultiValueSelect);
                    _this.DomainDataType = ko.observable(EntityWithDomainDataItemDTO.DomainDataType);
                    _this.DomainReferenceID = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceID);
                    _this.DomainReferenceTitle = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceTitle);
                    _this.DomainReferenceDescription = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceDescription);
                    _this.DomainReferenceValue = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceValue);
                    _this.DomainDataValue = ko.observable(EntityWithDomainDataItemDTO.DomainDataValue);
                    _this.DomainDataDomainReferenceID = ko.observable(EntityWithDomainDataItemDTO.DomainDataDomainReferenceID);
                    _this.DomainAccessValue = ko.observable(EntityWithDomainDataItemDTO.DomainAccessValue);
                }
                return _this;
            }
            EntityWithDomainDataItemViewModel.prototype.toData = function () {
                return {
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    NetworkUrl: this.NetworkUrl(),
                    OrganizationID: this.OrganizationID(),
                    Organization: this.Organization(),
                    OrganizationAcronym: this.OrganizationAcronym(),
                    ParentOrganizationID: this.ParentOrganizationID(),
                    DomainUseID: this.DomainUseID(),
                    DomainID: this.DomainID(),
                    ParentDomainID: this.ParentDomainID(),
                    DomainTitle: this.DomainTitle(),
                    DomainIsMultiValueSelect: this.DomainIsMultiValueSelect(),
                    DomainDataType: this.DomainDataType(),
                    DomainReferenceID: this.DomainReferenceID(),
                    DomainReferenceTitle: this.DomainReferenceTitle(),
                    DomainReferenceDescription: this.DomainReferenceDescription(),
                    DomainReferenceValue: this.DomainReferenceValue(),
                    DomainDataValue: this.DomainDataValue(),
                    DomainDataDomainReferenceID: this.DomainDataDomainReferenceID(),
                    DomainAccessValue: this.DomainAccessValue(),
                };
            };
            return EntityWithDomainDataItemViewModel;
        }(ViewModel));
        ViewModels.EntityWithDomainDataItemViewModel = EntityWithDomainDataItemViewModel;
        var DataSourceExtendedViewModel = (function (_super) {
            __extends(DataSourceExtendedViewModel, _super);
            function DataSourceExtendedViewModel(DataSourceExtendedDTO) {
                var _this = _super.call(this) || this;
                if (DataSourceExtendedDTO == null) {
                    _this.Organization = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.ID = ko.observable();
                    _this.Name = ko.observable();
                    _this.Acronym = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.AdapterSupportedID = ko.observable();
                    _this.AdapterSupported = ko.observable();
                }
                else {
                    _this.Organization = ko.observable(DataSourceExtendedDTO.Organization);
                    _this.NetworkID = ko.observable(DataSourceExtendedDTO.NetworkID);
                    _this.Network = ko.observable(DataSourceExtendedDTO.Network);
                    _this.ID = ko.observable(DataSourceExtendedDTO.ID);
                    _this.Name = ko.observable(DataSourceExtendedDTO.Name);
                    _this.Acronym = ko.observable(DataSourceExtendedDTO.Acronym);
                    _this.OrganizationID = ko.observable(DataSourceExtendedDTO.OrganizationID);
                    _this.AdapterSupportedID = ko.observable(DataSourceExtendedDTO.AdapterSupportedID);
                    _this.AdapterSupported = ko.observable(DataSourceExtendedDTO.AdapterSupported);
                }
                return _this;
            }
            DataSourceExtendedViewModel.prototype.toData = function () {
                return {
                    Organization: this.Organization(),
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    ID: this.ID(),
                    Name: this.Name(),
                    Acronym: this.Acronym(),
                    OrganizationID: this.OrganizationID(),
                    AdapterSupportedID: this.AdapterSupportedID(),
                    AdapterSupported: this.AdapterSupported(),
                };
            };
            return DataSourceExtendedViewModel;
        }(DataSourceViewModel));
        ViewModels.DataSourceExtendedViewModel = DataSourceExtendedViewModel;
        var UpdateAssignedPermissionViewModel = (function (_super) {
            __extends(UpdateAssignedPermissionViewModel, _super);
            function UpdateAssignedPermissionViewModel(UpdateAssignedPermissionDTO) {
                var _this = _super.call(this) || this;
                if (UpdateAssignedPermissionDTO == null) {
                    _this.Delete = ko.observable();
                    _this.SecurityGroupID = ko.observable();
                    _this.PermissionID = ko.observable();
                    _this.Allowed = ko.observable();
                }
                else {
                    _this.Delete = ko.observable(UpdateAssignedPermissionDTO.Delete);
                    _this.SecurityGroupID = ko.observable(UpdateAssignedPermissionDTO.SecurityGroupID);
                    _this.PermissionID = ko.observable(UpdateAssignedPermissionDTO.PermissionID);
                    _this.Allowed = ko.observable(UpdateAssignedPermissionDTO.Allowed);
                }
                return _this;
            }
            UpdateAssignedPermissionViewModel.prototype.toData = function () {
                return {
                    Delete: this.Delete(),
                    SecurityGroupID: this.SecurityGroupID(),
                    PermissionID: this.PermissionID(),
                    Allowed: this.Allowed(),
                };
            };
            return UpdateAssignedPermissionViewModel;
        }(AssignedPermissionViewModel));
        ViewModels.UpdateAssignedPermissionViewModel = UpdateAssignedPermissionViewModel;
        var DataSourceWithDomainDataItemViewModel = (function (_super) {
            __extends(DataSourceWithDomainDataItemViewModel, _super);
            function DataSourceWithDomainDataItemViewModel(DataSourceWithDomainDataItemDTO) {
                var _this = _super.call(this) || this;
                if (DataSourceWithDomainDataItemDTO == null) {
                    _this.DataSourceID = ko.observable();
                    _this.DataSource = ko.observable();
                    _this.DataSourceAcronym = ko.observable();
                    _this.DataSourceAdapterSupportedID = ko.observable();
                    _this.DataSourceAdapterSupported = ko.observable();
                    _this.SupportsCrossNetworkRequests = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.NetworkUrl = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Organization = ko.observable();
                    _this.OrganizationAcronym = ko.observable();
                    _this.ParentOrganizationID = ko.observable();
                    _this.DomainUseID = ko.observable();
                    _this.DomainID = ko.observable();
                    _this.ParentDomainID = ko.observable();
                    _this.DomainTitle = ko.observable();
                    _this.DomainIsMultiValueSelect = ko.observable();
                    _this.DomainDataType = ko.observable();
                    _this.DomainReferenceID = ko.observable();
                    _this.DomainReferenceTitle = ko.observable();
                    _this.DomainReferenceDescription = ko.observable();
                    _this.DomainReferenceValue = ko.observable();
                    _this.DomainDataValue = ko.observable();
                    _this.DomainDataDomainReferenceID = ko.observable();
                    _this.DomainAccessValue = ko.observable();
                }
                else {
                    _this.DataSourceID = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceID);
                    _this.DataSource = ko.observable(DataSourceWithDomainDataItemDTO.DataSource);
                    _this.DataSourceAcronym = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAcronym);
                    _this.DataSourceAdapterSupportedID = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAdapterSupportedID);
                    _this.DataSourceAdapterSupported = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAdapterSupported);
                    _this.SupportsCrossNetworkRequests = ko.observable(DataSourceWithDomainDataItemDTO.SupportsCrossNetworkRequests);
                    _this.NetworkID = ko.observable(DataSourceWithDomainDataItemDTO.NetworkID);
                    _this.Network = ko.observable(DataSourceWithDomainDataItemDTO.Network);
                    _this.NetworkUrl = ko.observable(DataSourceWithDomainDataItemDTO.NetworkUrl);
                    _this.OrganizationID = ko.observable(DataSourceWithDomainDataItemDTO.OrganizationID);
                    _this.Organization = ko.observable(DataSourceWithDomainDataItemDTO.Organization);
                    _this.OrganizationAcronym = ko.observable(DataSourceWithDomainDataItemDTO.OrganizationAcronym);
                    _this.ParentOrganizationID = ko.observable(DataSourceWithDomainDataItemDTO.ParentOrganizationID);
                    _this.DomainUseID = ko.observable(DataSourceWithDomainDataItemDTO.DomainUseID);
                    _this.DomainID = ko.observable(DataSourceWithDomainDataItemDTO.DomainID);
                    _this.ParentDomainID = ko.observable(DataSourceWithDomainDataItemDTO.ParentDomainID);
                    _this.DomainTitle = ko.observable(DataSourceWithDomainDataItemDTO.DomainTitle);
                    _this.DomainIsMultiValueSelect = ko.observable(DataSourceWithDomainDataItemDTO.DomainIsMultiValueSelect);
                    _this.DomainDataType = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataType);
                    _this.DomainReferenceID = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceID);
                    _this.DomainReferenceTitle = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceTitle);
                    _this.DomainReferenceDescription = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceDescription);
                    _this.DomainReferenceValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceValue);
                    _this.DomainDataValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataValue);
                    _this.DomainDataDomainReferenceID = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataDomainReferenceID);
                    _this.DomainAccessValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainAccessValue);
                }
                return _this;
            }
            DataSourceWithDomainDataItemViewModel.prototype.toData = function () {
                return {
                    DataSourceID: this.DataSourceID(),
                    DataSource: this.DataSource(),
                    DataSourceAcronym: this.DataSourceAcronym(),
                    DataSourceAdapterSupportedID: this.DataSourceAdapterSupportedID(),
                    DataSourceAdapterSupported: this.DataSourceAdapterSupported(),
                    SupportsCrossNetworkRequests: this.SupportsCrossNetworkRequests(),
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    NetworkUrl: this.NetworkUrl(),
                    OrganizationID: this.OrganizationID(),
                    Organization: this.Organization(),
                    OrganizationAcronym: this.OrganizationAcronym(),
                    ParentOrganizationID: this.ParentOrganizationID(),
                    DomainUseID: this.DomainUseID(),
                    DomainID: this.DomainID(),
                    ParentDomainID: this.ParentDomainID(),
                    DomainTitle: this.DomainTitle(),
                    DomainIsMultiValueSelect: this.DomainIsMultiValueSelect(),
                    DomainDataType: this.DomainDataType(),
                    DomainReferenceID: this.DomainReferenceID(),
                    DomainReferenceTitle: this.DomainReferenceTitle(),
                    DomainReferenceDescription: this.DomainReferenceDescription(),
                    DomainReferenceValue: this.DomainReferenceValue(),
                    DomainDataValue: this.DomainDataValue(),
                    DomainDataDomainReferenceID: this.DomainDataDomainReferenceID(),
                    DomainAccessValue: this.DomainAccessValue(),
                };
            };
            return DataSourceWithDomainDataItemViewModel;
        }(EntityWithDomainDataItemViewModel));
        ViewModels.DataSourceWithDomainDataItemViewModel = DataSourceWithDomainDataItemViewModel;
        var UserWithDomainDataItemViewModel = (function (_super) {
            __extends(UserWithDomainDataItemViewModel, _super);
            function UserWithDomainDataItemViewModel(UserWithDomainDataItemDTO) {
                var _this = _super.call(this) || this;
                if (UserWithDomainDataItemDTO == null) {
                    _this.UserID = ko.observable();
                    _this.UserName = ko.observable();
                    _this.UserSalutation = ko.observable();
                    _this.UserFirstName = ko.observable();
                    _this.UserMiddleName = ko.observable();
                    _this.UserLastName = ko.observable();
                    _this.UserEmailAddress = ko.observable();
                    _this.UserPhoneNumber = ko.observable();
                    _this.UserFaxNumber = ko.observable();
                    _this.UserIsActive = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.NetworkUrl = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Organization = ko.observable();
                    _this.OrganizationAcronym = ko.observable();
                    _this.ParentOrganizationID = ko.observable();
                    _this.DomainUseID = ko.observable();
                    _this.DomainID = ko.observable();
                    _this.ParentDomainID = ko.observable();
                    _this.DomainTitle = ko.observable();
                    _this.DomainIsMultiValueSelect = ko.observable();
                    _this.DomainDataType = ko.observable();
                    _this.DomainReferenceID = ko.observable();
                    _this.DomainReferenceTitle = ko.observable();
                    _this.DomainReferenceDescription = ko.observable();
                    _this.DomainReferenceValue = ko.observable();
                    _this.DomainDataValue = ko.observable();
                    _this.DomainDataDomainReferenceID = ko.observable();
                    _this.DomainAccessValue = ko.observable();
                }
                else {
                    _this.UserID = ko.observable(UserWithDomainDataItemDTO.UserID);
                    _this.UserName = ko.observable(UserWithDomainDataItemDTO.UserName);
                    _this.UserSalutation = ko.observable(UserWithDomainDataItemDTO.UserSalutation);
                    _this.UserFirstName = ko.observable(UserWithDomainDataItemDTO.UserFirstName);
                    _this.UserMiddleName = ko.observable(UserWithDomainDataItemDTO.UserMiddleName);
                    _this.UserLastName = ko.observable(UserWithDomainDataItemDTO.UserLastName);
                    _this.UserEmailAddress = ko.observable(UserWithDomainDataItemDTO.UserEmailAddress);
                    _this.UserPhoneNumber = ko.observable(UserWithDomainDataItemDTO.UserPhoneNumber);
                    _this.UserFaxNumber = ko.observable(UserWithDomainDataItemDTO.UserFaxNumber);
                    _this.UserIsActive = ko.observable(UserWithDomainDataItemDTO.UserIsActive);
                    _this.NetworkID = ko.observable(UserWithDomainDataItemDTO.NetworkID);
                    _this.Network = ko.observable(UserWithDomainDataItemDTO.Network);
                    _this.NetworkUrl = ko.observable(UserWithDomainDataItemDTO.NetworkUrl);
                    _this.OrganizationID = ko.observable(UserWithDomainDataItemDTO.OrganizationID);
                    _this.Organization = ko.observable(UserWithDomainDataItemDTO.Organization);
                    _this.OrganizationAcronym = ko.observable(UserWithDomainDataItemDTO.OrganizationAcronym);
                    _this.ParentOrganizationID = ko.observable(UserWithDomainDataItemDTO.ParentOrganizationID);
                    _this.DomainUseID = ko.observable(UserWithDomainDataItemDTO.DomainUseID);
                    _this.DomainID = ko.observable(UserWithDomainDataItemDTO.DomainID);
                    _this.ParentDomainID = ko.observable(UserWithDomainDataItemDTO.ParentDomainID);
                    _this.DomainTitle = ko.observable(UserWithDomainDataItemDTO.DomainTitle);
                    _this.DomainIsMultiValueSelect = ko.observable(UserWithDomainDataItemDTO.DomainIsMultiValueSelect);
                    _this.DomainDataType = ko.observable(UserWithDomainDataItemDTO.DomainDataType);
                    _this.DomainReferenceID = ko.observable(UserWithDomainDataItemDTO.DomainReferenceID);
                    _this.DomainReferenceTitle = ko.observable(UserWithDomainDataItemDTO.DomainReferenceTitle);
                    _this.DomainReferenceDescription = ko.observable(UserWithDomainDataItemDTO.DomainReferenceDescription);
                    _this.DomainReferenceValue = ko.observable(UserWithDomainDataItemDTO.DomainReferenceValue);
                    _this.DomainDataValue = ko.observable(UserWithDomainDataItemDTO.DomainDataValue);
                    _this.DomainDataDomainReferenceID = ko.observable(UserWithDomainDataItemDTO.DomainDataDomainReferenceID);
                    _this.DomainAccessValue = ko.observable(UserWithDomainDataItemDTO.DomainAccessValue);
                }
                return _this;
            }
            UserWithDomainDataItemViewModel.prototype.toData = function () {
                return {
                    UserID: this.UserID(),
                    UserName: this.UserName(),
                    UserSalutation: this.UserSalutation(),
                    UserFirstName: this.UserFirstName(),
                    UserMiddleName: this.UserMiddleName(),
                    UserLastName: this.UserLastName(),
                    UserEmailAddress: this.UserEmailAddress(),
                    UserPhoneNumber: this.UserPhoneNumber(),
                    UserFaxNumber: this.UserFaxNumber(),
                    UserIsActive: this.UserIsActive(),
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    NetworkUrl: this.NetworkUrl(),
                    OrganizationID: this.OrganizationID(),
                    Organization: this.Organization(),
                    OrganizationAcronym: this.OrganizationAcronym(),
                    ParentOrganizationID: this.ParentOrganizationID(),
                    DomainUseID: this.DomainUseID(),
                    DomainID: this.DomainID(),
                    ParentDomainID: this.ParentDomainID(),
                    DomainTitle: this.DomainTitle(),
                    DomainIsMultiValueSelect: this.DomainIsMultiValueSelect(),
                    DomainDataType: this.DomainDataType(),
                    DomainReferenceID: this.DomainReferenceID(),
                    DomainReferenceTitle: this.DomainReferenceTitle(),
                    DomainReferenceDescription: this.DomainReferenceDescription(),
                    DomainReferenceValue: this.DomainReferenceValue(),
                    DomainDataValue: this.DomainDataValue(),
                    DomainDataDomainReferenceID: this.DomainDataDomainReferenceID(),
                    DomainAccessValue: this.DomainAccessValue(),
                };
            };
            return UserWithDomainDataItemViewModel;
        }(EntityWithDomainDataItemViewModel));
        ViewModels.UserWithDomainDataItemViewModel = UserWithDomainDataItemViewModel;
        var NetworkViewModel = (function (_super) {
            __extends(NetworkViewModel, _super);
            function NetworkViewModel(NetworkDTO) {
                var _this = _super.call(this) || this;
                if (NetworkDTO == null) {
                    _this.Name = ko.observable();
                    _this.Url = ko.observable();
                    _this.ServiceUrl = ko.observable();
                    _this.ServiceUserName = ko.observable();
                    _this.ServicePassword = ko.observable();
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                else {
                    _this.Name = ko.observable(NetworkDTO.Name);
                    _this.Url = ko.observable(NetworkDTO.Url);
                    _this.ServiceUrl = ko.observable(NetworkDTO.ServiceUrl);
                    _this.ServiceUserName = ko.observable(NetworkDTO.ServiceUserName);
                    _this.ServicePassword = ko.observable(NetworkDTO.ServicePassword);
                    _this.ID = ko.observable(NetworkDTO.ID);
                    _this.Timestamp = ko.observable(NetworkDTO.Timestamp);
                }
                return _this;
            }
            NetworkViewModel.prototype.toData = function () {
                return {
                    Name: this.Name(),
                    Url: this.Url(),
                    ServiceUrl: this.ServiceUrl(),
                    ServiceUserName: this.ServiceUserName(),
                    ServicePassword: this.ServicePassword(),
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return NetworkViewModel;
        }(EntityDtoWithIDViewModel));
        ViewModels.NetworkViewModel = NetworkViewModel;
        var OrganizationViewModel = (function (_super) {
            __extends(OrganizationViewModel, _super);
            function OrganizationViewModel(OrganizationDTO) {
                var _this = _super.call(this) || this;
                if (OrganizationDTO == null) {
                    _this.Name = ko.observable();
                    _this.Acronym = ko.observable();
                    _this.ParentOrganizationID = ko.observable();
                    _this.NetworkID = ko.observable();
                    _this.ContactEmail = ko.observable();
                    _this.ContactFirstName = ko.observable();
                    _this.ContactLastName = ko.observable();
                    _this.ContactPhone = ko.observable();
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                else {
                    _this.Name = ko.observable(OrganizationDTO.Name);
                    _this.Acronym = ko.observable(OrganizationDTO.Acronym);
                    _this.ParentOrganizationID = ko.observable(OrganizationDTO.ParentOrganizationID);
                    _this.NetworkID = ko.observable(OrganizationDTO.NetworkID);
                    _this.ContactEmail = ko.observable(OrganizationDTO.ContactEmail);
                    _this.ContactFirstName = ko.observable(OrganizationDTO.ContactFirstName);
                    _this.ContactLastName = ko.observable(OrganizationDTO.ContactLastName);
                    _this.ContactPhone = ko.observable(OrganizationDTO.ContactPhone);
                    _this.ID = ko.observable(OrganizationDTO.ID);
                    _this.Timestamp = ko.observable(OrganizationDTO.Timestamp);
                }
                return _this;
            }
            OrganizationViewModel.prototype.toData = function () {
                return {
                    Name: this.Name(),
                    Acronym: this.Acronym(),
                    ParentOrganizationID: this.ParentOrganizationID(),
                    NetworkID: this.NetworkID(),
                    ContactEmail: this.ContactEmail(),
                    ContactFirstName: this.ContactFirstName(),
                    ContactLastName: this.ContactLastName(),
                    ContactPhone: this.ContactPhone(),
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return OrganizationViewModel;
        }(EntityDtoWithIDViewModel));
        ViewModels.OrganizationViewModel = OrganizationViewModel;
        var NetworkRequestTypeDefinitionViewModel = (function (_super) {
            __extends(NetworkRequestTypeDefinitionViewModel, _super);
            function NetworkRequestTypeDefinitionViewModel(NetworkRequestTypeDefinitionDTO) {
                var _this = _super.call(this) || this;
                if (NetworkRequestTypeDefinitionDTO == null) {
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.ProjectID = ko.observable();
                    _this.Project = ko.observable();
                    _this.RequestTypeID = ko.observable();
                    _this.RequestType = ko.observable();
                    _this.DataSourceID = ko.observable();
                    _this.DataSource = ko.observable();
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                else {
                    _this.NetworkID = ko.observable(NetworkRequestTypeDefinitionDTO.NetworkID);
                    _this.Network = ko.observable(NetworkRequestTypeDefinitionDTO.Network);
                    _this.ProjectID = ko.observable(NetworkRequestTypeDefinitionDTO.ProjectID);
                    _this.Project = ko.observable(NetworkRequestTypeDefinitionDTO.Project);
                    _this.RequestTypeID = ko.observable(NetworkRequestTypeDefinitionDTO.RequestTypeID);
                    _this.RequestType = ko.observable(NetworkRequestTypeDefinitionDTO.RequestType);
                    _this.DataSourceID = ko.observable(NetworkRequestTypeDefinitionDTO.DataSourceID);
                    _this.DataSource = ko.observable(NetworkRequestTypeDefinitionDTO.DataSource);
                    _this.ID = ko.observable(NetworkRequestTypeDefinitionDTO.ID);
                    _this.Timestamp = ko.observable(NetworkRequestTypeDefinitionDTO.Timestamp);
                }
                return _this;
            }
            NetworkRequestTypeDefinitionViewModel.prototype.toData = function () {
                return {
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    ProjectID: this.ProjectID(),
                    Project: this.Project(),
                    RequestTypeID: this.RequestTypeID(),
                    RequestType: this.RequestType(),
                    DataSourceID: this.DataSourceID(),
                    DataSource: this.DataSource(),
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return NetworkRequestTypeDefinitionViewModel;
        }(EntityDtoWithIDViewModel));
        ViewModels.NetworkRequestTypeDefinitionViewModel = NetworkRequestTypeDefinitionViewModel;
        var NetworkRequestTypeMappingViewModel = (function (_super) {
            __extends(NetworkRequestTypeMappingViewModel, _super);
            function NetworkRequestTypeMappingViewModel(NetworkRequestTypeMappingDTO) {
                var _this = _super.call(this) || this;
                if (NetworkRequestTypeMappingDTO == null) {
                    _this.NetworkID = ko.observable();
                    _this.Network = ko.observable();
                    _this.ProjectID = ko.observable();
                    _this.Project = ko.observable();
                    _this.RequestTypeID = ko.observable();
                    _this.RequestType = ko.observable();
                    _this.Routes = ko.observableArray();
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                else {
                    _this.NetworkID = ko.observable(NetworkRequestTypeMappingDTO.NetworkID);
                    _this.Network = ko.observable(NetworkRequestTypeMappingDTO.Network);
                    _this.ProjectID = ko.observable(NetworkRequestTypeMappingDTO.ProjectID);
                    _this.Project = ko.observable(NetworkRequestTypeMappingDTO.Project);
                    _this.RequestTypeID = ko.observable(NetworkRequestTypeMappingDTO.RequestTypeID);
                    _this.RequestType = ko.observable(NetworkRequestTypeMappingDTO.RequestType);
                    _this.Routes = ko.observableArray(NetworkRequestTypeMappingDTO.Routes == null ? null : NetworkRequestTypeMappingDTO.Routes.map(function (item) { return new NetworkRequestTypeDefinitionViewModel(item); }));
                    _this.ID = ko.observable(NetworkRequestTypeMappingDTO.ID);
                    _this.Timestamp = ko.observable(NetworkRequestTypeMappingDTO.Timestamp);
                }
                return _this;
            }
            NetworkRequestTypeMappingViewModel.prototype.toData = function () {
                return {
                    NetworkID: this.NetworkID(),
                    Network: this.Network(),
                    ProjectID: this.ProjectID(),
                    Project: this.Project(),
                    RequestTypeID: this.RequestTypeID(),
                    RequestType: this.RequestType(),
                    Routes: this.Routes == null ? null : this.Routes().map(function (item) { return item.toData(); }),
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return NetworkRequestTypeMappingViewModel;
        }(EntityDtoWithIDViewModel));
        ViewModels.NetworkRequestTypeMappingViewModel = NetworkRequestTypeMappingViewModel;
        var UserViewModel = (function (_super) {
            __extends(UserViewModel, _super);
            function UserViewModel(UserDTO) {
                var _this = _super.call(this) || this;
                if (UserDTO == null) {
                    _this.NetworkID = ko.observable();
                    _this.UserName = ko.observable();
                    _this.Salutation = ko.observable();
                    _this.FirstName = ko.observable();
                    _this.MiddleName = ko.observable();
                    _this.LastName = ko.observable();
                    _this.EmailAddress = ko.observable();
                    _this.PhoneNumber = ko.observable();
                    _this.FaxNumber = ko.observable();
                    _this.OrganizationID = ko.observable();
                    _this.Active = ko.observable();
                    _this.ID = ko.observable();
                    _this.Timestamp = ko.observable();
                }
                else {
                    _this.NetworkID = ko.observable(UserDTO.NetworkID);
                    _this.UserName = ko.observable(UserDTO.UserName);
                    _this.Salutation = ko.observable(UserDTO.Salutation);
                    _this.FirstName = ko.observable(UserDTO.FirstName);
                    _this.MiddleName = ko.observable(UserDTO.MiddleName);
                    _this.LastName = ko.observable(UserDTO.LastName);
                    _this.EmailAddress = ko.observable(UserDTO.EmailAddress);
                    _this.PhoneNumber = ko.observable(UserDTO.PhoneNumber);
                    _this.FaxNumber = ko.observable(UserDTO.FaxNumber);
                    _this.OrganizationID = ko.observable(UserDTO.OrganizationID);
                    _this.Active = ko.observable(UserDTO.Active);
                    _this.ID = ko.observable(UserDTO.ID);
                    _this.Timestamp = ko.observable(UserDTO.Timestamp);
                }
                return _this;
            }
            UserViewModel.prototype.toData = function () {
                return {
                    NetworkID: this.NetworkID(),
                    UserName: this.UserName(),
                    Salutation: this.Salutation(),
                    FirstName: this.FirstName(),
                    MiddleName: this.MiddleName(),
                    LastName: this.LastName(),
                    EmailAddress: this.EmailAddress(),
                    PhoneNumber: this.PhoneNumber(),
                    FaxNumber: this.FaxNumber(),
                    OrganizationID: this.OrganizationID(),
                    Active: this.Active(),
                    ID: this.ID(),
                    Timestamp: this.Timestamp(),
                };
            };
            return UserViewModel;
        }(EntityDtoWithIDViewModel));
        ViewModels.UserViewModel = UserViewModel;
    })(ViewModels = CNDS.ViewModels || (CNDS.ViewModels = {}));
})(CNDS || (CNDS = {}));
