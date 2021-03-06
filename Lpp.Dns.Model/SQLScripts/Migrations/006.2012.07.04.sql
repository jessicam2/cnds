update datamarts set isdeleted = 0 where isdeleted is null
update organizations set isdeleted = 0 where isdeleted is null
update [users] set isdeleted = 0 where isdeleted is null
update [groups] set isdeleted = 0 where isdeleted is null

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vwUsers]'))
DROP VIEW [dbo].[vwUsers]
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[DNS3_DataMarts]'))
DROP VIEW [dbo].[DNS3_DataMarts]
GO

create view vwUsers as
select u.*, cast( case when isnull(u.isdeleted,0) = 1 or isnull(o.isdeleted,0) = 1 then 1 else 0 end as bit ) as EffectiveIsDeleted
from users u inner join organizations o on u.organizationid = o.organizationid
go

CREATE VIEW [dbo].[DNS3_DataMarts]
AS
SELECT     d.DataMartId AS Id, d.DataMartName AS Name, d.Url, d.RequiresApproval, d.DataMartTypeId, d.AvailablePeriod, d.ContactEmail, d.ContactFirstName, d.ContactLastName, 
           d.ContactPhone, d.SpecialRequirements, d.UsageRestrictions, d.isDeleted, d.HealthPlanDescription, d.OrganizationId, d.IsGroupDataMart,
		   d.[SID],
		   cast( case when isnull(d.isdeleted,0) = 1 or isnull(o.isdeleted,0) = 1 then 1 else 0 end as bit) as EffectiveIsDeleted
FROM         dbo.DataMarts d
inner join organizations o on d.organizationid = o.organizationid

GO


IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[RoutingCounts]'))  drop view dbo.RoutingCounts
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[_RoutingCounts]'))  drop view dbo._RoutingCounts
go

set ansi_padding on
go

create view _RoutingCounts
with schemabinding
as
select QueryId,
	Sum(case when QueryStatusTypeId = 2 then 1 else 0 end) as Submitted,
	Sum(case when QueryStatusTypeId = 3 then 1 else 0 end) as Completed,
	Sum(case when QueryStatusTypeId = 4 then 1 else 0 end) as AwaitingRequestApproval,
	Sum(case when QueryStatusTypeId = 10 then 1 else 0 end) as AwaitingResponseApproval,
	Sum(case when QueryStatusTypeId = 5 then 1 else 0 end) as RejectedRequest,
	Sum(case when QueryStatusTypeId = 12 then 1 else 0 end) as RejectedResults,
	COUNT_BIG(*) as Total
from
	dbo.QueriesDataMarts
group by QueryId
go

create unique clustered index pk_ix on _RoutingCounts(QueryId) 
go

create index ix on _RoutingCounts(QueryId, Submitted, Completed, AwaitingRequestApproval, AwaitingResponseApproval, Total, RejectedRequest, RejectedResults)
go

create view RoutingCounts as select * from _RoutingCounts with(noexpand)
go