/// <reference path="../_rootlayout.ts" />
module Users.Details {
    export var vm: ViewModel;

    export class ViewModel extends Global.PageViewModel {
        private self: ViewModel;

        public User: Dns.ViewModels.UserViewModel;
        public Metadata: Controls.MetadataViewer.Index.ViewModel;
        public MetadataViewer: CNDS.ManageVisibility.Index.ViewModel;
        public IsProfile: boolean;
        public SecurityGroups: KnockoutObservableArray<CNDSSecurityGroupViewModel>;
        public Name: KnockoutObservable<string>;
        public AccountStatus: KnockoutComputed<string>;
        public ShowActivation: KnockoutComputed<boolean>;

        //Security
        public Security: Security.Acl.AclEditViewModel<Dns.ViewModels.AclUserViewModel>;
        public UserAcls: KnockoutObservableArray<Dns.ViewModels.AclUserViewModel>;

        //Events
        public Events: Events.Acl.EventAclEditViewModel<Dns.ViewModels.UserEventViewModel>;
        public UserEvents: KnockoutObservableArray<Dns.ViewModels.UserEventViewModel>;

        //Subscriptions
        public Subscriptions: KnockoutObservableArray<Dns.ViewModels.UserEventSubscriptionViewModel>;

        //Lists
        public OrganizationList: Dns.Interfaces.IOrganizationDTO[];
        public SecurityGroupTree: Dns.Interfaces.ITreeItemDTO[];
        public dsSecurityGroupsTree: kendo.data.HierarchicalDataSource;
        public SecurityGroupSelected: (e: kendo.ui.TreeViewSelectEvent) => boolean;
        public SubscribableEventsList: KnockoutObservableArray<EventViewModel>;

        public AssignedNotificationsList: KnockoutObservableArray<Dns.ViewModels.AssignedUserNotificationViewModel>;
        public dataSource: kendo.data.DataSource;
        //UI Events
        public RemoveSecurityGroup: (data: CNDSSecurityGroupViewModel) => void;

        //For temporarily stopping this.Active subscription callback
        public StopActiveCallback: boolean = false;

