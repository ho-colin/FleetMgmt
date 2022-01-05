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
    //LOUIS GHEYSENS
    public class BestuurderRepository : IBestuurderRepository {

        //Check of bestuurder bestaat
        public bool bestaatBestuurder(string rijks) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT COUNT(*) FROM [Bestuurder] WHERE Rijksregisternummer = @Rijksregisternummer";
            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    bool result = false;
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new SqlParameter("@Rijksregisternummer", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters["@Rijksregisternummer"].Value = rijks;

                    Console.WriteLine();
                    result = (int)cmd.ExecuteScalar() == 1 ? true : false;
                    return result;
                }
                catch (Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: " +
                        "bestaatBestuurder - Er werd geen bestuurder gevonden!", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        //Bewerkt de bestuurder
        public void bewerkBestuurder(Bestuurder bestuurder) {
            Bestuurder huidigeBestuurder = this.selecteerBestuurder(bestuurder.Rijksregisternummer);
            if (huidigeBestuurder.Equals(bestuurder)) throw new BestuurderRepositoryException
                    ("BestuurderRepository: bewerkBestuurder - Er werden geen " +
                 "verschillen gevonden!");
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                SqlTransaction transaction = null;
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    #region Bestuurder
                    if(bestuurder.Voornaam != huidigeBestuurder.Voornaam || bestuurder.Achternaam != huidigeBestuurder.Achternaam) {
                        StringBuilder queryBuilder = new StringBuilder("UPDATE Bestuurder SET ");
                        bool komma = false;
                        if (huidigeBestuurder.Rijksregisternummer != bestuurder.Rijksregisternummer) {
                            queryBuilder.Append(" rijksregisterNummer=@rijksregisterNummer");
                            komma = true;
                        }

                        if (huidigeBestuurder.Achternaam != bestuurder.Achternaam) {
                            if (komma) { queryBuilder.Append(","); } else komma = true;
                            queryBuilder.Append(" achternaam=@achternaam ");
                        }

                        if (huidigeBestuurder.Voornaam != bestuurder.Voornaam) {
                            if (komma) { queryBuilder.Append(","); } else komma = true;
                            queryBuilder.Append(" naam=@naam ");
                        }
                        queryBuilder.Append(" WHERE rijksregisternummer=@rijksregisternummer");

                        using (SqlCommand cmd = new(queryBuilder.ToString(), conn, transaction)) {
                            if (queryBuilder.ToString().Contains("@rijksregisternummer")) {
                                cmd.Parameters.AddWithValue("@rijksregisternummer", bestuurder.Rijksregisternummer == null ? DBNull.Value : bestuurder.Rijksregisternummer);
                            }

                            if (queryBuilder.ToString().Contains("@naam")) {
                                cmd.Parameters.AddWithValue("@naam", bestuurder.Voornaam == null ? DBNull.Value : bestuurder.Voornaam);
                            }

                            if (queryBuilder.ToString().Contains("@achternaam")) {
                                cmd.Parameters.AddWithValue("@achternaam", bestuurder.Achternaam == null ? DBNull.Value : bestuurder.Achternaam);
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region Tankkaart
                    // Als beide ingevuld zijn
                    if((huidigeBestuurder.Tankkaart != null && bestuurder.Tankkaart != null)) {
                        if(huidigeBestuurder.Tankkaart != bestuurder.Tankkaart) {
                            string query0 = "UPDATE Bestuurder SET TankkaartId=NULL WHERE TankkaartId=@tankkaartid";
                            string query1 = "UPDATE Tankkaart SET Bestuurder=NULL WHERE TankkaartId=@tankkaartid";
                            string query2 = "UPDATE Bestuurder SET TankkaartId=@tankkaartid WHERE Rijksregisternummer=@rijksregisternummer";
                            string query3 = "UPDATE Tankkaart SET Bestuurder=@bestuurder WHERE TankkaartId=@tankkaartId";

                            using (SqlCommand cmd0 = new SqlCommand(query0, conn, transaction)) {
                                cmd0.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                                cmd0.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd1 = new SqlCommand(query1, conn, transaction)) {
                                cmd1.Parameters.AddWithValue("@tankkaartid",huidigeBestuurder.Tankkaart.KaartNummer);
                                cmd1.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction)) {
                                cmd2.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                                cmd2.Parameters.AddWithValue("@rijksregisternummer", bestuurder.Rijksregisternummer);
                                cmd2.ExecuteNonQuery();
                            }

                            using (SqlCommand cmd3 = new SqlCommand(query3, conn, transaction)) {
                                cmd3.Parameters.AddWithValue("@bestuurder", bestuurder.Rijksregisternummer);
                                cmd3.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                                cmd3.ExecuteNonQuery();
                            }
                        }
                        // Nieuwe bestuurder heeft tankkaart oude niet
                    }else if(bestuurder.Tankkaart != null && huidigeBestuurder.Tankkaart == null) {
                        string query0 = "UPDATE Bestuurder SET TankkaartId=NULL WHERE TankkaartId=@tankkaartid";
                        string query1 = "UPDATE Bestuurder SET TankkaartId=@tankkaartid WHERE Rijksregisternummer=@rijksregisternummer";
                        string query2 = "UPDATE Tankkaart SET Bestuurder=@bestuurder WHERE Id=@tankkaartId";

                        using (SqlCommand cmd0 = new SqlCommand(query0, conn, transaction)) {
                            cmd0.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                            cmd0.ExecuteNonQuery();
                        }
                        using (SqlCommand cmd1 = new SqlCommand(query1, conn, transaction)) {
                            cmd1.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                            cmd1.Parameters.AddWithValue("@rijksregisternummer", bestuurder.Rijksregisternummer);
                            cmd1.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction)) {
                            cmd2.Parameters.AddWithValue("@bestuurder", bestuurder.Rijksregisternummer);
                            cmd2.Parameters.AddWithValue("@tankkaartid", bestuurder.Tankkaart.KaartNummer);
                            cmd2.ExecuteNonQuery();
                        }


                        // Nieuwe bestuurder heeft geen tankkaart oude wel
                    } else if(bestuurder.Tankkaart == null && huidigeBestuurder.Tankkaart != null) {
                        string query1 = "UPDATE Bestuurder SET TankkaartId=NULL WHERE Rijksregisternummer=@rijksregisternummer";
                        string query2 = "UPDATE Tankkaart SET Bestuurder=NULL WHERE TankkaartId=@tankkaartId";

                        using (SqlCommand cmd1 = new SqlCommand(query1, conn, transaction)) {
                            cmd1.Parameters.AddWithValue("@rijksregisternummer", bestuurder.Rijksregisternummer);
                            cmd1.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd2 = new SqlCommand(query2, conn, transaction)) {
                            cmd2.Parameters.AddWithValue("@tankkaartid", huidigeBestuurder.Tankkaart.KaartNummer);
                            cmd2.ExecuteNonQuery();
                        }
                    }

                    #endregion
                    transaction.Commit();

                }
                catch (Exception) {
                        transaction.Rollback();
                }
                finally {
                    conn.Close();
                    transaction.Dispose();
                }
            }

        }

        //Selecteert de bestuurder
        public Bestuurder selecteerBestuurder(string rijks) {
            string query = "SELECT b.*, tb.Brandstof, tk.Id, tk.Pincode, "+
                "tk.GeldigDatum, tk.Geblokkeerd, tk.Bestuurder, br.Id BehaaldId, "+
                "br.Categorie, br.Behaald, v.Merk, v.Model, v.Nummerplaat, v.Brandstof "+
                "voertuigBrandstof, v.TypeVoertuig, v.Kleur, v.AantalDeuren, "+
                "tv.Rijbewijs FROM bestuurder b "+
                "LEFT JOIN TankkaartBrandstof tb ON b.TankkaartId = tb.TankkaartId "+
                "LEFT JOIN Tankkaart tk ON b.TankkaartId = tk.Id " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer = br.Bestuurder "+
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer = v.Chassisnummer "+
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig = tv.TypeVoertuig "+
                "WHERE b.Rijksregisternummer = @Rijksregisternummer";
            Bestuurder b = null;
            Tankkaart t = null;
            List<Rijbewijs> r = null;
            Voertuig v = null;
            SqlConnection conn = ConnectionClass.getConnection();
            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Rijksregisternummer", rijks);

                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {

                            #region Bestuurder
                            if (!reader.IsDBNull(reader.GetOrdinal("Rijksregisternummer"))) {
                                b = new Bestuurder((string)reader["Rijksregisternummer"],
                                    (string)reader["Achternaam"],
                                    (string)reader["Naam"],
                                    (DateTime)reader["Geboortedatum"]);
                                //TODO: FIX RIJBEWIJS SYSTEEM
                                b.voegRijbewijsToe(new Rijbewijs("B", DateTime.Today));
                            }
                            #endregion

                            #region Tankkaart
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                                if(t == null) {
                                    string pincode = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) { pincode = (string)reader["Pincode"]; }
                                    int kaartnummerDB = ((int)reader["Id"]);
                                    DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                                    bool geblokkeerdDB = (bool)reader["Geblokkeerd"];
                                    t = new Tankkaart(kaartnummerDB, geldigDatumDB, pincode);
                                    t.zetGeblokkeerd(geblokkeerdDB);
                                    b.updateTankkaart(t);
                                }

                                string tankkaartBrandstofDB = (string)reader["Brandstof"];
                                TankkaartBrandstof ingelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), tankkaartBrandstofDB);
                                t.voegBrandstofToe(ingelezenData);
                            }
                            #endregion

                            #region Rijbewijs
                            if (r == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                    if (r == null) { r = new List<Rijbewijs>(); }
                                    if (!r.Contains(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]))) {
                                        r.Add(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]));
                                    }
                                }
                            }
                            #endregion

                            #region Voertuig
                            if (v == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {

                                    string kleur = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("kleur"))) kleur = (string)reader["kleur"];
                                    RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);

                                    v = new Voertuig(
                                        (BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), (string)reader["voertuigBrandstof"]),
                                        (string)reader["VoertuigChassisnummer"],
                                        kleur,
                                        (int)reader["AantalDeuren"],
                                        (string)reader["merk"],
                                        (string)reader["model"],
                                        new TypeVoertuig((string)reader["TypeVoertuig"], re),
                                        (string)reader["Nummerplaat"]);
                                }
                            }
                            #endregion

                        }
                        reader.Close();

                        if (b != null && r != null) { b.rijbewijzen = r; }
                        if (v != null) { b.updateVoertuig(v); }


                        return b;
                    }
                }catch(Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: " +
                        "selecteerBestuurder: Bestuurder werd niet geselecteerd", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        //Verkrijg de bestuurders aan de hand van verschillende parameters
        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, string naam, string achternaam, DateTime? geboortedatum, int? tankkaartId, string rijbewijs) {
            List<Bestuurder> lijstbestuurder = new List<Bestuurder>();
            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder(" SELECT b.*, tb.Brandstof, tk.Id, tk.Pincode,"+
                "tk.GeldigDatum, tk.Geblokkeerd, tk.Bestuurder, br.Id BehaaldId, "+
                "br.Categorie, br.Behaald, v.Merk, v.Model, v.Nummerplaat, v.Brandstof "+
                "voertuigBrandstof, v.TypeVoertuig, v.Kleur, v.AantalDeuren, "+
                "tv.Rijbewijs FROM bestuurder b "+
                "LEFT JOIN TankkaartBrandstof tb ON b.TankkaartId = tb.TankkaartId "+
                "LEFT JOIN Tankkaart tk ON b.TankkaartId = tk.Id "+
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer = br.Bestuurder "+
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer = v.Chassisnummer "+
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig = tv.TypeVoertuig");

            #region Optionele Parameters
            bool where = false;
            bool and = false;

            if (rijksregisternummer != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Rijksregisternummer=@rijksregisternummer");
            }

            if (naam != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Naam=@naam");
            }

            if (achternaam != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Achternaam=@achternaam ");
            }
            if (geboortedatum.GetHashCode() != 0) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Geboortedatum=@geboortedatum");
            }
            if (tankkaartId.HasValue) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.TankkaartId=@tankkaartid");
            }
            if (!string.IsNullOrWhiteSpace(rijbewijs)) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" br.Categorie=@rijbewijs");
            }
            #endregion

            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();
                if (query.ToString().Contains("@rijksregisternummer")) cmd.Parameters.AddWithValue("@rijksregisternummer", rijksregisternummer);
                if (query.ToString().Contains("@naam")) cmd.Parameters.AddWithValue("@naam", naam);
                if (query.ToString().Contains("@achternaam")) cmd.Parameters.AddWithValue("@achternaam", achternaam);
                if (query.ToString().Contains("@geboortedatum")) cmd.Parameters.AddWithValue("@geboortedatum", geboortedatum.Value.ToString("yyyy-MM-dd"));
                if (query.ToString().Contains("@tankkaartid")) cmd.Parameters.AddWithValue("@tankkaartid", tankkaartId);
                if (query.ToString().Contains("@rijbewijs")) cmd.Parameters.AddWithValue("@rijbewijs", rijbewijs);
                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        string laatstGebruikteRijksregisternummer = null;
                        List<TankkaartBrandstof> brandstoffen = null;
                        Tankkaart dbTankkaart = null;
                        List<Rijbewijs> dbRijbewijzen = null;
                        Voertuig dbVoertuig = null;
                        Bestuurder dbBestuurder = null;
                        while (reader.Read()) {


                            #region Bestuurder
                            if (laatstGebruikteRijksregisternummer != (string)reader["Rijksregisternummer"]) {
                                if (dbBestuurder != null) {
                                    if (dbRijbewijzen != null && dbBestuurder != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x)); }
                                    lijstbestuurder.Add(dbBestuurder);
                                    dbBestuurder = null;
                                    dbRijbewijzen = null;
                                    dbTankkaart = null;
                                    dbVoertuig = null;
                                }
                                laatstGebruikteRijksregisternummer = (string)reader["Rijksregisternummer"];
                                dbBestuurder = new Bestuurder(
                                 (string)reader["Rijksregisternummer"],
                                 (string)reader["Achternaam"],
                                 (string)reader["Naam"],
                                 (DateTime)reader["Geboortedatum"]);
                                //TODO: FIX RIJBEWIJS SYSTEEM
                                dbBestuurder.voegRijbewijsToe(new Rijbewijs("B", DateTime.Today));
                            }
                            #endregion

                            #region Tankkaart
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                                if(dbTankkaart == null) {
                                    string pincode = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) { pincode = (string)reader["Pincode"]; }
                                    int kaartnummerDB = ((int)reader["Id"]);
                                    DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                                    bool geblokkeerdDB = (bool)reader["Geblokkeerd"];
                                    dbTankkaart = new(kaartnummerDB, geldigDatumDB, pincode, dbBestuurder, brandstoffen, geblokkeerdDB);
                                    dbTankkaart.updateInBezitVan(dbBestuurder);                                
                                }
                                TankkaartBrandstof brandstofDB = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                dbTankkaart.voegBrandstofToe(brandstofDB);
                            }
                            #endregion

                            #region Voertuig & TypeVoertuig
                            //Voertuig && TypeVoertuig
                            if (dbVoertuig == null && !reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {
                                BrandstofEnum voertuigBrandstof = (BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), (string)reader["voertuigBrandstof"]);
                                RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);
                                string kleur = reader.IsDBNull(reader.GetOrdinal("Kleur")) ? null : (string)reader["Kleur"];
                                dbVoertuig = new Voertuig(voertuigBrandstof, (string)reader["VoertuigChassisnummer"], kleur,
                                    (int)reader["AantalDeuren"], (string)reader["Merk"], (string)reader["Model"],
                                    new TypeVoertuig((string)reader["TypeVoertuig"], re), (string)reader["Nummerplaat"]);
                                dbBestuurder.updateVoertuig(dbVoertuig);

                            }
                        }
                        #endregion
                        reader.Close();
                        if (dbBestuurder != null && !lijstbestuurder.Contains(dbBestuurder)) { lijstbestuurder.Add(dbBestuurder); }
                        return lijstbestuurder;
                    }
                }catch(Exception ex) {
                    throw new BestuurderRepositoryException(ex.Message, ex);
                }
                finally {
                    conn.Close();
                }
            }
        }


        //Verijder een bestuurder
        public void verwijderBestuurder(Bestuurder bestuurder) {
            if (!bestaatBestuurder(bestuurder.Rijksregisternummer)) throw new 
                    BestuurderRepositoryException("BestuurderRepository: verwijderBestuurder - Bestuurder " +
                "bestaat niet!");
            string queryTankkaart = "UPDATE TANKKAART SET Bestuurder = NULL WHERE Id = @Id";
            string queryBestuurder = "DELETE FROM Bestuurder WHERE rijksregisternummer=@rijksregisternummer";
            SqlConnection conn = ConnectionClass.getConnection();
            using SqlCommand cmdBestuurder = new(queryBestuurder, conn);
            using SqlCommand cmdTankkaart = new(queryTankkaart, conn);
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();
            try {
                #region Tankkaart
                if (bestuurder.Tankkaart != null) {
                    cmdTankkaart.Transaction = transaction;
                    cmdTankkaart.Parameters.AddWithValue("@Id", bestuurder.Tankkaart.KaartNummer);
                    cmdTankkaart.ExecuteNonQuery();
                }

                #endregion

                #region Bestuurder
                cmdBestuurder.Transaction = transaction;
                cmdBestuurder.Parameters.AddWithValue("@rijksregisternummer", bestuurder.Rijksregisternummer);
                cmdBestuurder.ExecuteNonQuery();

                transaction.Commit();
                #endregion

            }
            catch (Exception ex) {
                transaction.Rollback();
                throw new BestuurderRepositoryException("BestuurdersRepository: verwijderBestuurder - Gefaald", ex);
            }
            finally {
                conn.Close();
            }

        }
        

        //Voegt een bestuurder toe
        public Bestuurder voegBestuurderToe(Bestuurder bestuurder) {
            SqlConnection conn = ConnectionClass.getConnection();
            string queryOne = "INSERT INTO Bestuurder(Naam, Achternaam, " +
                "Geboortedatum, Rijksregisternummer";
            string queryTwo="INSERT INTO BestuurderRijbewijs(Bestuurder, Categorie, Behaald)" +
                "OUTPUT INSERTED.Id VALUES(@Bestuurder," +
                "@Categorie, @Behaald";
            string queryThree = "UPDATE TANKKAART SET Bestuurder=@Rijksregisternummer WHERE Id=@TankkaartId";

            if (bestuurder.Tankkaart != null && !string.IsNullOrWhiteSpace(bestuurder.Tankkaart.KaartNummer.ToString())) {
                queryOne += ", TankkaartId ";
            }
            queryOne += ") " +
                "VALUES(@Naam, @Achternaam, @Geboortedatum, @Rijksregisternummer";

            if (bestuurder.Tankkaart != null && !string.IsNullOrWhiteSpace(bestuurder.Tankkaart.KaartNummer.ToString())) {
                    queryOne += ",@TankkaartId ";
                }
                queryOne += ")";
                using(SqlCommand cmdOne = conn.CreateCommand())
                using (SqlCommand cmdTwo = conn.CreateCommand())
                using(SqlCommand cmdThree = conn.CreateCommand()) {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                try {
                    #region Bestuurder
                    cmdOne.Transaction = trans;
                    cmdOne.Parameters.Add(new SqlParameter("@Rijksregisternummer", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                    cmdOne.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Naam"].Value = bestuurder.Voornaam;
                    cmdOne.Parameters.Add(new SqlParameter("@Achternaam", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Achternaam"].Value = bestuurder.Achternaam;
                    cmdOne.Parameters.Add(new SqlParameter("@Geboortedatum", SqlDbType.Date));
                    cmdOne.Parameters["@Geboortedatum"].Value = bestuurder.GeboorteDatum;
                    if (queryOne.Contains("@TankkaartId")) {
                        cmdOne.Parameters.Add(new SqlParameter("@TankkaartId", SqlDbType.Int));
                        cmdOne.Parameters["@TankkaartId"].Value = bestuurder.Tankkaart.KaartNummer;
                    }
                    cmdOne.CommandText = queryOne;
                    cmdOne.ExecuteNonQuery();

                    #endregion

                    #region Rijbewijs
                    if (bestuurder.rijbewijzen.Count > 0) {
                        Rijbewijs rijbewijs = null;
                        cmdTwo.Transaction = trans;
                        cmdTwo.CommandText = queryTwo;
                        foreach(var r in bestuurder.rijbewijzen) {
                            cmdTwo.Parameters.Add(new SqlParameter("@Bestuurder", SqlDbType.NVarChar));
                            cmdTwo.Parameters["Bestuurder"].Value = bestuurder.Rijksregisternummer;
                            cmdTwo.Parameters.Add(new SqlParameter("@Categorie", SqlDbType.NVarChar));
                            cmdTwo.Parameters["Categorie"].Value = rijbewijs.Categorie;
                            cmdTwo.Parameters.Add(new SqlParameter("@Behaald", SqlDbType.NVarChar));
                            cmdTwo.Parameters["Behaald"].Value = rijbewijs.BehaaldOp;
                            cmdTwo.ExecuteScalar();
                        }
                    }
                    #endregion

                    #region Tankkaart
                    if (bestuurder.Tankkaart != null) {
                        cmdThree.Transaction = trans;
                        cmdThree.CommandText = queryThree;
                        cmdThree.Parameters.Add(new SqlParameter("@TankkaartId", SqlDbType.Int));
                        cmdThree.Parameters["@TankkaartId"].Value = bestuurder.Tankkaart.KaartNummer;
                        cmdThree.Parameters.Add(new SqlParameter("@Rijksregisternummer", SqlDbType.NVarChar));
                        cmdThree.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                        cmdThree.ExecuteNonQuery();
                    }
                    #endregion Tankkaart
                    trans.Commit();
                    return bestuurder;
                }
                catch (Exception ex) {
                    trans.Rollback();
                }
                finally {
                    conn.Close();
                }
                return null;
                }
            }

        }
    }