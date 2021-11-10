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
            SqlConnection conn = ConnectionClass.getConnection();
            return conn;
        }

        public bool bestaatVoertuig(Voertuig voertuig) {
            SqlConnection conn = getConnection();

            StringBuilder query = new StringBuilder("SELECT Count(*) FROM Voertuig WHERE " +
                "Chassisnummer=@chassisnummer AND " +
                "Model=@model " +
                "Merk=@merk " +
                "Nummerplaat=@nummerplaat AND " +
                "Brandstof=@brandstof AND " +
                "TypeVoertuig=@typevoertuig");

            bool and = true; //DEZE IS ALTIJD TRUE SINDS ER MANDATORY PROPERTIES ZIJN, STAAT HIER GEWOON VOOR DUIDELIJKHEID

            #region optioneleParametersMethodes
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                if (and) query.Append(" AND  Kleur=@kleur ");
                else { and = true; query.Append(" Kleur=@kleur "); }
            }

            if (voertuig.AantalDeuren > 0) {
                if (and) query.Append(" AND AantalDeuren=@aantaldeuren ");
                else { and = true; query.Append(" AantalDeuren=@aantaldeuren "); }
            }

            if (voertuig.Bestuurder != null) {
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
                    throw new VoertuigRepositoryException("Er heeft zich een fout voorgedaan!", ex);
                } finally {
                    conn.Close();
                }
            }
        }

        //Gelieve te joinen met alle ander tabellen zodat we een volledig compleet object hebben adhv wat er ingevuld is
        public Voertuig geefVoertuig(string chassisnummer) {
            SqlConnection connection = getConnection();
            string query = "SELECT * FROM Voertuig WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                command.Parameters["@chassisnummer"].Value = chassisnummer;
                command.CommandText = query;
                connection.Open();
                try {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), "B"); //HIER NOG AANPASSEN NAAR READER
                    Voertuig voertuig = new Voertuig(
                        (BrandstofEnum)Enum.Parse(typeof(Enum), 
                        (string)reader["Brandstof"]),
                        (string)reader["Chassisnummer"], 
                        (string)reader["Kleur"], 
                        (int)reader["AantalDeuren"], 
                        (string)reader["Merk"],
                       (string)reader["Model"], 
                       new TypeVoertuig((string)reader["TypeVoertuig"], re), 
                       (string)reader["Nummerplaat"]);
                    return voertuig;
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository : GeefVoertuig", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public IEnumerable<(Voertuig,Bestuurder,Tankkaart)> toonVoertuigen(string chassisnummer, string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt = true) {
            List<(Voertuig, Bestuurder, Tankkaart)> voertuigen = new List<(Voertuig, Bestuurder, Tankkaart)>();
            SqlConnection connection = getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.model, v.TypeVoertuig, v.Brandstof, v.Kleur, v.AantalDeuren, " +
                "b.Naam, b.Voornaam, b.Adres, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Id, t.Pincode, t.GeldigheidDatum, t.Geblokkkeerd " +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.BestuurderId = b.Id " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id ";
            bool AND = false;
            bool WHERE = false;
            if (!string.IsNullOrWhiteSpace(chassisnummer)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                AND = true;
                if (strikt)
                    query += "Chassisnummer=@chassisnummer";
                else
                    query += " UPPER(Chassisnummer)=UPPER(@chassisnummer)";
            }
            if (!string.IsNullOrWhiteSpace(merk)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " Merk=@merk";
                else
                    query += " UPPER(Merk)=UPPER(@merk)";
            }
            if (!string.IsNullOrWhiteSpace(model)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " Model=@model";
                else
                    query += " UPPER(Model)=UPPER(@model)";
            }
            if (!string.IsNullOrWhiteSpace(typeVoertuig)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " TypeVoertuig=@typeVoertuig";
                else
                    query += " UPPER(TypeVoertuig)=UPPER(@typeVoertuig)";
            }
            if (!string.IsNullOrWhiteSpace(brandstof)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " Brandstof=@brandstof";
                else
                    query += " UPPER(Brandstof)=UPPER(@brandstof)";
            }
            if (!string.IsNullOrWhiteSpace(kleur)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += " Kleur=@kleur";
                else
                    query += " UPPER(Kleur)=UPPER(@kleur)";
            }
            if (aantalDeuren != null) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " AantalDeuren=@aantalDeuren";
            }

            using (SqlCommand command = connection.CreateCommand()) {
                if (!string.IsNullOrWhiteSpace(chassisnummer)) {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = chassisnummer;
                }
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
                        string chassisnummerDB = (string)reader["Chassisnummer"];
                        string merkDB = (string)reader["Merk"];
                        string modelDB = (string)reader["Model"];
                        string nummerplaatDB = (string)reader["Nummerplaat"];
                        string brandstofDB = (string)reader["Brandstof"];
                        TypeVoertuig typeVoertuigDB = new TypeVoertuig((string)reader["TypeVoertuig"], RijbewijsEnum.B); //HIER NOG FIXEN DAT ER EEN RIJBEWIJS ENUM WORDT INGELEZEN
                        string kleurDB = (string)reader["Kleur"];
                        int aantalDeurenDB = (int)reader["AantalDeuren"];

                        string rijksregisternummerDB = (string)reader["Rijksregisternummer"];
                        string naamDB = (string)reader["Naam"];
                        string achternaamDB = (string)reader["Achternaam"];
                        DateTime geboortedatumDB = (DateTime)reader["Geboortedatum"];

                        int kaartnummerDB = ((int)reader["Id"]);
                        DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                        string pincodeDB = (string)reader["Pincode"];

                        Bestuurder bestuurder = new Bestuurder(rijksregisternummerDB, naamDB, achternaamDB, geboortedatumDB);
                        Tankkaart tankkaart = new Tankkaart(kaartnummerDB, geldigDatumDB, pincodeDB);
                        Voertuig voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), brandstofDB), chassisnummerDB, kleurDB, aantalDeurenDB,
                            merkDB, modelDB, typeVoertuigDB, nummerplaatDB);
                        voertuigen.Add((voertuig,bestuurder, tankkaart));
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
            SqlConnection connection = getConnection();
            string query = "UPDATE voertuig SET AantalDeuren=@aantalDeuren WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = voertuig.Chassisnummer;
                    command.Parameters.Add(new SqlParameter("@aantalDeuren", SqlDbType.Int));
                    command.Parameters["@aantalDeuren"].Value = voertuig.AantalDeuren;
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: updateAantalDeuren", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public void updateBestuurder(Voertuig voertuig, Bestuurder bestuurder) {
            SqlConnection connection = getConnection();
            string query = "UPDATE Voertuig SET BestuuderId=@bestuuderId WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                try {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = voertuig.Chassisnummer;
                    command.Parameters.Add(new SqlParameter("@bestuurderId", SqlDbType.Int));
                    if (bestuurder == null)
                        command.Parameters["@bestuurderId"].Value = DBNull.Value;
                    else
                        command.Parameters["@bestuurderId"].Value = voertuig.Bestuurder.Id;
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: updateBestuurder", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public void updateKleur(Voertuig voertuig, string kleur) {
            SqlConnection connection = getConnection();
            string query = "UPDATE Voertuig SET Kleur=@kleur WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@chassisnummer"].Value = voertuig.Chassisnummer;
                    command.Parameters.Add(new SqlParameter("@kleur", SqlDbType.NVarChar));
                    command.Parameters["@kleur"].Value = voertuig.Kleur;
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: updateKleur", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public void verwijderVoertuig(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string query = "DELETE FROM Voertuig WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
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
            string query = "INSERT INTO Voertuig(Chassisnummer, Merk, Model, Nummerplaat, Brandstof, TypeVoertuig) " +
                "VALUES (@chassisnummer, @merk, @model, @nummerplaat, @brandstof, @typeVoertuig)";
            using (SqlCommand command = connection.CreateCommand()) {
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
