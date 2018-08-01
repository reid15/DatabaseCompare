-- Source Server Name: RobLT\SS2017
-- Source Database Name: SourceDatabase
-- Target Server Name: RobLT\SS2017
-- Target Database Name: TargetDatabase
-- Run Date: 7/28/2018 3:00:53 PM

USE [TargetDatabase];
GO

CREATE SCHEMA [schema2] AUTHORIZATION [dbo];
GO

-- Drop Objects
DROP VIEW IF EXISTS [dbo].[ViewToDrop];
DROP PROCEDURE IF EXISTS [dbo].[ProcToDrop];
DROP TABLE IF EXISTS dbo.TableToDrop;

-- Create Objects
CREATE TABLE [dbo].[ReferenceTable](
	[ReferenceId] [int] NOT NULL,
	[ReferenceType] [varchar](20) NOT NULL
);

CREATE TABLE [schema2].[TestTable2](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[ReferenceId] [int] NULL,
	[ModifiedDate] [datetime] NOT NULL
);


ALTER TABLE [dbo].[TestTable1] ALTER COLUMN [SortOrder] int NOT NULL;
ALTER TABLE [dbo].[TestTable1] ADD [ReferenceId] int NULL;
ALTER TABLE [dbo].[TestTable1] ADD [ModifiedDate] datetime NOT NULL;
ALTER TABLE [dbo].[TestTable1] ADD [ComputedColumn] int NULL;
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
ALTER   procedure dbo.GetTestTable1 (
@RecordId int
)
as

select RecordId, DisplayName, SortOrder
from dbo.TestTable1
where RecordId = @RecordId;
GO

GO
ALTER   procedure dbo.ProcToAlter
as

select 1;
GO
GO
ALTER   view dbo.ViewToAlter
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
ALTER   function dbo.FunctionToAlter(@inputText varchar(50))
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
CREATE NONCLUSTERED INDEX [IX_TestTable1_ReferenceId] ON [dbo].[TestTable1]
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [UQ__TestTabl__55A193B72543074F] UNIQUE NONCLUSTERED 
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
ALTER TABLE [dbo].[ReferenceTable] ADD  CONSTRAINT [PK_ReferenceTable] PRIMARY KEY CLUSTERED 
(
	[ReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [dbo].[TestTable1] ADD  CONSTRAINT [PK_TestTable1] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

GO
ALTER TABLE [schema2].[TestTable2] ADD  CONSTRAINT [PK_TestTable2] PRIMARY KEY CLUSTERED 
(
	[RecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
GO
ALTER TABLE [dbo].[TestTable1]  WITH CHECK ADD  CONSTRAINT [FK__TestTable__Refer__5E1FF51F] FOREIGN KEY([ReferenceId])
REFERENCES [dbo].[ReferenceTable] ([ReferenceId])
ALTER TABLE [dbo].[TestTable1] CHECK CONSTRAINT [FK__TestTable__Refer__5E1FF51F]

GO

GO
ALTER TABLE [schema2].[TestTable2]  WITH CHECK ADD  CONSTRAINT [FK__TestTable__Refer__61F08603] FOREIGN KEY([ReferenceId])
REFERENCES [dbo].[ReferenceTable] ([ReferenceId])
ALTER TABLE [schema2].[TestTable2] CHECK CONSTRAINT [FK__TestTable__Refer__61F08603]

GO
GO
ALTER TABLE [dbo].[TestTable1] ADD CONSTRAINT [DF__TestTable__Modif__5F141958] DEFAULT (getdate()) FOR [ModifiedDate];
GO
GO
ALTER TABLE [dbo].[TestTable1] ADD CONSTRAINT [CK__TestTable__Displ__5D2BD0E6] CHECK (len([DisplayName])>(0));
GO


