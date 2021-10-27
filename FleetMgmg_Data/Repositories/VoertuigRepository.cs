using FleetMgmg_Data.Exceptions;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Repositories {
    public class VoertuigRepository : IVoertuigRepository {

        private SqlConnection getConnection() {
            SqlConnection conn = new SqlConnection(ConnectionClass.connectionString);
            return conn;
        }

        public bool bestaatVoertuig(Voertuig voertuig) {
            SqlConnection conn = getConnection();

            StringBuilder query = new StringBuilder("SELECT Count(*) FROM voertuig WHERE " +
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
                    return (int)reader[0] > 0;
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("Er heeft zich een fout voorgedaan!",ex);
                } finally {
                    conn.Close();
                }
            }

        }

        public IEnumerable<Voertuig> toonVoertuigen(string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt=true) {
            List<Voertuig> voertuigen = new List<Voertuig>();
            SqlConnection connection = getConnection();
            string query = "SELECT * FROM voertuig WHERE ";
            bool AND = false;
            if (!string.IsNullOrWhiteSpace(merk)) {
                AND = true;
                if (strikt)
                    query += " merk=@merk";
                else
                    query += " UPPER(merk)=UPPER(@merk)";
            }
            if (!string.IsNullOrWhiteSpace(model)) {
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " model=@model";
                else
                    query += " UPPER(model)=UPPER(@model)";
            }
            if (!string.IsNullOrWhiteSpace(typeVoertuig)) {
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " typeVoertuig=@typeVoertuig";
                else
                    query += " UPPER(typeVoertuig)=UPPER(@typeVoertuig)";
            }
            if (!string.IsNullOrWhiteSpace(brandstof)) {
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " brandstof=@brandstof";
                else
                    query += " UPPER(brandstof)=UPPER(@brandstof)";
            }
            if (!string.IsNullOrWhiteSpace(kleur)) {
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " kleur=@kleur";
                else
                    query += " UPPER(kleur)=UPPER(@kleur)";
            }
            if (aantalDeuren != null) {
                if (AND) query += " AND "; else AND = true;
                query += " aantalDeuren=@aantalDeuren";
            }
            using(SqlCommand command = connection.CreateCommand()) {
                if (!string.IsNullOrWhiteSpace(merk)) {
                    command.Parameters.Add(new SqlParameter("@merk", SqlDbType.NVarChar));
                    command.Parameters["@merk"].Value = merk;
                }
                if (!string.IsNullOrWhiteSpace(model)) {
                    command.Parameters.Add(new SqlParameter("@model", SqlDbType.NVarChar));
                    command.Parameters["@model"].Value = model;
                }
                if (!string.IsNullOrWhiteSpace(typeVoertuig)) {
                    command.Parameters.Add(new SqlParameter("@typeVoertuig", SqlDbType.NVarChar));
                    command.Parameters["@typeVoertuig"].Value = typeVoertuig;
                }
                if (!string.IsNullOrWhiteSpace(brandstof)) {
                    command.Parameters.Add(new SqlParameter("@brandstof", SqlDbType.NVarChar));
                    command.Parameters["@brandstof"].Value = brandstof;
                }
                if (!string.IsNullOrWhiteSpace(kleur)) {
                    command.Parameters.Add(new SqlParameter("@kleur", SqlDbType.NVarChar));
                    command.Parameters["@kleur"].Value = kleur;
                }
                if (aantalDeuren != null) {
                    command.Parameters.Add(new SqlParameter("@aantalDeuren", SqlDbType.Int));
                    command.Parameters["@aantalDeuren"].Value = aantalDeuren;
                }
                command.CommandText = query;

                connection.Open();
                try {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) {
                        string chassisnummerDB = (string)reader["chassisnummer"];
                        string merkDB = (string)reader["merk"];
                        string modelDB = (string)reader["model"];
                        string nummerplaatDB = (string)reader["nummerplaat"];
                        string brandstofDB = (string)reader["brandstof"];
                        string typeVoertuigDB = (string)reader["typeVoertuig"];
                        string kleurDB = (string)reader["kleur"];
                        int aantalDeurenDB = (int)reader["aantalDeuren"];
                        Voertuig voertuig = new Voertuig((Brandstof)Enum.Parse(typeof(Enum), brandstofDB), chassisnummerDB, kleurDB, aantalDeurenDB,
                            merkDB, modelDB, typeVoertuigDB, nummerplaatDB);
                        voertuigen.Add(voertuig);
                    }
                    reader.Close();

                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository : toonVoertuigen", ex);
                } finally {
                    connection.Close();
                }
                return voertuigen;
            }

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
            SqlConnection connection = getConnection();
            string query = "DELETE FROM voertuig WHERE chassisnummer=@chassisnummer";
            using(SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = voertuig.Chassisnummer;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: verwijderVoertuig", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public void voegVoertuigToe(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO voertuig(chassisnummer, merk, model, nummerplaat, brandstof, typeVoertuig) " +
                "VALUES (@chassisnummer, @merk, @model, @nummerplaat, @brandstof, @typeVoertuig)";
            using(SqlCommand command = connection.CreateCommand()) {
                try {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = voertuig.Chassisnummer;
                    command.Parameters.Add(new SqlParameter("@merk", SqlDbType.NVarChar));
                    command.Parameters["@merk"].Value = voertuig.Merk;
                    command.Parameters.Add(new SqlParameter("@model", SqlDbType.NVarChar));
                    command.Parameters["@model"].Value = voertuig.Model;
                    command.Parameters.Add(new SqlParameter("@nummerplaat", SqlDbType.NVarChar));
                    command.Parameters["@nummerplaat"].Value = voertuig.Nummerplaat;
                    command.Parameters.Add(new SqlParameter("@brandstof", SqlDbType.NVarChar));
                    command.Parameters["@brandstof"].Value = voertuig.Brandstof;
                    command.Parameters.Add(new SqlParameter("@typeVoertuig", SqlDbType.NVarChar));
                    command.Parameters["@typeVoertuig"].Value = voertuig.TypeVoertuig;
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository: voegVoertuigToe", ex);
                } finally {
                    connection.Close();
                }
            }
        }
    }
}
