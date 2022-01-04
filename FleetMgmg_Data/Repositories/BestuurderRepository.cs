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

                    #region Rijbewijs
                    if (huidigeBestuurder.rijbewijzen != bestuurder.rijbewijzen) {
                        DataTable table = new DataTable();
                        table.TableName = "BestuurderRijbewijs";

                        table.Columns.Add("Id", typeof(int));
                        table.Columns.Add("Bestuurder", typeof(string));


                        table.Columns.Add("Categorie", typeof(string));
                        table.Columns.Add("Behaald", typeof(DateTime));

                        foreach (Rijbewijs rijbewijs in huidigeBestuurder.rijbewijzen) {
                            var row = table.NewRow();
                            row["Id"] = rijbewijs.ToString();
                            row["Bestuurder"] = bestuurder.Rijksregisternummer;
                            row["Categorie"] = rijbewijs.Categorie;
                            row["Behaald"] = rijbewijs.BehaaldOp;
                            table.Rows.Add(row);
                        }
                        if (huidigeBestuurder.rijbewijzen.Count > 0) {
                            string tkDelQuery = "DELETE FROM BestuurderRijbewijs WHERE Id=@Id";
                            using (SqlCommand deleteCmd = new SqlCommand(tkDelQuery, conn, transaction)) {
                                deleteCmd.Parameters.AddWithValue("@Id", bestuurder.Rijksregisternummer);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction)) {
                            sqlBulkCopy.DestinationTableName = table.TableName;
                            sqlBulkCopy.WriteToServer(table);

                        }
                    }
                    #endregion

                    #region Bestuurder
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
                        queryBuilder.Append(" naam=@naam");
                    }

                    if (huidigeBestuurder.GeboorteDatum != bestuurder.GeboorteDatum) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" geboorteDatum=@geboorteDatum ");
                    }

                    if (huidigeBestuurder.Tankkaart != bestuurder.Tankkaart) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" Tankkaaart=@TankkaartId ");
                    }

                    if (huidigeBestuurder.Voertuig != bestuurder.Voertuig) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" Voertuig=@VoertuigChassisnummer");
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

                        if (queryBuilder.ToString().Contains("@geboorteDatum")) {
                            cmd.Parameters.AddWithValue("@geboorteDatum", bestuurder.GeboorteDatum.ToString("dd-MM-yyyy"));
                        }

                        if (queryBuilder.ToString().Contains("@TankkaartId")) {
                            cmd.Parameters.AddWithValue("@TankkaartId", bestuurder.Tankkaart.KaartNummer.Equals(0) ? DBNull.Value : bestuurder.Tankkaart.KaartNummer);
                        }

                        if (queryBuilder.ToString().Contains("@VoertuigChassisnummer")) {
                            cmd.Parameters.AddWithValue("@VoertuigChassisnummer", bestuurder.Voertuig.Chassisnummer.Equals(0) ? DBNull.Value : bestuurder.Voertuig.Chassisnummer);
                        }

                        cmd.ExecuteNonQuery();
                    }
                    #endregion

                    #region Tankkaart
                    if((huidigeBestuurder.Tankkaart != null && bestuurder.Tankkaart != null)) {
                        if(huidigeBestuurder.Tankkaart != bestuurder.Tankkaart) {
                            string query = "UPDATE Tankkaart SET Bestuurder WHERE ";
                        }
                    }

                    #endregion
                    transaction.Commit();

                }
                catch (Exception ex) {
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
            List<TankkaartBrandstof> tankkaartBrandstof = null;
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
                                b = new((string)reader["Rijksregisternummer"],
                                    (string)reader["Naam"],
                                    (string)reader["Achternaam"],
                                    (DateTime)reader["Geboortedatum"]);
                            }
                            #endregion

                            #region Tankkaart
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

                                Tankkaart tk = new(kaartnummerDB, geldigDatumDB, pincode, b, tankkaartBrandstof, geblokkeerdDB);
                                TankkaartBrandstof ingelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), tankkaartBrandstofDB);
                                if (!tankkaartBrandstof.Contains(ingelezenData)) {
                                    tankkaartBrandstof.Add(ingelezenData);
                                }
                                b.updateTankkaart(tk);
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
        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, string naam, string achternaam, DateTime? geboortedatum) {
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
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig = tv.TypeVoertuig ");

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
            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();
                if (query.ToString().Contains("@rijksregisternummer")) cmd.Parameters.AddWithValue
                        ("@rijksregisternummer", rijksregisternummer);
                if (query.ToString().Contains("@naam")) cmd.Parameters.AddWithValue("@naam", naam);
                if (query.ToString().Contains("@achternaam")) cmd.Parameters.AddWithValue("@achternaam", achternaam);
                if (query.ToString().Contains("@geboortedatum")) cmd.Parameters.AddWithValue
                        ("@geboortedatum", geboortedatum.Value.ToString("yyyy-MM-dd"));
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
                            if (reader.IsDBNull(reader.GetOrdinal("Rijksregisternummer")))throw new BestuurderRepositoryException("BestuurderRepository: toonBestuurders - Geen bestuurder gevonden!");
                            if (laatstGebruikteRijksregisternummer != (string)reader["Rijksregisternummer"]) {
                                if (dbBestuurder != null) {
                                    if (dbRijbewijzen != null && dbBestuurder != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x)); }
                                    //if (dbVoertuig != null && dbBestuurder != null) { dbBestuurder.updateVoertuig(dbVoertuig); }
                                    //if (dbTankkaart != null && dbBestuurder != null) { dbBestuurder.updateTankkaart(dbTankkaart); }
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
                                string pincode = null;
                                if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) {
                                    pincode = (string)reader["Pincode"];
                                }
                                int kaartnummerDB = ((int)reader["Id"]);
                                DateTime geldigDatumDB = (DateTime)reader["GeldigDatum"];
                                string tankkaartBrandstofDB = null;
                                if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {
                                    tankkaartBrandstofDB = (string)reader["Brandstof"];
                                }
                                bool geblokkeerdDB = (bool)reader["Geblokkeerd"];

                                Tankkaart tk = new(kaartnummerDB, geldigDatumDB, pincode, dbBestuurder, brandstoffen, geblokkeerdDB);
                                TankkaartBrandstof ingelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), tankkaartBrandstofDB);
                                dbBestuurder.updateTankkaart(tk);
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