        constructor(
            screenPermissions: any[],
            canApproveReject: boolean,
            user: Dns.Interfaces.IUserDTO,
            metadataViewer: Controls.MetadataViewer.Index.ViewModel,
            domainViewer: CNDS.ManageVisibility.Index.ViewModel,
            securityGroups: ICNDSSecurityGroupDTO[],
            userAcls: Dns.Interfaces.IAclUserDTO[],
            userEvents: Dns.Interfaces.IUserEventDTO[],
            subscribedEvents: Dns.Interfaces.IUserEventSubscriptionDTO[],
            organizationList: Dns.Interfaces.IOrganizationDTO[],
            securityGroupTree: Dns.Interfaces.ITreeItemDTO[],
            permissionList: Dns.Interfaces.IPermissionDTO[],
            eventList: Dns.Interfaces.IEventDTO[],
            subscribableEventsList: Dns.Interfaces.IEventDTO[],
            assignedNotificationsList: Dns.Interfaces.IAssignedUserNotificationDTO[],
            cndsSecurityGroups: Dns.Interfaces.ICNDSSecurityGroupUserDTO,
            bindingControl: JQuery) {

            super(bindingControl, screenPermissions);
            this.self = this;
            this.IsProfile = User.ID == user.ID;
            this.User = new Dns.ViewModels.UserViewModel(user);
            this.SecurityGroups = ko.observableArray((securityGroups||[]).map((sg) => {
                return new CNDSSecurityGroupViewModel(sg);
            }));

            // Allow activate visibility for the login user only if s/he has Approve Registration rights
            // for the new user's organization either via group ownership or the new user's organization is null.
            this.ShowActivation = ko.computed<boolean>(() => {
                return canApproveReject;
            });
            this.Metadata = metadataViewer;
            this.MetadataViewer = domainViewer;
            this.User.Active.subscribe((value) => {
                if (this.StopActiveCallback == true) {
                    this.StopActiveCallback = false;
                    return;
                }

                if (value) {
                    if (typeof this.User.ID() === 'undefined' || this.User.ID() == null) {
                        this.StopActiveCallback = true;
                        this.User.Active(false);
                        Global.Helpers.ShowAlert("Validation Error", "<p>Set password before marking user active.</p>");
                        return;
                    }
                    else {
                        Dns.WebApi.Users.HasPassword(this.User.ID()).done((hasPassword: boolean[]) => {
                            if (hasPassword.length > 0 && hasPassword[0] == false) {
                                this.StopActiveCallback = true;
                                this.User.Active(false);
                                Global.Helpers.ShowAlert("Validation Error", "<p>Set password before marking user active.</p>");
                                return;
                            }
                        }).fail(() => {
                            this.StopActiveCallback = true;
                            this.User.Active(false);
                            Global.Helpers.ShowAlert("Validation Error", "<p>Set password before marking user active.</p>");
                            return;
                        });;
                    }

                    if (!this.User.ActivatedOn())
                        this.User.ActivatedOn(new Date(Date.now()));
                }

                if (value) {
                    this.User.RejectedByID(null);
                    this.User.RejectedOn(null);
                    this.User.RejectReason(null);
                    this.User.RejectedBy(null);
                    this.User.DeactivatedByID(null);
                    this.User.DeactivatedOn(null);
                    this.User.DeactivatedOn(null);
                    this.User.DeactivatedBy(null);
                    $("#tbUserData").click();
                } else {
                    Global.Helpers.ShowDialog("Deactivate User: " + vm.User.FirstName() + " " + vm.User.LastName(), "/users/deactivate", ["close"], 600, 400).done((reason: string) => {
                        if (reason != null) {
                            this.User.DeactivatedBy("Me");
                            this.User.DeactivatedByID(User.ID);
                            this.User.DeactivatedOn(new Date());
                            this.User.DeactivationReason(reason);
                        } else {
                            this.User.Active(true);
                        }
                    });
                }
            });

            //Permissions
            this.UserAcls = ko.observableArray(userAcls.map((a) => {
                return new Dns.ViewModels.AclUserViewModel(a);
            }));

            this.Security = new Security.Acl.AclEditViewModel<Dns.ViewModels.AclUserViewModel>(permissionList, securityGroupTree, this.UserAcls, [{ Field: "UserID", Value: this.User ? this.User.ID() : null }], Dns.ViewModels.AclUserViewModel);

            //Events
            this.UserEvents = ko.observableArray(userEvents.map((e) => {
                return new Dns.ViewModels.UserEventViewModel(e);
            }));

            this.Events = new Events.Acl.EventAclEditViewModel<Dns.ViewModels.UserEventViewModel>(eventList, securityGroupTree, this.UserEvents, [{ Field: "UserID", Value: this.User.ID() }], Dns.ViewModels.UserEventViewModel);

            //Subscriptions
            this.Subscriptions = ko.observableArray(subscribedEvents.map((se) => {
                return new Dns.ViewModels.UserEventSubscriptionViewModel(se);
            }));
            this.SubscribableEventsList = ko.observableArray(subscribableEventsList.map((el) => {
                return new EventViewModel(this, el);
            }));

            this.AssignedNotificationsList = ko.observableArray(assignedNotificationsList.map((el) => {
                return new Dns.ViewModels.AssignedUserNotificationViewModel(el);
            }));

            //Lists
            this.OrganizationList = organizationList;
            this.SecurityGroupTree = securityGroupTree;

            this.dsSecurityGroupsTree = new kendo.data.HierarchicalDataSource({
                data: this.SecurityGroupTree,
                schema: {
                    model: {
                        id: "ID",
                        hasChildren: "HasChildren",
                        children: "SubItems"
                    }
                }
            });

            this.SecurityGroupSelected = (e: kendo.ui.TreeViewSelectEvent) => {
                var tree: kendo.ui.TreeView = $("#tvSecurityGroupSelector").data("kendoTreeView");

                var node = tree.dataItem(e.node);

                if (!node || !node.id) {
                    e.preventDefault();
                    tree.expand(e.node);
                    return;
                }

                var hasGroup = false;
                this.self.SecurityGroups().forEach((g) => {
                    if (g.ID() == node.id) {
                        hasGroup = true;
                        return;
                    }
                });

                if (!hasGroup) {
                    //Do the add of the group here with an empty Acl
                    this.self.SecurityGroups.push(new CNDSSecurityGroupViewModel({
                        ID: node.id,
                        Name: node["Name"],
                        Path: node["Path"],
                        ParentSecurityGroup: node["ParentSecurityGroup"],
                        Kind: node["Kind"],
                        Owner: node["Owner"],
                        OwnerID: node["OwnerID"],
                        ParentSecurityGroupID: node["ParentSecurityGroupID"],
                        Timestamp: node["Timestamp"],
                        Type: 1
                    }));
                }
                //Close the drop down.
                $('#btnAddSecurityGroup').dropdown('toggle');
                return false;
            }

            //Account Status Badge
            this.AccountStatus = ko.computed<string>(() => {
                return this.User.Deleted() ? "Deleted" : this.User.RejectedOn() != null ? "Rejected" : this.User.DeactivatedOn() != null ? "Deactivated" : this.User.ActivatedOn() == null && !this.User.Active() ? "Pending" : this.User.Active() ? "Active" : "Locked";
            });

            //Title
            this.Name = ko.observable(user.FirstName + " " + user.LastName);
            this.User.FirstName.subscribe(this.UpdateName);
            this.User.LastName.subscribe(this.UpdateName);
            this.WatchTitle(this.Name, "User: ");
            if (cndsSecurityGroups.SecurityGroups != null && cndsSecurityGroups.SecurityGroups.length > 0) {
                ko.utils.arrayForEach(cndsSecurityGroups.SecurityGroups, (item) => {
                    //debugger;
                    this.SecurityGroups.push(new CNDSSecurityGroupViewModel(<ICNDSSecurityGroupDTO>{
                        ID: item.ID,
                        Name: item.Name,
                        ParentSecurityGroup: null,
                        ParentSecurityGroupID: null,
                        Kind: CNDSSecurityGroupKinds.CNDS,
                        Path: 'CNDS\\' + item.Name,
                        Owner: "CNDS",
                        Type: CNDSSecurityGroupTypes.CNDS,
                        OwnerID: null,
                        Timestamp: null 
                    }))
                });
            }

            //Events
            this.RemoveSecurityGroup = (data: CNDSSecurityGroupViewModel) => {
                Global.Helpers.ShowConfirm("Removal Confirmation", "<p>Are you sure that you wish to remove " + this.self.Name() + " from the " + data.Path() + " group?</p>").done(() => {
                    this.self.SecurityGroups.remove(data);
                });
            }
        }

