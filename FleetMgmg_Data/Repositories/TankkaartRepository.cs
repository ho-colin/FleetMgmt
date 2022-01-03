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
    public class TankkaartRepository : ITankkaartRepository {

        public bool bestaatTankkaart(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT Count(*) FROM Tankkaart WHERE Id=@id";

            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                conn.Open();

                try {
                    cmd.Parameters.AddWithValue("@id", id);

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0) return true; return false;
                } catch (Exception ex) {
                    throw new TankkaartRepositoryException("Er heeft zich een fout voorgedaan!", ex);
                    throw;
                } finally { conn.Close(); }
            }
        }

        public void bewerkTankkaart(Tankkaart tankkaart) {
            Tankkaart huidigeKaart = this.selecteerTankkaart(tankkaart.KaartNummer);
            if(tankkaart.Equals(huidigeKaart)) throw new TankkaartRepositoryException("TankkaartRepository : bewerkTankkaart - Er is niks verandert!");

            using (SqlConnection conn = ConnectionClass.getConnection()) {
                SqlTransaction transaction = null;
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    #region TankkaartBrandstofUpdate
                    if (!huidigeKaart.Brandstoffen.Equals(tankkaart.Brandstoffen)) {
                        #region DataTable aanmaken en populaten
                        DataTable table = new DataTable();
                        table.TableName = "TankkaartBrandstof";

                        table.Columns.Add("TankkaartId", typeof(int));
                        table.Columns.Add("Brandstof", typeof(string));

                        foreach(TankkaartBrandstof brandstof in tankkaart.Brandstoffen) {
                            var row = table.NewRow();
                            row["TankkaartId"] = tankkaart.KaartNummer;
                            row["Brandstof"] = brandstof.ToString();
                            table.Rows.Add(row);
                        }
                        #endregion
                        if(huidigeKaart.Brandstoffen != null) {
                            string tkDelQuery = "DELETE FROM TankkaartBrandstof WHERE TankkaartId=@id";
                            using (SqlCommand deleteCmd = new SqlCommand(tkDelQuery, conn, transaction)) {
                                deleteCmd.Parameters.AddWithValue("@id", tankkaart.KaartNummer);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                        using(SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction)) {
                            sqlBulkCopy.DestinationTableName = table.TableName;
                            sqlBulkCopy.WriteToServer(table);

                        }
                    }
                    #endregion
                    #region TankkaartUpdate
                    StringBuilder updateQuery = new StringBuilder("UPDATE TankKaart SET ");
                    bool kommaZetten = false;

                    if(huidigeKaart.Pincode != tankkaart.Pincode) {
                        updateQuery.Append(" Pincode=@pincode ");
                        kommaZetten = true;
                    }

                    if(huidigeKaart.GeldigheidsDatum != tankkaart.GeldigheidsDatum) {
                        if (kommaZetten) { updateQuery.Append(","); } else kommaZetten = true;
                        updateQuery.Append(" GeldigDatum=CAST(@geldigdatum AS date)");
                    }

                    if(huidigeKaart.Geblokkeerd != tankkaart.Geblokkeerd) {
                        if (kommaZetten) { updateQuery.Append(","); } else kommaZetten = true;
                        updateQuery.Append(" Geblokkeerd=@geblokkeerd");
                    }

                    if(huidigeKaart.InBezitVan != tankkaart.InBezitVan) {
                        if (kommaZetten) { updateQuery.Append(","); } else kommaZetten = true;
                        updateQuery.Append(" Bestuurder=@bestuurder ");
                    }

                    updateQuery.Append(" WHERE Id=@id");

                    using(SqlCommand updateCmd = new SqlCommand(updateQuery.ToString(), conn, transaction)) {
                        #region Nullchecks for DB + toekennen waardes
                        if (updateQuery.ToString().Contains("@pincode")) {
                            updateCmd.Parameters.AddWithValue("@pincode", tankkaart.Pincode == null ? DBNull.Value : tankkaart.Pincode);
                        }

                        if (updateQuery.ToString().Contains("@geldigdatum")) {
                            updateCmd.Parameters.AddWithValue("@geldigdatum", tankkaart.GeldigheidsDatum.ToString("yyyy/MM/dd"));
                        }

                        if (updateQuery.ToString().Contains("@geblokkeerd")) {
                            updateCmd.Parameters.AddWithValue("@geblokkeerd", tankkaart.Geblokkeerd ? "1" : "0");
                        }

                        if (updateQuery.ToString().Contains("@bestuurder")) {
                            updateCmd.Parameters.AddWithValue("@bestuurder", tankkaart.InBezitVan == null ? DBNull.Value : tankkaart.InBezitVan.Rijksregisternummer);
                        }

                        updateCmd.Parameters.AddWithValue("@id", tankkaart.KaartNummer);

                        updateCmd.ExecuteNonQuery();
                        #endregion
                    }
                    #endregion
                    #region BestuurderUpdate
                    if(tankkaart.InBezitVan != null) {
                        // Als de bestuurder al een tankkaart zou hebben wordt deze overschreven met deze tankkaart.
                        string query = "UPDATE Bestuurder SET TankkaartId=@tankkaartid WHERE Rijksregisternummer=@rijksregisternummer";
                        using (SqlCommand cmd = new SqlCommand(query,conn,transaction)) {
                            cmd.Parameters.AddWithValue("@tankkaartid", tankkaart.KaartNummer);
                            cmd.Parameters.AddWithValue("@rijksregisternummer", tankkaart.InBezitVan.Rijksregisternummer);

                            cmd.ExecuteNonQuery();
                        }
                    } else {
                        //Als de bestuurder NULL is wordt het veld met verkregen tankkaartid op null gezet in de bestuurderskollom
                        string query = "UPDATE Bestuurder SET TankkaartId=NULL WHERE TankkaartId=@tankkaartid";
                        using(SqlCommand cmd = new SqlCommand(query, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@tankkaartid", tankkaart.KaartNummer);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    
                    #endregion
                    transaction.Commit();
                } catch (Exception ex) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex2) {
                        throw new TankkaartRepositoryException("TankkaartRepository : bewerkTankkaart - Rollback failed!!!",ex2);
                    }
                    throw new TankkaartRepositoryException("TankkaartRepository : bewerkTankkaart", ex);
                } finally { conn.Close(); transaction.Dispose(); }
            }
        }

        public Tankkaart selecteerTankkaart(int id) {
            string query = "SELECT t.*,tb.Brandstof,b.Naam,b.Achternaam,b.Geboortedatum,b.VoertuigChassisnummer,br.Id BehaaldId,br.Categorie,br.Behaald,v.Merk,v.Model,v.Nummerplaat,v.Brandstof voertuigBrandstof,v.TypeVoertuig,v.Kleur,v.AantalDeuren,tv.Rijbewijs FROM tankkaart t " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id=tb.TankkaartId " +
                "LEFT JOIN Bestuurder b ON t.Bestuurder=b.Rijksregisternummer " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer=br.Bestuurder " +
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer=v.Chassisnummer " +
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig=tv.TypeVoertuig " +
                "WHERE t.Id=@id";

            Tankkaart tk = null;
            List<TankkaartBrandstof> tankkaartBrandstof = null;
            Bestuurder b = null;
            List<FleetMgmt_Business.Objects.Rijbewijs> bestuurderRijbewijs = null;

            Voertuig v = null;


            SqlConnection conn = ConnectionClass.getConnection();
            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            if (tk == null) {
                                tk = new Tankkaart((int)reader["id"], (DateTime)reader["GeldigDatum"], null, null, null, (bool)reader["Geblokkeerd"]);
                                if (!reader.IsDBNull(reader.GetOrdinal("Pincode"))) { tk.updatePincode((string)reader["Pincode"]); }
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {
                                if (tankkaartBrandstof == null) { tankkaartBrandstof = new List<TankkaartBrandstof>(); }
                                TankkaartBrandstof tb = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                if (!tankkaartBrandstof.Contains(tb)) {
                                    tankkaartBrandstof.Add(tb);
                                }
                            }
                            if (b == null && !reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                                b = new Bestuurder((string)reader["Bestuurder"], (string)reader["Achternaam"], (string)reader["Naam"], (DateTime)reader["Geboortedatum"]);
                            }
                            if (bestuurderRijbewijs == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                    if (bestuurderRijbewijs == null) { bestuurderRijbewijs = new List<Rijbewijs>(); }
                                    if (!bestuurderRijbewijs.Contains(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]))) {
                                        bestuurderRijbewijs.Add(new Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]));
                                    }
                                }
                            }
                            if (v == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {

                                    string kleur = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("kleur"))) kleur = (string)reader["kleur"];
                                    RijbewijsEnum re = (RijbewijsEnum) Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);

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
                        }
                        reader.Close();

                        tk.updateInBezitVan(b);
                        tk.zetBrandstoffen(tankkaartBrandstof);
                        if (b != null && bestuurderRijbewijs != null) { b.rijbewijzen = bestuurderRijbewijs; }
                        if(b!=null && v != null) v.updateBestuurder(b);

                        return tk;
                    }
                } catch (Exception ex) {
                    var dbex = new TankkaartRepositoryException("TankkaartRepository : selecteerTankkaart",ex);
                    dbex.Data.Add("Id", id);
                    throw dbex;
                } finally { conn.Close(); }
            }
        }

        public IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurder, bool? geblokkeerd, TankkaartBrandstof? brandstof) {
            List<Tankkaart> kaarten = new List<Tankkaart>();
            SqlConnection conn = ConnectionClass.getConnection();

            #region TANKKAART AANMAKEN
            StringBuilder query = new StringBuilder("SELECT t.*,tb.Brandstof,b.Naam,b.Achternaam,b.Geboortedatum,b.VoertuigChassisnummer,br.Id BehaaldId,br.Categorie,br.Behaald,v.Merk,v.Model,v.Nummerplaat,v.Brandstof voertuigBrandstof,v.TypeVoertuig,v.Kleur,v.AantalDeuren,tv.Rijbewijs FROM tankkaart t " +
                "LEFT JOIN TankkaartBrandstof tb ON t.Id=tb.TankkaartId " +
                "LEFT JOIN Bestuurder b ON t.Bestuurder=b.Rijksregisternummer " +
                "LEFT JOIN BestuurderRijbewijs br ON b.Rijksregisternummer=br.Bestuurder " +
                "LEFT JOIN Voertuig v ON b.VoertuigChassisnummer=v.Chassisnummer " +
                "LEFT JOIN TypeVoertuig tv ON v.TypeVoertuig=tv.TypeVoertuig ");
            #region QUERY OPBOUWEN ADHV INGEVULDE WAARDES
            bool where = false;
            bool and = false;

            if(id != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.Id=@id ");              
            }

            if(geldigheidsDatum != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.GeldigDatum=@geldigheidsDatum");
            }

            if (!string.IsNullOrWhiteSpace(bestuurder)) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.Bestuurder=@bestuurder");
            }

            if(geblokkeerd != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.Geblokkeerd=@geblokkeerd ");
            }

            if(brandstof != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                //WAARDE VAN GEJOINDE TABEL
                query.Append(" tb.Brandstof=@brandstof");
            }
            #endregion
            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                if (query.ToString().Contains("@id")) cmd.Parameters.AddWithValue("@id", id.Value);
                if (query.ToString().Contains("@geldigheidsDatum")) cmd.Parameters.AddWithValue("@geldigheidsDatum", geldigheidsDatum.Value.ToString("yyyy-MM-dd"));
                if (query.ToString().Contains("@bestuurder")) cmd.Parameters.AddWithValue("@bestuurder", bestuurder);
                if (query.ToString().Contains("@geblokkeerd")) cmd.Parameters.AddWithValue("@geblokkeerd", geblokkeerd.Value);
                if (query.ToString().Contains("@brandstof")) cmd.Parameters.AddWithValue("@brandstof", brandstof.ToString());

                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        int laatsGebruiktId = 0;

                        List<TankkaartBrandstof> brandstoffen = null;
                        Tankkaart dbTankkaart = null;

                        Bestuurder dbBestuurder = null;
                        List<FleetMgmt_Business.Objects.Rijbewijs> dbRijbewijzen = null;

                        Voertuig dbVoertuig = null;
                        while (reader.Read()) {
                            #region Tankkaart
                            if (reader.IsDBNull(reader.GetOrdinal("Id"))) throw new TankkaartRepositoryException("TankkaartRepository : geefTankkaarten - Geen Tankkaart ID gevonden!");
                            if(laatsGebruiktId != (int)reader["Id"]) {
                                if (dbTankkaart != null) {
                                    if (brandstoffen != null && dbTankkaart != null) { dbTankkaart.zetBrandstoffen(brandstoffen); }
                                    if (dbBestuurder != null && dbRijbewijzen != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x)); }
                                    if (dbBestuurder != null && dbVoertuig != null) { dbBestuurder.updateVoertuig(dbVoertuig); }
                                    if (dbBestuurder != null && dbTankkaart != null) { dbTankkaart.updateInBezitVan(dbBestuurder); }

                                    kaarten.Add(dbTankkaart); 
                                    brandstoffen = null; 
                                    dbBestuurder = null; 
                                    dbRijbewijzen = null; 
                                }
                                laatsGebruiktId = (int)reader["Id"];

                                dbTankkaart = new Tankkaart(
                                    (int) reader["Id"],
                                    (DateTime) reader["GeldigDatum"],
                                    reader.IsDBNull(reader.GetOrdinal("Pincode")) ? null : (string)reader["Pincode"],
                                    null,
                                    null, 
                                    (bool)reader["Geblokkeerd"]);
                            }
                            #endregion
                            #region TankkaartBrandstoffen
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {

                                if (brandstoffen == null) brandstoffen = new List<TankkaartBrandstof>();
                                TankkaartBrandstof inGelezenData = (TankkaartBrandstof)Enum.Parse(typeof(TankkaartBrandstof), (string)reader["Brandstof"]);
                                if (!brandstoffen.Contains(inGelezenData)){ brandstoffen.Add(inGelezenData); }
                            }
                            #endregion
                            #region Bestuurder
                            if (!reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                                if(dbBestuurder == null) {
                                    dbBestuurder = new Bestuurder(
                                        (string)reader["Bestuurder"],
                                        (string)reader["Achternaam"],
                                        (string)reader["Naam"],
                                        (DateTime)reader["Geboortedatum"]);
                                }
                            }
                            #endregion
                            #region BestuurderRijbewijs
                            if (dbBestuurder != null && !reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                if (dbRijbewijzen == null) dbRijbewijzen = new List<FleetMgmt_Business.Objects.Rijbewijs>();
                                FleetMgmt_Business.Objects.Rijbewijs gelezenRijbewijs = new FleetMgmt_Business.Objects.Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]);
                                if (!dbRijbewijzen.Contains(gelezenRijbewijs)) { dbRijbewijzen.Add(gelezenRijbewijs); }
                            }
                            #endregion
                            #region Voertuig
                            if(dbVoertuig == null && !reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))){
                                BrandstofEnum voertuigBrandstof = (BrandstofEnum)Enum.Parse(typeof(BrandstofEnum), (string)reader["voertuigBrandstof"]);
                                RijbewijsEnum re = (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]);
                                string kleur = reader.IsDBNull(reader.GetOrdinal("Kleur")) ? null : (string)reader["Kleur"];

                                dbVoertuig = new Voertuig(voertuigBrandstof, (string)reader["VoertuigChassisnummer"], kleur, (int)reader["AantalDeuren"], (string)reader["Merk"], (string)reader["Model"], new TypeVoertuig((string)reader["TypeVoertuig"], re), (string)reader["Nummerplaat"]);
                            }
                            #endregion

                        }                      
                        reader.Close();

                        #region Laatste ook nog toevoegen
                        if (dbTankkaart != null) {
                            if (brandstoffen != null && dbTankkaart != null) { dbTankkaart.zetBrandstoffen(brandstoffen); }
                            if (dbBestuurder != null && dbRijbewijzen != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x)); }
                            if (dbBestuurder != null && dbVoertuig != null) { dbBestuurder.updateVoertuig(dbVoertuig); }
                            if (dbBestuurder != null && dbTankkaart != null) { dbTankkaart.updateInBezitVan(dbBestuurder); }
                            kaarten.Add(dbTankkaart);
                        } 
                        #endregion
                    }
                } catch (Exception ex) {
                    throw new TankkaartRepositoryException("TankkaartRepository : geefTankkaarten - Er heeft zich een fout voorgedaan!",ex);
                }finally { conn.Close(); }
            }
            #endregion
            return kaarten;
        }

        public void verwijderTankkaart(int id) {
            if (!bestaatTankkaart(id)) throw new TankkaartRepositoryException("TankkaartRepository : verwijderTankkaart - Tankkaart bestaat niet! ");

            Tankkaart tk = this.selecteerTankkaart(id);

            using(SqlConnection conn = ConnectionClass.getConnection()) {
                SqlTransaction transaction = null;
                try {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    #region Zet TKId van user naar null
                    if(tk.InBezitVan != null) {
                        string uQuery = "UPDATE Bestuurder SET TankkaartId=NULL WHERE TankkaartId=@id";
                        using(SqlCommand cmd = new SqlCommand(uQuery, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id",id);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    #endregion

                    #region Verwijder TK
                    string query = "DELETE FROM Tankkaart WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(query, conn, transaction)) {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    #endregion
                    transaction.Commit();
                } catch (Exception ex) {
                    try {
                        transaction.Rollback();
                    } catch (Exception ex2) {
                        throw new TankkaartRepositoryException("TankkaartRepository : verwijderTankkaart - Rollback failed!!!",ex2);
                    }
                    throw new TankkaartRepositoryException("TankkaartRepository : verwijderTankkaart", ex);
                } finally { conn.Close(); transaction.Dispose(); }
            }
        }

        public Tankkaart voegTankkaartToe(Tankkaart tankkaart) {
            int insertedId = 0;

            string inbezitvan = null;
            if(tankkaart.InBezitVan != null) {
                inbezitvan = tankkaart.InBezitVan.Rijksregisternummer;

                IEnumerable<Tankkaart> listje = geefTankkaarten(null, null, inbezitvan, null, null);

                if (listje.Count() > 0) throw new TankkaartRepositoryException("TankkaartRepositoryException : voegTankkaartToe - Opgegeven bestuurder heeft al een tankkaart!");
            }

            SqlConnection conn = ConnectionClass.getConnection();
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();

            try {
                StringBuilder query = new StringBuilder("INSERT INTO Tankkaart (GeldigDatum, Geblokkeerd");

                #region Optionele Te Vullen Velden
                if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                    query.Append(", Pincode ");
                }

                if (!string.IsNullOrWhiteSpace(inbezitvan)) {
                    query.Append(", Bestuurder ");
                }
                #endregion
                query.Append(") OUTPUT inserted.Id VALUES (@geldigdatum,@geblokkeerd");

                #region Opionele Te vullen Velden Parameter variabels
                if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                    query.Append(",@pincode");
                }

                if (!string.IsNullOrWhiteSpace(inbezitvan)) {
                    query.Append(",@bestuurder");
                }
                #endregion
                query.Append(")");


                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.Transaction = transaction;

                    try {
                        cmd.CommandText = query.ToString();
                        Console.WriteLine(query.ToString());

                        cmd.Parameters.AddWithValue("@geldigdatum", tankkaart.GeldigheidsDatum.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@geblokkeerd", tankkaart.Geblokkeerd);

                        if (query.ToString().Contains("@pincode")) cmd.Parameters.AddWithValue("@pincode", tankkaart.Pincode);
                        if (query.ToString().Contains("@bestuurder")) cmd.Parameters.AddWithValue("@bestuurder", tankkaart.InBezitVan.Rijksregisternummer);

                        insertedId = (int)cmd.ExecuteScalar();
                        tankkaart.zetKaartnummer(insertedId);
                    } catch (Exception ex) {
                        throw new TankkaartRepositoryException("TankkaartRepository : voegTankkaartToe - Er heeft zich een fout voorgedaan!", ex);
                    }
                }

                //Als er brandstoffen zijn toegevoegd bij het aanmaken van de tankkaart!
                if (tankkaart.Brandstoffen != null) {

                    string tankkaartBrandstofQuery = "INSERT INTO TankkaartBrandstof (TankkaartId,Brandstof) VALUES (@tankkaartid,@brandstof)";

                    using (SqlCommand cmd2 = conn.CreateCommand()) {
                        cmd2.Transaction = transaction;
                        try {

                            cmd2.CommandText = tankkaartBrandstofQuery;
                            cmd2.Parameters.Add(new SqlParameter("@tankkaartId", SqlDbType.Int));
                            cmd2.Parameters.Add(new SqlParameter("@brandstof", SqlDbType.NVarChar));
                            cmd2.Parameters["@tankkaartId"].Value = insertedId;
                            foreach (TankkaartBrandstof brandstof in tankkaart.Brandstoffen) {
                                cmd2.Parameters["@brandstof"].Value = brandstof.ToString();
                                cmd2.ExecuteNonQuery();
                            }
                        } catch (Exception ex) {
                            throw new TankkaartRepositoryException("TankkaartRepository : voegTankkaartToe - Er heeft zich een probleem voorgedaan!", ex);
                        }
                    }

                }
                if (tankkaart.InBezitVan != null) {
                    string bestuurderQuery = "UPDATE Bestuurder SET TankkaartId = @tankkaartid WHERE Rijksregisternummer=@rijksregisternummer";
                    using (SqlCommand cmd3 = conn.CreateCommand()) {
                        cmd3.Transaction = transaction;

                        cmd3.CommandText = bestuurderQuery;
                        cmd3.Parameters.AddWithValue("@rijksregisternummer", tankkaart.InBezitVan.Rijksregisternummer);
                        cmd3.Parameters.AddWithValue("@tankkaartid", insertedId);

                        try {
                            cmd3.ExecuteNonQuery();
                        } catch (Exception ex) {
                            throw new TankkaartRepositoryException("TankkaartRepository : voegTankkaartToe - Er heeft zich een probleem voorgedaan!", ex);
                        }
                    }
                }
                transaction.Commit();
                return tankkaart;
            } catch (Exception ex) {
                try {
                    transaction.Rollback();
                } catch (Exception ex2) {
                    throw new TankkaartRepositoryException("TankkaartRepository : bewerkTankkaart - Rollback failed!!!", ex2);
                }
                throw new TankkaartRepositoryException(ex.Message, ex);
            } finally { transaction.Dispose(); conn.Close(); }
        }
    }
}
