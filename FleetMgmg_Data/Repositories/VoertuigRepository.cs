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
    public class VoertuigRepository : IVoertuigRepository {

        private SqlConnection getConnection() {
            SqlConnection conn = new SqlConnection(SqlConnString.connectionString);
            return conn;
        }

        public bool bestaatVoertuig(Voertuig voertuig) {
            SqlConnection conn = getConnection();

            StringBuilder query = new StringBuilder("SELECT * FROM voertuig WHERE " +
                "Chassisnummer=@chassisnummer AND " +
                "Model=@model "+
                "Merk=@merk "+
                "Nummerplaat=@nummerplaat AND " +
                "Brandstof=@brandstof AND " +
                "TypeVoertuig=@typevoertuig");

            bool and = true; //DEZE IS ALTIJD TRUE SINDS ER MANDATORY PROPERTIES ZIJN, STAAT HIER GEWOON VOOR DUIDELIJKHEID

            #region optioneleParametersMethodes
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                if (and) query.Append(" AND  Kleur=@kleur ");
                else { and = true; query.Append(" Kleur=@kleur "); }
            }

            if(voertuig.AantalDeuren > 0) {
                if (and) query.Append(" AND AantalDeuren=@aantaldeuren ");
                else { and = true; query.Append(" AantalDeuren=@aantaldeuren "); }
            }

            if(voertuig.Bestuurder != null) {
                if (and) query.Append(" AND BestuurderId=@bestuurderid ");
                else { and = true; query.Append("BestuurderId=@bestuurderid "); }
            }
            #endregion

            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                #region mandatoryParameters
                cmd.Parameters.AddWithValue("@merk", voertuig.Merk);
                cmd.Parameters.AddWithValue("@model", voertuig.Model);
                cmd.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                cmd.Parameters.AddWithValue("@nummerplaat", voertuig.Nummerplaat);
                cmd.Parameters.AddWithValue("@brandstof", voertuig.Brandstof);
                cmd.Parameters.AddWithValue("@typevoertuig", voertuig.TypeVoertuig);
                #endregion

                #region optioneleParametersValues
                if (query.ToString().Contains("@kleur")) cmd.Parameters.AddWithValue("@kleur", voertuig.Kleur);
                if (query.ToString().Contains("@aantaldeuren")) cmd.Parameters.AddWithValue("@aantaldeuren", voertuig.AantalDeuren);
                if (query.ToString().Contains("@bestuurderid")) cmd.Parameters.AddWithValue("@bestuurderid", voertuig.Bestuurder.Id);
                #endregion

                conn.Open();
                try {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    Voertuig v = new Voertuig(
                        (Brandstof) Enum.Parse(typeof(Brandstof), (string) reader["brandstof"]),
                        (string)reader["chassisnummer"],
                        (string)reader["kleur"],
                        (int)reader["AantalDeuren"],
                        (string)reader["merk"],
                        (string)reader["model"],
                        (string)reader["TypeVoertuig"],
                        (string)reader["nummerplaat"]);

                    return v != null;
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("Er heeft zich een fout voorgedaan!",ex);
                }
            }

        }

        public Voertuig geefVoertuig(Voertuig voertuig) {
            throw new NotImplementedException();
        }

        public IEnumerable<Voertuig> toonVoertuigen() {
            throw new NotImplementedException();
        }

        public void updateAantalDeuren(Voertuig voertuig, int aantal) {
            throw new NotImplementedException();
        }

        public void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder) {
            throw new NotImplementedException();
        }

        public void updateKleur(Voertuig voertuig, string kleur) {
            throw new NotImplementedException();
        }

        public void verwijderVoertuig(Voertuig voertuig) {
            throw new NotImplementedException();
        }

        public void voegVoertuigToe(Voertuig voertuig) {
            throw new NotImplementedException();
        }
    }
}
