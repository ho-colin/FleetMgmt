using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FleetMgmg_Data.Exceptions;

namespace FleetMgmg_Data {
    public static class ConnectionClass  {

        public static string connectionString = @"HIERKOMT-CONNECTIONSTRING";

        public static SqlConnection getConnection() {
            try {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                return sqlConnection;
            }
            catch (Exception ex) {
                throw new ConnectionException("ConnectionClass : ConnectionException - Kon geen verbinding initialiseren!", ex);
            }
        }
    }
}
