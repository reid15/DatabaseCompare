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
        UserDefinedTableType
    }

    public struct TableSQL
    {
        public string CreateTableSQL;
        public string ColumnSQL;
        public string DropSQL;

        public TableSQL(string createTableSQL, string columnSQL, string dropSQL)
        {
            CreateTableSQL = createTableSQL;
            ColumnSQL = columnSQL;
            DropSQL = dropSQL;
        }
    }

    public struct DatabaseObject
    {
        public DatabaseObjectType DatabaseObjectType;
        public string SchemaName;
        public string ObjectName;
        public string ObjectText;

        public DatabaseObject(DatabaseObjectType databaseObjectType, string schemaName, string objectName, string objectText)
        {
            DatabaseObjectType = databaseObjectType;
            SchemaName = schemaName;
            ObjectName = objectName;
            ObjectText = objectText;
        }
    }
}
