-- Source Server Name: RobLT\SS2017
-- Source Database Name: SourceDatabase
-- Target Server Name: RobLT\SS2017
-- Target Database Name: TargetDatabase
-- Run Date: 7/28/2018 8:09:08 PM

USE [TargetDatabase];
GO

SET IDENTITY_INSERT dbo.DataCompare ON;

UPDATE [dbo].[DataCompare] SET
[ModifiedDate] = '2018-07-28 20:01:00'
, [SortOrder] = 1
, [IsActive] = 0
WHERE RecordId = 1;

INSERT INTO [dbo].[DataCompare]([RecordId], [ModifiedDate], [DisplayName], [SortOrder], [IsActive], [BinaryData])
VALUES (2, '2018-07-28 20:02:00', 'NewRecord', 2, 1, 0x01E240);

DELETE FROM [dbo].[DataCompare]
WHERE RecordId = 3;

SET IDENTITY_INSERT dbo.DataCompare OFF;



GO
