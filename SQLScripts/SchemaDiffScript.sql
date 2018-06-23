-- Source Server Name: RobLT\SS2017
-- Source Database Name: SourceDatabase
-- Target Server Name: RobLT\SS2017
-- Target Database Name: TargetDatabase
-- Run Date: 4/7/2018 1:25:08 PM

USE [TargetDatabase];
GO

CREATE SCHEMA [schema2] AUTHORIZATION [dbo];
GO

CREATE TABLE [dbo].[ReferenceTable](
	[ReferenceId] [int] NOT NULL,
	[ReferenceType] [varchar](20) NOT NULL
);

CREATE TABLE [dbo].[TestTable1](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[ReferenceId] [int] NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ComputedColumn]  AS ([RecordID]+[SortOrder])
);

CREATE TABLE [schema2].[TestTable2](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[ReferenceId] [int] NULL,
	[ModifiedDate] [datetime] NOT NULL
);




GO
GO
CREATE TYPE [dbo].[PersonTableType] AS TABLE(
	[PersonId] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL
)

GO


GO
create   procedure dbo.GetPerson(
@PersonTableType PersonTableType readonly
)
as

select PersonId, FirstName + ' ' + LastName as [Name]
from @PersonTableType
order by PersonId;
GO

GO
create   procedure dbo.GetTestTable1 (
@RecordId int
)
as

select RecordId, DisplayName, SortOrder
from dbo.TestTable1
where RecordId = @RecordId;
GO

GO
create   procedure dbo.ProcToAlter
as

select 1;
GO


GO
create   view dbo.ViewToAlter
as

select RecordId, DisplayName 
from dbo.TestTable1;
GO

GO
create   view dbo.vwGetAllTestTable1
as

select RecordId, DisplayName, SortOrder
from dbo.TestTable1;
GO


GO
create   function dbo.FunctionToAlter(@inputText varchar(50))
returns bit
as

begin
	return 0;
end
GO

GO
create   function schema2.IsHotDog(@inputText varchar(50))
returns bit
as

begin
	if (@InputText = 'Hot Dog')
		return 1;

	return 0;
end
GO


GO
create trigger dbo.trgTestTable1
on dbo.TestTable1
after insert,update
as

update t
set ModifiedDate = getdate()
from dbo.TestTable1 as t
join inserted as i
	on i.RecordId = t.RecordId;


GO

GO
create trigger schema2.trgTestTable2
on schema2.TestTable2
after insert,update
as

update t
set ModifiedDate = getdate()
from dbo.TestTable1 as t
join inserted as i
	on i.RecordId = t.RecordId;


GO


GO
ALTER TABLE [dbo].[ReferenceTable] ADD  CONSTRAINT [PK__Referenc__E1A99A192B784799] PRIMARY KEY CLUSTERED 
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
CREATE NONCLUSTERED INDEX [IX_TestTable1_ReferenceId] ON [dbo].[TestTable1]
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [PK__TestTabl__FBDF78E9FD16159C] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [UQ__TestTabl__55A193B742F5576D] UNIQUE NONCLUSTERED 
(
	[SortOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
CREATE NONCLUSTERED INDEX [IX_TestTable2_ReferenceId] ON [schema2].[TestTable2]
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [schema2].[TestTable2] ADD  CONSTRAINT [PK__TestTabl__FBDF78E98AA4FFF2] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO


GO
ALTER TABLE [dbo].[TestTable1]  WITH CHECK ADD  CONSTRAINT [FK__TestTable__Refer__22401542] FOREIGN KEY([ReferenceId])
REFERENCES [dbo].[ReferenceTable] ([ReferenceId])
ALTER TABLE [dbo].[TestTable1] CHECK CONSTRAINT [FK__TestTable__Refer__22401542]

GO

GO
ALTER TABLE [schema2].[TestTable2]  WITH CHECK ADD  CONSTRAINT [FK__TestTable__Refer__2704CA5F] FOREIGN KEY([ReferenceId])
REFERENCES [dbo].[ReferenceTable] ([ReferenceId])
ALTER TABLE [schema2].[TestTable2] CHECK CONSTRAINT [FK__TestTable__Refer__2704CA5F]

GO


GO
ALTER TABLE [dbo].[TestTable1] ADD CONSTRAINT [DF__TestTable__Modif__2334397B] DEFAULT (getdate()) FOR [ModifiedDate];
GO


GO
ALTER TABLE [dbo].[TestTable1] ADD CONSTRAINT [CK__TestTable__Displ__214BF109] CHECK (len([DisplayName])>(0));
GO



