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
    public class BestuurderRepository : IBestuurderRepository {


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

        public void bewerkBestuurder(Bestuurder bestuurder) {
            Bestuurder huidigieBestuurder = this.selecteerBestuurder(bestuurder.Rijksregisternummer);
            if (huidigieBestuurder.Equals(bestuurder)) throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - Er werden geen " +
                 "verschillen gevonden!");
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                SqlTransaction transaction = null;
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    if (huidigieBestuurder.rijbewijzen != bestuurder.rijbewijzen) {
                        DataTable table = new DataTable();
                        table.TableName = "BestuurderRijbewijs";

                        table.Columns.Add("Id", typeof(int));
                        table.Columns.Add("BestuurderId", typeof(int));


                        table.Columns.Add("Categorie", typeof(string));
                        table.Columns.Add("Behaald", typeof(DateTime));

                        foreach (Rijbewijs rijbewijs in huidigieBestuurder.rijbewijzen) {
                            var row = table.NewRow();
                            row["Id"] = rijbewijs.ToString();
                            row["BestuurderId"] = bestuurder.Rijksregisternummer;
                            row["Categorie"] = rijbewijs.Categorie;
                            row["Behaald"] = rijbewijs.BehaaldOp;
                            table.Rows.Add(row);
                        }
                        if (huidigieBestuurder.rijbewijzen != null) {
                            string tkDelQuery = "DELETE FROM BestuurderRijbewijs WHERE Id=@id";
                            using (SqlCommand deleteCmd = new SqlCommand(tkDelQuery, conn, transaction)) {
                                deleteCmd.Parameters.AddWithValue("@id", bestuurder.Rijksregisternummer);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction)) {
                            sqlBulkCopy.DestinationTableName = table.TableName;
                            sqlBulkCopy.WriteToServer(table);

                        }
                    }
                    StringBuilder queryBuilder = new StringBuilder("UPDATE Bestuurder SET ");
                    bool komma = false;
                    if (huidigieBestuurder.Rijksregisternummer != bestuurder.Rijksregisternummer) {
                        queryBuilder.Append(" RijksregisterNummer=@rijksregisterNummer");
                        komma = true;
                    }

                    if (huidigieBestuurder.Naam != bestuurder.Naam) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" Naam=@naam ");
                    }

                    if (huidigieBestuurder.Voornaam != bestuurder.Voornaam) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" voornaam=@voornaam");
                    }

                    if (huidigieBestuurder.GeboorteDatum != bestuurder.GeboorteDatum) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" GeboorteDatum=@geboorteDatum ");
                    }

                    if (huidigieBestuurder.Tankkaart != bestuurder.Tankkaart) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" Tankkaaart=@TankkaartId ");
                    }

                    if (huidigieBestuurder.Voertuig != bestuurder.Voertuig) {
                        if (komma) { queryBuilder.Append(","); } else komma = true;
                        queryBuilder.Append(" Voertuig=@VoertuigChassisnummer");
                    }


                    queryBuilder.Append(" WHERE Id=@id");

                    using (SqlCommand cmd = new(queryBuilder.ToString(), conn, transaction)) {
                        if (queryBuilder.ToString().Contains("@rijksregisterNummer")) {
                            cmd.Parameters.AddWithValue("@rijksregisterNummer", bestuurder.Rijksregisternummer == null ? DBNull.Value : bestuurder.Rijksregisternummer);
                        }

                        if (queryBuilder.ToString().Contains("@naam")) {
                            cmd.Parameters.AddWithValue("@naam", bestuurder.Naam == null ? DBNull.Value : bestuurder.Naam);
                        }

                        if (queryBuilder.ToString().Contains("@voornaam")) {
                            cmd.Parameters.AddWithValue("@voornaam", bestuurder.Voornaam == null ? DBNull.Value : bestuurder.Voornaam);
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
                    transaction.Commit();

                }
                catch (Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - Rollback gefaald", ex);
                    try {
                        transaction.Rollback();
                    }catch(Exception e) {
                        throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - gefaald", e);
                    }
                }
                finally {
                    conn.Close();
                    transaction.Dispose();
                }
            }

        }

        public Bestuurder selecteerBestuurder(string rijks) {
            string query = "SELECT b.*,tb.Brandstof,tk.Pincode, tk.GeldigDatun, tk.Geblokkeerd, tk.Bestuurder ,br.Id BehaaldId, " +
                "br.Categorie,br.Behaald,v.Merk,v.Model,v.Nummerplaat,v.Brandstof voertuigBrandstof, " +
                "v.TypeVoertuig,v.Kleur,v.AantalDeuren," +
                "tv.Rijbewijs FROM bestuurder b " +
                "LEFT JOIN TankkaartBrandstof tb ON b.Id=tb.TankkaartId " +
                "LEFT JOIN Tankkaart b ON b.TankkaartId=t.BestuurderId " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer=br.Bestuurder " +
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer=v.Chassisnummer " +
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig=tv.TypeVoertuig " +
                "WHERE t.Id=@id";
            Bestuurder b = null;
            Tankkaart t = null;
            List<TankkaartBrandstof> tankkaartBrandstof = null;
            Voertuig v = null;
            List<Rijbewijs> r = null;

            SqlConnection conn = ConnectionClass.getConnection();
            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Bestuurder", rijks);

                conn.Open();
                try {
                    using(SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if(b == null) {
                                b = new Bestuurder((string)reader["Bestuurder"], (string)reader["naam"], 
                                    (string)reader["voornaam"], (DateTime)reader["geboortedatum"]);
                            }

                            if(t == null  && !reader.IsDBNull(reader.GetOrdinal("Tankkaart"))) {
                                t = new Tankkaart((int)reader["Kaartnummer"], (DateTime)reader["geldidheidsdatum"],
                                    (string)reader["pincode"], (Bestuurder)reader["Bestuurder"], 
                                    (List<TankkaartBrandstof>)reader["brandstoffen"], 
                                    (bool)reader["geblokkeerd"]);
;
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {
                                if (tankkaartBrandstof == null) { tankkaartBrandstof = new List<TankkaartBrandstof>(); }
                                TankkaartBrandstof tb = (TankkaartBrandstof)Enum.Parse
                                    (typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                if (!tankkaartBrandstof.Contains(tb)) {
                                    tankkaartBrandstof.Add(tb);
                                }
                            }

                            if (v == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {
                                    string kleur = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("kleur"))) kleur = (string)reader["kleur"];
                                    RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);
                                    v = new Voertuig(
                                        (BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), (string)reader["voertuigBrandstof"]), 
                                        (string)reader["VoertuigChassisnummer"], kleur, (int)reader["AantalDeuren"],
                                        (string)reader["merk"],
                                        (string)reader["model"],
                                        new TypeVoertuig((string)reader["TypeVoertuig"], re),
                                        (string)reader["Nummerplaat"]);
                                }
                            }

                            if (r == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                    if (r == null) { r = new List<Rijbewijs>(); }
                                    if (!r.Contains(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Categorie"]))) {
                                        r.Add(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]));
                                    }
                                }
                            }
                        }

                        reader.Close();

                        t.updateInBezitVan(b);
                        b.updateTankkaart(t);
                        b.updateVoertuig(v);


                        if (b != null && r != null) { b.rijbewijzen = r; }
                        if (b != null) v.updateBestuurder(b);

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

        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, string naam, string voornaam, DateTime? geboortedatum) {
            List<Bestuurder> lijstbestuurder = new List<Bestuurder>();
            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder(" SELECT b.*, tb.Brandstof, tk.Id, tk.Pincode,"+
                "tk.GeldigDatum, tk.Geblokkeerd, tk.Bestuurder, br.Id BehaaldId, "+
                "br.Categorie, br.Behaald, v.Merk, v.Model, v.Nummerplaat, v.Brandstof "+
                "voertuigBrandstof, v.TypeVoertuig, v.Kleur, v.AantalDeuren, "+
                "tv.Rijbewijs FROM bestuurder b "+
                "LEFT JOIN TankkaartBrandstof tb ON b.TankkaartId = tb.TankkaartId "+
                "LEFT JOIN Tankkaart tk ON b.TankkaartId = tk.Bestuurder "+
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

            if (voornaam != null) {
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
                if (query.ToString().Contains("@achternaam")) cmd.Parameters.AddWithValue("@achternaam", voornaam);
                if (query.ToString().Contains("@geboortedatum")) cmd.Parameters.AddWithValue
                        ("@geboortedatum", geboortedatum.Value.ToString("yyyy-MM-dd"));
                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        List<TankkaartBrandstof> brandstoffen = null;
                        Tankkaart dbTankkaart = null;
                        List<Rijbewijs> dbRijbewijzen = null;
                        Voertuig dbVoertuig = null;
                        while (reader.Read()) {
                          //Bestuurder
                          Bestuurder dbBestuurder = new Bestuurder(
                          (string)reader["Rijksregisternummer"],
                          (string)reader["Achternaam"],
                          (string)reader["Naam"],
                          (DateTime)reader["Geboortedatum"]);
                            //Tankkaart
                            if (!reader.IsDBNull(reader.GetOrdinal("TankkaartId"))) {
                                dbTankkaart = new Tankkaart((int)reader["Id"], (DateTime)reader["GeldigDatum"],
                                  (string)reader["Pincode"], (Bestuurder)reader["Bestuurder"],
                                  (List<TankkaartBrandstof>)reader["Brandstoffen"],
                                  (bool)reader["Geblokkeerd"]);
                                if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {

                                    if (brandstoffen == null) brandstoffen = new List<TankkaartBrandstof>();
                                    TankkaartBrandstof inGelezenData = (TankkaartBrandstof)Enum.Parse
                                        (typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                    if (!brandstoffen.Contains(inGelezenData)) { brandstoffen.Add(inGelezenData);
                                        dbTankkaart.zetBrandstoffen(brandstoffen);
                                    }
                                    dbTankkaart.updateInBezitVan(dbBestuurder);
                                }
                            }
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
                            lijstbestuurder.Add(dbBestuurder);
                        } 
                        reader.Close();
                    }
                }catch(Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: toonBestuurders - gefaald", ex);
                }
                finally {
                    conn.Close();
                }
                return lijstbestuurder;
            }
        }



        public void verwijderBestuurder(string rijks) {
            if (!bestaatBestuurder(rijks)) throw new BestuurderRepositoryException("BestuurderRepository: " +
                "verwijderBestuurder - bestuurder bestaat niet!");
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "DELETE FROM Bestuurder WHERE Id=@rijksregisternummer";

            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@rijksregisternummer", rijks);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex) {
                    throw new TankkaartRepositoryException("BestuurderRepository : verwijderBestuurder - Gefaald!", ex);
                }
                finally { conn.Close(); }
            }
        }

        public Bestuurder voegBestuurderToe(Bestuurder bestuurder) {
            SqlConnection conn = ConnectionClass.getConnection();
            string queryOne = "INSERT INTO Bestuurder(Naam, Achternaam, " +
                "Geboortedatum, Rijksregisternummer";
            string queryTwo="INSERT INTO BestuurderRijbewijs(Bestuurder, Categorie, Behaald)" +
                "OUTPUT INSERTED.Id VALUES(@Bestuurder," +
                "@Categorie, @Behaald";
            string queryThree = "UPDATE TANKAART SET id=@BestuurderId WHERE TankkaartId=@id";

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
                    cmdOne.Transaction = trans;
                    cmdOne.Parameters.Add(new SqlParameter("@Rijksregisternummer", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                    cmdOne.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Naam"].Value = bestuurder.Voornaam;
                    cmdOne.Parameters.Add(new SqlParameter("@Achternaam", SqlDbType.NVarChar));
                    cmdOne.Parameters["@Achternaam"].Value = bestuurder.Naam;
                    cmdOne.Parameters.Add(new SqlParameter("@Geboortedatum", SqlDbType.Date));
                    cmdOne.Parameters["@Geboortedatum"].Value = bestuurder.GeboorteDatum;
                    if (queryOne.Contains("@TankkaartId")) {
                        cmdOne.Parameters.Add(new SqlParameter("@TankkaartId", SqlDbType.Int));
                        cmdOne.Parameters["@TankkaartId"].Value = bestuurder.Tankkaart.KaartNummer;
                    }
                    cmdOne.CommandText = queryOne;
                    cmdOne.ExecuteNonQuery();
                    if(bestuurder.rijbewijzen.Count > 0) {
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
                    if(bestuurder.Tankkaart != null) {
                        cmdThree.Transaction = trans;
                        cmdThree.CommandText = queryThree;
                        cmdThree.Parameters.Add(new SqlParameter("@TankkaartId", SqlDbType.Int));
                        cmdThree.Parameters["@TankkaartId"].Value = bestuurder.Tankkaart.KaartNummer;
                        cmdThree.Parameters.Add(new SqlParameter("@Rijksregisternummer", SqlDbType.NVarChar));
                        cmdThree.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                        cmdThree.ExecuteNonQuery();
                    }
                    trans.Commit();
                    return bestuurder;
                }
                catch (Exception ex) {
                    trans.Rollback();
                    Console.WriteLine(ex.Message);
                }
                finally {
                    conn.Close();
                }
                return null;
                }
            }

        }
    }