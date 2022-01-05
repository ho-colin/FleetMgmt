using FleetMgmg_Data.Exceptions;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Repositories {
    public class RijbewijsRepository : IRijbewijsRepository {
        public bool heeftRijbewijs(RijbewijsEnum r, Bestuurder b) {
            string query = "SELECT Count(*) FROM BestuurderRijbewijs WHERE Bestuurder=@bestuurder AND Categorie=@categorie";
            using(SqlConnection conn = ConnectionClass.getConnection()) {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("@bestuurder", b.Rijksregisternummer);
                    cmd.Parameters.AddWithValue("@categorie", r.ToString());

                    try {
                        int aantal = (int)cmd.ExecuteScalar();
                        if (aantal > 0) return true; return false;
                    } catch (Exception ex) {
                        throw new RijbewijsRepositoryException(ex.Message,ex);
                    } finally { conn.Close(); }
                }
            }
        }

        public List<Rijbewijs> toonRijbewijzen(Bestuurder b) {
            throw new NotImplementedException();
        }

        public void verwijderRijbewijs(RijbewijsEnum r, Bestuurder b) {
            throw new NotImplementedException();
        }

        public void voegRijbewijsToe(Rijbewijs r, Bestuurder b) {
            throw new NotImplementedException();
        }
    }
}
