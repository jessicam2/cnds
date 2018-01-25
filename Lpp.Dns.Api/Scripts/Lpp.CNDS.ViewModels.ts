/// <reference path='../../Lpp.Pmn.Resources/node_modules/@types/knockout.mapping/index.d.ts' />
/// <reference path='Lpp.CNDS.Interfaces.ts' />
module CNDS.ViewModels {
	 export class ViewModel<D>{
	 	 constructor() {
	 	 }
	 	 public update(obj: any) {
	 	 	 for(var prop in obj) {
	 	 	 	 this[prop](obj[prop]);
	 	 	 }
	 	 }
	 }
	 export class EntityDtoViewModel<T> extends ViewModel<T> {
	 	 constructor(BaseDTO?: T)
	 	 {
	 	 	  super();
	 	 }
	 	  public toData(): CNDS.Interfaces.IEntityDto {
	 	 	  return {
	 	 	 };
	 	 }
	 }
	 export class EntityDtoWithIDViewModel<T> extends EntityDtoViewModel<T> {
	 	 public ID: KnockoutObservable<any>;
	 	 public Timestamp: KnockoutObservable<any>;
	 	 constructor(BaseDTO?: T)
	 	 {
	 	 	 super(BaseDTO);
	 	 	 if (BaseDTO == null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	 }
	 	 }
	 	  public toData(): CNDS.Interfaces.IEntityDto {
	 	 	  return {
	 	 	 	 ID: this.ID(),
	 	 	 	 Timestamp: this.Timestamp(),
	 	 	 };
	 	 }
	 }
	 export class DataSourceViewModel extends ViewModel<CNDS.Interfaces.IDataSourceDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public Acronym: KnockoutObservable<string>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public AdapterSupportedID: KnockoutObservable<any>;
	 	 public AdapterSupported: KnockoutObservable<string>;
	 	 constructor(DataSourceDTO?: CNDS.Interfaces.IDataSourceDTO)
	 	  {
	 	 	  super();
	 	 	 if (DataSourceDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Acronym = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.AdapterSupportedID = ko.observable<any>();
	 	 	 	 this.AdapterSupported = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DataSourceDTO.ID);
	 	 	 	 this.Name = ko.observable(DataSourceDTO.Name);
	 	 	 	 this.Acronym = ko.observable(DataSourceDTO.Acronym);
	 	 	 	 this.OrganizationID = ko.observable(DataSourceDTO.OrganizationID);
	 	 	 	 this.AdapterSupportedID = ko.observable(DataSourceDTO.AdapterSupportedID);
	 	 	 	 this.AdapterSupported = ko.observable(DataSourceDTO.AdapterSupported);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDataSourceDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	 	Acronym: this.Acronym(),
	 	 	 	OrganizationID: this.OrganizationID(),
	 	 	 	AdapterSupportedID: this.AdapterSupportedID(),
	 	 	 	AdapterSupported: this.AdapterSupported(),
	 	 	  };
	 	  }



	 }
	 export class OrganizationSearchViewModel extends ViewModel<CNDS.Interfaces.IOrganizationSearchDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public Name: KnockoutObservable<string>;
	 	 public ContactInformation: KnockoutObservable<string>;
	 	 constructor(OrganizationSearchDTO?: CNDS.Interfaces.IOrganizationSearchDTO)
	 	  {
	 	 	  super();
	 	 	 if (OrganizationSearchDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.ContactInformation = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(OrganizationSearchDTO.ID);
	 	 	 	 this.NetworkID = ko.observable(OrganizationSearchDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(OrganizationSearchDTO.Network);
	 	 	 	 this.Name = ko.observable(OrganizationSearchDTO.Name);
	 	 	 	 this.ContactInformation = ko.observable(OrganizationSearchDTO.ContactInformation);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IOrganizationSearchDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	Network: this.Network(),
	 	 	 	Name: this.Name(),
	 	 	 	ContactInformation: this.ContactInformation(),
	 	 	  };
	 	  }



	 }
	 export class DataSourceSearchViewModel extends ViewModel<CNDS.Interfaces.IDataSourceSearchDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public Organization: KnockoutObservable<string>;
	 	 public Name: KnockoutObservable<string>;
	 	 public ContactInformation: KnockoutObservable<string>;
	 	 constructor(DataSourceSearchDTO?: CNDS.Interfaces.IDataSourceSearchDTO)
	 	  {
	 	 	  super();
	 	 	 if (DataSourceSearchDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.ContactInformation = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DataSourceSearchDTO.ID);
	 	 	 	 this.NetworkID = ko.observable(DataSourceSearchDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(DataSourceSearchDTO.Network);
	 	 	 	 this.OrganizationID = ko.observable(DataSourceSearchDTO.OrganizationID);
	 	 	 	 this.Organization = ko.observable(DataSourceSearchDTO.Organization);
	 	 	 	 this.Name = ko.observable(DataSourceSearchDTO.Name);
	 	 	 	 this.ContactInformation = ko.observable(DataSourceSearchDTO.ContactInformation);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDataSourceSearchDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	Network: this.Network(),
	 	 	 	OrganizationID: this.OrganizationID(),
	 	 	 	Organization: this.Organization(),
	 	 	 	Name: this.Name(),
	 	 	 	ContactInformation: this.ContactInformation(),
	 	 	  };
	 	  }



	 }
	 export class DataSourceTransferViewModel extends ViewModel<CNDS.Interfaces.IDataSourceTransferDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public Acronym: KnockoutObservable<string>;
	 	 public AdapterSupportedID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public Metadata: KnockoutObservableArray<DomainDataViewModel>;
	 	 constructor(DataSourceTransferDTO?: CNDS.Interfaces.IDataSourceTransferDTO)
	 	  {
	 	 	  super();
	 	 	 if (DataSourceTransferDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Acronym = ko.observable<any>();
	 	 	 	 this.AdapterSupportedID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DataSourceTransferDTO.ID);
	 	 	 	 this.Name = ko.observable(DataSourceTransferDTO.Name);
	 	 	 	 this.Acronym = ko.observable(DataSourceTransferDTO.Acronym);
	 	 	 	 this.AdapterSupportedID = ko.observable(DataSourceTransferDTO.AdapterSupportedID);
	 	 	 	 this.NetworkID = ko.observable(DataSourceTransferDTO.NetworkID);
	 	 	 	 this.OrganizationID = ko.observable(DataSourceTransferDTO.OrganizationID);
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>(DataSourceTransferDTO.Metadata == null ? null : DataSourceTransferDTO.Metadata.map((item) => {return new DomainDataViewModel(item);}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDataSourceTransferDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	 	Acronym: this.Acronym(),
	 	 	 	AdapterSupportedID: this.AdapterSupportedID(),
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	OrganizationID: this.OrganizationID(),
	 	 	 	Metadata: this.Metadata == null ? null : this.Metadata().map((item) => {return item.toData();}),
	 	 	  };
	 	  }



	 }
	 export class AddRemoveDomainUseViewModel extends ViewModel<CNDS.Interfaces.IAddRemoveDomainUseDTO>{
	 	 public AddDomainUse: KnockoutObservableArray<any>;
	 	 public RemoveDomainUse: KnockoutObservableArray<any>;
	 	 constructor(AddRemoveDomainUseDTO?: CNDS.Interfaces.IAddRemoveDomainUseDTO)
	 	  {
	 	 	  super();
	 	 	 if (AddRemoveDomainUseDTO== null) {
	 	 	 	 this.AddDomainUse = ko.observableArray<any>();
	 	 	 	 this.RemoveDomainUse = ko.observableArray<any>();
	 	 	  }else{
	 	 	 	 this.AddDomainUse = ko.observableArray<any>(AddRemoveDomainUseDTO.AddDomainUse == null ? null : AddRemoveDomainUseDTO.AddDomainUse.map((item) => {return item;}));
	 	 	 	 this.RemoveDomainUse = ko.observableArray<any>(AddRemoveDomainUseDTO.RemoveDomainUse == null ? null : AddRemoveDomainUseDTO.RemoveDomainUse.map((item) => {return item;}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IAddRemoveDomainUseDTO{
	 	 	  return {
	 	 	 	AddDomainUse: this.AddDomainUse(),
	 	 	 	RemoveDomainUse: this.RemoveDomainUse(),
	 	 	  };
	 	  }



	 }
	 export class DomainDataViewModel extends ViewModel<CNDS.Interfaces.IDomainDataDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public EntityID: KnockoutObservable<any>;
	 	 public DomainUseID: KnockoutObservable<any>;
	 	 public Value: KnockoutObservable<string>;
	 	 public DomainReferenceID: KnockoutObservable<any>;
	 	 public SequenceNumber: KnockoutObservable<number>;
	 	 public Visibility: KnockoutObservable<CNDS.Enums.AccessType>;
	 	 constructor(DomainDataDTO?: CNDS.Interfaces.IDomainDataDTO)
	 	  {
	 	 	  super();
	 	 	 if (DomainDataDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.EntityID = ko.observable<any>();
	 	 	 	 this.DomainUseID = ko.observable<any>();
	 	 	 	 this.Value = ko.observable<any>();
	 	 	 	 this.DomainReferenceID = ko.observable<any>();
	 	 	 	 this.SequenceNumber = ko.observable<any>();
	 	 	 	 this.Visibility = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DomainDataDTO.ID);
	 	 	 	 this.EntityID = ko.observable(DomainDataDTO.EntityID);
	 	 	 	 this.DomainUseID = ko.observable(DomainDataDTO.DomainUseID);
	 	 	 	 this.Value = ko.observable(DomainDataDTO.Value);
	 	 	 	 this.DomainReferenceID = ko.observable(DomainDataDTO.DomainReferenceID);
	 	 	 	 this.SequenceNumber = ko.observable(DomainDataDTO.SequenceNumber);
	 	 	 	 this.Visibility = ko.observable(DomainDataDTO.Visibility);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDomainDataDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	EntityID: this.EntityID(),
	 	 	 	DomainUseID: this.DomainUseID(),
	 	 	 	Value: this.Value(),
	 	 	 	DomainReferenceID: this.DomainReferenceID(),
	 	 	 	SequenceNumber: this.SequenceNumber(),
	 	 	 	Visibility: this.Visibility(),
	 	 	  };
	 	  }



	 }
	 export class DomainViewModel extends ViewModel<CNDS.Interfaces.IDomainDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public DomainUseID: KnockoutObservable<any>;
	 	 public ParentDomainID: KnockoutObservable<any>;
	 	 public Title: KnockoutObservable<string>;
	 	 public IsMultiValue: KnockoutObservable<boolean>;
	 	 public EnumValue: KnockoutObservable<any>;
	 	 public DataType: KnockoutObservable<string>;
	 	 public EntityType: KnockoutObservable<CNDS.Enums.EntityType>;
	 	 public ChildMetadata: KnockoutObservableArray<DomainViewModel>;
	 	 public References: KnockoutObservableArray<DomainReferenceViewModel>;
	 	 constructor(DomainDTO?: CNDS.Interfaces.IDomainDTO)
	 	  {
	 	 	  super();
	 	 	 if (DomainDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.DomainUseID = ko.observable<any>();
	 	 	 	 this.ParentDomainID = ko.observable<any>();
	 	 	 	 this.Title = ko.observable<any>();
	 	 	 	 this.IsMultiValue = ko.observable<any>();
	 	 	 	 this.EnumValue = ko.observable<any>();
	 	 	 	 this.DataType = ko.observable<any>();
	 	 	 	 this.EntityType = ko.observable<any>();
	 	 	 	 this.ChildMetadata = ko.observableArray<DomainViewModel>();
	 	 	 	 this.References = ko.observableArray<DomainReferenceViewModel>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DomainDTO.ID);
	 	 	 	 this.DomainUseID = ko.observable(DomainDTO.DomainUseID);
	 	 	 	 this.ParentDomainID = ko.observable(DomainDTO.ParentDomainID);
	 	 	 	 this.Title = ko.observable(DomainDTO.Title);
	 	 	 	 this.IsMultiValue = ko.observable(DomainDTO.IsMultiValue);
	 	 	 	 this.EnumValue = ko.observable(DomainDTO.EnumValue);
	 	 	 	 this.DataType = ko.observable(DomainDTO.DataType);
	 	 	 	 this.EntityType = ko.observable(DomainDTO.EntityType);
	 	 	 	 this.ChildMetadata = ko.observableArray<DomainViewModel>(DomainDTO.ChildMetadata == null ? null : DomainDTO.ChildMetadata.map((item) => {return new DomainViewModel(item);}));
	 	 	 	 this.References = ko.observableArray<DomainReferenceViewModel>(DomainDTO.References == null ? null : DomainDTO.References.map((item) => {return new DomainReferenceViewModel(item);}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDomainDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	DomainUseID: this.DomainUseID(),
	 	 	 	ParentDomainID: this.ParentDomainID(),
	 	 	 	Title: this.Title(),
	 	 	 	IsMultiValue: this.IsMultiValue(),
	 	 	 	EnumValue: this.EnumValue(),
	 	 	 	DataType: this.DataType(),
	 	 	 	EntityType: this.EntityType(),
	 	 	 	ChildMetadata: this.ChildMetadata == null ? null : this.ChildMetadata().map((item) => {return item.toData();}),
	 	 	 	References: this.References == null ? null : this.References().map((item) => {return item.toData();}),
	 	 	  };
	 	  }



	 }
	 export class DomainReferenceViewModel extends ViewModel<CNDS.Interfaces.IDomainReferenceDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public DomainID: KnockoutObservable<any>;
	 	 public ParentDomainReferenceID: KnockoutObservable<any>;
	 	 public Title: KnockoutObservable<string>;
	 	 public Description: KnockoutObservable<string>;
	 	 public Value: KnockoutObservable<string>;
	 	 constructor(DomainReferenceDTO?: CNDS.Interfaces.IDomainReferenceDTO)
	 	  {
	 	 	  super();
	 	 	 if (DomainReferenceDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.DomainID = ko.observable<any>();
	 	 	 	 this.ParentDomainReferenceID = ko.observable<any>();
	 	 	 	 this.Title = ko.observable<any>();
	 	 	 	 this.Description = ko.observable<any>();
	 	 	 	 this.Value = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(DomainReferenceDTO.ID);
	 	 	 	 this.DomainID = ko.observable(DomainReferenceDTO.DomainID);
	 	 	 	 this.ParentDomainReferenceID = ko.observable(DomainReferenceDTO.ParentDomainReferenceID);
	 	 	 	 this.Title = ko.observable(DomainReferenceDTO.Title);
	 	 	 	 this.Description = ko.observable(DomainReferenceDTO.Description);
	 	 	 	 this.Value = ko.observable(DomainReferenceDTO.Value);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDomainReferenceDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	DomainID: this.DomainID(),
	 	 	 	ParentDomainReferenceID: this.ParentDomainReferenceID(),
	 	 	 	Title: this.Title(),
	 	 	 	Description: this.Description(),
	 	 	 	Value: this.Value(),
	 	 	  };
	 	  }



	 }
	 export class NetworkTransferViewModel extends ViewModel<CNDS.Interfaces.INetworkTransferDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public Url: KnockoutObservable<string>;
	 	 public ServiceUrl: KnockoutObservable<string>;
	 	 public ServiceUserName: KnockoutObservable<string>;
	 	 public ServicePassword: KnockoutObservable<string>;
	 	 constructor(NetworkTransferDTO?: CNDS.Interfaces.INetworkTransferDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkTransferDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Url = ko.observable<any>();
	 	 	 	 this.ServiceUrl = ko.observable<any>();
	 	 	 	 this.ServiceUserName = ko.observable<any>();
	 	 	 	 this.ServicePassword = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(NetworkTransferDTO.ID);
	 	 	 	 this.Name = ko.observable(NetworkTransferDTO.Name);
	 	 	 	 this.Url = ko.observable(NetworkTransferDTO.Url);
	 	 	 	 this.ServiceUrl = ko.observable(NetworkTransferDTO.ServiceUrl);
	 	 	 	 this.ServiceUserName = ko.observable(NetworkTransferDTO.ServiceUserName);
	 	 	 	 this.ServicePassword = ko.observable(NetworkTransferDTO.ServicePassword);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkTransferDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	 	Url: this.Url(),
	 	 	 	ServiceUrl: this.ServiceUrl(),
	 	 	 	ServiceUserName: this.ServiceUserName(),
	 	 	 	ServicePassword: this.ServicePassword(),
	 	 	  };
	 	  }



	 }
	 export class OrganizationTransferViewModel extends ViewModel<CNDS.Interfaces.IOrganizationTransferDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public Acronym: KnockoutObservable<string>;
	 	 public ParentOrganizationID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Metadata: KnockoutObservableArray<DomainDataViewModel>;
	 	 public ContactEmail: KnockoutObservable<string>;
	 	 public ContactFirstName: KnockoutObservable<string>;
	 	 public ContactLastName: KnockoutObservable<string>;
	 	 public ContactPhone: KnockoutObservable<string>;
	 	 constructor(OrganizationTransferDTO?: CNDS.Interfaces.IOrganizationTransferDTO)
	 	  {
	 	 	  super();
	 	 	 if (OrganizationTransferDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Acronym = ko.observable<any>();
	 	 	 	 this.ParentOrganizationID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>();
	 	 	 	 this.ContactEmail = ko.observable<any>();
	 	 	 	 this.ContactFirstName = ko.observable<any>();
	 	 	 	 this.ContactLastName = ko.observable<any>();
	 	 	 	 this.ContactPhone = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(OrganizationTransferDTO.ID);
	 	 	 	 this.Name = ko.observable(OrganizationTransferDTO.Name);
	 	 	 	 this.Acronym = ko.observable(OrganizationTransferDTO.Acronym);
	 	 	 	 this.ParentOrganizationID = ko.observable(OrganizationTransferDTO.ParentOrganizationID);
	 	 	 	 this.NetworkID = ko.observable(OrganizationTransferDTO.NetworkID);
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>(OrganizationTransferDTO.Metadata == null ? null : OrganizationTransferDTO.Metadata.map((item) => {return new DomainDataViewModel(item);}));
	 	 	 	 this.ContactEmail = ko.observable(OrganizationTransferDTO.ContactEmail);
	 	 	 	 this.ContactFirstName = ko.observable(OrganizationTransferDTO.ContactFirstName);
	 	 	 	 this.ContactLastName = ko.observable(OrganizationTransferDTO.ContactLastName);
	 	 	 	 this.ContactPhone = ko.observable(OrganizationTransferDTO.ContactPhone);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IOrganizationTransferDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	 	Acronym: this.Acronym(),
	 	 	 	ParentOrganizationID: this.ParentOrganizationID(),
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	Metadata: this.Metadata == null ? null : this.Metadata().map((item) => {return item.toData();}),
	 	 	 	ContactEmail: this.ContactEmail(),
	 	 	 	ContactFirstName: this.ContactFirstName(),
	 	 	 	ContactLastName: this.ContactLastName(),
	 	 	 	ContactPhone: this.ContactPhone(),
	 	 	  };
	 	  }



	 }
	 export class NetworkProjectRequestTypeDataMartViewModel extends ViewModel<CNDS.Interfaces.INetworkProjectRequestTypeDataMartDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public ProjectID: KnockoutObservable<any>;
	 	 public Project: KnockoutObservable<string>;
	 	 public DataMartID: KnockoutObservable<any>;
	 	 public DataMart: KnockoutObservable<string>;
	 	 public RequestTypeID: KnockoutObservable<any>;
	 	 public RequestType: KnockoutObservable<string>;
	 	 constructor(NetworkProjectRequestTypeDataMartDTO?: CNDS.Interfaces.INetworkProjectRequestTypeDataMartDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkProjectRequestTypeDataMartDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.ProjectID = ko.observable<any>();
	 	 	 	 this.Project = ko.observable<any>();
	 	 	 	 this.DataMartID = ko.observable<any>();
	 	 	 	 this.DataMart = ko.observable<any>();
	 	 	 	 this.RequestTypeID = ko.observable<any>();
	 	 	 	 this.RequestType = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(NetworkProjectRequestTypeDataMartDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(NetworkProjectRequestTypeDataMartDTO.Network);
	 	 	 	 this.ProjectID = ko.observable(NetworkProjectRequestTypeDataMartDTO.ProjectID);
	 	 	 	 this.Project = ko.observable(NetworkProjectRequestTypeDataMartDTO.Project);
	 	 	 	 this.DataMartID = ko.observable(NetworkProjectRequestTypeDataMartDTO.DataMartID);
	 	 	 	 this.DataMart = ko.observable(NetworkProjectRequestTypeDataMartDTO.DataMart);
	 	 	 	 this.RequestTypeID = ko.observable(NetworkProjectRequestTypeDataMartDTO.RequestTypeID);
	 	 	 	 this.RequestType = ko.observable(NetworkProjectRequestTypeDataMartDTO.RequestType);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkProjectRequestTypeDataMartDTO{
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
	 	  }



	 }
	 export class SearchViewModel extends ViewModel<CNDS.Interfaces.ISearchDTO>{
	 	 public DomainIDs: KnockoutObservableArray<any>;
	 	 public DomainReferencesIDs: KnockoutObservableArray<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 constructor(SearchDTO?: CNDS.Interfaces.ISearchDTO)
	 	  {
	 	 	  super();
	 	 	 if (SearchDTO== null) {
	 	 	 	 this.DomainIDs = ko.observableArray<any>();
	 	 	 	 this.DomainReferencesIDs = ko.observableArray<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.DomainIDs = ko.observableArray<any>(SearchDTO.DomainIDs == null ? null : SearchDTO.DomainIDs.map((item) => {return item;}));
	 	 	 	 this.DomainReferencesIDs = ko.observableArray<any>(SearchDTO.DomainReferencesIDs == null ? null : SearchDTO.DomainReferencesIDs.map((item) => {return item;}));
	 	 	 	 this.NetworkID = ko.observable(SearchDTO.NetworkID);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISearchDTO{
	 	 	  return {
	 	 	 	DomainIDs: this.DomainIDs(),
	 	 	 	DomainReferencesIDs: this.DomainReferencesIDs(),
	 	 	 	NetworkID: this.NetworkID(),
	 	 	  };
	 	  }



	 }
	 export class AssignedPermissionViewModel extends ViewModel<CNDS.Interfaces.IAssignedPermissionDTO>{
	 	 public SecurityGroupID: KnockoutObservable<any>;
	 	 public PermissionID: KnockoutObservable<any>;
	 	 public Allowed: KnockoutObservable<boolean>;
	 	 constructor(AssignedPermissionDTO?: CNDS.Interfaces.IAssignedPermissionDTO)
	 	  {
	 	 	  super();
	 	 	 if (AssignedPermissionDTO== null) {
	 	 	 	 this.SecurityGroupID = ko.observable<any>();
	 	 	 	 this.PermissionID = ko.observable<any>();
	 	 	 	 this.Allowed = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.SecurityGroupID = ko.observable(AssignedPermissionDTO.SecurityGroupID);
	 	 	 	 this.PermissionID = ko.observable(AssignedPermissionDTO.PermissionID);
	 	 	 	 this.Allowed = ko.observable(AssignedPermissionDTO.Allowed);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IAssignedPermissionDTO{
	 	 	  return {
	 	 	 	SecurityGroupID: this.SecurityGroupID(),
	 	 	 	PermissionID: this.PermissionID(),
	 	 	 	Allowed: this.Allowed(),
	 	 	  };
	 	  }



	 }
	 export class PermissionViewModel extends ViewModel<CNDS.Interfaces.IPermissionDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public Description: KnockoutObservable<string>;
	 	 constructor(PermissionDTO?: CNDS.Interfaces.IPermissionDTO)
	 	  {
	 	 	  super();
	 	 	 if (PermissionDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Description = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(PermissionDTO.ID);
	 	 	 	 this.Name = ko.observable(PermissionDTO.Name);
	 	 	 	 this.Description = ko.observable(PermissionDTO.Description);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IPermissionDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	 	Description: this.Description(),
	 	 	  };
	 	  }



	 }
	 export class SecurityGroupViewModel extends ViewModel<CNDS.Interfaces.ISecurityGroupDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 constructor(SecurityGroupDTO?: CNDS.Interfaces.ISecurityGroupDTO)
	 	  {
	 	 	  super();
	 	 	 if (SecurityGroupDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(SecurityGroupDTO.ID);
	 	 	 	 this.Name = ko.observable(SecurityGroupDTO.Name);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISecurityGroupDTO{
	 	 	  return {
	 	 	 	ID: this.ID(),
	 	 	 	Name: this.Name(),
	 	 	  };
	 	  }



	 }
	 export class SecurityGroupUserViewModel extends ViewModel<CNDS.Interfaces.ISecurityGroupUserDTO>{
	 	 public UserID: KnockoutObservable<any>;
	 	 public SecurityGroups: KnockoutObservableArray<SecurityGroupViewModel>;
	 	 constructor(SecurityGroupUserDTO?: CNDS.Interfaces.ISecurityGroupUserDTO)
	 	  {
	 	 	  super();
	 	 	 if (SecurityGroupUserDTO== null) {
	 	 	 	 this.UserID = ko.observable<any>();
	 	 	 	 this.SecurityGroups = ko.observableArray<SecurityGroupViewModel>();
	 	 	  }else{
	 	 	 	 this.UserID = ko.observable(SecurityGroupUserDTO.UserID);
	 	 	 	 this.SecurityGroups = ko.observableArray<SecurityGroupViewModel>(SecurityGroupUserDTO.SecurityGroups == null ? null : SecurityGroupUserDTO.SecurityGroups.map((item) => {return new SecurityGroupViewModel(item);}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISecurityGroupUserDTO{
	 	 	  return {
	 	 	 	UserID: this.UserID(),
	 	 	 	SecurityGroups: this.SecurityGroups == null ? null : this.SecurityGroups().map((item) => {return item.toData();}),
	 	 	  };
	 	  }



	 }
	 export class UserTransferViewModel extends ViewModel<CNDS.Interfaces.IUserTransferDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public UserName: KnockoutObservable<string>;
	 	 public Salutation: KnockoutObservable<string>;
	 	 public FirstName: KnockoutObservable<string>;
	 	 public MiddleName: KnockoutObservable<string>;
	 	 public LastName: KnockoutObservable<string>;
	 	 public EmailAddress: KnockoutObservable<string>;
	 	 public PhoneNumber: KnockoutObservable<string>;
	 	 public FaxNumber: KnockoutObservable<string>;
	 	 public Active: KnockoutObservable<boolean>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public Metadata: KnockoutObservableArray<DomainDataViewModel>;
	 	 constructor(UserTransferDTO?: CNDS.Interfaces.IUserTransferDTO)
	 	  {
	 	 	  super();
	 	 	 if (UserTransferDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.UserName = ko.observable<any>();
	 	 	 	 this.Salutation = ko.observable<any>();
	 	 	 	 this.FirstName = ko.observable<any>();
	 	 	 	 this.MiddleName = ko.observable<any>();
	 	 	 	 this.LastName = ko.observable<any>();
	 	 	 	 this.EmailAddress = ko.observable<any>();
	 	 	 	 this.PhoneNumber = ko.observable<any>();
	 	 	 	 this.FaxNumber = ko.observable<any>();
	 	 	 	 this.Active = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(UserTransferDTO.ID);
	 	 	 	 this.NetworkID = ko.observable(UserTransferDTO.NetworkID);
	 	 	 	 this.UserName = ko.observable(UserTransferDTO.UserName);
	 	 	 	 this.Salutation = ko.observable(UserTransferDTO.Salutation);
	 	 	 	 this.FirstName = ko.observable(UserTransferDTO.FirstName);
	 	 	 	 this.MiddleName = ko.observable(UserTransferDTO.MiddleName);
	 	 	 	 this.LastName = ko.observable(UserTransferDTO.LastName);
	 	 	 	 this.EmailAddress = ko.observable(UserTransferDTO.EmailAddress);
	 	 	 	 this.PhoneNumber = ko.observable(UserTransferDTO.PhoneNumber);
	 	 	 	 this.FaxNumber = ko.observable(UserTransferDTO.FaxNumber);
	 	 	 	 this.Active = ko.observable(UserTransferDTO.Active);
	 	 	 	 this.OrganizationID = ko.observable(UserTransferDTO.OrganizationID);
	 	 	 	 this.Metadata = ko.observableArray<DomainDataViewModel>(UserTransferDTO.Metadata == null ? null : UserTransferDTO.Metadata.map((item) => {return new DomainDataViewModel(item);}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IUserTransferDTO{
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
	 	 	 	Metadata: this.Metadata == null ? null : this.Metadata().map((item) => {return item.toData();}),
	 	 	  };
	 	  }



	 }
	 export class ResubmitRouteViewModel extends ViewModel<CNDS.Interfaces.IResubmitRouteDTO>{
	 	 public ResponseID: KnockoutObservable<any>;
	 	 public RequestDatamartID: KnockoutObservable<any>;
	 	 public Message: KnockoutObservable<string>;
	 	 constructor(ResubmitRouteDTO?: CNDS.Interfaces.IResubmitRouteDTO)
	 	  {
	 	 	  super();
	 	 	 if (ResubmitRouteDTO== null) {
	 	 	 	 this.ResponseID = ko.observable<any>();
	 	 	 	 this.RequestDatamartID = ko.observable<any>();
	 	 	 	 this.Message = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ResponseID = ko.observable(ResubmitRouteDTO.ResponseID);
	 	 	 	 this.RequestDatamartID = ko.observable(ResubmitRouteDTO.RequestDatamartID);
	 	 	 	 this.Message = ko.observable(ResubmitRouteDTO.Message);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IResubmitRouteDTO{
	 	 	  return {
	 	 	 	ResponseID: this.ResponseID(),
	 	 	 	RequestDatamartID: this.RequestDatamartID(),
	 	 	 	Message: this.Message(),
	 	 	  };
	 	  }



	 }
	 export class UpdateDataMartPriorityAndDueDateViewModel extends ViewModel<CNDS.Interfaces.IUpdateDataMartPriorityAndDueDateDTO>{
	 	 public RequestDataMartID: KnockoutObservable<any>;
	 	 public Priority: KnockoutObservable<number>;
	 	 public DueDate: KnockoutObservable<Date>;
	 	 constructor(UpdateDataMartPriorityAndDueDateDTO?: CNDS.Interfaces.IUpdateDataMartPriorityAndDueDateDTO)
	 	  {
	 	 	  super();
	 	 	 if (UpdateDataMartPriorityAndDueDateDTO== null) {
	 	 	 	 this.RequestDataMartID = ko.observable<any>();
	 	 	 	 this.Priority = ko.observable<any>();
	 	 	 	 this.DueDate = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.RequestDataMartID = ko.observable(UpdateDataMartPriorityAndDueDateDTO.RequestDataMartID);
	 	 	 	 this.Priority = ko.observable(UpdateDataMartPriorityAndDueDateDTO.Priority);
	 	 	 	 this.DueDate = ko.observable(UpdateDataMartPriorityAndDueDateDTO.DueDate);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IUpdateDataMartPriorityAndDueDateDTO{
	 	 	  return {
	 	 	 	RequestDataMartID: this.RequestDataMartID(),
	 	 	 	Priority: this.Priority(),
	 	 	 	DueDate: this.DueDate(),
	 	 	  };
	 	  }



	 }
	 export class SubmitRequestViewModel extends ViewModel<CNDS.Interfaces.ISubmitRequestDTO>{
	 	 public SourceNetworkID: KnockoutObservable<any>;
	 	 public SourceRequestID: KnockoutObservable<any>;
	 	 public SerializedSourceRequest: KnockoutObservable<string>;
	 	 public Routes: KnockoutObservableArray<SubmitRouteViewModel>;
	 	 public Documents: KnockoutObservableArray<SubmitRequestDocumentDetailsViewModel>;
	 	 constructor(SubmitRequestDTO?: CNDS.Interfaces.ISubmitRequestDTO)
	 	  {
	 	 	  super();
	 	 	 if (SubmitRequestDTO== null) {
	 	 	 	 this.SourceNetworkID = ko.observable<any>();
	 	 	 	 this.SourceRequestID = ko.observable<any>();
	 	 	 	 this.SerializedSourceRequest = ko.observable<any>();
	 	 	 	 this.Routes = ko.observableArray<SubmitRouteViewModel>();
	 	 	 	 this.Documents = ko.observableArray<SubmitRequestDocumentDetailsViewModel>();
	 	 	  }else{
	 	 	 	 this.SourceNetworkID = ko.observable(SubmitRequestDTO.SourceNetworkID);
	 	 	 	 this.SourceRequestID = ko.observable(SubmitRequestDTO.SourceRequestID);
	 	 	 	 this.SerializedSourceRequest = ko.observable(SubmitRequestDTO.SerializedSourceRequest);
	 	 	 	 this.Routes = ko.observableArray<SubmitRouteViewModel>(SubmitRequestDTO.Routes == null ? null : SubmitRequestDTO.Routes.map((item) => {return new SubmitRouteViewModel(item);}));
	 	 	 	 this.Documents = ko.observableArray<SubmitRequestDocumentDetailsViewModel>(SubmitRequestDTO.Documents == null ? null : SubmitRequestDTO.Documents.map((item) => {return new SubmitRequestDocumentDetailsViewModel(item);}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISubmitRequestDTO{
	 	 	  return {
	 	 	 	SourceNetworkID: this.SourceNetworkID(),
	 	 	 	SourceRequestID: this.SourceRequestID(),
	 	 	 	SerializedSourceRequest: this.SerializedSourceRequest(),
	 	 	 	Routes: this.Routes == null ? null : this.Routes().map((item) => {return item.toData();}),
	 	 	 	Documents: this.Documents == null ? null : this.Documents().map((item) => {return item.toData();}),
	 	 	  };
	 	  }



	 }
	 export class SubmitRouteViewModel extends ViewModel<CNDS.Interfaces.ISubmitRouteDTO>{
	 	 public NetworkRouteDefinitionID: KnockoutObservable<any>;
	 	 public DueDate: KnockoutObservable<Date>;
	 	 public Priority: KnockoutObservable<number>;
	 	 public SourceRequestDataMartID: KnockoutObservable<any>;
	 	 public SourceResponseID: KnockoutObservable<any>;
	 	 public RequestDocumentIDs: KnockoutObservableArray<any>;
	 	 constructor(SubmitRouteDTO?: CNDS.Interfaces.ISubmitRouteDTO)
	 	  {
	 	 	  super();
	 	 	 if (SubmitRouteDTO== null) {
	 	 	 	 this.NetworkRouteDefinitionID = ko.observable<any>();
	 	 	 	 this.DueDate = ko.observable<any>();
	 	 	 	 this.Priority = ko.observable<any>();
	 	 	 	 this.SourceRequestDataMartID = ko.observable<any>();
	 	 	 	 this.SourceResponseID = ko.observable<any>();
	 	 	 	 this.RequestDocumentIDs = ko.observableArray<any>();
	 	 	  }else{
	 	 	 	 this.NetworkRouteDefinitionID = ko.observable(SubmitRouteDTO.NetworkRouteDefinitionID);
	 	 	 	 this.DueDate = ko.observable(SubmitRouteDTO.DueDate);
	 	 	 	 this.Priority = ko.observable(SubmitRouteDTO.Priority);
	 	 	 	 this.SourceRequestDataMartID = ko.observable(SubmitRouteDTO.SourceRequestDataMartID);
	 	 	 	 this.SourceResponseID = ko.observable(SubmitRouteDTO.SourceResponseID);
	 	 	 	 this.RequestDocumentIDs = ko.observableArray<any>(SubmitRouteDTO.RequestDocumentIDs == null ? null : SubmitRouteDTO.RequestDocumentIDs.map((item) => {return item;}));
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISubmitRouteDTO{
	 	 	  return {
	 	 	 	NetworkRouteDefinitionID: this.NetworkRouteDefinitionID(),
	 	 	 	DueDate: this.DueDate(),
	 	 	 	Priority: this.Priority(),
	 	 	 	SourceRequestDataMartID: this.SourceRequestDataMartID(),
	 	 	 	SourceResponseID: this.SourceResponseID(),
	 	 	 	RequestDocumentIDs: this.RequestDocumentIDs(),
	 	 	  };
	 	  }



	 }
	 export class SubmitRequestDocumentDetailsViewModel extends ViewModel<CNDS.Interfaces.ISubmitRequestDocumentDetailsDTO>{
	 	 public SourceRequestDataSourceID: KnockoutObservable<any>;
	 	 public RevisionSetID: KnockoutObservable<any>;
	 	 public DocumentID: KnockoutObservable<any>;
	 	 public Name: KnockoutObservable<string>;
	 	 public IsViewable: KnockoutObservable<boolean>;
	 	 public Kind: KnockoutObservable<string>;
	 	 public MimeType: KnockoutObservable<string>;
	 	 public FileName: KnockoutObservable<string>;
	 	 public Length: KnockoutObservable<number>;
	 	 public Description: KnockoutObservable<string>;
	 	 constructor(SubmitRequestDocumentDetailsDTO?: CNDS.Interfaces.ISubmitRequestDocumentDetailsDTO)
	 	  {
	 	 	  super();
	 	 	 if (SubmitRequestDocumentDetailsDTO== null) {
	 	 	 	 this.SourceRequestDataSourceID = ko.observable<any>();
	 	 	 	 this.RevisionSetID = ko.observable<any>();
	 	 	 	 this.DocumentID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.IsViewable = ko.observable<any>();
	 	 	 	 this.Kind = ko.observable<any>();
	 	 	 	 this.MimeType = ko.observable<any>();
	 	 	 	 this.FileName = ko.observable<any>();
	 	 	 	 this.Length = ko.observable<any>();
	 	 	 	 this.Description = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.SourceRequestDataSourceID = ko.observable(SubmitRequestDocumentDetailsDTO.SourceRequestDataSourceID);
	 	 	 	 this.RevisionSetID = ko.observable(SubmitRequestDocumentDetailsDTO.RevisionSetID);
	 	 	 	 this.DocumentID = ko.observable(SubmitRequestDocumentDetailsDTO.DocumentID);
	 	 	 	 this.Name = ko.observable(SubmitRequestDocumentDetailsDTO.Name);
	 	 	 	 this.IsViewable = ko.observable(SubmitRequestDocumentDetailsDTO.IsViewable);
	 	 	 	 this.Kind = ko.observable(SubmitRequestDocumentDetailsDTO.Kind);
	 	 	 	 this.MimeType = ko.observable(SubmitRequestDocumentDetailsDTO.MimeType);
	 	 	 	 this.FileName = ko.observable(SubmitRequestDocumentDetailsDTO.FileName);
	 	 	 	 this.Length = ko.observable(SubmitRequestDocumentDetailsDTO.Length);
	 	 	 	 this.Description = ko.observable(SubmitRequestDocumentDetailsDTO.Description);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISubmitRequestDocumentDetailsDTO{
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
	 	  }



	 }
	 export class SetRoutingStatusViewModel extends ViewModel<CNDS.Interfaces.ISetRoutingStatusDTO>{
	 	 public ResponseID: KnockoutObservable<any>;
	 	 public RoutingStatus: KnockoutObservable<number>;
	 	 public Message: KnockoutObservable<string>;
	 	 constructor(SetRoutingStatusDTO?: CNDS.Interfaces.ISetRoutingStatusDTO)
	 	  {
	 	 	  super();
	 	 	 if (SetRoutingStatusDTO== null) {
	 	 	 	 this.ResponseID = ko.observable<any>();
	 	 	 	 this.RoutingStatus = ko.observable<any>();
	 	 	 	 this.Message = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ResponseID = ko.observable(SetRoutingStatusDTO.ResponseID);
	 	 	 	 this.RoutingStatus = ko.observable(SetRoutingStatusDTO.RoutingStatus);
	 	 	 	 this.Message = ko.observable(SetRoutingStatusDTO.Message);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.ISetRoutingStatusDTO{
	 	 	  return {
	 	 	 	ResponseID: this.ResponseID(),
	 	 	 	RoutingStatus: this.RoutingStatus(),
	 	 	 	Message: this.Message(),
	 	 	  };
	 	  }



	 }
	 export class ActiveUserViewModel extends ViewModel<CNDS.Interfaces.IActiveUserDTO>{
	 	 public ID: KnockoutObservable<any>;
	 	 public PmnID: KnockoutObservable<any>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public PmnOrganizationID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public UserName: KnockoutObservable<string>;
	 	 public Network: KnockoutObservable<string>;
	 	 public Organization: KnockoutObservable<string>;
	 	 constructor(ActiveUserDTO?: CNDS.Interfaces.IActiveUserDTO)
	 	  {
	 	 	  super();
	 	 	 if (ActiveUserDTO== null) {
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.PmnID = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.PmnOrganizationID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.UserName = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.ID = ko.observable(ActiveUserDTO.ID);
	 	 	 	 this.PmnID = ko.observable(ActiveUserDTO.PmnID);
	 	 	 	 this.OrganizationID = ko.observable(ActiveUserDTO.OrganizationID);
	 	 	 	 this.PmnOrganizationID = ko.observable(ActiveUserDTO.PmnOrganizationID);
	 	 	 	 this.NetworkID = ko.observable(ActiveUserDTO.NetworkID);
	 	 	 	 this.UserName = ko.observable(ActiveUserDTO.UserName);
	 	 	 	 this.Network = ko.observable(ActiveUserDTO.Network);
	 	 	 	 this.Organization = ko.observable(ActiveUserDTO.Organization);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IActiveUserDTO{
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
	 	  }



	 }
	 export class EntityWithDomainDataItemViewModel extends ViewModel<CNDS.Interfaces.IEntityWithDomainDataItemDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public NetworkUrl: KnockoutObservable<string>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public Organization: KnockoutObservable<string>;
	 	 public OrganizationAcronym: KnockoutObservable<string>;
	 	 public ParentOrganizationID: KnockoutObservable<any>;
	 	 public DomainUseID: KnockoutObservable<any>;
	 	 public DomainID: KnockoutObservable<any>;
	 	 public ParentDomainID: KnockoutObservable<any>;
	 	 public DomainTitle: KnockoutObservable<string>;
	 	 public DomainIsMultiValueSelect: KnockoutObservable<boolean>;
	 	 public DomainDataType: KnockoutObservable<string>;
	 	 public DomainReferenceID: KnockoutObservable<any>;
	 	 public DomainReferenceTitle: KnockoutObservable<string>;
	 	 public DomainReferenceDescription: KnockoutObservable<string>;
	 	 public DomainReferenceValue: KnockoutObservable<string>;
	 	 public DomainDataValue: KnockoutObservable<string>;
	 	 public DomainDataDomainReferenceID: KnockoutObservable<any>;
	 	 public DomainAccessValue: KnockoutObservable<number>;
	 	 constructor(EntityWithDomainDataItemDTO?: CNDS.Interfaces.IEntityWithDomainDataItemDTO)
	 	  {
	 	 	  super();
	 	 	 if (EntityWithDomainDataItemDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.NetworkUrl = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	 	 this.OrganizationAcronym = ko.observable<any>();
	 	 	 	 this.ParentOrganizationID = ko.observable<any>();
	 	 	 	 this.DomainUseID = ko.observable<any>();
	 	 	 	 this.DomainID = ko.observable<any>();
	 	 	 	 this.ParentDomainID = ko.observable<any>();
	 	 	 	 this.DomainTitle = ko.observable<any>();
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable<any>();
	 	 	 	 this.DomainDataType = ko.observable<any>();
	 	 	 	 this.DomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainReferenceTitle = ko.observable<any>();
	 	 	 	 this.DomainReferenceDescription = ko.observable<any>();
	 	 	 	 this.DomainReferenceValue = ko.observable<any>();
	 	 	 	 this.DomainDataValue = ko.observable<any>();
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainAccessValue = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(EntityWithDomainDataItemDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(EntityWithDomainDataItemDTO.Network);
	 	 	 	 this.NetworkUrl = ko.observable(EntityWithDomainDataItemDTO.NetworkUrl);
	 	 	 	 this.OrganizationID = ko.observable(EntityWithDomainDataItemDTO.OrganizationID);
	 	 	 	 this.Organization = ko.observable(EntityWithDomainDataItemDTO.Organization);
	 	 	 	 this.OrganizationAcronym = ko.observable(EntityWithDomainDataItemDTO.OrganizationAcronym);
	 	 	 	 this.ParentOrganizationID = ko.observable(EntityWithDomainDataItemDTO.ParentOrganizationID);
	 	 	 	 this.DomainUseID = ko.observable(EntityWithDomainDataItemDTO.DomainUseID);
	 	 	 	 this.DomainID = ko.observable(EntityWithDomainDataItemDTO.DomainID);
	 	 	 	 this.ParentDomainID = ko.observable(EntityWithDomainDataItemDTO.ParentDomainID);
	 	 	 	 this.DomainTitle = ko.observable(EntityWithDomainDataItemDTO.DomainTitle);
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable(EntityWithDomainDataItemDTO.DomainIsMultiValueSelect);
	 	 	 	 this.DomainDataType = ko.observable(EntityWithDomainDataItemDTO.DomainDataType);
	 	 	 	 this.DomainReferenceID = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceID);
	 	 	 	 this.DomainReferenceTitle = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceTitle);
	 	 	 	 this.DomainReferenceDescription = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceDescription);
	 	 	 	 this.DomainReferenceValue = ko.observable(EntityWithDomainDataItemDTO.DomainReferenceValue);
	 	 	 	 this.DomainDataValue = ko.observable(EntityWithDomainDataItemDTO.DomainDataValue);
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable(EntityWithDomainDataItemDTO.DomainDataDomainReferenceID);
	 	 	 	 this.DomainAccessValue = ko.observable(EntityWithDomainDataItemDTO.DomainAccessValue);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IEntityWithDomainDataItemDTO{
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
	 	  }



	 }
	 export class DataSourceExtendedViewModel extends DataSourceViewModel{
	 	 public Organization: KnockoutObservable<string>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 constructor(DataSourceExtendedDTO?: CNDS.Interfaces.IDataSourceExtendedDTO)
	 	  {
	 	 	  super();
	 	 	 if (DataSourceExtendedDTO== null) {
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Acronym = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.AdapterSupportedID = ko.observable<any>();
	 	 	 	 this.AdapterSupported = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.Organization = ko.observable(DataSourceExtendedDTO.Organization);
	 	 	 	 this.NetworkID = ko.observable(DataSourceExtendedDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(DataSourceExtendedDTO.Network);
	 	 	 	 this.ID = ko.observable(DataSourceExtendedDTO.ID);
	 	 	 	 this.Name = ko.observable(DataSourceExtendedDTO.Name);
	 	 	 	 this.Acronym = ko.observable(DataSourceExtendedDTO.Acronym);
	 	 	 	 this.OrganizationID = ko.observable(DataSourceExtendedDTO.OrganizationID);
	 	 	 	 this.AdapterSupportedID = ko.observable(DataSourceExtendedDTO.AdapterSupportedID);
	 	 	 	 this.AdapterSupported = ko.observable(DataSourceExtendedDTO.AdapterSupported);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDataSourceExtendedDTO{
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
	 	  }



	 }
	 export class UpdateAssignedPermissionViewModel extends AssignedPermissionViewModel{
	 	 public Delete: KnockoutObservable<boolean>;
	 	 constructor(UpdateAssignedPermissionDTO?: CNDS.Interfaces.IUpdateAssignedPermissionDTO)
	 	  {
	 	 	  super();
	 	 	 if (UpdateAssignedPermissionDTO== null) {
	 	 	 	 this.Delete = ko.observable<any>();
	 	 	 	 this.SecurityGroupID = ko.observable<any>();
	 	 	 	 this.PermissionID = ko.observable<any>();
	 	 	 	 this.Allowed = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.Delete = ko.observable(UpdateAssignedPermissionDTO.Delete);
	 	 	 	 this.SecurityGroupID = ko.observable(UpdateAssignedPermissionDTO.SecurityGroupID);
	 	 	 	 this.PermissionID = ko.observable(UpdateAssignedPermissionDTO.PermissionID);
	 	 	 	 this.Allowed = ko.observable(UpdateAssignedPermissionDTO.Allowed);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IUpdateAssignedPermissionDTO{
	 	 	  return {
	 	 	 	Delete: this.Delete(),
	 	 	 	SecurityGroupID: this.SecurityGroupID(),
	 	 	 	PermissionID: this.PermissionID(),
	 	 	 	Allowed: this.Allowed(),
	 	 	  };
	 	  }



	 }
	 export class DataSourceWithDomainDataItemViewModel extends EntityWithDomainDataItemViewModel{
	 	 public DataSourceID: KnockoutObservable<any>;
	 	 public DataSource: KnockoutObservable<string>;
	 	 public DataSourceAcronym: KnockoutObservable<string>;
	 	 public DataSourceAdapterSupportedID: KnockoutObservable<any>;
	 	 public DataSourceAdapterSupported: KnockoutObservable<string>;
	 	 public SupportsCrossNetworkRequests: KnockoutObservable<boolean>;
	 	 constructor(DataSourceWithDomainDataItemDTO?: CNDS.Interfaces.IDataSourceWithDomainDataItemDTO)
	 	  {
	 	 	  super();
	 	 	 if (DataSourceWithDomainDataItemDTO== null) {
	 	 	 	 this.DataSourceID = ko.observable<any>();
	 	 	 	 this.DataSource = ko.observable<any>();
	 	 	 	 this.DataSourceAcronym = ko.observable<any>();
	 	 	 	 this.DataSourceAdapterSupportedID = ko.observable<any>();
	 	 	 	 this.DataSourceAdapterSupported = ko.observable<any>();
	 	 	 	 this.SupportsCrossNetworkRequests = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.NetworkUrl = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	 	 this.OrganizationAcronym = ko.observable<any>();
	 	 	 	 this.ParentOrganizationID = ko.observable<any>();
	 	 	 	 this.DomainUseID = ko.observable<any>();
	 	 	 	 this.DomainID = ko.observable<any>();
	 	 	 	 this.ParentDomainID = ko.observable<any>();
	 	 	 	 this.DomainTitle = ko.observable<any>();
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable<any>();
	 	 	 	 this.DomainDataType = ko.observable<any>();
	 	 	 	 this.DomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainReferenceTitle = ko.observable<any>();
	 	 	 	 this.DomainReferenceDescription = ko.observable<any>();
	 	 	 	 this.DomainReferenceValue = ko.observable<any>();
	 	 	 	 this.DomainDataValue = ko.observable<any>();
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainAccessValue = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.DataSourceID = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceID);
	 	 	 	 this.DataSource = ko.observable(DataSourceWithDomainDataItemDTO.DataSource);
	 	 	 	 this.DataSourceAcronym = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAcronym);
	 	 	 	 this.DataSourceAdapterSupportedID = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAdapterSupportedID);
	 	 	 	 this.DataSourceAdapterSupported = ko.observable(DataSourceWithDomainDataItemDTO.DataSourceAdapterSupported);
	 	 	 	 this.SupportsCrossNetworkRequests = ko.observable(DataSourceWithDomainDataItemDTO.SupportsCrossNetworkRequests);
	 	 	 	 this.NetworkID = ko.observable(DataSourceWithDomainDataItemDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(DataSourceWithDomainDataItemDTO.Network);
	 	 	 	 this.NetworkUrl = ko.observable(DataSourceWithDomainDataItemDTO.NetworkUrl);
	 	 	 	 this.OrganizationID = ko.observable(DataSourceWithDomainDataItemDTO.OrganizationID);
	 	 	 	 this.Organization = ko.observable(DataSourceWithDomainDataItemDTO.Organization);
	 	 	 	 this.OrganizationAcronym = ko.observable(DataSourceWithDomainDataItemDTO.OrganizationAcronym);
	 	 	 	 this.ParentOrganizationID = ko.observable(DataSourceWithDomainDataItemDTO.ParentOrganizationID);
	 	 	 	 this.DomainUseID = ko.observable(DataSourceWithDomainDataItemDTO.DomainUseID);
	 	 	 	 this.DomainID = ko.observable(DataSourceWithDomainDataItemDTO.DomainID);
	 	 	 	 this.ParentDomainID = ko.observable(DataSourceWithDomainDataItemDTO.ParentDomainID);
	 	 	 	 this.DomainTitle = ko.observable(DataSourceWithDomainDataItemDTO.DomainTitle);
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable(DataSourceWithDomainDataItemDTO.DomainIsMultiValueSelect);
	 	 	 	 this.DomainDataType = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataType);
	 	 	 	 this.DomainReferenceID = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceID);
	 	 	 	 this.DomainReferenceTitle = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceTitle);
	 	 	 	 this.DomainReferenceDescription = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceDescription);
	 	 	 	 this.DomainReferenceValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainReferenceValue);
	 	 	 	 this.DomainDataValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataValue);
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable(DataSourceWithDomainDataItemDTO.DomainDataDomainReferenceID);
	 	 	 	 this.DomainAccessValue = ko.observable(DataSourceWithDomainDataItemDTO.DomainAccessValue);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IDataSourceWithDomainDataItemDTO{
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
	 	  }



	 }
	 export class UserWithDomainDataItemViewModel extends EntityWithDomainDataItemViewModel{
	 	 public UserID: KnockoutObservable<any>;
	 	 public UserName: KnockoutObservable<string>;
	 	 public UserSalutation: KnockoutObservable<string>;
	 	 public UserFirstName: KnockoutObservable<string>;
	 	 public UserMiddleName: KnockoutObservable<string>;
	 	 public UserLastName: KnockoutObservable<string>;
	 	 public UserEmailAddress: KnockoutObservable<string>;
	 	 public UserPhoneNumber: KnockoutObservable<string>;
	 	 public UserFaxNumber: KnockoutObservable<string>;
	 	 public UserIsActive: KnockoutObservable<boolean>;
	 	 constructor(UserWithDomainDataItemDTO?: CNDS.Interfaces.IUserWithDomainDataItemDTO)
	 	  {
	 	 	  super();
	 	 	 if (UserWithDomainDataItemDTO== null) {
	 	 	 	 this.UserID = ko.observable<any>();
	 	 	 	 this.UserName = ko.observable<any>();
	 	 	 	 this.UserSalutation = ko.observable<any>();
	 	 	 	 this.UserFirstName = ko.observable<any>();
	 	 	 	 this.UserMiddleName = ko.observable<any>();
	 	 	 	 this.UserLastName = ko.observable<any>();
	 	 	 	 this.UserEmailAddress = ko.observable<any>();
	 	 	 	 this.UserPhoneNumber = ko.observable<any>();
	 	 	 	 this.UserFaxNumber = ko.observable<any>();
	 	 	 	 this.UserIsActive = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.NetworkUrl = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Organization = ko.observable<any>();
	 	 	 	 this.OrganizationAcronym = ko.observable<any>();
	 	 	 	 this.ParentOrganizationID = ko.observable<any>();
	 	 	 	 this.DomainUseID = ko.observable<any>();
	 	 	 	 this.DomainID = ko.observable<any>();
	 	 	 	 this.ParentDomainID = ko.observable<any>();
	 	 	 	 this.DomainTitle = ko.observable<any>();
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable<any>();
	 	 	 	 this.DomainDataType = ko.observable<any>();
	 	 	 	 this.DomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainReferenceTitle = ko.observable<any>();
	 	 	 	 this.DomainReferenceDescription = ko.observable<any>();
	 	 	 	 this.DomainReferenceValue = ko.observable<any>();
	 	 	 	 this.DomainDataValue = ko.observable<any>();
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable<any>();
	 	 	 	 this.DomainAccessValue = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.UserID = ko.observable(UserWithDomainDataItemDTO.UserID);
	 	 	 	 this.UserName = ko.observable(UserWithDomainDataItemDTO.UserName);
	 	 	 	 this.UserSalutation = ko.observable(UserWithDomainDataItemDTO.UserSalutation);
	 	 	 	 this.UserFirstName = ko.observable(UserWithDomainDataItemDTO.UserFirstName);
	 	 	 	 this.UserMiddleName = ko.observable(UserWithDomainDataItemDTO.UserMiddleName);
	 	 	 	 this.UserLastName = ko.observable(UserWithDomainDataItemDTO.UserLastName);
	 	 	 	 this.UserEmailAddress = ko.observable(UserWithDomainDataItemDTO.UserEmailAddress);
	 	 	 	 this.UserPhoneNumber = ko.observable(UserWithDomainDataItemDTO.UserPhoneNumber);
	 	 	 	 this.UserFaxNumber = ko.observable(UserWithDomainDataItemDTO.UserFaxNumber);
	 	 	 	 this.UserIsActive = ko.observable(UserWithDomainDataItemDTO.UserIsActive);
	 	 	 	 this.NetworkID = ko.observable(UserWithDomainDataItemDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(UserWithDomainDataItemDTO.Network);
	 	 	 	 this.NetworkUrl = ko.observable(UserWithDomainDataItemDTO.NetworkUrl);
	 	 	 	 this.OrganizationID = ko.observable(UserWithDomainDataItemDTO.OrganizationID);
	 	 	 	 this.Organization = ko.observable(UserWithDomainDataItemDTO.Organization);
	 	 	 	 this.OrganizationAcronym = ko.observable(UserWithDomainDataItemDTO.OrganizationAcronym);
	 	 	 	 this.ParentOrganizationID = ko.observable(UserWithDomainDataItemDTO.ParentOrganizationID);
	 	 	 	 this.DomainUseID = ko.observable(UserWithDomainDataItemDTO.DomainUseID);
	 	 	 	 this.DomainID = ko.observable(UserWithDomainDataItemDTO.DomainID);
	 	 	 	 this.ParentDomainID = ko.observable(UserWithDomainDataItemDTO.ParentDomainID);
	 	 	 	 this.DomainTitle = ko.observable(UserWithDomainDataItemDTO.DomainTitle);
	 	 	 	 this.DomainIsMultiValueSelect = ko.observable(UserWithDomainDataItemDTO.DomainIsMultiValueSelect);
	 	 	 	 this.DomainDataType = ko.observable(UserWithDomainDataItemDTO.DomainDataType);
	 	 	 	 this.DomainReferenceID = ko.observable(UserWithDomainDataItemDTO.DomainReferenceID);
	 	 	 	 this.DomainReferenceTitle = ko.observable(UserWithDomainDataItemDTO.DomainReferenceTitle);
	 	 	 	 this.DomainReferenceDescription = ko.observable(UserWithDomainDataItemDTO.DomainReferenceDescription);
	 	 	 	 this.DomainReferenceValue = ko.observable(UserWithDomainDataItemDTO.DomainReferenceValue);
	 	 	 	 this.DomainDataValue = ko.observable(UserWithDomainDataItemDTO.DomainDataValue);
	 	 	 	 this.DomainDataDomainReferenceID = ko.observable(UserWithDomainDataItemDTO.DomainDataDomainReferenceID);
	 	 	 	 this.DomainAccessValue = ko.observable(UserWithDomainDataItemDTO.DomainAccessValue);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IUserWithDomainDataItemDTO{
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
	 	  }



	 }
	 export class NetworkEntityViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.INetworkEntityDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public EntityType: KnockoutObservable<CNDS.Enums.EntityType>;
	 	 public NetworkEntityID: KnockoutObservable<any>;
	 	 constructor(NetworkEntityDTO?: CNDS.Interfaces.INetworkEntityDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkEntityDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.EntityType = ko.observable<any>();
	 	 	 	 this.NetworkEntityID = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(NetworkEntityDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(NetworkEntityDTO.Network);
	 	 	 	 this.EntityType = ko.observable(NetworkEntityDTO.EntityType);
	 	 	 	 this.NetworkEntityID = ko.observable(NetworkEntityDTO.NetworkEntityID);
	 	 	 	 this.ID = ko.observable(NetworkEntityDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(NetworkEntityDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkEntityDTO{
	 	 	  return {
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	Network: this.Network(),
	 	 	 	EntityType: this.EntityType(),
	 	 	 	NetworkEntityID: this.NetworkEntityID(),
	 	 	 	ID: this.ID(),
	 	 	 	Timestamp: this.Timestamp(),
	 	 	  };
	 	  }



	 }
	 export class NetworkViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.INetworkDTO>{
	 	 public Name: KnockoutObservable<string>;
	 	 public Url: KnockoutObservable<string>;
	 	 public ServiceUrl: KnockoutObservable<string>;
	 	 public ServiceUserName: KnockoutObservable<string>;
	 	 public ServicePassword: KnockoutObservable<string>;
	 	 constructor(NetworkDTO?: CNDS.Interfaces.INetworkDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkDTO== null) {
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Url = ko.observable<any>();
	 	 	 	 this.ServiceUrl = ko.observable<any>();
	 	 	 	 this.ServiceUserName = ko.observable<any>();
	 	 	 	 this.ServicePassword = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.Name = ko.observable(NetworkDTO.Name);
	 	 	 	 this.Url = ko.observable(NetworkDTO.Url);
	 	 	 	 this.ServiceUrl = ko.observable(NetworkDTO.ServiceUrl);
	 	 	 	 this.ServiceUserName = ko.observable(NetworkDTO.ServiceUserName);
	 	 	 	 this.ServicePassword = ko.observable(NetworkDTO.ServicePassword);
	 	 	 	 this.ID = ko.observable(NetworkDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(NetworkDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkDTO{
	 	 	  return {
	 	 	 	Name: this.Name(),
	 	 	 	Url: this.Url(),
	 	 	 	ServiceUrl: this.ServiceUrl(),
	 	 	 	ServiceUserName: this.ServiceUserName(),
	 	 	 	ServicePassword: this.ServicePassword(),
	 	 	 	ID: this.ID(),
	 	 	 	Timestamp: this.Timestamp(),
	 	 	  };
	 	  }



	 }
	 export class OrganizationViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.IOrganizationDTO>{
	 	 public Name: KnockoutObservable<string>;
	 	 public Acronym: KnockoutObservable<string>;
	 	 public ParentOrganizationID: KnockoutObservable<any>;
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public ContactEmail: KnockoutObservable<string>;
	 	 public ContactFirstName: KnockoutObservable<string>;
	 	 public ContactLastName: KnockoutObservable<string>;
	 	 public ContactPhone: KnockoutObservable<string>;
	 	 constructor(OrganizationDTO?: CNDS.Interfaces.IOrganizationDTO)
	 	  {
	 	 	  super();
	 	 	 if (OrganizationDTO== null) {
	 	 	 	 this.Name = ko.observable<any>();
	 	 	 	 this.Acronym = ko.observable<any>();
	 	 	 	 this.ParentOrganizationID = ko.observable<any>();
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.ContactEmail = ko.observable<any>();
	 	 	 	 this.ContactFirstName = ko.observable<any>();
	 	 	 	 this.ContactLastName = ko.observable<any>();
	 	 	 	 this.ContactPhone = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.Name = ko.observable(OrganizationDTO.Name);
	 	 	 	 this.Acronym = ko.observable(OrganizationDTO.Acronym);
	 	 	 	 this.ParentOrganizationID = ko.observable(OrganizationDTO.ParentOrganizationID);
	 	 	 	 this.NetworkID = ko.observable(OrganizationDTO.NetworkID);
	 	 	 	 this.ContactEmail = ko.observable(OrganizationDTO.ContactEmail);
	 	 	 	 this.ContactFirstName = ko.observable(OrganizationDTO.ContactFirstName);
	 	 	 	 this.ContactLastName = ko.observable(OrganizationDTO.ContactLastName);
	 	 	 	 this.ContactPhone = ko.observable(OrganizationDTO.ContactPhone);
	 	 	 	 this.ID = ko.observable(OrganizationDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(OrganizationDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IOrganizationDTO{
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
	 	  }



	 }
	 export class NetworkRequestTypeDefinitionViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.INetworkRequestTypeDefinitionDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public ProjectID: KnockoutObservable<any>;
	 	 public Project: KnockoutObservable<string>;
	 	 public RequestTypeID: KnockoutObservable<any>;
	 	 public RequestType: KnockoutObservable<string>;
	 	 public DataSourceID: KnockoutObservable<any>;
	 	 public DataSource: KnockoutObservable<string>;
	 	 constructor(NetworkRequestTypeDefinitionDTO?: CNDS.Interfaces.INetworkRequestTypeDefinitionDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkRequestTypeDefinitionDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.ProjectID = ko.observable<any>();
	 	 	 	 this.Project = ko.observable<any>();
	 	 	 	 this.RequestTypeID = ko.observable<any>();
	 	 	 	 this.RequestType = ko.observable<any>();
	 	 	 	 this.DataSourceID = ko.observable<any>();
	 	 	 	 this.DataSource = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(NetworkRequestTypeDefinitionDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(NetworkRequestTypeDefinitionDTO.Network);
	 	 	 	 this.ProjectID = ko.observable(NetworkRequestTypeDefinitionDTO.ProjectID);
	 	 	 	 this.Project = ko.observable(NetworkRequestTypeDefinitionDTO.Project);
	 	 	 	 this.RequestTypeID = ko.observable(NetworkRequestTypeDefinitionDTO.RequestTypeID);
	 	 	 	 this.RequestType = ko.observable(NetworkRequestTypeDefinitionDTO.RequestType);
	 	 	 	 this.DataSourceID = ko.observable(NetworkRequestTypeDefinitionDTO.DataSourceID);
	 	 	 	 this.DataSource = ko.observable(NetworkRequestTypeDefinitionDTO.DataSource);
	 	 	 	 this.ID = ko.observable(NetworkRequestTypeDefinitionDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(NetworkRequestTypeDefinitionDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkRequestTypeDefinitionDTO{
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
	 	  }



	 }
	 export class NetworkRequestTypeMappingViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.INetworkRequestTypeMappingDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public Network: KnockoutObservable<string>;
	 	 public ProjectID: KnockoutObservable<any>;
	 	 public Project: KnockoutObservable<string>;
	 	 public RequestTypeID: KnockoutObservable<any>;
	 	 public RequestType: KnockoutObservable<string>;
	 	 public Routes: KnockoutObservableArray<NetworkRequestTypeDefinitionViewModel>;
	 	 constructor(NetworkRequestTypeMappingDTO?: CNDS.Interfaces.INetworkRequestTypeMappingDTO)
	 	  {
	 	 	  super();
	 	 	 if (NetworkRequestTypeMappingDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.Network = ko.observable<any>();
	 	 	 	 this.ProjectID = ko.observable<any>();
	 	 	 	 this.Project = ko.observable<any>();
	 	 	 	 this.RequestTypeID = ko.observable<any>();
	 	 	 	 this.RequestType = ko.observable<any>();
	 	 	 	 this.Routes = ko.observableArray<NetworkRequestTypeDefinitionViewModel>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(NetworkRequestTypeMappingDTO.NetworkID);
	 	 	 	 this.Network = ko.observable(NetworkRequestTypeMappingDTO.Network);
	 	 	 	 this.ProjectID = ko.observable(NetworkRequestTypeMappingDTO.ProjectID);
	 	 	 	 this.Project = ko.observable(NetworkRequestTypeMappingDTO.Project);
	 	 	 	 this.RequestTypeID = ko.observable(NetworkRequestTypeMappingDTO.RequestTypeID);
	 	 	 	 this.RequestType = ko.observable(NetworkRequestTypeMappingDTO.RequestType);
	 	 	 	 this.Routes = ko.observableArray<NetworkRequestTypeDefinitionViewModel>(NetworkRequestTypeMappingDTO.Routes == null ? null : NetworkRequestTypeMappingDTO.Routes.map((item) => {return new NetworkRequestTypeDefinitionViewModel(item);}));
	 	 	 	 this.ID = ko.observable(NetworkRequestTypeMappingDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(NetworkRequestTypeMappingDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.INetworkRequestTypeMappingDTO{
	 	 	  return {
	 	 	 	NetworkID: this.NetworkID(),
	 	 	 	Network: this.Network(),
	 	 	 	ProjectID: this.ProjectID(),
	 	 	 	Project: this.Project(),
	 	 	 	RequestTypeID: this.RequestTypeID(),
	 	 	 	RequestType: this.RequestType(),
	 	 	 	Routes: this.Routes == null ? null : this.Routes().map((item) => {return item.toData();}),
	 	 	 	ID: this.ID(),
	 	 	 	Timestamp: this.Timestamp(),
	 	 	  };
	 	  }



	 }
	 export class UserViewModel extends EntityDtoWithIDViewModel<CNDS.Interfaces.IUserDTO>{
	 	 public NetworkID: KnockoutObservable<any>;
	 	 public UserName: KnockoutObservable<string>;
	 	 public Salutation: KnockoutObservable<string>;
	 	 public FirstName: KnockoutObservable<string>;
	 	 public MiddleName: KnockoutObservable<string>;
	 	 public LastName: KnockoutObservable<string>;
	 	 public EmailAddress: KnockoutObservable<string>;
	 	 public PhoneNumber: KnockoutObservable<string>;
	 	 public FaxNumber: KnockoutObservable<string>;
	 	 public OrganizationID: KnockoutObservable<any>;
	 	 public Active: KnockoutObservable<boolean>;
	 	 constructor(UserDTO?: CNDS.Interfaces.IUserDTO)
	 	  {
	 	 	  super();
	 	 	 if (UserDTO== null) {
	 	 	 	 this.NetworkID = ko.observable<any>();
	 	 	 	 this.UserName = ko.observable<any>();
	 	 	 	 this.Salutation = ko.observable<any>();
	 	 	 	 this.FirstName = ko.observable<any>();
	 	 	 	 this.MiddleName = ko.observable<any>();
	 	 	 	 this.LastName = ko.observable<any>();
	 	 	 	 this.EmailAddress = ko.observable<any>();
	 	 	 	 this.PhoneNumber = ko.observable<any>();
	 	 	 	 this.FaxNumber = ko.observable<any>();
	 	 	 	 this.OrganizationID = ko.observable<any>();
	 	 	 	 this.Active = ko.observable<any>();
	 	 	 	 this.ID = ko.observable<any>();
	 	 	 	 this.Timestamp = ko.observable<any>();
	 	 	  }else{
	 	 	 	 this.NetworkID = ko.observable(UserDTO.NetworkID);
	 	 	 	 this.UserName = ko.observable(UserDTO.UserName);
	 	 	 	 this.Salutation = ko.observable(UserDTO.Salutation);
	 	 	 	 this.FirstName = ko.observable(UserDTO.FirstName);
	 	 	 	 this.MiddleName = ko.observable(UserDTO.MiddleName);
	 	 	 	 this.LastName = ko.observable(UserDTO.LastName);
	 	 	 	 this.EmailAddress = ko.observable(UserDTO.EmailAddress);
	 	 	 	 this.PhoneNumber = ko.observable(UserDTO.PhoneNumber);
	 	 	 	 this.FaxNumber = ko.observable(UserDTO.FaxNumber);
	 	 	 	 this.OrganizationID = ko.observable(UserDTO.OrganizationID);
	 	 	 	 this.Active = ko.observable(UserDTO.Active);
	 	 	 	 this.ID = ko.observable(UserDTO.ID);
	 	 	 	 this.Timestamp = ko.observable(UserDTO.Timestamp);
	 	 	 }
	 	 }

	 	 public toData(): CNDS.Interfaces.IUserDTO{
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
	 	  }



	 }
}

