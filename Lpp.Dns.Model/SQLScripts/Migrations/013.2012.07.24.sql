if exists( select * from sys.indexes where name = 'IX_EventId' and object_id = object_id('AuditPropertyValues') )
	drop index AuditPropertyValues.IX_EventId
create index IX_EventId on AuditPropertyValues(EventId)
go

if exists( select * from sys.indexes where name = 'IX_Time' and object_id = object_id('AuditEvents') )
	drop index AuditEvents.IX_Time
create index IX_Time on AuditEvents([Time])
go

if exists( select * from sys.objects where object_id = object_id('dbo.SecurityTargetsObjects') )
	drop table SecurityTargetsObjects
go
if exists( select * from sys.foreign_keys where name = 'FK_Parent' and parent_object_id = object_id('SecurityObjects') )
	alter table SecurityObjects drop constraint FK_Parent
go

declare @text nvarchar(max)
select @text = 'alter table SecurityObjects drop constraint [' + name + ']'
from sys.foreign_keys where parent_object_id = object_id('dbo.SecurityObjects') and name like 'FK__SecurityO__Paren__%'
if @text is not null exec sp_sqlexec @text
go

declare @text nvarchar(max)
select @text = 'alter table SecurityObjects drop constraint [' + name + ']'
from sys.key_constraints where parent_object_id = object_id('dbo.SecurityObjects') and name like 'PK__Security__%'
if @text is not null exec sp_sqlexec @text
go

if exists( select * from sys.key_constraints where name = 'PK' and parent_object_id = object_id('SecurityObjects') )
	alter table SecurityObjects drop constraint PK
go

if exists( select * from sys.indexes where name = 'TreeTag_LeftIndex' and object_id = object_id('SecurityObjects') )
	drop index SecurityObjects.TreeTag_LeftIndex
	
create clustered index TreeTag_LeftIndex on SecurityObjects(TreeTag, LeftIndex)
go

alter table SecurityObjects add constraint PK primary key (Id)
go

alter table SecurityObjects add constraint FK_Parent foreign key (ParentId) references SecurityObjects(Id)
go