        public onActivate(e) {
            if (e != undefined && e.item.id == "tbAuthentication") {
                $('#AuthGrid').kendoGrid({
                    dataSource: {
                        type: "webapi",
                        serverPaging: true,
                        serverSorting: true,
                        pageSize: 25,
                        transport: {
                            read: {
                                url: Global.Helpers.GetServiceUrl("/Users/ListSuccessfulAudits?$orderby=DateTime desc&$filter=Success ne false and UserID eq " + vm.User.ID() + ""),
                            }
                        },
                        schema: {
                            model: kendo.data.Model.define(Dns.Interfaces.KendoModelUserAuthenticationDTO)
                        },
                    },
                    sortable: true,
                    filterable: {
                        operators: {
                            date: {
                                gt: 'Is after',
                                lt: 'Is before'
                            }
                        }
                    },
                    resizable: true,
                    reorderable: true,
                    scrollable: {
                        virtual: true
                    },
                    pageable: false,
                    columns: [
                        { field: 'DateTime', title: 'Date', format: Constants.DateTimeFormatter, width: 180 },
                        { field: 'Description', title: 'Description' }
                    ]
                }).data("kendoGrid");
            }
        }

        private UpdateName(value: string) {
            vm.Name(vm.User.FirstName() + " " + vm.User.LastName());
        }

        public Reject() {
            Global.Helpers.ShowDialog("Reject User Registration: " + vm.User.FirstName() + " " + vm.User.LastName(), "/users/rejectregistration", ["close"], 600, 400).done((reason) => {
                if (reason) {
                    this.User.RejectedBy("Me");
                    this.User.RejectedOn(new Date());
                    this.User.RejectReason(reason);
                    this.User.RejectedByID(User.ID);
                }
            });
        }

        public ChangePassword() {
            this.Save(null, null, false).done(() => {
                Global.Helpers.ShowDialog("Change Password", "/users/changepassword?ID=" + this.User.ID(), ["Close"], 500, 300);
            });
        }

        public Delete() {            
            Global.Helpers.ShowConfirm("Delete Confirmation", "<p>Are you sure you wish to delete this user?</p>").done(() => {
                if (vm.User.ID()) {
                    Dns.WebApi.Users.Delete([vm.User.ID()]).done(() => {
                        window.location.href = '/users';
                    });
                } else {
                    window.location.href = "/users"; //Return if they're new.
                }
            });
        }

        public Cancel() {
            window.history.back();
        }

