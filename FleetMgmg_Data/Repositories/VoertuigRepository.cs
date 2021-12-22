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
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@Chassisnummer"].Value = chassisnummer;
                    int exists = (int)command.ExecuteScalar();
                    if (exists > 0) return true; return false;
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: BestaatVoertuig(chassisnummer) - Er heeft een fout voorgedaan!", ex);
                } finally {
                    connection.Close();
                }
            }
        }
        public bool bestaatVoertuig(Voertuig voertuig) {
            SqlConnection conn = getConnection();

            StringBuilder query = new StringBuilder("SELECT Count(*) FROM Voertuig WHERE " +
                "Model=@model AND " +
                "Merk=@merk AND " +
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
                if (and) query.Append(" AND Bestuurder=@bestuurderid ");
                else { and = true; query.Append("Bestuurder=@bestuurderid "); }
            }
            #endregion

            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                #region mandatoryParameters
                cmd.Parameters.AddWithValue("@merk", voertuig.Merk);
                cmd.Parameters.AddWithValue("@model", voertuig.Model);
                cmd.Parameters.AddWithValue("@nummerplaat", voertuig.Nummerplaat);
                cmd.Parameters.AddWithValue("@brandstof", voertuig.Brandstof);
                cmd.Parameters.AddWithValue("@typevoertuig", voertuig.TypeVoertuig.Type.ToString());
                #endregion

                #region optioneleParametersValues
                if (query.ToString().Contains("@kleur")) cmd.Parameters.AddWithValue("@kleur", voertuig.Kleur);
                if (query.ToString().Contains("@aantaldeuren")) cmd.Parameters.AddWithValue("@aantaldeuren", voertuig.AantalDeuren);
                if (query.ToString().Contains("@bestuurderid")) cmd.Parameters.AddWithValue("@bestuurderid", voertuig.Bestuurder.Rijksregisternummer);
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
            Bestuurder br = null;
            SqlConnection connection = getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.Model, v.TypeVoertuig, v.Brandstof, v.Kleur, v.AantalDeuren, v.Nummerplaat, v.Bestuurder," +
                "b.Naam, b.Achternaam, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Id, t.Pincode, t.GeldigDatum, t.Geblokkeerd , tb.Brandstof, tb.TankkaartId, tv.Rijbewijs " +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.Bestuurder = b.Rijksregisternummer " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId " +
                "LEFT JOIN TypeVoertuig tv ON tv.TypeVoertuig = v.TypeVoertuig " +
                "WHERE Chassisnummer=@Chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                command.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                command.Parameters["@Chassisnummer"].Value = chassisnummer;
                command.CommandText = query;
                connection.Open();
                try {
                    SqlDataReader reader = command.ExecuteReader();
                    List<TankkaartBrandstof> brandstoffen = null;
                    while (reader.Read()) {
                        //Voertuigen
                        string chassisnummerDB = (string)reader["Chassisnummer"];
                        string merkDB = (string)reader["Merk"];
                        string modelDB = (string)reader["Model"];
                        string nummerplaatDB = (string)reader["Nummerplaat"];
                        string brandstofDB = (string)reader["Brandstof"];
                        string typeVoertuigDB = (string)reader["TypeVoertuig"];
                        string rijbewijsDB = (string)reader["Rijbewijs"];
                        string kleurDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Kleur"))) {
                            kleurDB = (string)reader["Kleur"];
                        }
                        int aantalDeurenDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("AantalDeuren"))) {
                            aantalDeurenDB = (int)reader["AantalDeuren"];
                        }
                        int bestuurderIdDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                            bestuurderIdDB = (int)reader["Bestuurder"];
                        }
                        TypeVoertuig tv = new TypeVoertuig(typeVoertuigDB, ((RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), rijbewijsDB)));

                        voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), brandstofDB),
                            chassisnummerDB, kleurDB, aantalDeurenDB,
                        merkDB, modelDB, tv, nummerplaatDB, br);

                        //Bestuurder
                        if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                            br = new((string)reader["Rijksregisternummer"],
                                (string)reader["Naam"],
                                (string)reader["Achternaam"],
                                (DateTime)reader["Geboortedatum"]);
                        }

                        //Tankkaart
                        if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                            string pincode = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) {
                                pincode = (string)reader["Pincode"];
                            }
                            int kaartnummerDB = ((int)reader["Id"]);
                            DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                            string tankkaartBrandstofDB = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartBrandstof"))) {
                                tankkaartBrandstofDB = (string)reader["TankkaartBrandstof"];
                            }
                            bool geblokkeerdDB = (bool)reader["Geblokkeerd"];

                            Tankkaart tk = new(kaartnummerDB, geldigDatumDB, pincode, br, brandstoffen, geblokkeerdDB);
                            TankkaartBrandstof ingelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), tankkaartBrandstofDB);
                            if (!brandstoffen.Contains(ingelezenData)) {
                                brandstoffen.Add(ingelezenData);
                            }
                            br.updateTankkaart(tk);
                        }

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
            string kleur, int? aantalDeuren, string bestuurderId) {
            List<Voertuig> voertuigen = new List<Voertuig>();
            Bestuurder br = null;
            SqlConnection connection = getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.Model, v.TypeVoertuig, v.Brandstof, v.Kleur, v.AantalDeuren, v.Nummerplaat, v.Bestuurder," +
                "b.Naam, b.Achternaam, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Id, t.Pincode, t.GeldigDatum, t.Geblokkeerd , tb.Brandstof, tb.TankkaartId, tv.Rijbewijs " +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.Bestuurder = b.Rijksregisternummer " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId " +
                "LEFT JOIN TypeVoertuig tv ON tv.TypeVoertuig = v.TypeVoertuig ";
            bool AND = false;
            bool WHERE = false;
            if (!string.IsNullOrWhiteSpace(merk)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                AND = true;
                query += " v.Merk=@merk";
            }
            if (!string.IsNullOrWhiteSpace(model)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.Model=@model";
            }
            if (!string.IsNullOrWhiteSpace(typeVoertuig)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.TypeVoertuig=@typeVoertuig";
            }
            if (!string.IsNullOrWhiteSpace(brandstof)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.Brandstof=@brandstof";
            }
            if (!string.IsNullOrWhiteSpace(kleur)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.Kleur=@kleur";
            }
            if (aantalDeuren != null) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.AantalDeuren=@aantalDeuren";
            }
            if (!string.IsNullOrWhiteSpace(bestuurderId)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " v.Bestuurder=@bestuurder";
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
                if (bestuurderId != null) {
                    command.Parameters.Add(new SqlParameter("@bestuurder", SqlDbType.Int));
                    command.Parameters["@bestuurder"].Value = bestuurderId;
                }
                command.CommandText = query;

                connection.Open();
                try {
                    SqlDataReader reader = command.ExecuteReader();
                    List<TankkaartBrandstof> brandstoffen = null;
                    while (reader.Read()) {
                        //Voertuigen
                        string chassisnummerDB = (string)reader["Chassisnummer"];
                        string merkDB = (string)reader["Merk"];
                        string modelDB = (string)reader["Model"];
                        string nummerplaatDB = (string)reader["Nummerplaat"];
                        string brandstofDB = (string)reader["Brandstof"];
                        string typeVoertuigDB = (string)reader["TypeVoertuig"];
                        string rijbewijsDB = (string)reader["Rijbewijs"];
                        string kleurDB = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Kleur"))) {
                            kleurDB = (string)reader["Kleur"];
                        }
                        int aantalDeurenDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("AantalDeuren"))) {
                            aantalDeurenDB = (int)reader["AantalDeuren"];
                        }
                        int bestuurderIdDB = 0;
                        if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                            bestuurderIdDB = (int)reader["Bestuurder"];
                        }
                        TypeVoertuig tv = new TypeVoertuig(typeVoertuigDB, ((RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), rijbewijsDB)));

                        Voertuig voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), brandstofDB),
                            chassisnummerDB, kleurDB, aantalDeurenDB,
                        merkDB, modelDB, tv, nummerplaatDB, br);

                        //Bestuurder
                        if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                            br = new((string)reader["Rijksregisternummer"],
                                (string)reader["Naam"],
                                (string)reader["Achternaam"],
                                (DateTime)reader["Geboortedatum"]);
                        }

                        //Tankkaart
                        if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                            string pincode = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) {
                                pincode = (string)reader["Pincode"];
                            }
                            int kaartnummerDB = ((int)reader["Id"]);
                            DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                            string tankkaartBrandstofDB = null;
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartBrandstof"))) {
                                tankkaartBrandstofDB = (string)reader["TankkaartBrandstof"];
                            }
                            bool geblokkeerdDB = (bool)reader["Geblokkeerd"];

                            Tankkaart tk = new(kaartnummerDB, geldigDatumDB, pincode, br, brandstoffen, geblokkeerdDB);
                            TankkaartBrandstof ingelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), tankkaartBrandstofDB);
                            if (!brandstoffen.Contains(ingelezenData)) {
                                brandstoffen.Add(ingelezenData);
                            }
                            br.updateTankkaart(tk);
                        }
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
        public void bewerkVoertuig_GeenBestuurder(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string query = "UPDATE Voertuig SET " +
                "Merk=@Merk,Model=@Model,Nummerplaat=@Nummerplaat,Brandstof=@Brandstof,TypeVoertuig=@TypeVoertuig,Kleur=@Kleur,AantalDeuren=@AantalDeuren,Bestuurder=@Bestuurder " +
                "WHERE Chassisnummer=@Chassisnummer";
            using(SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    command.Parameters.AddWithValue("@Merk", voertuig.Merk);
                    command.Parameters.AddWithValue("@Model", voertuig.Model);
                    command.Parameters.AddWithValue("@Nummerplaat", voertuig.Nummerplaat);
                    command.Parameters.AddWithValue("@Brandstof", voertuig.Brandstof);
                    command.Parameters.AddWithValue("@TypeVoertuig", voertuig.TypeVoertuig.Type.ToString());
                    if (voertuig.AantalDeuren.ToString() == null || voertuig.AantalDeuren <= 1) {
                        command.Parameters.AddWithValue("@AantalDeuren", DBNull.Value);
                    } else {
                        command.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren);
                    }
                    if (voertuig.Kleur == null) {
                        command.Parameters.AddWithValue("@Kleur", DBNull.Value);
                    } else {
                        command.Parameters.AddWithValue("@Kleur", voertuig.Kleur);
                    }
                    command.Parameters.AddWithValue("@Bestuurder", DBNull.Value);
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: bewerkVoertuig", ex);
                } finally {
                    connection.Close();
                }
            }
        }
        public void bewerkVoertuig_BestuurderToevoegen(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string queryV = "UPDATE Voertuig SET " +
                "Merk=@Merk,Model=@Model,Nummerplaat=@Nummerplaat,Brandstof=@Brandstof,TypeVoertuig=@TypeVoertuig,Kleur=@Kleur,AantalDeuren=@AantalDeuren,Bestuurder=@BestuurderId " +
                "WHERE Chassisnummer=@Chassisnummer";
            string queryB = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Rijksregisternummer=@Rijksregisternummer";
            using (SqlCommand commandV = connection.CreateCommand())
            using (SqlCommand commandB = connection.CreateCommand()) {
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
                    commandV.Parameters.AddWithValue("@TypeVoertuig", voertuig.TypeVoertuig.Type.ToString());
                    if (voertuig.AantalDeuren.ToString() == null || voertuig.AantalDeuren <= 1) {
                        commandV.Parameters.AddWithValue("@AantalDeuren", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren);
                    }
                    if (voertuig.Kleur == null) {
                        commandV.Parameters.AddWithValue("@Kleur", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@Kleur", voertuig.Kleur);
                    }
                    commandV.Parameters.AddWithValue("@BestuurderId", voertuig.Bestuurder.Rijksregisternummer);
                    commandV.CommandText = queryV;
                    //waardes voor bestuurdertabel                     
                    commandB.Parameters.AddWithValue("@Rijksregisternummer", voertuig.Bestuurder.Rijksregisternummer);
                    commandB.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
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
        public void bewerkVoertuig_BestuurderVerwijderen(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string queryV = "UPDATE Voertuig SET " +
                "Merk=@Merk,Model=@Model,Nummerplaat=@Nummerplaat,Brandstof=@Brandstof,TypeVoertuig=@TypeVoertuig,Kleur=@Kleur,AantalDeuren=@AantalDeuren,Bestuurder=@BestuurderId " +
                "WHERE Chassisnummer=@Chassisnummer";
            string queryB = "UPDATE Bestuurder SET VoertuigChassisnummer=@LegeChassisnummer WHERE VoertuigChassisnummer=@Chassisnummer";
            using (SqlCommand commandV = connection.CreateCommand())
            using (SqlCommand commandB = connection.CreateCommand()) {
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
                    commandV.Parameters.AddWithValue("@TypeVoertuig", voertuig.TypeVoertuig.Type.ToString());
                    if (voertuig.AantalDeuren.ToString() == null || voertuig.AantalDeuren <= 1) {
                        commandV.Parameters.AddWithValue("@AantalDeuren", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren);
                    }
                    if (voertuig.Kleur == null) {
                        commandV.Parameters.AddWithValue("@Kleur", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@Kleur", voertuig.Kleur);
                    }
                    commandV.Parameters.AddWithValue("@BestuurderId", DBNull.Value);
                    commandV.CommandText = queryV;
                    //waardes voor bestuurdertabel
                    commandB.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    commandB.Parameters.AddWithValue("@LegeChassisnummer", DBNull.Value);
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
        public void bewerkVoertuig_BestuurderWisselen(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string queryV = "UPDATE Voertuig SET " +
                "Merk=@Merk,Model=@Model,Nummerplaat=@Nummerplaat,Brandstof=@Brandstof,TypeVoertuig=@TypeVoertuig,Kleur=@Kleur,AantalDeuren=@AantalDeuren,Bestuurder=@BestuurderId " +
                "WHERE Chassisnummer=@Chassisnummer";
            string queryBV = "UPDATE Bestuurder SET VoertuigChassisnummer=@LegeChassisnummer WHERE VoertuigChassisnummer=@Chassisnummer";
            string queryBN = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Rijksregisternummer=@Rijksregisternummer";
            using (SqlCommand commandV = connection.CreateCommand())
            using (SqlCommand commandBV = connection.CreateCommand())
            using (SqlCommand commandBN = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                commandV.Transaction = transaction;
                commandBV.Transaction = transaction;
                commandBN.Transaction = transaction;
                try {
                    //waardes voor voertuigtabel
                    commandV.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    commandV.Parameters.AddWithValue("@Merk", voertuig.Merk);
                    commandV.Parameters.AddWithValue("@Model", voertuig.Model);
                    commandV.Parameters.AddWithValue("@Nummerplaat", voertuig.Nummerplaat);
                    commandV.Parameters.AddWithValue("@Brandstof", voertuig.Brandstof);
                    commandV.Parameters.AddWithValue("@TypeVoertuig", voertuig.TypeVoertuig.Type.ToString());
                    if (voertuig.AantalDeuren.ToString() == null || voertuig.AantalDeuren <= 1) {
                        commandV.Parameters.AddWithValue("@AantalDeuren", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren);
                    }
                    if (voertuig.Kleur == null) {
                        commandV.Parameters.AddWithValue("@Kleur", DBNull.Value);
                    } else {
                        commandV.Parameters.AddWithValue("@Kleur", voertuig.Kleur);
                    }
                    commandV.Parameters.AddWithValue("@BestuurderId", voertuig.Bestuurder.Rijksregisternummer);
                    commandV.CommandText = queryV;
                    //waardes voor bestuurdertabel chassinummer verwijderen bij vorige bestuurder
                    commandBV.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    commandBV.Parameters.AddWithValue("@LegeChassisnummer", DBNull.Value);
                    commandBV.CommandText = queryBV;
                    //waardes voor bestuurdertabel chassisnummer plaatsen bij nieuwe bestuurder                     
                    commandBN.Parameters.AddWithValue("@Rijksregisternummer", voertuig.Bestuurder.Rijksregisternummer);
                    commandBN.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                    commandBN.CommandText = queryBN;
                    commandV.ExecuteNonQuery();
                    commandBV.ExecuteNonQuery();
                    commandBN.ExecuteNonQuery();
                    transaction.Commit();

                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new VoertuigRepositoryException("VoertuigRepository: bewerkVoertuig", ex);
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

        public Voertuig voegVoertuigToe(Voertuig voertuig) {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO Voertuig(Chassisnummer, Merk, Model, Nummerplaat, Brandstof, TypeVoertuig";
            string queryB = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Rijksregisternummer=@Rijksregisternummer"; 
            //Kleur, AantalDeuren, BestuurderId is optioneel
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                query += ", Kleur ";
            }
            if (!string.IsNullOrWhiteSpace(voertuig.AantalDeuren.ToString())) {
                query += ", AantalDeuren";
            }
            if (voertuig.Bestuurder != null && !string.IsNullOrWhiteSpace(voertuig.Bestuurder.Rijksregisternummer)) {
                query += ", Bestuurder";
            }
            query += ") VALUES (@Chassisnummer, @Merk, @Model, @Nummerplaat, @Brandstof, @TypeVoertuig";
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                query += ",@Kleur";
            }
            if (!string.IsNullOrWhiteSpace(voertuig.AantalDeuren.ToString())) {
                query += ",@AantalDeuren";
            }
            if (voertuig.Bestuurder != null && !string.IsNullOrWhiteSpace(voertuig.Bestuurder.Rijksregisternummer)) {
                query += ",@Bestuurder";
            }
            query += ")";
            using (SqlCommand command = connection.CreateCommand())
            using (SqlCommand commandB = connection.CreateCommand()) {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try {
                    command.Transaction = transaction;
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
                    command.Parameters["@typeVoertuig"].Value = voertuig.TypeVoertuig.Type.ToString();
                    if (query.Contains("@Kleur")) {
                        command.Parameters.Add(new SqlParameter("@Kleur", SqlDbType.NVarChar));
                        command.Parameters["@Kleur"].Value = voertuig.Kleur;
                    }
                    if (query.Contains("@AantalDeuren")) {
                        command.Parameters.Add(new SqlParameter("@Aantaldeuren", SqlDbType.Int));
                        command.Parameters["@AantalDeuren"].Value = voertuig.AantalDeuren;
                    }
                    if (query.Contains("@Bestuurder")) {
                        command.Parameters.Add(new SqlParameter("@Bestuurder", SqlDbType.Int));
                        command.Parameters["@Bestuurder"].Value = voertuig.Bestuurder.Rijksregisternummer;
                    }
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                    //voertuig ook toevoegen aan bestuurder als er één opgegeven is
                    if (voertuig.Bestuurder != null) {
                        commandB.Transaction = transaction;
                        commandB.CommandText = queryB;
                        commandB.Parameters.Add(new SqlParameter("@Rijksregisternummer", SqlDbType.Int));
                        commandB.Parameters["@Rijksregisternummer"].Value = voertuig.Bestuurder.Rijksregisternummer;
                        commandB.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                        commandB.Parameters["@Chassisnummer"].Value = voertuig.Chassisnummer;
                        commandB.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return voertuig;
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new VoertuigRepositoryException("VoertuigRepository: voegVoertuigToe - ", ex);
                } finally {
                    connection.Close();
                }
            }
        }
    }
}