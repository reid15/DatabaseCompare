
-- Drop all user created objects in a database.
-- Drop foreign keys first, functions and types last.

use TargetDatabase;

set nocount on;

declare Diff_Cursor cursor
   for 

select case when o.[type] = 'F' then 'alter table [' + schema_name(p.schema_id) + '].[' + object_name(p.object_id) + '] ' +  
		  'drop constraint [' + o.[name] + ']'
	when o.[type] = 'U' then 'drop table [' + schema_name(o.schema_id) + '].[' + o.[name] + ']' 
	when o.[type] = 'P' then 'drop procedure [' + schema_name(o.schema_id) + '].[' + o.[name] + ']'
	when o.[type] = 'V' then 'drop view [' + schema_name(o.schema_id) + '].[' + o.[name] + ']'
	when o.[type] in ('FN', 'TF', 'FT') then 'drop function [' + schema_name(o.schema_id) + '].[' + o.[name] + ']'
	else 'drop ' + o.[name] 
	end as DropSQL,
	case when o.[type] in ('FN', 'TF') then 2
	when o.[type] = 'F' then 0
	when o.[Type] = 'U' then 9
	else 1 end as SortOrder
from sys.objects as o
left join sys.objects as p
	on p.object_id = o.parent_object_id
where o.Is_MS_Shipped = 0 and
	o.[type] not in ('D', 'PK', 'C', 'UQ', 'TR')
union all
select 'drop type [' + [name] + ']' as DropSQL, 2 as SortOrder
from sys.types
where is_user_defined = 1
union all
select 'drop schema ' + s.[name], 10 as SortOrder
from sys.schemas as s
join sys.schemas as d
	on d.schema_id = s.principal_id
where d.[Name] = 'dbo'
	and s.schema_id <> s.principal_id
order by 2

declare @SQL nvarchar(4000)
declare @SortOrder tinyint

open Diff_Cursor

fetch next from Diff_Cursor into @SQL, @SortOrder

while @@FETCH_STATUS = 0

begin
	print @SQL
	exec sp_executesql @SQL

	fetch next from Diff_Cursor into @SQL, @SortOrder
end

close Diff_Cursor

deallocate Diff_Cursor

go
