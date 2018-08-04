using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCompare
{
    public class DataCompare
    {
        private DataCompareConfiguration config;

        public string GetDataCompare(
            DataCompareConfiguration dataCompareConfiguration
        )
        {
            config = dataCompareConfiguration;
            var sourceDatabase = SqlSchema.GetDatabase(config.SourceServerName, config.SourceDatabaseName);
            var targetDatabase = SqlSchema.GetDatabase(config.TargetServerName, config.TargetDatabaseName);

            var returnScript = new StringBuilder();
            returnScript.AppendLine("-- Source Server Name: " + config.SourceServerName);
            returnScript.AppendLine("-- Source Database Name: " + config.SourceDatabaseName);
            returnScript.AppendLine("-- Target Server Name: " + config.TargetServerName);
            returnScript.AppendLine("-- Target Database Name: " + config.TargetDatabaseName);
            returnScript.AppendLine("-- Run Date: " + DateTime.Now.ToString());
            returnScript.AppendLine();
            returnScript.AppendLine("USE [" + config.TargetDatabaseName + "];");
            returnScript.AppendLine("GO");
            returnScript.AppendLine();

            returnScript.AppendLine(GetDataChanges(sourceDatabase));
            returnScript.AppendLine();
            returnScript.AppendLine("GO");

            return returnScript.ToString();
        }

        private string GetDataChanges(
            Database sourceDatabase
        )
        {
            var returnSQL = new StringBuilder();
            var tableList = GetTablesToCompare(sourceDatabase);
            foreach(var item in tableList)
            {
                var result = CompareTable(item);
                if (result.Length > 0)
                {
                    returnSQL.AppendLine(result);
                }
            }
            return returnSQL.ToString();
        }

        private List<Table> GetTablesToCompare(Database sourceDatabase)
        {
            var returnList = new List<Table>();

            foreach (Table table in sourceDatabase.Tables)
            {
                if (!table.IsSystemObject)
                {
                    if (table.ExtendedProperties.Contains("IsReferenceTable"))
                    {
                        if (table.ExtendedProperties["IsReferenceTable"].Value.ToString().ToUpper() == "TRUE")
                        {
                            returnList.Add(table);
                        }
                    }
                }
            }

            return returnList;
        }

        private string CompareTable(
            Table table
        )
        {
            string sql = GetTableSQL(table);
            var sourceConnectionString = DatabaseCommon.DataAccess.GetConnectionString(config.SourceServerName, config.SourceDatabaseName);
            var sourceData = DatabaseCommon.DataAccess.GetDataTable(sourceConnectionString, sql);
            var targetConnectionString = DatabaseCommon.DataAccess.GetConnectionString(config.TargetServerName, config.TargetDatabaseName);
            var targetData = DatabaseCommon.DataAccess.GetDataTable(targetConnectionString, sql);

            return CompareDataTables(sourceData, targetData, table);
        }

        private string GetTableSQL(Table table)
        {
            var returnSQL = new StringBuilder();
            returnSQL.Append("SELECT ");
            bool isFirstColumn = true;
            foreach (Column column in table.Columns)
            {
                if (!isFirstColumn)
                {
                    returnSQL.Append(", ");
                }
                returnSQL.Append(column.Name);
                isFirstColumn = false;
            }
            
            returnSQL.AppendFormat(" FROM [{0}].[{1}];", table.Schema, table.Name);

            return returnSQL.ToString();
        }

        private List<Column> GetPrimaryKeyColumns(Table table)
        {
            var returnList = new List<Column>();

            foreach (Column column in table.Columns)
            {
                if(column.InPrimaryKey)
                {
                    returnList.Add(column);
                }
            }
            return returnList;
        }

        private string CompareDataTables(
            DataTable sourceDataTable,
            DataTable targetDataTable,
            Table sourceTable
        )
        {
            var pkColumns = GetPrimaryKeyColumns(sourceTable);
            var returnSQL = new StringBuilder();
            bool hasInsert = false;

            foreach (DataRow row in sourceDataTable.Rows)
            {
                var targetRow = GetRowFromDataTable(targetDataTable, row, pkColumns);
                if (targetRow == null)
                {
                    returnSQL.AppendLine(ScriptInsert(row, sourceTable));
                    hasInsert = true;
                } else
                {
                    string updateSQL = ScriptUpdate(row, targetRow, sourceTable);
                    if (updateSQL.Length > 0)
                    {
                        returnSQL.AppendLine(updateSQL);
                    }
                }
            }

            foreach (DataRow targetRow in targetDataTable.Rows)
            {
                var sourceRow = GetRowFromDataTable(sourceDataTable, targetRow, pkColumns);
                if (sourceRow == null)
                {
                    returnSQL.AppendLine(ScriptDelete(targetRow, sourceTable));
                }
            }

            if (hasInsert && DatabaseCommon.DatabaseSchema.HasIdentityColumn(sourceTable))
            {
                string identityClause = "SET IDENTITY_INSERT " + sourceTable.Schema + "." + sourceTable.Name;
                returnSQL.Insert(0, identityClause + " ON;" + Environment.NewLine + Environment.NewLine);
                returnSQL.AppendLine(identityClause + " OFF;");
            }
            return returnSQL.ToString();
        }

        private DataRow GetRowFromDataTable(
            DataTable dataTable,
            DataRow dataRow,
            List<Column> pkColumns
        )
        {
            string where = GetWhere(dataRow, pkColumns);
            var result = dataTable.Select(where);
            if (result.Count() == 0)
            {
                return null;
            }
            return result[0];
        }

        private string GetWhere(
            DataRow dataRow,
            List<Column> pkColumns
        )
        {
            string where = "";
            foreach (Column column in pkColumns)
            {
                if (where.Length > 0)
                {
                    where += "and ";
                }
                where += column.Name + " = " + DatabaseCommon.ScriptData.DelimitData(dataRow[column.Name], column);
            }
            return where;
        }

        private string ScriptInsert(
            DataRow dataRow,
            Table table
        )
        {
            var returnSQL = new StringBuilder();
            var valuesList = new StringBuilder();
            
            returnSQL.Append("INSERT INTO ");
            returnSQL.Append("[" + table.Schema + "].[" + table.Name + "](");

            bool firstColumn = true;
            foreach(Column column in table.Columns)
            {
                if (!firstColumn)
                {
                    returnSQL.Append(", ");
                    valuesList.Append(", ");
                }
                returnSQL.Append("[" + column.Name + "]");
                valuesList.Append(DatabaseCommon.ScriptData.DelimitData(dataRow[column.Name], column));
                firstColumn = false;
            }
            returnSQL.AppendLine(")");
            returnSQL.AppendLine("VALUES (" + valuesList.ToString() + ");");

            return returnSQL.ToString();
        }

        private string ScriptUpdate(
            DataRow sourceRow,
            DataRow targetRow,
            Table sourceTable
        )
        {
            var updateSQL = new StringBuilder();

            bool firstColumn = true;
            foreach (Column column in sourceTable.Columns)
            {
                if (!column.InPrimaryKey)
                {
                    if (!IsEqual(sourceRow[column.Name], targetRow[column.Name]))
                     {
                        if (!firstColumn)
                        {
                            updateSQL.Append(", ");
                        }

                        updateSQL.Append("[" + column.Name + "] = ");
                        updateSQL.AppendLine(DatabaseCommon.ScriptData.DelimitData(sourceRow[column.Name], column));
                        firstColumn = false;
                    }
                }
            }

            if (updateSQL.ToString().Length == 0)
            {
                return string.Empty;
            }

            var returnSQL = new StringBuilder();
            returnSQL.AppendLine("UPDATE [" + sourceTable.Schema + "].[" + sourceTable.Name + "] SET");
            returnSQL.Append(updateSQL.ToString());
            returnSQL.AppendLine("WHERE " + GetWhere(sourceRow, GetPrimaryKeyColumns(sourceTable)) + ";");

            return returnSQL.ToString();
        }

        private string ScriptDelete(
            DataRow row,
            Table sourceTable
        )
        {
            var returnSQL = new StringBuilder();
            returnSQL.Append("DELETE FROM ");
            returnSQL.AppendLine("[" + sourceTable.Schema + "].[" + sourceTable.Name + "]");
            returnSQL.AppendLine("WHERE " + GetWhere(row, GetPrimaryKeyColumns(sourceTable)) + ";");

            return returnSQL.ToString();
        }

        private bool IsEqual(
            object value1,
            object value2
        )
        {
            if(value1.GetType().Name == "Byte[]" && value1.GetType().Name == "Byte[]")
            {
                return (((Byte[])value1).SequenceEqual((Byte[])value1));
            }
            return Equals(value1, value2);
        }

    }
}
