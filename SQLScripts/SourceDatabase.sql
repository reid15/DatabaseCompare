
-- Source Database

use SourceDatabase;

-- Cleanup
drop table if exists dbo.TestTable1;
drop table if exists schema2.TestTable2;

-- Create 2nd schema
if not exists (select * from sys.schemas where [Name] = 'schema2')
	exec sp_executesql N'create schema schema2';
go

-- Create source objects

drop table if exists dbo.ReferenceTable;

create table dbo.ReferenceTable(
ReferenceId int not null primary key,
ReferenceType varchar(20) not null
);

drop table if exists dbo.TestTable1;

create table dbo.TestTable1(
RecordId int not null identity(1,1) primary key,
DisplayName varchar(50) not null check(len(DisplayName) > 0),
SortOrder int not null unique,
ReferenceId int null references dbo.ReferenceTable(ReferenceId),
ModifiedDate datetime not null default getdate(),
ComputedColumn as RecordID + SortOrder
);
go

create index IX_TestTable1_ReferenceId on dbo.TestTable1(ReferenceId);

go
create trigger dbo.trgTestTable1
on dbo.TestTable1
after insert,update
as

update t
set ModifiedDate = getdate()
from dbo.TestTable1 as t
join inserted as i
	on i.RecordId = t.RecordId;

go

drop table if exists schema2.TestTable2;

create table schema2.TestTable2(
RecordId int not null identity(1,1) primary key,
DisplayName varchar(50) not null,
SortOrder int not null,
ReferenceId int null references dbo.ReferenceTable(ReferenceId),
ModifiedDate datetime not null
);

go

create index IX_TestTable2_ReferenceId on schema2.TestTable2(ReferenceId);

go
create trigger schema2.trgTestTable2
on schema2.TestTable2
after insert,update
as

update t
set ModifiedDate = getdate()
from dbo.TestTable1 as t
join inserted as i
	on i.RecordId = t.RecordId;

go

create or alter procedure dbo.GetTestTable1 (
@RecordId int
)
as

select RecordId, DisplayName, SortOrder
from dbo.TestTable1
where RecordId = @RecordId;

go

create or alter view dbo.vwGetAllTestTable1
as

select RecordId, DisplayName, SortOrder
from dbo.TestTable1;

go

create or alter function schema2.IsHotDog(@inputText varchar(50))
returns bit
as

begin
	if (@InputText = 'Hot Dog')
		return 1;

	return 0;
end
go

-- Objects to alter

drop procedure if exists dbo.ProcToAlter;
go
create or alter procedure dbo.ProcToAlter
as

select 1;
go

drop view if exists dbo.ViewToAlter;
go
create or alter view dbo.ViewToAlter
as

select RecordId, DisplayName 
from dbo.TestTable1;
go

drop function if exists dbo.FunctionToAlter;
go
create or alter function dbo.FunctionToAlter(@inputText varchar(50))
returns bit
as

begin
	return 0;
end
go

-- Table type

create type PersonTableType as table
(
PersonId int not null,
FirstName varchar(50) not null,
LastName varchar(50) not null
);
go

create or alter procedure dbo.GetPerson(
@PersonTableType PersonTableType readonly
)
as

select PersonId, FirstName + ' ' + LastName as [Name]
from @PersonTableType
order by PersonId;

go

-- For Data Comparison

drop table if exists dbo.DataCompare;

create table dbo.DataCompare (
RecordId int not null identity(1,1) primary key,
ModifiedDate datetime not null,
DisplayName varchar(50) not null,
SortOrder int not null
);

set identity_insert dbo.DataCompare on;

insert into dbo.DataCompare (RecordId, ModifiedDate, DisplayName, SortOrder) 
values (1, getdate(), 'ModifiedRecord', 1);
insert into dbo.DataCompare (RecordId, ModifiedDate, DisplayName, SortOrder) 
values (2, getdate(), 'NewRecord', 2);

set identity_insert dbo.DataCompare off;

exec sys.sp_addextendedproperty @name=N'IsReferenceTable', @value=N'TRUE' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DataCompare'
go


