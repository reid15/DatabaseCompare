using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCompare
{
    public static class ScriptObjects
    {
        public static string ScriptIndex(
            Index index
        )
        {
            var options = new ScriptingOptions();
            options.DriIncludeSystemNames = true;
            options.NoFileGroup = true;

            var returnSQL = new StringBuilder();
            var indexScript = index.Script(options);
            foreach (var item in indexScript)
            {
                returnSQL.AppendLine(item);
            }
            return returnSQL.ToString();
        }

        public static string ScriptForeignKey(
            ForeignKey foreignKey
        )
        {
            var options = new ScriptingOptions();
            options.DriIncludeSystemNames = true;

            var returnSQL = new StringBuilder();
            var fkScript = foreignKey.Script(options);
            foreach(var item  in fkScript)
            {
                returnSQL.AppendLine(item);
            }
            return returnSQL.ToString();

            //Table parentTable = (Table)foreignKey.Parent;

            //StringBuilder returnSQL = new StringBuilder();
            //returnSQL.Append("ALTER TABLE [" + parentTable.Schema + "].[" + parentTable.Name + "] ");
            //if (foreignKey.IsChecked)
            //{
            //    returnSQL.Append("WITH CHECK ");
            //}
            //returnSQL.Append("ADD FOREIGN KEY(");

            //bool firstColumn = true;
            //foreach(Column column in foreignKey.Columns)
            //{
            //    if (!firstColumn)
            //    {
            //        returnSQL.Append(",");
            //    }
            //    columnList += "[" + column.Name + "]";
            //}

            //ALTER TABLE[dbo].[TestTable1] WITH CHECK ADD FOREIGN KEY([ReferenceId])
            //REFERENCES[dbo].[ReferenceTable] ([ReferenceId])

    }

        public static string ScriptTable(
            Table table
        )
        {
            ScriptingOptions options = GetTableScriptingOptions();
            StringBuilder returnScript = new StringBuilder();
            var scriptedObject = table.Script(options);
            foreach (string line in scriptedObject)
            {
                if (!line.StartsWith("SET ANSI_NULLS") && !line.StartsWith("SET QUOTED_IDENTIFIER"))
                {
                    returnScript.AppendLine(line.Trim() + ";");
                }
            }

            return returnScript.ToString();
        }

        private static ScriptingOptions GetTableScriptingOptions()
        {
            var options = new ScriptingOptions();
            options.DriAll = false;
            options.Indexes = false;
            options.NoCollation = true;
            options.NoFileGroup = true;
            options.PrimaryObject = true;
            options.ScriptBatchTerminator = true;
            options.Triggers = false;

            return options;
        }

        public static string ScriptColumn(
            Column column,
            bool isNewColumn
        )
        {
            Table parentTable = (Table)column.Parent;
            string returnSQL = "ALTER TABLE [" + parentTable.Schema + "].[" + parentTable.Name + "] ";
            returnSQL += (isNewColumn ? "ADD" : "ALTER COLUMN");
            returnSQL += " [" + column.Name + "] ";
            returnSQL += GetDataType(column.DataType);
            returnSQL += " " + (column.Nullable ? "NULL" : "NOT NULL");
            returnSQL += ";";

            return returnSQL;
        }

        private static string GetDataType(
            DataType dataType
        )
        {
            string returnSQL = dataType.Name;
            switch (dataType.Name)
            {
                case "decimal":
                case "numeric":
                    returnSQL += "(" + dataType.NumericPrecision.ToString() + "," + dataType.NumericScale.ToString() + ")";
                    break;
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    returnSQL += "(" + (dataType.MaximumLength == -1 ? "max" : dataType.MaximumLength.ToString()) + ")";
                    break;
                default:
                    break;
            }
            return returnSQL;
        }

        public static string ScriptCheck(
            Check item
        )
        {
            Table parentTable = (Table)item.Parent;
            return "ALTER TABLE [" + parentTable.Schema + "].[" + parentTable.Name + "] ADD CONSTRAINT [" + item.Name + "] " +
                "CHECK " + item.Text + ";";
        }

        public static string ScriptDefault(
            DefaultConstraint defaultConstraint,
            string tableSchema,
            string tableName,
            string columnName
        )
        {
            return "ALTER TABLE [" + tableSchema + "].[" + tableName + "] ADD CONSTRAINT [" + defaultConstraint.Name + 
                "] DEFAULT " + defaultConstraint.Text + " FOR [" + columnName + "];";
        }

        public static string ScriptUserDefinedTableType(
            UserDefinedTableType tableType
        )
        {
            var options = new ScriptingOptions();
            options.DriIncludeSystemNames = true;
            options.NoFileGroup = true;
            options.NoCollation = true;

            var returnSQL = new StringBuilder();
            var dbScript = tableType.Script(options);
            foreach (var item in dbScript)
            {
                returnSQL.AppendLine(item);
            }
            return returnSQL.ToString();
        }

    }
}
