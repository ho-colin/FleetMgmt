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
        public bool bestaatVoertuig(string chassisnummer) {
            SqlConnection connection = getConnection();
            string query = "SELECT Count(*) FROM Voertuig WHERE Chassisnummer=@Chassisnummer";
            using(SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@Chassisnummer"].Value = chassisnummer;
                    int exists = (int)command.ExecuteScalar();
                    if (exists > 0) return true; return false;
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: BestaatVoertuig(chassisnummer) - Er heeft een fout voorgedaan!",ex);
                } finally {
                    connection.Close();
                }
            }
        }
        public bool bestaatVoertuig(Voertuig voertuig) {
            SqlConnection conn = getConnection();

            StringBuilder query = new StringBuilder("SELECT Count(*) FROM Voertuig WHERE " +
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
                    throw new VoertuigRepositoryException("VoertuigRepository: BestaatVoertuig - Er heeft zich een fout voorgedaan!", ex);
                } finally {
                    conn.Close();
                }
            }
        }

        public Voertuig geefVoertuig(string chassisnummer) {
            Voertuig voertuig = null;
            SqlConnection connection = getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.model, v.TypeVoertuig, v.Brandstof, v.Kleur, v.AantalDeuren, v.BestuurderId " +
                "b.Naam, b.Voornaam, b.Adres, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Id, t.Pincode, t.GeldigheidDatum, t.Geblokkkeerd , tb.TankkaartBrandstof" +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.BestuurderId = b.Id " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId " +
                "WHERE Chassisnummer=@Chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                command.Parameters.Add(new SqlParameter("@chassisnummer", SqlDbType.NVarChar));
                command.Parameters["@chassisnummer"].Value = chassisnummer;
                command.CommandText = query;
                connection.Open();
                try {
                    SqlDataReader reader = command.ExecuteReader();
                    List<TankkaartBrandstof> brandstoffen = null;
                    while (reader.Read()) {
                        string chassisnummerDB = (string)reader["Chassisnummer"];
                        string merkDB = (string)reader["Merk"];
                        string modelDB = (string)reader["Model"];
                        string nummerplaatDB = (string)reader["Nummerplaat"];
                        string brandstofDB = (string)reader["Brandstof"];
                        string typeVoertuigDB = (string)reader["TypeVoertuig"];
                        string kleurDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Kleur"))) {
                            kleurDB = (string)reader["Kleur"];
                        }
                        int aantalDeurenDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("AantalDeuren"))) {
                            aantalDeurenDB = (int)reader["AantalDeuren"];
                        }
                        int bestuurderIdDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("BestuurderId"))) {
                            bestuurderIdDB = (int)reader["BestuurderId"];
                        }
                        string rijksregisternummerDB = (string)reader["Rijksregisternummer"];
                        string naamDB = (string)reader["Naam"];
                        string achternaamDB = (string)reader["Achternaam"];
                        string adresDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Adres"))) {
                            adresDB = (string)reader["Adres"];
                        }
                        DateTime geboortedatumDB = (DateTime)reader["Geboortedatum"];

                        string kaartnummerDB = ((int)reader["Id"]).ToString();
                        DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                        string pincodeDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) {
                            pincodeDB = (string)reader["Pincode"];
                        }
                        string tankkaartBrandstofDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("TankkaartBrandstof"))) {
                            tankkaartBrandstofDB = (string)reader["TankkaartBrandstof"];
                        }

                        Bestuurder bestuurder = new Bestuurder(rijksregisternummerDB, naamDB, achternaamDB, geboortedatumDB);
                        Tankkaart tankkaart = new Tankkaart(kaartnummerDB, geldigDatumDB, pincodeDB, bestuurder, brandstoffen);
                        brandstoffen.Add((TankkaartBrandstof)Enum.Parse(typeof(Enum), tankkaartBrandstofDB));
                        voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(Enum), brandstofDB), chassisnummerDB, kleurDB, aantalDeurenDB,
                            merkDB, modelDB, (TypeVoertuig)Enum.Parse(typeof(Enum), typeVoertuigDB), nummerplaatDB, bestuurder);return voertuig;
                    }
                    reader.Close();
                    return voertuig;
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository : GeefVoertuig", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public IEnumerable<Voertuig> toonVoertuigen(string merk, string model, string typeVoertuig, string brandstof,
            string kleur, int? aantalDeuren, bool strikt = true) {
            List<Voertuig> voertuigen = new List<Voertuig>();
            SqlConnection connection = getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.model, v.TypeVoertuig, v.Brandstof, v.Kleur, v.AantalDeuren, v.BestuurderId " +
                "b.Naam, b.Voornaam, b.Adres, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Id, t.Pincode, t.GeldigheidDatum, t.Geblokkkeerd , tb.TankkaartBrandstof" +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.BestuurderId = b.Id " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId";
            bool AND = false;
            bool WHERE = false;            
            if (!string.IsNullOrWhiteSpace(merk)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                AND = true;
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
                    List<TankkaartBrandstof> brandstoffen = null;                    
                    while (reader.Read()) {
                        string chassisnummerDB = (string)reader["Chassisnummer"];
                        string merkDB = (string)reader["Merk"];
                        string modelDB = (string)reader["Model"];
                        string nummerplaatDB = (string)reader["Nummerplaat"];
                        string brandstofDB = (string)reader["Brandstof"];
                        string typeVoertuigDB = (string)reader["TypeVoertuig"];
                        string kleurDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Kleur"))) {
                            kleurDB = (string)reader["Kleur"]; 
                        }
                        int aantalDeurenDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("AantalDeuren"))) {
                            aantalDeurenDB = (int)reader["AantalDeuren"];
                        }
                        int bestuurderIdDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("BestuurderId"))) {
                            bestuurderIdDB = (int)reader["BestuurderId"];
                        }
                        string rijksregisternummerDB = (string)reader["Rijksregisternummer"];
                        string naamDB = (string)reader["Naam"];
                        string achternaamDB = (string)reader["Achternaam"];
                        string adresDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Adres"))) {
                            adresDB = (string)reader["Adres"];
                        }
                        DateTime geboortedatumDB = (DateTime)reader["Geboortedatum"];

                        string kaartnummerDB = ((int)reader["Id"]).ToString();
                        DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                        string pincodeDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) {
                            pincodeDB = (string)reader["Pincode"];
                        }
                        string tankkaartBrandstofDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("TankkaartBrandstof"))) {
                            tankkaartBrandstofDB = (string)reader["TankkaartBrandstof"];
                        }
                        
                        Bestuurder bestuurder = new Bestuurder(rijksregisternummerDB, naamDB, achternaamDB, geboortedatumDB);
                        Tankkaart tankkaart = new Tankkaart(kaartnummerDB, geldigDatumDB, pincodeDB, bestuurder,brandstoffen);                       
                        brandstoffen.Add((TankkaartBrandstof)Enum.Parse(typeof(Enum), tankkaartBrandstofDB));                       
                        Voertuig voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(Enum), brandstofDB), chassisnummerDB, kleurDB, aantalDeurenDB,
                            merkDB, modelDB, (TypeVoertuig)Enum.Parse(typeof(Enum),typeVoertuigDB), nummerplaatDB,bestuurder);
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
        public void bewerkVoertuig(Voertuig voertuig, Bestuurder bestuurder) {
            Voertuig huidigVoertuig = this.geefVoertuig(voertuig.Chassisnummer);
            SqlConnection connection = getConnection();
            string queryV = "UPDATE Voertuig SET " +
                "Merk=@Merk,Model=@Model,Nummerplaat=@Nummerplaat,Brandstof=@Brandstof,TypeVoertuig=@TypeVoertuig,Kleur=@Kleur,AantalDeuren=@AantalDeuren,BestuurderId=@BestuurderId " +
                "WHERE Chassisnummer=@Chassisnummer";
            string queryB = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Id=@BestuurderId";
            using(SqlCommand commandV = connection.CreateCommand())
            using(SqlCommand commandB = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                commandV.Transaction = transaction;
                commandB.Transaction = transaction;
                try {
                    //waardes voor voertuigtabel
                    commandV.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    commandV.Parameters.AddWithValue("@Merk", voertuig.Merk);
                    commandV.Parameters.AddWithValue("@Model", voertuig.Model);
                    commandV.Parameters.AddWithValue("@Nummerplaat", voertuig.Nummerplaat);
                    commandV.Parameters.AddWithValue("@Brandstof", voertuig.Brandstof);
                    commandV.Parameters.AddWithValue("@TypeVoertuig", voertuig.TypeVoertuig.ToString());
                    if (voertuig.AantalDeuren.ToString() == null || voertuig.AantalDeuren <= 1) {
                        commandV.Parameters.AddWithValue("@AantalDeuren", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren);
                    }
                    if(voertuig.Kleur == null) {
                        commandV.Parameters.AddWithValue("@Kleur", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@Kleur", voertuig.Kleur);
                    }if(bestuurder == null) {
                        commandV.Parameters.AddWithValue("@BestuurderId", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@BestuurderId", bestuurder.Id);
                    }
                    commandV.CommandText = queryV;
                    //waardes voor bestuurdertabel                     
                    if(huidigVoertuig.Bestuurder != null && voertuig.Bestuurder == null) {
                        commandB.Parameters.AddWithValue("@BestuurderId", bestuurder.Id);
                        commandB.Parameters.AddWithValue("@Chassisnummer", DBNull.Value);
                    }else if(huidigVoertuig.Bestuurder == null && voertuig.Bestuurder != null) {
                        commandB.Parameters.AddWithValue("@BestuurderId", bestuurder.Id);
                        commandB.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    }
                    if(huidigVoertuig.Bestuurder != null && voertuig.Bestuurder != null) {
                        maakChassinummerLeeg(huidigVoertuig.Bestuurder, connection, transaction);
                        commandB.Parameters.AddWithValue("@BestuurderId", bestuurder.Id);
                        commandB.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    }
                    commandB.CommandText = queryB;
                    commandV.ExecuteNonQuery();
                    commandB.ExecuteNonQuery();
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new VoertuigRepositoryException("VoertuigRepository: bewerkVoertuig", ex);
                } finally {
                    connection.Close();
                }
            }
        }
        public void maakChassinummerLeeg(Bestuurder bestuurder, SqlConnection sqlConnection= null,SqlTransaction transaction = null) {
            SqlConnection connection;
            if (sqlConnection is null)
                connection = getConnection();
            else connection = sqlConnection;
            string query = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Id=@BestuurderId";
            using(SqlCommand command = connection.CreateCommand()) {
                if (transaction != null) command.Transaction = transaction;
                if (connection.State != ConnectionState.Open) connection.Open();
                try {                    
                    command.Parameters.AddWithValue("@BestuuderId", bestuurder.Id);
                    command.Parameters.AddWithValue("@Chassisnummer", DBNull.Value);
                    command.CommandText = query;
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: maakChassisnummerLeeg",ex);
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
            string query = "INSERT INTO Voertuig(Chassisnummer, Merk, Model, Nummerplaat, Brandstof, TypeVoertuig";
            //Kleur, AantalDeuren, BestuurderId is optioneel
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                query += ", Kleur ";
            }
            if(!string.IsNullOrWhiteSpace(voertuig.AantalDeuren.ToString())) {
                query += ", AantalDeuren";
            }
            if (!string.IsNullOrWhiteSpace(voertuig.Bestuurder.Id.ToString())) {
                query += ", BestuurderId";
            }
            query += ") VALUES (@Chassisnummer, @Merk, @Model, @Nummerplaat, @Brandstof, @TypeVoertuig";
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                query += ",@Kleur";
            }
            if (!string.IsNullOrWhiteSpace(voertuig.AantalDeuren.ToString())) {
                query += ",@AantalDeuren";
            }
            if (!string.IsNullOrWhiteSpace(voertuig.Bestuurder.Id.ToString())) {
                query += ",@BestuurderId";
            }
            query += ")";
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
                    command.Parameters["@brandstof"].Value = voertuig.Brandstof.ToString();
                    command.Parameters.Add(new SqlParameter("@typeVoertuig", SqlDbType.NVarChar));
                    command.Parameters["@typeVoertuig"].Value = voertuig.TypeVoertuig.ToString();
                    if (query.Contains("@Kleur")) {
                        command.Parameters.Add(new SqlParameter("@Kleur", SqlDbType.NVarChar));
                        command.Parameters["@Kleur"].Value = voertuig.Kleur;
                    }
                    if (query.Contains("@AantalDeuren")) {
                        command.Parameters.Add(new SqlParameter("@Aantaldeuren", SqlDbType.Int));
                        command.Parameters["@AantalDeuren"].Value = voertuig.AantalDeuren;
                    }
                    if (query.Contains("@BestuurderId")) {
                        command.Parameters.Add(new SqlParameter("@BestuurderId", SqlDbType.Int));
                        command.Parameters["@BestuurderId"].Value = voertuig.Bestuurder.Id;
                    }
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository: voegVoertuigToe - ", ex);
                } finally {
                    connection.Close();
                }
                //voertuig ook toevoegen aan bestuurder als er één opgegeven is
                if(voertuig.Bestuurder.Id.ToString() != null) {
                    string queryB = "INSERT INTO Bestuurder (VoertuigChassisnummer) VALUES (@Chassisnummer)";
                    using(SqlCommand commandB = connection.CreateCommand()) {
                        connection.Open();
                        try {
                            commandB.CommandText = queryB;
                            commandB.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                            commandB.Parameters["@Chassisnummer"].Value = voertuig.Chassisnummer;
                            commandB.ExecuteNonQuery();
                        } catch (Exception ex) {

                            throw new VoertuigRepositoryException("VoertuigRepository: VoegVoertuigToe - Voertuig aan bestuurder toewijzen", ex);
                        } finally {
                            connection.Close();
                        }
                    }
                }
            }
        }
    }
}
