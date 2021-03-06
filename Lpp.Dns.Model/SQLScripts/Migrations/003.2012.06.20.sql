IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[UsedRequestTypes]'))  drop view dbo.UsedRequestTypes
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vwUsedRequestTypes]'))  drop view dbo.vwUsedRequestTypes
go

create view vwUsedRequestTypes
with schemabinding
as select count_big(*) as [count], querytypeid as requesttypeid from dbo.queries group by querytypeid
go

create unique clustered index ix on vwusedrequesttypes(requesttypeid)
go

create view UsedRequestTypes
as select RequestTypeId from vwUsedRequestTypes with(noexpand)
go