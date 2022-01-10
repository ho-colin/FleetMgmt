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
    //COLIN MEERSCHMAN
    public class VoertuigRepository : IVoertuigRepository {

        public bool bestaatVoertuig(string chassisnummer) {
            SqlConnection connection = ConnectionClass.getConnection();
            string query = "SELECT Count(*) FROM Voertuig WHERE Chassisnummer=@Chassisnummer";
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@Chassisnummer", SqlDbType.NVarChar));
                    command.Parameters["@Chassisnummer"].Value = chassisnummer;
                    int exists = (int)command.ExecuteScalar();
                    if (exists > 0) return true; return false;
                }catch (Exception ex) {

                    throw new VoertuigRepositoryException("VoertuigRepository: BestaatVoertuig - Er heeft een fout voorgedaan!", ex);
                }finally {
                    connection.Close();
                }
            }
        }

        public bool bestaatVoertuigNmrPlaat(string nummerplaat) {
            SqlConnection connection = ConnectionClass.getConnection();
            string query = "SELECT Count(*) FROM Voertuig WHERE Nummerplaat=@nummerplaat";
            using (SqlCommand command = connection.CreateCommand()) {
                connection.Open();
                try {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@nummerplaat", nummerplaat);
                    int exists = (int)command.ExecuteScalar();
                    if (exists > 0) return true; return false;
                } catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository: BestaatVoertuig - Er heeft een fout voorgedaan!", ex);
                } finally {
                    connection.Close();
                }
            }
        }

        public Voertuig geefVoertuig(string chassisnummer) {
            Voertuig voertuig = null;
            Bestuurder bestuurder = null;
            Tankkaart tankkaart = null;
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.Model, v.TypeVoertuig, v.Brandstof VoertuigBrandstof, v.Kleur, v.AantalDeuren, v.Nummerplaat," +
                "b.Naam, b.Achternaam, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Pincode, t.GeldigDatum, t.Geblokkeerd , tb.Brandstof, tb.TankkaartId, tv.Rijbewijs, br.Categorie, br.Behaald " +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.Bestuurder = b.Rijksregisternummer " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId " +
                "LEFT JOIN TypeVoertuig tv ON tv.TypeVoertuig = v.TypeVoertuig " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer = br.Bestuurder " +
                "WHERE Chassisnummer=@chassisnummer";
            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@chassisnummer", chassisnummer);
                conn.Open();

                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        
                        while (reader.Read()) {

                            #region Voertuig

                            //Hier maken we plain voertuig aan.
                            if(voertuig == null) {
                                BrandstofEnum brandstofDB = (BrandstofEnum) Enum.Parse(typeof(BrandstofEnum), (string)reader["VoertuigBrandstof"]);
                                string chassisnummerDB = (string)reader["Chassisnummer"];
                                string kleurDB = reader.IsDBNull(reader.GetOrdinal("Kleur")) ? null : (string)reader["Kleur"];
                                string merkDB = (string)reader["Merk"];
                                string modelDB = (string)reader["Model"];
                                RijbewijsEnum rijbewijsDB = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);
                                TypeVoertuig typeVoertuigDB = new TypeVoertuig((string)reader["TypeVoertuig"], rijbewijsDB);
                                string nummerplaatDB = (string)reader["Nummerplaat"];

                                int? aantalDeurenDB = reader.IsDBNull(reader.GetOrdinal("AantalDeuren")) ? null : (int)reader["AantalDeuren"];

                                voertuig = new Voertuig(brandstofDB, chassisnummerDB, kleurDB, aantalDeurenDB, merkDB, modelDB, typeVoertuigDB, nummerplaatDB);
                            }
                            #endregion

                            #region Bestuurder
                            if(bestuurder == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("Rijksregisternummer"))) {
                                    string rijksregisterDB = (string)reader["Rijksregisternummer"];
                                    string achternaamDB = (string)reader["Achternaam"];
                                    string voornaamDB = (string)reader["Naam"];
                                    DateTime geboortedatumDB = (DateTime)reader["Geboortedatum"];

                                    bestuurder = new Bestuurder(rijksregisterDB, achternaamDB, voornaamDB, geboortedatumDB);
                                }
                            }
                            #endregion

                            #region Rijbewijs
                            if (!reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                bestuurder.voegRijbewijsToe(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]));
                            }
                            #endregion

                            #region Tankkaart
                            if (tankkaart == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {

                                    int tankkaartIdDB = (int)reader["TankkaartId"];
                                    DateTime geldigdatumDB = (DateTime)reader["GeldigDatum"];
                                    string pincodeDB = reader.IsDBNull(reader.GetOrdinal("Pincode")) ? null : (string)reader["Pincode"];

                                    tankkaart = new Tankkaart(tankkaartIdDB, geldigdatumDB, pincodeDB);
                                    tankkaart.zetGeblokkeerd((bool)reader["Geblokkeerd"]);
                                    tankkaart.updateInBezitVan(bestuurder);

                                    TankkaartBrandstof brandstofDB = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                    tankkaart.voegBrandstofToe(brandstofDB);
                                }
                            } else {
                                TankkaartBrandstof brandstofDB = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                tankkaart.voegBrandstofToe(brandstofDB);
                            }
                            #endregion

                        }
                        reader.Close();
                        return voertuig;
                    }
                }
                    
                catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository : GeefVoertuig", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        public IEnumerable<Voertuig> toonVoertuigen(string chassisnummer, string merk, string model, string typeVoertuig, string brandstof, string kleur, int? aantalDeuren, string bestuurderRijksregisternummer, string nummerplaat) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT v.Chassisnummer, v.Merk, v.Model, v.TypeVoertuig, v.Brandstof VoertuigBrandstof, v.Kleur, v.AantalDeuren, v.Nummerplaat, " +
                "b.Naam, b.Achternaam, b.Geboortedatum, b.Rijksregisternummer, " +
                "t.Pincode, t.GeldigDatum, t.Geblokkeerd , " +
                "tb.Brandstof, tb.TankkaartId, tv.Rijbewijs " +
                "FROM Voertuig v " +
                "LEFT JOIN Bestuurder b ON v.Bestuurder = b.Rijksregisternummer " +
                "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId " +
                "LEFT JOIN TypeVoertuig tv ON tv.TypeVoertuig = v.TypeVoertuig ";

            #region Optionele Parameters
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
            if (!string.IsNullOrWhiteSpace(bestuurderRijksregisternummer)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " b.Rijksregisternummer=@rijksregisternummer";
            }
            if (!string.IsNullOrWhiteSpace(nummerplaat)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                query += " Nummerplaat=@nummerplaat";
            }
            #endregion

            using (SqlCommand cmd = conn.CreateCommand()) {

                cmd.CommandText = query;

                #region Parameters waarden toekennen
                if (!string.IsNullOrWhiteSpace(merk)) { cmd.Parameters.AddWithValue("@merk", merk); }
                if (!string.IsNullOrWhiteSpace(model)) { cmd.Parameters.AddWithValue("@model", model); }
                if (!string.IsNullOrWhiteSpace(typeVoertuig)) { cmd.Parameters.AddWithValue("@typeVoertuig", typeVoertuig); }
                if (!string.IsNullOrWhiteSpace(brandstof)) { cmd.Parameters.AddWithValue("@brandstof", brandstof); }
                if (!string.IsNullOrWhiteSpace(kleur)) { cmd.Parameters.AddWithValue("@kleur", kleur); }
                if (aantalDeuren != null) {  cmd.Parameters.AddWithValue("@aantalDeuren", aantalDeuren); }
                if (!string.IsNullOrWhiteSpace(bestuurderRijksregisternummer)) { cmd.Parameters.AddWithValue("@rijksregisternummer", bestuurderRijksregisternummer); }
                if (!string.IsNullOrWhiteSpace(nummerplaat)) { cmd.Parameters.AddWithValue("@nummerplaat", nummerplaat); }
                #endregion
                conn.Open();

                List<Voertuig> voertuigen = new List<Voertuig>();
                Voertuig voertuig = null;
                try {
                    SqlDataReader reader = cmd.ExecuteReader();
                                       
                    Bestuurder bestuurder = null;
                    Tankkaart tankkaart = null;
                    string vorigChassisnummer = "";
                    while (reader.Read()) {

                        #region Voertuig

                        if(voertuig != null && vorigChassisnummer != (string)reader["Chassisnummer"]) {
                            voertuigen.Add(voertuig);
                            voertuig = null;
                            bestuurder = null;
                            tankkaart = null;
                        }

                        if(voertuig == null) {
                            vorigChassisnummer = (string)reader["Chassisnummer"];
                            string chassisnummerDB = (string)reader["Chassisnummer"];
                            string merkDB = (string)reader["Merk"];
                            string modelDB = (string)reader["Model"];
                            string typeVoertuigDB = (string)reader["TypeVoertuig"];
                            BrandstofEnum brandstofDB = (BrandstofEnum) Enum.Parse(typeof(BrandstofEnum), (string)reader["VoertuigBrandstof"]);
                            string kleurDB = null;
                            int? aantalDeurenDB = null;
                            string nummerplaatDB = (string)reader["Nummerplaat"];
                            RijbewijsEnum rijbewijsDB = (RijbewijsEnum) Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);

                            if (!reader.IsDBNull(reader.GetOrdinal("Kleur"))) { kleurDB = (string)reader["Kleur"]; }
                            if (!reader.IsDBNull(reader.GetOrdinal("AantalDeuren"))) { aantalDeurenDB = (int)reader["AantalDeuren"]; }

                            TypeVoertuig tv = new TypeVoertuig(typeVoertuigDB, rijbewijsDB);

                            voertuig = new Voertuig(brandstofDB, chassisnummerDB, kleurDB, aantalDeurenDB, merkDB, modelDB, tv, nummerplaatDB);
                        }
                        #endregion

                        #region Bestuurder
                        if(bestuurder == null) {
                            if (!reader.IsDBNull(reader.GetOrdinal("Rijksregisternummer"))) {
                                string rijksregisternmrDB = (string)reader["Rijksregisternummer"];
                                string voornaamDB = (string)reader["Naam"];
                                string achternaamDB = (string)reader["Achternaam"];
                                DateTime geboortedtmDB = (DateTime)reader["Geboortedatum"];
                                bestuurder = new Bestuurder(rijksregisternmrDB, achternaamDB, voornaamDB, geboortedtmDB);

                                //TODO: FIX VOEGRIJBEWIJSTOE SYSTEEM
                                bestuurder.voegRijbewijsToe(new Rijbewijs("B", geboortedtmDB));
                                bestuurder.updateVoertuig(voertuig);                               
                            }
                        }
                        #endregion

                        #region Tankkaart
                        if(tankkaart == null) {
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                                int kaartnummerDB = (int)reader["TankkaartId"];
                                DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                                string pincodeDB = reader.IsDBNull(reader.GetOrdinal("Pincode")) ? null : (string)reader["Pincode"];
                                tankkaart = new Tankkaart(kaartnummerDB, geldigDatumDB, pincodeDB);

                                tankkaart.zetGeblokkeerd((bool)reader["Geblokkeerd"]);
                                tankkaart.updateInBezitVan(bestuurder);

                                TankkaartBrandstof brandstofDB = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                tankkaart.voegBrandstofToe(brandstofDB);
                            }
                        } else {
                            TankkaartBrandstof brandstofDB = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                            tankkaart.voegBrandstofToe(brandstofDB);
                        }
                        #endregion
                    }
                    reader.Close();

                    if (!voertuigen.Contains(voertuig)) { voertuigen.Add(voertuig); }

                    return voertuigen;
                }catch (Exception ex) {
                    throw new VoertuigRepositoryException("VoertuigRepository : toonVoertuigen - "+ex.Message, ex);
                }finally { conn.Close(); }
            }

        }

        public void verwijderVoertuig(Voertuig voertuig) {
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                SqlTransaction transaction = null;
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    #region zetBestuurder NULL in eigen tabel en BestuurderTabel
                    if(voertuig.Bestuurder != null) {
                        string query1 = "UPDATE Bestuurder SET VoertuigChassisnummer=NULL WHERE VoertuigChassisnummer=@chassisnummer";
                        string query2 = "UPDATE Voertuig SET Bestuurder=NULL WHERE Chassisnummer=@chassisnummer";

                        using(SqlCommand cmd1 = new SqlCommand(query1, conn, transaction)) {
                            cmd1.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                            cmd1.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction)) {
                            cmd2.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region verwijderVoertuig
                    string query = "DELETE FROM Voertuig WHERE Chassisnummer=@chassisnummer";
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction)) {
                        cmd.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                        cmd.ExecuteNonQuery();
                    }
                    #endregion

                    transaction.Commit();
                } catch (Exception ex) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex2) {
                        throw new VoertuigRepositoryException("VoertuigRepository : verwijderVoertuig - Transaction rollback failed!!",ex2);
                    }
                    throw new VoertuigRepositoryException(ex.Message, ex);
                } finally { transaction.Dispose(); conn.Close(); }
            }
        }

        public Voertuig voegVoertuigToe(Voertuig voertuig) {
            #region Query
            string query = "INSERT INTO Voertuig(Chassisnummer, Merk, Model, Nummerplaat, Brandstof, TypeVoertuig";
            //Kleur, AantalDeuren, BestuurderId is optioneel
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) { query += ", Kleur "; }
            if (voertuig.AantalDeuren.HasValue) { query += ", AantalDeuren"; }
            if (voertuig.Bestuurder != null) { query += ", Bestuurder"; }
            query += ") VALUES (@Chassisnummer, @Merk, @Model, @Nummerplaat, @Brandstof, @TypeVoertuig";
            if (!string.IsNullOrWhiteSpace(voertuig.Kleur)) {
                query += ",@Kleur";
            }
            if (voertuig.AantalDeuren.HasValue) { query += ",@AantalDeuren"; }
            if (voertuig.Bestuurder != null) { query += ",@Bestuurder"; }
            query += ")";
            #endregion
            #region QueryB
            string queryB = "UPDATE Bestuurder SET VoertuigChassisnummer=@Chassisnummer WHERE Rijksregisternummer=@Rijksregisternummer";
            #endregion
            SqlTransaction transaction = null;
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                try {
                    
                    conn.Open();
                    transaction = conn.BeginTransaction();
                    #region Voertuig
                    using (SqlCommand cmd1 = new SqlCommand(query, conn, transaction)) {

                        cmd1.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                        cmd1.Parameters.AddWithValue("@merk", voertuig.Merk);
                        cmd1.Parameters.AddWithValue("@model", voertuig.Model);
                        cmd1.Parameters.AddWithValue("@nummerplaat", voertuig.Nummerplaat);
                        cmd1.Parameters.AddWithValue("@brandstof", voertuig.Brandstof.ToString());
                        cmd1.Parameters.AddWithValue("@typeVoertuig", voertuig.TypeVoertuig.Type);

                        if (query.Contains("@Kleur")) { cmd1.Parameters.AddWithValue("@Kleur", voertuig.Kleur); }
                        if (query.Contains("@AantalDeuren")) { cmd1.Parameters.AddWithValue("@AantalDeuren", voertuig.AantalDeuren); }
                        if (query.Contains("@Bestuurder")) { cmd1.Parameters.AddWithValue("@Bestuurder", voertuig.Bestuurder.Rijksregisternummer); }

                        cmd1.ExecuteNonQuery();
                    }
                    #endregion

                    #region Bestuurder
                    if(voertuig.Bestuurder != null) {
                        using (SqlCommand cmd2 = new SqlCommand(queryB, conn, transaction)) {

                            cmd2.Parameters.AddWithValue("@Chassisnummer", voertuig.Chassisnummer);
                            cmd2.Parameters.AddWithValue("@Rijksregisternummer", voertuig.Bestuurder.Rijksregisternummer);

                            cmd2.ExecuteNonQuery();
                        }
                    }
                    #endregion
                    transaction.Commit();
                    return voertuig;
                } catch (Exception ex) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex2) {
                        throw new VoertuigRepositoryException("VoertuigRepository : voegVoertuigToe - Transaction rollback failed!!",ex2);
                    }
                    throw new VoertuigRepositoryException("VoertuigRepository : "+ex.Message,ex);
                } finally { transaction.Dispose(); conn.Close(); }
            }
        }

        public void bewerkVoertuig(Voertuig voertuig) {
            Voertuig huidigVoertuig = this.geefVoertuig(voertuig.Chassisnummer);
            if (voertuig.Equals(huidigVoertuig)) throw new VoertuigRepositoryException("VoertuigRepository : bewerkVoertuig - Er is niks verandert!"); 
            string query1 = "UPDATE Voertuig SET Merk=@merk,Model=@model,Brandstof=@brandstof,TypeVoertuig=@typevoertuig,Kleur=@kleur,AantalDeuren=@aantaldeuren,Bestuurder=@bestuurder WHERE Chassisnummer=@chassisnummer";
            string query2 = "UPDATE Bestuurder SET VoertuigChassisnummer=NULL WHERE VoertuigChassisnummer=@chassisnummer";
            SqlTransaction transaction = null;
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    #region Voertuig
                    using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction)) {
                        cmd2.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                        cmd2.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd1 = new SqlCommand(query1, conn, transaction)) {
                        cmd1.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                        cmd1.Parameters.AddWithValue("@merk", voertuig.Merk);
                        cmd1.Parameters.AddWithValue("@model", voertuig.Model);
                        cmd1.Parameters.AddWithValue("@brandstof", voertuig.Brandstof.ToString());
                        cmd1.Parameters.AddWithValue("@typevoertuig", voertuig.TypeVoertuig.Type);
                        cmd1.Parameters.AddWithValue("@kleur", string.IsNullOrWhiteSpace(voertuig.Kleur) ? DBNull.Value : voertuig.Kleur);
                        cmd1.Parameters.AddWithValue("@aantaldeuren", voertuig.AantalDeuren.HasValue ? voertuig.AantalDeuren : DBNull.Value);
                        cmd1.Parameters.AddWithValue("@bestuurder", voertuig.Bestuurder == null ? DBNull.Value : voertuig.Bestuurder.Rijksregisternummer);

                        cmd1.ExecuteNonQuery();
                    }
                    #endregion

                    #region Bestuurder
                    if(voertuig.Bestuurder != null) {
                        string nieuwQuery = "UPDATE Bestuurder SET VoertuigChassisnummer=@chassisnummer WHERE Rijksregisternummer=@rijksregisternummer";
                        using (SqlCommand cmd1 = new SqlCommand(nieuwQuery, conn, transaction)) {
                            cmd1.Parameters.AddWithValue("@chassisnummer", voertuig.Chassisnummer);
                            cmd1.Parameters.AddWithValue("@rijksregisternummer", voertuig.Bestuurder.Rijksregisternummer);

                            cmd1.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    transaction.Commit();
                } catch (Exception ex) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex2) {
                        throw new VoertuigRepositoryException("VoertuigRepository : bewerkVoertuig - Transaction rollback failed!!!",ex2);
                    }
                    throw new VoertuigRepositoryException(ex.Message,ex);
                } finally { transaction.Dispose(); conn.Close(); }
            }
        }
    }
}