using System;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace DatabaseCompare
{
    public class SchemaCompare
    {

        public string GetSchemaCompare(
            string sourceServerName,
            string sourceDatabaseName,
            string targetServerName,
            string targetDatabaseName
        )
        {
            var sourceDatabase = SqlSchema.GetDatabase(sourceServerName, sourceDatabaseName);
            var targetDatabase = SqlSchema.GetDatabase(targetServerName, targetDatabaseName);

            var returnScript = new StringBuilder();
            returnScript.AppendLine("-- Source Server Name: " + sourceServerName);
            returnScript.AppendLine("-- Source Database Name: " + sourceDatabaseName);
            returnScript.AppendLine("-- Target Server Name: " + targetServerName);
            returnScript.AppendLine("-- Target Database Name: " + targetDatabaseName);
            returnScript.AppendLine("-- Run Date: " + DateTime.Now.ToString());
            returnScript.AppendLine();
            returnScript.AppendLine("USE [" + targetDatabaseName + "];");
            returnScript.AppendLine("GO");
            returnScript.AppendLine();

            returnScript.AppendLine(GetDatabaseObjects(sourceDatabase, targetDatabase));

            return returnScript.ToString();
        }

        private string GetDatabaseObjects(
            Database sourceDatabase,
            Database targetDatabase
        )
        {
            var returnScript = new StringBuilder();
            returnScript.AppendLine(CompareSchemas(sourceDatabase.Schemas, targetDatabase.Schemas));

            // Create all tables before creating foreign keys, triggers, etc, so that the order of table creation/altering won't matter
            TableCompare tableCompare = new TableCompare();
            var tableSQL = tableCompare.CompareTables(sourceDatabase.Tables, targetDatabase.Tables);
            returnScript.AppendLine(tableSQL.CreateTableSQL);
            returnScript.AppendLine(tableSQL.ColumnSQL);
            returnScript.AppendLine(tableSQL.DropSQL);
            returnScript.AppendLine("GO");

            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetUserDefinedTableTypes(sourceDatabase), SqlSchema.GetUserDefinedTableTypes(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetStoredProcedures(sourceDatabase), SqlSchema.GetStoredProcedures(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetViews(sourceDatabase), SqlSchema.GetViews(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetUserDefinedFunctions(sourceDatabase), SqlSchema.GetUserDefinedFunctions(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetTriggers(sourceDatabase), SqlSchema.GetTriggers(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetIndexes(sourceDatabase), SqlSchema.GetIndexes(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetForeignKeys(sourceDatabase), SqlSchema.GetForeignKeys(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetDefaults(sourceDatabase), SqlSchema.GetDefaults(targetDatabase)));
            returnScript.AppendLine(CompareDatabaseObjects(SqlSchema.GetChecks(sourceDatabase), SqlSchema.GetChecks(targetDatabase)));
            
            return returnScript.ToString();
        }

        private string CompareDatabaseObjects(
            List<DatabaseObject> sourceObjects,
            List<DatabaseObject> targetObjects
        )
        {
            StringBuilder returnScript = new StringBuilder();

            foreach (DatabaseObject sourceItem in sourceObjects)
            {
                var targetItem = (targetObjects.FirstOrDefault(n => n.SchemaName == sourceItem.SchemaName && n.ObjectName == sourceItem.ObjectName));
                if (targetItem.DatabaseObjectType == DatabaseObjectType.NoObject)
                {
                    returnScript.AppendLine(ScriptNewObject(sourceItem));
                } else
                {
                    if (!IsObjectTextEqual(sourceItem.ObjectText, targetItem.ObjectText))
                    {
                        returnScript.AppendLine(ScriptAlteredObject(sourceItem));
                    }
                }
            }

            foreach (DatabaseObject targetItem in targetObjects)
            {
                var sourceItem = (sourceObjects.FirstOrDefault(n => n.SchemaName == targetItem.SchemaName && n.ObjectName == targetItem.ObjectName));
                if (sourceItem.DatabaseObjectType == DatabaseObjectType.NoObject)
                {
                    returnScript.AppendLine(ScriptDropObject(targetItem));
                }
            }

            return returnScript.ToString();
        }

        private bool IsObjectTextEqual(
            string sourceObjectText,
            string targetObjectText
        )
        {
            return sourceObjectText.Trim().Equals(targetObjectText.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        private string CompareSchemas(
            SchemaCollection sourceSchemas,
            SchemaCollection targetSchema
        )
        {
            var returnScript = "";
            foreach (Schema item in sourceSchemas)
            {
                if (!item.IsSystemObject && !targetSchema.Contains(item.Name))
                {
                    returnScript += "CREATE SCHEMA [" + item.Name + "] AUTHORIZATION [" + item.Owner + "];" + Environment.NewLine + "GO" + Environment.NewLine;
                }
            }
            return returnScript;
        }

        private string ScriptAlteredObject(
            DatabaseObject sourceObject
        )
        {
            var returnSQL = new StringBuilder();
            returnSQL.AppendLine("GO");
            returnSQL.AppendLine("ALTER" + sourceObject.ObjectText.Substring(6).TrimEnd());
            returnSQL.AppendLine("GO");
            return returnSQL.ToString();
        }

        private string ScriptNewObject(DatabaseObject item)
        {
            var returnSQL = new StringBuilder();
            returnSQL.AppendLine("GO");
            returnSQL.AppendLine(item.ObjectText);
            returnSQL.AppendLine("GO");
            return returnSQL.ToString();
        }
       
        private string ScriptDropObject(
            DatabaseObject databaseObject
        )
        {
            return String.Format("DROP {0} IF EXISTS [{1}].[{2}];", GetObjectTypeName(databaseObject.DatabaseObjectType), 
                databaseObject.SchemaName, databaseObject.ObjectName);
        }

        private string GetObjectTypeName(
            DatabaseObjectType objectType
        )
        {
            switch(objectType)
            {
                case DatabaseObjectType.StoredProcedure:
                    return "PROCEDURE";
                    break;
                case DatabaseObjectType.View:
                    return "VIEW";
                    break;
                default:
                    throw new ApplicationException("GetObjectTypeName: Unhandled Type - " + objectType.ToString());
            }
        }
    }
}
