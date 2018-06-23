using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCompare
{
    public class DataAccess
    {
        public static DataTable GetDataTable(
            string serverName,
            string databaseName,
            string sql
        )
        {
            string connectionString = GetConnectionString(serverName, databaseName);
            var returnDataset = new DataSet();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;

                    var dataAdapter = new SqlDataAdapter(command);
                    dataAdapter.Fill(returnDataset);
                }
            }
            return returnDataset.Tables[0];
        }

        private static string GetConnectionString(
            string serverName,
            string databaseName
       )
       {
            string connectionString = "server=" + serverName + ";database=" + databaseName + ";";
            connectionString += "integrated security=sspi;";
            return connectionString;
        }
    }
}