        public Save(data, e, showPrompt: boolean = true): JQueryDeferred<void> {
            var deferred = $.Deferred<void>();
            var self = this;
            if (this.User.Fax() == "")
                this.User.Fax(null);
            if (this.User.Phone() == "")
                this.User.Phone(null);
            if (this.User.OrganizationID() == undefined || this.User.OrganizationID() == null) {
                Global.Helpers.ShowAlert("Validation Error", "<p>Please ensure that this user belongs to a Organization.</p>");
                return;
            }

            if (!this.Validate()) {
                deferred.reject();
                return;
            }

            if (this.HasPermission(Permissions.User.ManageSecurity)) {
                if (this.User.Active() && this.User.OrganizationID() == null) {
                    Global.Helpers.ShowAlert("Validation Error", "<p>Please ensure that you have selected an organization.</p>");
                    deferred.reject();
                    return;
                }

                if (this.User.Active() && this.HasPermission(Permissions.User.ManageSecurity) && this.SecurityGroups().length == 0) {
                    Global.Helpers.ShowAlert("Validation Error", "<p>Please ensure that this active user belongs to at least one security group.</p>");
                    return;
                }
            }


            var user = this.User.toData();
            var meta = []
            ko.utils.arrayForEach(self.Metadata.NonGroupedMetadata, (item) => {
                meta.push(item.ToData())
            });
            ko.utils.arrayForEach(self.Metadata.GroupedMetadata, (item) => {
                meta.push(item.ToData())
            });
            user.Metadata = meta;

            //Always test save the primary entity first before anything else.
            Dns.WebApi.Users.InsertOrUpdate([user]).done((users) => {
                this.User.ID(users[0].ID);
                this.User.Timestamp(users[0].Timestamp);

                window.history.replaceState(null, window.document.title, "/users/details?ID=" + users[0].ID);
                var visibilities: Dns.Interfaces.IMetadataDTO[] = [];
                ko.utils.arrayForEach(self.MetadataViewer.Metadata, (item) => {
                    visibilities.push(item.toData());
                });
                $.when<any>(
                    Dns.WebApi.Users.UpdateUserVisibility(visibilities)
                ).done(() => {
                    if (this.HasPermission(Permissions.User.ManageSecurity)) {
                        var userAcls = this.UserAcls().map((a) => {
                            a.UserID(this.User.ID());
                            return a.toData();
                        });

                        var userEvents = this.UserEvents().map((e) => {
                            e.UserID(this.User.ID());
                            return e.toData();
                        });
                        var uSecurityGroups: Dns.Interfaces.ISecurityGroupDTO[] = [];
                        var cndsSecurityGroups: Dns.Interfaces.ICNDSSecurityGroupUserDTO = { UserID: self.User.ID(), SecurityGroups: [] };
                        ko.utils.arrayForEach(vm.SecurityGroups(), (item) => {
                            if (item.Kind() == CNDSSecurityGroupKinds.CNDS)
                                cndsSecurityGroups.SecurityGroups.push({
                                    ID: item.ID(),
                                    Name: item.Name()
                                });
                            else {
                                uSecurityGroups.push({
                                    ID: item.ID(),
                                    Name: item.Name(),
                                    Path: item.Path(),
                                    Type: self.ConvertToSecurityGroupEnum(item.Type()),
                                    OwnerID: item.OwnerID(),
                                    Owner: item.Owner(),
                                    ParentSecurityGroup: item.ParentSecurityGroup(),
                                    ParentSecurityGroupID: item.ParentSecurityGroupID(),
                                    Kind: self.ConvertToSecurityKindEnum(item.Kind())
                                });
                            }
                        });


                        var userSecurityGroups: Dns.Interfaces.IUpdateUserSecurityGroupsDTO = {
                            UserID: this.User.ID(),
                            Groups: uSecurityGroups
                        };
                    }

                    if (this.HasPermission(Permissions.User.ManageNotifications)) {
                        var userSubscriptions = this.Subscriptions().map((s) => {
                            var subscription: Dns.Interfaces.IUserEventSubscriptionDTO = {
                                EventID: s.EventID(),
                                Frequency: s.Frequency(),
                                LastRunTime: s.LastRunTime(),
                                NextDueTime: s.NextDueTime(),
                                FrequencyForMy: s.FrequencyForMy(),
                                UserID: this.User.ID()
                            };
                            return subscription;
                        });
                    }
                    //debugger;
                    $.when<any>(
                        userAcls == null ? null : Dns.WebApi.Security.UpdateUserPermissions(userAcls),
                        userEvents == null ? null : Dns.WebApi.Events.UpdateUserEventPermissions(userEvents),
                        userSecurityGroups == null ? null : Dns.WebApi.Users.UpdateSecurityGroups(userSecurityGroups),
                        cndsSecurityGroups.SecurityGroups.length == 0 ? null : Dns.WebApi.CNDSSecurity.SecurityGroupUsersUpdate(cndsSecurityGroups),
                        userSubscriptions == null ? null : Dns.WebApi.Users.UpdateSubscribedEvents(userSubscriptions)
                    ).done(() => {
                        if (showPrompt) {
                            Global.Helpers.ShowAlert("Save", "<p>Save completed successfully!</p>").done(() => {
                                deferred.resolve();
                            });
                        } else {
                            deferred.resolve();
                        }
                    });
                });
            }).fail(() => {
                deferred.reject();
            });

            return deferred;
        }

