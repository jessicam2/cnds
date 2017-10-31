module CNDS.Search.Interfaces {
    export interface IRequestTypeSelection {
        RequestType: string;
        RequestTypeID: any;
        Project: string;
        ProjectID: any;
        Routes: Dns.Interfaces.ICNDSNetworkProjectRequestTypeDataMartDTO[];
    }
}