using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Smo;

namespace DatabaseCompare
{
    public class SqlSchema
    {
        public static Database GetDatabase(
            string serverName,
            string databaseName
        )
        {
            Server server = new Server(serverName);
            server.SetDefaultInitFields(typeof(Table), "IsSystemObject");
            server.SetDefaultInitFields(typeof(View), "IsSystemObject");
            server.SetDefaultInitFields(typeof(StoredProcedure), "IsSystemObject");
            server.SetDefaultInitFields(typeof(UserDefinedFunction), "IsSystemObject");

            if (server == null)
            {
                throw new ApplicationException("Server not found");
            }
            var database = server.Databases[databaseName];
            if (database == null)
            {
                throw new ApplicationException("Database not found");
            }
            return database;
        }

        private static List<DatabaseObject> GetDatabaseObjects(
            SchemaCollectionBase collection,
            DatabaseObjectType databaseObjectType
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach (ScriptSchemaObjectBase item in collection)
            {
                if (!(bool)item.Properties["IsSystemObject"].Value)
                {
                    returnList.Add(new DatabaseObject(databaseObjectType, item.Schema, item.Name, item.Properties["Text"].Value.ToString().Trim()));
                }
            }

            return returnList;
        }

        public static List<DatabaseObject> GetStoredProcedures(
            Database database
        )
        {
            return GetDatabaseObjects(database.StoredProcedures, DatabaseObjectType.StoredProcedure);
        }

        public static List<DatabaseObject> GetViews(
            Database database
        )
        {
            return GetDatabaseObjects(database.Views, DatabaseObjectType.View);
        }

        public static List<DatabaseObject> GetUserDefinedFunctions(
            Database database
        )
        {
            return GetDatabaseObjects(database.UserDefinedFunctions, DatabaseObjectType.UserDefinedFunction);
        }

        public static List<DatabaseObject> GetUserDefinedTableTypes(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();
            foreach (UserDefinedTableType item in database.UserDefinedTableTypes)
            {
                returnList.Add(new DatabaseObject(DatabaseObjectType.UserDefinedTableType, item.Schema, item.Name, ScriptObjects.ScriptUserDefinedTableType(item)));
            }
            return returnList;
        }

        public static List<DatabaseObject> GetTriggers(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach(Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    foreach (Trigger item in table.Triggers)
                    {
                        string schemaName = ((Table)item.Parent).Schema;
                        returnList.Add(new DatabaseObject(DatabaseObjectType.Trigger, schemaName, item.Name, item.TextHeader + item.TextBody));
                    }
                }
            }
            
            return returnList;
        }

        public static List<DatabaseObject> GetForeignKeys(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    foreach (ForeignKey item in table.ForeignKeys)
                    {
                        string schemaName = ((Table)item.Parent).Schema;
                        returnList.Add(new DatabaseObject(DatabaseObjectType.ForeignKey, schemaName, item.Name, ScriptObjects.ScriptForeignKey(item)));
                    }
                }
            }

            return returnList;
        }

        public static List<DatabaseObject> GetIndexes(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    foreach (Index item in table.Indexes)
                    {
                        string schemaName = ((Table)item.Parent).Schema;
                        returnList.Add(new DatabaseObject(DatabaseObjectType.Index, schemaName, item.Name, ScriptObjects.ScriptIndex(item)));
                    }
                }
            }

            return returnList;
        }

        public static List<DatabaseObject> GetDefaults(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    foreach (Column column in table.Columns)
                    {
                        if (column.DefaultConstraint != null)
                        {
                            string defaultScript = ScriptObjects.ScriptDefault(column.DefaultConstraint, table.Schema, table.Name, column.Name);
                            returnList.Add(new DatabaseObject(DatabaseObjectType.Default, table.Schema, column.DefaultConstraint.Name, defaultScript));
                        }
                    }
                }
            }

            return returnList;
        }

        public static List<DatabaseObject> GetChecks(
            Database database
        )
        {
            var returnList = new List<DatabaseObject>();

            foreach (Table table in database.Tables)
            {
                if (!table.IsSystemObject)
                {
                    foreach (Check item in table.Checks)
                    {
                        string schemaName = ((Table)item.Parent).Schema;
                        returnList.Add(new DatabaseObject(DatabaseObjectType.Check, schemaName, item.Name, ScriptObjects.ScriptCheck(item)));
                    }
                }
            }

            return returnList;
        }
    }
}