        public SelectSecurityGroup() {

            Global.Helpers.ShowDialog("Add Security Group", "/Users/SecurityGroups", ["close"], 950, 550).done((result: ICNDSSecurityGroupDTO) => {
                if (!result)
                    return;
                this.SecurityGroups.push(<any>{
                    ID: ko.observable(result.ID),
                    Name: ko.observable(result.Name),
                    Path: ko.observable(result.Path),
                    Type:  ko.observable(result.Type),
                    OwnerID:  ko.observable(result.OwnerID),
                    Owner:  ko.observable(result.Owner),
                    ParentSecurityGroup: ko.observable(result.ParentSecurityGroup),
                    ParentSecurityGroupID: ko.observable(result.ParentSecurityGroupID),
                    Kind: ko.observable(result.Kind)
                });
                
            });

        }

        public HasEventNotificationDetails(eventID) {
            return this.AssignedNotificationsList().filter(function (item) { return item.EventID() == eventID() }).length > 0;
        }

        public GetEventNotificationDetails(eventID) {
            var filtered = this.AssignedNotificationsList().filter(function (item) { return item.EventID() == eventID() });
            //    forEach((v) => {
            //        v.Description(v.Level() == "Global" && v.Description() == "" ? "Global" : v.Description());
            //});

            var arrItems = [];
            filtered.forEach((item) => {
                if (item.Level() == "Global" && item.Description() == "")
                    item.Description("Global");
                arrItems.push(item.toData());
            });

            var dsResults = new kendo.data.DataSource({
                data: arrItems,
                sort: [{ field: 'Level' }],
                group: [{field: 'Level' }]
            });

            return dsResults;
        }

        public SecurityGroupMenu_Click(data, e: JQueryEventObject): boolean {
            e.stopPropagation();
            return false;
        }

        public ConvertToSecurityGroupEnum(old: CNDSSecurityGroupTypes): Dns.Enums.SecurityGroupTypes{
            switch (old) {
                case CNDSSecurityGroupTypes.Project:
                    return Dns.Enums.SecurityGroupTypes.Project;
                case CNDSSecurityGroupTypes.Organization:
                    return Dns.Enums.SecurityGroupTypes.Organization;
            }
        }

        public ConvertToSecurityKindEnum(old: CNDSSecurityGroupKinds): Dns.Enums.SecurityGroupKinds {
            switch (old) {
                case CNDSSecurityGroupKinds.Administrators:
                    return Dns.Enums.SecurityGroupKinds.Administrators;
                case CNDSSecurityGroupKinds.Custom:
                    return Dns.Enums.SecurityGroupKinds.Custom;
                case CNDSSecurityGroupKinds.Everyone:
                    return Dns.Enums.SecurityGroupKinds.Everyone;
                case CNDSSecurityGroupKinds.Investigators:
                    return Dns.Enums.SecurityGroupKinds.Investigators;
                case CNDSSecurityGroupKinds.EnhancedInvestigators:
                    return Dns.Enums.SecurityGroupKinds.EnhancedInvestigators;
                case CNDSSecurityGroupKinds.QueryAdministrators:
                    return Dns.Enums.SecurityGroupKinds.QueryAdministrators;
                case CNDSSecurityGroupKinds.DataMartAdministrators:
                    return Dns.Enums.SecurityGroupKinds.DataMartAdministrators;
                case CNDSSecurityGroupKinds.Observers:
                    return Dns.Enums.SecurityGroupKinds.Observers;
                case CNDSSecurityGroupKinds.Users:
                    return Dns.Enums.SecurityGroupKinds.Users;
                case CNDSSecurityGroupKinds.GroupDataMartAdministrator:
                    return Dns.Enums.SecurityGroupKinds.GroupDataMartAdministrator;
            }
        }
    }

