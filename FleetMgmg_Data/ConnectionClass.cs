using FleetMgmg_Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data {
    public static class ConnectionClass {

        private static string connectionString = @"Data Source=DESKTOP-P24EBNO\SQLEXPRESS;Initial Catalog=FleetMgmt;Integrated Security=True";

        public static SqlConnection getConnection() {
            try {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                return sqlConnection;
            } catch (Exception ex) {
                throw new ConnectionException("ConnectionClass : ConnectionException - Kon geen verbinding initialiseren!", ex);
            }
        }
    }
}
