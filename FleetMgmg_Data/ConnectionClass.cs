using FleetMgmg_Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data {
    //COLIN MEERSCHMAN
    public static class ConnectionClass {

        public static SqlConnection getConnection() {
            try {
                SqlConnection sqlConnection = new SqlConnection(verkrijgConnectionString());
                return sqlConnection;
            } catch (Exception ex) {
                throw new ConnectionException("ConnectionClass : ConnectionException - Kon geen verbinding initialiseren!", ex);
            }
        }

        private static string verkrijgConnectionString() {
            var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fleetFolder = Path.Combine(documentPath, "FleetManagement");

            var connectionFile = Path.Combine(fleetFolder + "/connectionString.txt");

            if (!Directory.Exists(fleetFolder)) { Directory.CreateDirectory(fleetFolder); }
            if(!File.Exists(connectionFile)) {
                using(StreamWriter sw = File.CreateText(connectionFile)) {
                    sw.WriteLine(@"workstation id=fleetmgmt.mssql.somee.com;packet size=4096;user id=ddecaluwe_SQLLogin_1;pwd=q76z5jwcr9;data source=fleetmgmt.mssql.somee.com;persist security info=False;initial catalog=fleetmgmt");
                }
            }

            return File.ReadLines(connectionFile).First();
        }
    }
}
