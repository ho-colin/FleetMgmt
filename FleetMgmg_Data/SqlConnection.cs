using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data {
    public static class SqlConnection {

        private static string connectionString = @"HIERKOMT-CONNECTIONSTRING";

        public static SqlConnection getConnection() {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            return sqlConnection;
        }

    }
}
