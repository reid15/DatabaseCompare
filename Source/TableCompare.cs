using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Text;

namespace DatabaseCompare
{
    public class TableCompare
    {
        public CompareSQL CompareTables(
            TableCollection sourceTables,
            TableCollection targetTables
        )
        {
            var tableSQL = new StringBuilder();
            var columnSQL = new StringBuilder();
            var dropSQL = new StringBuilder();

            foreach (Table sourceTable in sourceTables)
            {
                if (targetTables.Contains(sourceTable.Name, sourceTable.Schema))
                {
                    Table targetTable = targetTables[sourceTable.Name, sourceTable.Schema];
                    columnSQL.AppendLine(CompareTable(sourceTable, targetTable));
                }
                else
                {
                    tableSQL.AppendLine(ScriptObjects.ScriptTable(sourceTable));
                }
            }

            foreach(Table targetTable in targetTables)
            {
                if (!sourceTables.Contains(targetTable.Name, targetTable.Schema))
                {
                    dropSQL.AppendLine("DROP TABLE IF EXISTS " + targetTable.Schema + "." + targetTable.Name + ";");
                }
            }
            tableSQL.Append(columnSQL.ToString());
            return new CompareSQL(dropSQL.ToString(), tableSQL.ToString());
        }

        private string CompareTable(
            Table sourceTable,
            Table targetTable
        )
        {
            StringBuilder returnSQL = new StringBuilder();
            foreach (Column sourceColumn in sourceTable.Columns)
            {
                if (targetTable.Columns.Contains(sourceColumn.Name))
                {
                    Column targetColumn = targetTable.Columns[sourceColumn.Name];
                    if (!IsColumnEqual(sourceColumn, targetColumn))
                    {
                        returnSQL.AppendLine(ScriptObjects.ScriptColumn(sourceColumn, false));
                    }
                } else
                {
                    returnSQL.AppendLine(ScriptObjects.ScriptColumn(sourceColumn, true));
                }
            }
            return returnSQL.ToString();
        }

        private bool IsColumnEqual(
            Column sourceColumn,
            Column targetColumn
        )
        {
            // Comparing the Data Type objects will check for data length as well
            if (sourceColumn.DataType.Equals(targetColumn.DataType) && (sourceColumn.Nullable == targetColumn.Nullable))
            {
                return true;
            } else
            {
                return false;
            }
        }

    }
}