    function init() {
        var id: any = $.url().param("ID");
        var organizationId: any = $.url().param("OrganizationID");
        var defaultScreenPermissions = [
            Permissions.User.ChangeCertificate,
            Permissions.User.ChangeLogin,
            Permissions.User.ChangePassword,
            Permissions.User.Delete,
            Permissions.User.Edit,
            Permissions.User.ManageNotifications,
            Permissions.User.ManageSecurity,
            Permissions.User.View,
            Permissions.User.ManageCNDSVisibility
        ];

        $.when<any>(
            id == null ? null : Dns.WebApi.Users.GetPermissions([id], defaultScreenPermissions),
            id == null ? null : Dns.WebApi.Users.Get(id),
            id == null ? null : Dns.WebApi.Security.GetUserPermissions(id),
            id == null ? null : Dns.WebApi.Events.GetUserEventPermissions(id),
            Dns.WebApi.Users.GetGlobalPermission(Permissions.Organization.ApproveRejectRegistrations),
            id == null ? Dns.WebApi.Organizations.List() : Dns.WebApi.Organizations.GetForUser(id),
            Dns.WebApi.Security.GetAvailableSecurityGroupTree(),
            Dns.WebApi.Security.GetPermissionsByLocation([Dns.Enums.PermissionAclTypes.Users]),
            id == User.ID ? Dns.WebApi.Events.GetEventsByLocation([Dns.Enums.PermissionAclTypes.Users, Dns.Enums.PermissionAclTypes.UserProfile]) : Dns.WebApi.Events.GetEventsByLocation([Dns.Enums.PermissionAclTypes.Users]),
            id == null ? null : Dns.WebApi.Users.GetSubscribableEvents(id, null, null, "Name"),
            id == null ? null : Dns.WebApi.Users.GetAssignedNotifications(id)

         ).done((
            screenPermissions: any[],
            users,
            userAcls,
            userEvents,
            canApproveRejectGlobally,
            organizationList,
            securityGroupTree,
            permissionList,
            eventList,
            subscribableEventsList,
            assignedNotificationsList
         ) => {
            var user: Dns.Interfaces.IUserDTO;
            if (users == null || users.length == 0) {
                // New user
                user = new Dns.ViewModels.UserViewModel().toData();
                user.FirstName = "";
                user.MiddleName = "";
                user.LastName = "";
                user.OrganizationID = organizationId;
                user.Active = false;
                user.Deleted = false;

                screenPermissions = defaultScreenPermissions;

                Dns.WebApi.Users.GetAvailableUserMetadata().done((result) => {
                    user.Metadata = result || [];
                })
             } else {
                // Existing user
                 user = users[0];
                 if (!user.Active) {
                     Dns.WebApi.Users.GetAvailableUserMetadata().done((result) => {
                         user.Metadata = result || [];
                     })
                 }
            }

            // If it's the current user (editing own profile), ensure they have view, edit and manage notification rights
            if (user.ID == User.ID) {
                $.merge(screenPermissions, [Permissions.User.View.toLowerCase(), Permissions.User.Edit.toLowerCase(), Permissions.User.ChangeLogin.toLowerCase(), Permissions.User.ManageNotifications.toLowerCase()]);
                screenPermissions = $.unique(screenPermissions);
             }
            var deferred = $.Deferred<void>();
            var subscribedEvents: Dns.Interfaces.IUserEventSubscriptionDTO[] = [];
            var securityGroups: ICNDSSecurityGroupDTO[] = [];
            var cndsSecurityGroups: Dns.Interfaces.ICNDSSecurityGroupUserDTO;
            var canApproveRejectOrgLevel: boolean = false; //Set in the deferred below

            deferred.done(() => {
                $(() => {
                    var bindingControl = $("#Content");
                    var metadataViewer = Controls.MetadataViewer.Index.init($('#userMetadata'), user.Metadata || []);
                    var domainVisViewer = CNDS.ManageVisibility.Index.init($('#userVisibility'), user.Metadata || []);
                    vm = new ViewModel(screenPermissions,
                        (canApproveRejectGlobally && canApproveRejectGlobally.length >= 1 && canApproveRejectGlobally[0]) || canApproveRejectOrgLevel,
                        user,
                        metadataViewer,
                        domainVisViewer,
                        securityGroups || [],
                        userAcls || [],
                        userEvents || [],
                        subscribedEvents || [],
                        organizationList || [],
                        securityGroupTree,
                        permissionList || [],
                        eventList || [],
                        subscribableEventsList || [],
                        assignedNotificationsList || [],
                        cndsSecurityGroups,
                        bindingControl);

                    ko.applyBindings(vm, bindingControl[0]);
                });
            });

            $.when<any>(
                id && screenPermissions.indexOf(Permissions.User.ManageSecurity.toLowerCase()) > -1 ? Dns.WebApi.Users.MemberOfSecurityGroups(id) : null,
                id && user.Active && screenPermissions.indexOf(Permissions.User.ManageSecurity.toLowerCase()) > -1 ? Dns.WebApi.CNDSSecurity.GetUserSecurityGroups(id) : null,
                user.ID && (screenPermissions.indexOf(Permissions.User.ManageNotifications.toLowerCase()) > -1 || screenPermissions.indexOf(Permissions.User.ManageNotifications) > -1) ? Dns.WebApi.Users.GetSubscribedEvents(user.ID) : null,
                user.OrganizationID == null ? null : Dns.WebApi.Organizations.GetPermissions([user.OrganizationID], [Permissions.Organization.ApproveRejectRegistrations])
            ).done((sg, cndsSGs, events, permission) => {
                //debugger;
                subscribedEvents = events;
                cndsSecurityGroups = cndsSGs != null ? cndsSGs[0] : { UserID: user.ID, SecurityGroups: {} };
                securityGroups = sg;
                canApproveRejectOrgLevel = (permission == null || permission.length == 0) ? false : permission[0].toUpperCase() == Permissions.Organization.ApproveRejectRegistrations;
                deferred.resolve();
            });
        });
    }

