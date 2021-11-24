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


        public bool bestaatBestuurder(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT COUNT(*) FROM [Bestuurder] WHERE Id = @Id";
            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    bool result = false;
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.Int));
                    cmd.Parameters["@Id"].Value = id;

                    Console.WriteLine();
                    result = (int)cmd.ExecuteScalar() == 1 ? true : false;
                    return result;
                }
                catch (Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: bestaatBestuurder - Er werd geen bestuurder gevonden!", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        public void bewerkBestuurder(Bestuurder bestuurder) {
            Bestuurder huidigieBestuurder = this.selecteerBestuurder(bestuurder.Id);
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
                            row["BestuurderId"] = bestuurder.Id;
                            row["Categorie"] = rijbewijs.Categorie;
                            row["Behaald"] = rijbewijs.BehaaldOp;
                            table.Rows.Add(row);
                        }
                        if (huidigieBestuurder.rijbewijzen != null) {
                            string tkDelQuery = "DELETE FROM BestuurderRijbewijs WHERE Id=@id";
                            using (SqlCommand deleteCmd = new SqlCommand(tkDelQuery, conn, transaction)) {
                                deleteCmd.Parameters.AddWithValue("@id", bestuurder.Id);
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
                    if(huidigieBestuurder.Rijksregisternummer != bestuurder.Rijksregisternummer) {
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

                    using(SqlCommand cmd = new(queryBuilder.ToString(), conn, transaction)) {
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
                            cmd.Parameters.AddWithValue("@geboorteTankkaartIdDatum", bestuurder.Tankkaart.Equals(0) ? DBNull.Value : bestuurder.Tankkaart);
                        }

                        if (queryBuilder.ToString().Contains("@VoertuigChassisnummer")) {
                            cmd.Parameters.AddWithValue("@VoertuigChassisnummer", bestuurder.Voertuig.Equals(0) ? DBNull.Value : bestuurder.Voertuig);
                        }

                        cmd.Parameters.AddWithValue("@id", bestuurder.Id);

                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();

                    }catch (Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - Rollback gefaald", ex);
                    try {
                        transaction.Rollback();
                    }catch(Exception e) {
                        throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - gefaald", ex);
                    }
                }
                finally {
                    conn.Close();
                    transaction.Dispose();
                }
            }

        }

        public Bestuurder selecteerBestuurder(int id) {
            string query = "SELECT b.*,tb.Brandstof,tk.Pincode, tk.GeldigDatun, tk.Geblokkeerd, tk.Bestuurder ,br.Id BehaaldId, " +
                "br.Categorie,br.Behaald,v.Merk,v.Model,v.Nummerplaat,v.Brandstof voertuigBrandstof, v.TypeVoertuig,v.Kleur,v.AantalDeuren," +
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
                cmd.Parameters.AddWithValue("@Bestuurder", id);

                conn.Open();
                try {
                    using(SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if(b == null) {
                                b = new Bestuurder((string)reader["Bestuurder"], (string)reader["naam"], (string)reader["voornaam"], (DateTime)reader["geboortedatum"]);
                            }

                            if(t == null  && !reader.IsDBNull(reader.GetOrdinal("Tankkaart"))) {
                                t = new Tankkaart((int)reader["Kaartnummer"], (DateTime)reader["geldidheidsdatum"],
                                    (string)reader["pincode"], (Bestuurder)reader["Bestuurder"], (List<TankkaartBrandstof>)reader["brandstoffen"], 
                                    (bool)reader["geblokkeerd"]);
;
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {
                                if (tankkaartBrandstof == null) { tankkaartBrandstof = new List<TankkaartBrandstof>(); }
                                TankkaartBrandstof tb = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
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
                    throw new BestuurderRepositoryException("BestuurderRepository: selecteerBestuurder: Bestuurder werd niet geselecteerd", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        public IEnumerable<Bestuurder> toonBestuurders(int? id, string rijksregisternummer, string naam, string voornaam, DateTime? geboortedatum, Rijbewijs rijbewijs) {
            List<Bestuurder> lijstbestuurder = new List<Bestuurder>();
            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder("SELECT b.*,tb.Brandstof,tk.Pincode, tk.GeldigDatun, tk.Geblokkeerd, tk.Bestuurder ,br.Id BehaaldId, " +
                "br.Categorie,br.Behaald,v.Merk,v.Model,v.Nummerplaat,v.Brandstof voertuigBrandstof, v.TypeVoertuig,v.Kleur,v.AantalDeuren," +
                "tv.Rijbewijs FROM bestuurder b " +
                "LEFT JOIN TankkaartBrandstof tb ON b.Id=tb.TankkaartId " +
                "LEFT JOIN Tankkaart b ON b.TankkaartId=t.BestuurderId " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer=br.Bestuurder " +
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer=v.Chassisnummer " +
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig=tv.TypeVoertuig ");

            bool where = false;
            bool and = false;


            if (id != 0) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Id=@id ");
            }

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
                query.Append(" b.Voornaam=@voornaam ");
            }

            if (geboortedatum.GetHashCode() != 0) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" b.Geboortedatum=@geboortedatum");
            }

            if (rijbewijs != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;

                query.Append(" bId=@bestuurderId");
            }

            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                if (query.ToString().Contains("@Rijksregisternummer")) cmd.Parameters.AddWithValue("@Rijksregisternummer", rijksregisternummer);
                if (query.ToString().Contains("@Naam")) cmd.Parameters.AddWithValue("@Naam", naam);
                if (query.ToString().Contains("@Voornaam")) cmd.Parameters.AddWithValue("@Voornaam", voornaam);
                if (query.ToString().Contains("@Geboortedatum")) cmd.Parameters.AddWithValue("@Geboortedatum", geboortedatum.Value.ToString("yyyy-MM-dd"));

                conn.Open();

                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        int laatsGebruiktId = 0;

                        List<TankkaartBrandstof> brandstoffen = null;
                        Tankkaart dbTankkaart = null;

                        Bestuurder dbBestuurder = null;
                        List<Rijbewijs> dbRijbewijzen = null;

                        Voertuig dbVoertuig = null;
                        while (reader.Read()) {
                            #region Tankkaart
                            if (reader.IsDBNull(reader.GetOrdinal("Id")))
                                throw new TankkaartRepositoryException("TankkaartRepository : geefTankkaarten - TankkaartId werd niet gevonden!");
                            if (laatsGebruiktId != (int)reader["Id"]) {
                                laatsGebruiktId = (int)reader["Id"];

                                dbTankkaart = new Tankkaart((int)reader["Kaartnummer"], (DateTime)reader["geldidheidsdatum"],
                                    (string)reader["pincode"], (Bestuurder)reader["Bestuurder"], (List<TankkaartBrandstof>)reader["brandstoffen"],
                                    (bool)reader["geblokkeerd"]); ;


                                if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {

                                    if (brandstoffen == null) brandstoffen = new List<TankkaartBrandstof>();
                                    TankkaartBrandstof inGelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                    if (!brandstoffen.Contains(inGelezenData)) { brandstoffen.Add(inGelezenData); }
                                }
                                if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                                    if (dbBestuurder == null) {
                                        dbBestuurder = new Bestuurder(
                                            (string)reader["Bestuurder"],
                                            (string)reader["Achternaam"],
                                            (string)reader["Naam"],
                                            (DateTime)reader["Geboortedatum"]);
                                    }

                                }
                                if (dbBestuurder != null && !reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                    if (dbRijbewijzen == null) dbRijbewijzen = new List<Rijbewijs>();
                                    Rijbewijs gelezenRijbewijs =
                                        new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]);
                                    if (!dbRijbewijzen.Contains(gelezenRijbewijs)) { dbRijbewijzen.Add(gelezenRijbewijs); }
                                }
                                if (dbVoertuig == null && !reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {
                                    BrandstofEnum voertuigBrandstof = (BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), (string)reader["voertuigBrandstof"]);
                                    RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);
                                    string kleur = reader.IsDBNull(reader.GetOrdinal("Kleur")) ? null : (string)reader["Kleur"];

                                    dbVoertuig = new Voertuig(voertuigBrandstof, (string)reader["VoertuigChassisnummer"], kleur,
                                        (int)reader["AantalDeuren"], (string)reader["Merk"], (string)reader["Model"],
                                        new TypeVoertuig((string)reader["TypeVoertuig"], re), (string)reader["Nummerplaat"]);

                                }

                            }
                            if (brandstoffen != null && dbTankkaart != null) { dbTankkaart.zetBrandstoffen(brandstoffen); }
                            if (dbBestuurder != null && dbRijbewijzen != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x)); }
                            if (dbBestuurder != null && dbVoertuig != null) { dbBestuurder.updateVoertuig(dbVoertuig); }
                        }    if (dbBestuurder != null && dbTankkaart != null) { dbTankkaart.updateInBezitVan(dbBestuurder); }
                        reader.Close();
                        if (dbBestuurder != null) lijstbestuurder.Add(dbBestuurder);
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



        public void verwijderBestuurder(int id) {
            if (!bestaatBestuurder(id)) throw new BestuurderRepositoryException("BestuurderRepository: verwijderBestuurder - bestuurder bestaat niet!");
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "DELETE FROM Bestuurder WHERE Id=@rijksregisternummer";

            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@rijksregisternummer", id);
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
            string query = "INSERT INTO Bestuurder(Naam, Achternaam, Geboortedatum, Rijksregisternummer) VALUES(@Naam, @Achternaam, @Geboortedatum, " +
                "@Rijksregisternummer, " +
                ")";
            using(SqlCommand cmd = conn.CreateCommand()) {
                try {
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter("@Naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@Achternaam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@Geboortedatum", System.Data.SqlDbType.Date));
                    cmd.Parameters.Add(new SqlParameter("@Rijksregisternummer", System.Data.SqlDbType.NVarChar));
                    cmd.CommandText = query;
                    cmd.Parameters["@Naam"].Value = bestuurder.Voornaam;
                    cmd.Parameters["@Achternaam"].Value = bestuurder.Naam;
                    cmd.Parameters["@Geboortedatum"].Value = bestuurder.GeboorteDatum;
                    cmd.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                    return new Bestuurder(bestuurder.Rijksregisternummer, bestuurder.Naam, bestuurder.Voornaam, bestuurder.GeboorteDatum);

                }catch(Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: voegBestuurderToe - Geen bestuurder werd toegevoegd!", ex);
                }
                finally {
                    conn.Close();
                }
            }

        }
    }
}
#endregion