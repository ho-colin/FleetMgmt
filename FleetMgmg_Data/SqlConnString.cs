using FleetMgmg_Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data {
    public static class SqlConnString {

        private static string connectionString = @"HIERKOMT-CONNECTIONSTRING";

        public static SqlConnection getConnection() {
            try {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                return sqlConnection;
            } catch (Exception ex) {
                throw new ConnectionException("SqlConnectionClass : ConnectionException - Kon geen verbinding initialiseren!", ex);
            }
        }
    }
}
