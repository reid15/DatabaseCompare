using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCompare
{

    public enum DatabaseObjectType
    {
        NoObject,
        Table,
        StoredProcedure,
        View,
        UserDefinedFunction,
        Trigger,
        Index,
        ForeignKey,
        Default,
        Check,
        UserDefinedTableType,
        PrimaryKey
    }

    public struct DatabaseObject
    {
        public DatabaseObjectType DatabaseObjectType;
        public string SchemaName;
        public string ObjectName;
        public string ObjectText;
        public string ParentObjectName;

        public DatabaseObject(DatabaseObjectType databaseObjectType, string schemaName, string objectName, string objectText, string parentObjectName = "")
        {
            DatabaseObjectType = databaseObjectType;
            SchemaName = schemaName;
            ObjectName = objectName;
            ObjectText = objectText;
            ParentObjectName = parentObjectName;
        }
    }

    public struct DataCompareConfiguration
    {
        public string SourceServerName;
        public string SourceDatabaseName;
        public string TargetServerName;
        public string TargetDatabaseName;

        public DataCompareConfiguration(string sourceServerName, string sourceDatabaseName, string targetServerName, string targetDatabaseName)
        {
            SourceServerName = sourceServerName;
            SourceDatabaseName = sourceDatabaseName;
            TargetServerName = targetServerName;
            TargetDatabaseName = targetDatabaseName;
        }
    }

    public struct CompareSQL
    {
        public string DropSQL;
        public string CreateSQL;

        public CompareSQL(string dropSQL, string createSQL)
        {
            DropSQL = dropSQL;
            CreateSQL = createSQL;
        }
    }

}