    init();

    export class EventViewModel extends Dns.ViewModels.EventViewModel {
        public Frequency: KnockoutComputed<Dns.Enums.Frequencies>;
        public FrequencyForMy: KnockoutComputed<Dns.Enums.Frequencies>;
        public SupportsMyFrequency: KnockoutObservable<Boolean>;

        constructor(vm: ViewModel, event: Dns.Interfaces.IEventDTO) {
            super(event);
            var subscriptionEvent = new Dns.ViewModels.UserEventSubscriptionViewModel({
                EventID: this.ID(),
                Frequency: null,
                FrequencyForMy: null,
                LastRunTime: null,
                NextDueTime: null,
                UserID: vm.User.ID()
            });

            this.SupportsMyFrequency = ko.observable(event.SupportsMyNotifications);
            
            this.FrequencyForMy = ko.computed<Dns.Enums.Frequencies>({
                read: () => {
                    for (var j = 0; j < vm.Subscriptions().length; j++) {
                        var sub = vm.Subscriptions()[j];
                        if (sub.EventID() == this.ID())
                            return sub.FrequencyForMy();
                    }

                    return null;
                },
                write: (value) => {
                    for (var j = 0; j < vm.Subscriptions().length; j++) {
                        var sub = vm.Subscriptions()[j];
                        if (sub.EventID() == this.ID()) {
                            sub.FrequencyForMy(value);
                            return;
                        }
                    }
                    
                    subscriptionEvent.FrequencyForMy(value);

                    vm.Subscriptions.push(subscriptionEvent);
                }

            });
            this.Frequency = ko.computed<Dns.Enums.Frequencies>({
                read: () => {
                    for (var j = 0; j < vm.Subscriptions().length; j++) {
                        var sub = vm.Subscriptions()[j];
                        if (sub.EventID() == this.ID())
                            return sub.Frequency();
                    }

                    return null;
                },
                write: (value) => {
                    for (var j = 0; j < vm.Subscriptions().length; j++) {
                        var sub = vm.Subscriptions()[j];
                        if (sub.EventID() == this.ID()) {
                            sub.Frequency(value);
                            return;
                        }
                    }

                    subscriptionEvent.Frequency(value);

                    vm.Subscriptions.push(subscriptionEvent);
                }
            });
        }
    }

    export enum CNDSSecurityGroupTypes{
        Organization = 1,
        Project = 2,
        CNDS = 3
    }

    export enum CNDSSecurityGroupKinds {
        Custom = 0,
        Everyone = 1,
        Administrators = 2,
        Investigators = 3,
        EnhancedInvestigators = 4,
        QueryAdministrators = 5,
        DataMartAdministrators = 6,
        Observers = 7,
        Users = 8,
        GroupDataMartAdministrator = 9,
        CNDS = 10
    }

