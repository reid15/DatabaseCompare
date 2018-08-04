using System;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Generic;
using System.Linq;

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

            List<CompareSQL> compareSqlList = new List<CompareSQL>();
            // Add objects in the order they should be created
            TableCompare tableCompare = new TableCompare();
            compareSqlList.Add(tableCompare.CompareTables(sourceDatabase.Tables, targetDatabase.Tables));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetUserDefinedTableTypes(sourceDatabase), SqlSchema.GetUserDefinedTableTypes(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetStoredProcedures(sourceDatabase), SqlSchema.GetStoredProcedures(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetViews(sourceDatabase), SqlSchema.GetViews(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetUserDefinedFunctions(sourceDatabase), SqlSchema.GetUserDefinedFunctions(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetTriggers(sourceDatabase), SqlSchema.GetTriggers(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetIndexes(sourceDatabase), SqlSchema.GetIndexes(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetPrimaryKeys(sourceDatabase), SqlSchema.GetPrimaryKeys(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetForeignKeys(sourceDatabase), SqlSchema.GetForeignKeys(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetDefaults(sourceDatabase), SqlSchema.GetDefaults(targetDatabase)));
            compareSqlList.Add(CompareDatabaseObjects(SqlSchema.GetChecks(sourceDatabase), SqlSchema.GetChecks(targetDatabase)));

            var dropScript = new StringBuilder();
            var createScript = new StringBuilder();
            foreach (CompareSQL item in compareSqlList)
            {
                if (item.CreateSQL.Trim().Length > 0)
                {
                    createScript.AppendLine(item.CreateSQL.Trim());
                }
            }
            compareSqlList.Reverse();
            foreach (CompareSQL item in compareSqlList)
            {
                if (item.DropSQL.Trim().Length > 0)
                {
                    dropScript.AppendLine(item.DropSQL.Trim());
                }
            }

            returnScript.AppendLine("-- Drop Objects");
            returnScript.AppendLine(dropScript.ToString());
            returnScript.AppendLine("-- Create Objects");
            returnScript.AppendLine(createScript.ToString());

            return returnScript.ToString();
        }

        private CompareSQL CompareDatabaseObjects(
            List<DatabaseObject> sourceObjects,
            List<DatabaseObject> targetObjects
        )
        {
            StringBuilder returnScript = new StringBuilder();
            StringBuilder dropScript = new StringBuilder();

            foreach (DatabaseObject targetItem in targetObjects)
            {
                var sourceItem = (sourceObjects.FirstOrDefault(n => n.SchemaName == targetItem.SchemaName && n.ObjectName == targetItem.ObjectName));
                if (sourceItem.DatabaseObjectType == DatabaseObjectType.NoObject)
                {
                    dropScript.AppendLine(ScriptDropObject(targetItem));
                }
            }

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

            return new CompareSQL(dropScript.ToString(), returnScript.ToString());
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
            if (databaseObject.DatabaseObjectType == DatabaseObjectType.PrimaryKey || databaseObject.DatabaseObjectType == DatabaseObjectType.ForeignKey)
            {
                return String.Format("ALTER TABLE [{0}].[{1}] DROP {2} IF EXISTS [{3}];", databaseObject.SchemaName, databaseObject.ParentObjectName,
                    GetObjectTypeName(databaseObject.DatabaseObjectType), databaseObject.ObjectName);
            }
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
                case DatabaseObjectType.View:
                    return "VIEW";
                case DatabaseObjectType.PrimaryKey:
                    return "CONSTRAINT";
                case DatabaseObjectType.ForeignKey:
                    return "CONSTRAINT";
                default:
                    throw new ApplicationException("GetObjectTypeName: Unhandled Type - " + objectType.ToString());
            }
        }
    }
}