    export var CNDSSecurityGroupTypesTranslation: Dns.Structures.KeyValuePair[] = [
        { value: CNDSSecurityGroupTypes.Organization, text: 'Organization' },
        { value: CNDSSecurityGroupTypes.Project, text: 'Project' },
        { value: CNDSSecurityGroupTypes.CNDS, text: 'CNDS' },
    ]

    export var SecurityGroupKindsTranslation: Dns.Structures.KeyValuePair[] = [
        { value: CNDSSecurityGroupKinds.Custom, text: 'Custom' },
        { value: CNDSSecurityGroupKinds.Everyone, text: 'Everyone' },
        { value: CNDSSecurityGroupKinds.Administrators, text: 'Administrators' },
        { value: CNDSSecurityGroupKinds.Investigators, text: 'Investigators' },
        { value: CNDSSecurityGroupKinds.EnhancedInvestigators, text: 'Enhanced Investigators' },
        { value: CNDSSecurityGroupKinds.QueryAdministrators, text: 'Query Administrators' },
        { value: CNDSSecurityGroupKinds.DataMartAdministrators, text: 'DataMart Administrators' },
        { value: CNDSSecurityGroupKinds.Observers, text: 'Observers' },
        { value: CNDSSecurityGroupKinds.Users, text: 'User' },
        { value: CNDSSecurityGroupKinds.GroupDataMartAdministrator, text: 'Group DataMart Administrator' },
        { value: CNDSSecurityGroupKinds.CNDS, text: 'CNDS' },
    ]

    export interface ICNDSSecurityGroupDTO extends Dns.Interfaces.IEntityDtoWithID {
        Name: string;
        Path: string;
        OwnerID: any;
        Owner: string;
        ParentSecurityGroupID?: any;
        ParentSecurityGroup: string;
        Kind: CNDSSecurityGroupKinds;
        Type: CNDSSecurityGroupTypes;
    }

    export class CNDSSecurityGroupViewModel extends Dns.ViewModels.EntityDtoWithIDViewModel<ICNDSSecurityGroupDTO>{
        public Name: KnockoutObservable<string>;
        public Path: KnockoutObservable<string>;
        public OwnerID: KnockoutObservable<any>;
        public Owner: KnockoutObservable<string>;
        public ParentSecurityGroupID: KnockoutObservable<any>;
        public ParentSecurityGroup: KnockoutObservable<string>;
        public Kind: KnockoutObservable<CNDSSecurityGroupKinds>;
        public Type: KnockoutObservable<CNDSSecurityGroupTypes>;
        constructor(SecurityGroupDTO?: ICNDSSecurityGroupDTO) {
            super();
            if (SecurityGroupDTO == null) {
                this.Name = ko.observable<any>();
                this.Path = ko.observable<any>();
                this.OwnerID = ko.observable<any>();
                this.Owner = ko.observable<any>();
                this.ParentSecurityGroupID = ko.observable<any>();
                this.ParentSecurityGroup = ko.observable<any>();
                this.Kind = ko.observable<any>();
                this.Type = ko.observable<any>();
                this.ID = ko.observable<any>();
                this.Timestamp = ko.observable<any>();
            } else {
                this.Name = ko.observable(SecurityGroupDTO.Name);
                this.Path = ko.observable(SecurityGroupDTO.Path);
                this.OwnerID = ko.observable(SecurityGroupDTO.OwnerID);
                this.Owner = ko.observable(SecurityGroupDTO.Owner);
                this.ParentSecurityGroupID = ko.observable(SecurityGroupDTO.ParentSecurityGroupID);
                this.ParentSecurityGroup = ko.observable(SecurityGroupDTO.ParentSecurityGroup);
                this.Kind = ko.observable(SecurityGroupDTO.Kind);
                this.Type = ko.observable(SecurityGroupDTO.Type);
                this.ID = ko.observable(SecurityGroupDTO.ID);
                this.Timestamp = ko.observable(SecurityGroupDTO.Timestamp);
            }
        }

        public toData(): ICNDSSecurityGroupDTO {
            return {
                Name: this.Name(),
                Path: this.Path(),
                OwnerID: this.OwnerID(),
                Owner: this.Owner(),
                ParentSecurityGroupID: this.ParentSecurityGroupID(),
                ParentSecurityGroup: this.ParentSecurityGroup(),
                Kind: this.Kind(),
                Type: this.Type(),
                ID: this.ID(),
                Timestamp: this.Timestamp(),
            };
        }



    }
}