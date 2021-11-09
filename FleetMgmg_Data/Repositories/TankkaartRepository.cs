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
            #region tankkaart tabel
            StringBuilder query = new StringBuilder("UPDATE Tankkaart SET ");


            if(tankkaart.InBezitVan == null) {
                if(huidigeKaart.InBezitVan != null) {
                    query.Append(" BestuurderId=@BestuurderIdLEEG ");
                }
            } else { // tankkaart.inbezitvan WEL een value heeft
                if(huidigeKaart.InBezitVan == null) {
                    query.Append(" BestuurderId=@bestuurderId ");
                }
            }

            if(tankkaart.Geblokkeerd != huidigeKaart.Geblokkeerd) {
                query.Append(" Geblokkeerd=@geblokkeerd ");
            }

            if(tankkaart.Pincode != huidigeKaart.Pincode) {
                query.Append(" Pincode=@pincode ");
            }

            if(tankkaart.GeldigheidsDatum != huidigeKaart.GeldigheidsDatum) {
                query.Append(" GeldigDatum=@geldigdatum ");
            }

            query.Append(" WHERE id=@id");
            #endregion
            SqlConnection conn = ConnectionClass.getConnection();

            using(SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query.ToString();

                    if (query.ToString().Contains("@bestuurderIdLEEG")) cmd.Parameters.AddWithValue("@bestuurderIdLEEG", DBNull.Value);
                    if (query.ToString().Contains("@bestuurderId")) cmd.Parameters.AddWithValue("@bestuurderId", tankkaart.InBezitVan.Id);
                    if (query.ToString().Contains("@geblokkeerd")) cmd.Parameters.AddWithValue("@geblokkeerd", tankkaart.Geblokkeerd ? "1" : "0");
                    if (query.ToString().Contains("@pincode")) cmd.Parameters.AddWithValue("@pincode", tankkaart.Pincode);
                    if (query.ToString().Contains("@geldigdatum")) cmd.Parameters.AddWithValue("@geldigdatum", tankkaart.GeldigheidsDatum.ToString("yyyy-MM-dd"));
                    if (query.ToString().Contains("@id")) cmd.Parameters.AddWithValue("@id", tankkaart.KaartNummer);


                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new TankkaartRepositoryException("TankkaartRepository : bewerkTankkaart - Er heeft zich een fout voorgedaan!",ex);
                } finally { conn.Close(); }
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
            List<string> tankkaartBrandstof = null;
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
                                tk = new Tankkaart((int)reader["id"], (DateTime)reader["GeldigDatum"], (string)reader["pincode"], null, null);
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {
                                if (tankkaartBrandstof == null) { tankkaartBrandstof = new List<string>(); }
                                if (!tankkaartBrandstof.Contains((string)reader["Brandstof"])) {
                                    tankkaartBrandstof.Add((string)reader["Brandstof"]);
                                }
                            }
                            if (b == null && !reader.IsDBNull(reader.GetOrdinal("Bestuurder"))) {
                                b = new Bestuurder((string)reader["Bestuurder"], (string)reader["Achternaam"], (string)reader["Naam"], (DateTime)reader["Geboortedatum"]);
                            }
                            if (bestuurderRijbewijs == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("Categorie"))) {
                                    if (bestuurderRijbewijs == null) { bestuurderRijbewijs = new List<FleetMgmt_Business.Objects.Rijbewijs>(); }
                                    if (!bestuurderRijbewijs.Contains(new FleetMgmt_Business.Objects.Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]))) {
                                        bestuurderRijbewijs.Add(new FleetMgmt_Business.Objects.Rijbewijs((string)reader["Categorie"], (DateTime)reader["Behaald"]));
                                    }
                                }
                            }
                            if (v == null) {
                                if (!reader.IsDBNull(reader.GetOrdinal("VoertuigChassisnummer"))) {

                                    string kleur = null;
                                    if (!reader.IsDBNull(reader.GetOrdinal("kleur"))) kleur = (string)reader["kleur"];

                                    v = new Voertuig((Brandstof)Enum.Parse(typeof(Brandstof), (string)reader["voertuigBrandstof"]), (string)reader["VoertuigChassisnummer"], kleur, (int)reader["AantalDeuren"], (string)reader["merk"], (string)reader["model"], (string)reader["TypeVoertuig"], (string)reader["Nummerplaat"]);
                                }
                            }
                        }
                        reader.Close();

                        tk.updateInBezitVan(b);
                        tk.zetBrandstoffen(tankkaartBrandstof);
                        if (b != null && bestuurderRijbewijs != null) { b.rijbewijzen = bestuurderRijbewijs; }
                        if(b!=null) v.updateBestuurder(b);

                        return tk;
                    }
                } catch (Exception ex) {
                    var dbex = new TankkaartRepositoryException("TankkaartRepository : selecteerTankkaart",ex);
                    dbex.Data.Add("Id", id);
                    throw dbex;
                } finally { conn.Close(); }
            }
        }

        public IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurder, bool? geblokkeerd, Brandstof? brandstof) {
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
                if (query.ToString().Contains("@bestuurderId")) cmd.Parameters.AddWithValue("@bestuurder", bestuurder);
                if (query.ToString().Contains("@geblokkeerd")) cmd.Parameters.AddWithValue("@geblokkeerd", geblokkeerd.Value ? "1" : "0");
                if (query.ToString().Contains("@brandstof")) cmd.Parameters.AddWithValue("@brandstof", brandstof.ToString());

                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        int laatsGebruiktId = 0;

                        List<string> brandstoffen = null;
                        Tankkaart dbTankkaart = null;

                        Bestuurder dbBestuurder = null;
                        List<FleetMgmt_Business.Objects.Rijbewijs> dbRijbewijzen = null;

                        Voertuig dbVoertuig = null;
                        while (reader.Read()) {
                            #region Tankkaart
                            if (reader.IsDBNull(reader.GetOrdinal("Id"))) throw new TankkaartRepositoryException("TankkaartRepository : geefTankkaarten - Geen Tankkaart ID gevonden!");
                            if(laatsGebruiktId != (int)reader["Id"]) {
                                laatsGebruiktId = (int)reader["Id"];

                                dbTankkaart = new Tankkaart(
                                    (int) reader["Id"],
                                    (DateTime)reader["GeldigDatum"],
                                    reader.IsDBNull(reader.GetOrdinal("Pincode")) ? null : (string)reader["Pincode"],
                                    null,
                                    null);
                            }
                            #endregion
                            #region TankkaartBrandstoffen
                            if (!reader.IsDBNull(reader.GetOrdinal("Brandstof"))) {

                                if (brandstoffen == null) brandstoffen = new List<string>();
                                if (!brandstoffen.Contains((string)reader["Brandstof"])) brandstoffen.Add((string)reader["Brandstof"]);
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
                                Brandstof voertuigBrandstof = (Brandstof)Enum.Parse(typeof(Brandstof), (string)reader["voertuigBrandstof"]);
                                string kleur = reader.IsDBNull(reader.GetOrdinal("Kleur")) ? null : (string)reader["Kleur"];

                                dbVoertuig = new Voertuig(voertuigBrandstof, (string)reader["VoertuigChassisnummer"], kleur, (int)reader["AantalDeuren"], (string)reader["Merk"], (string)reader["Model"], (string)reader["TypeVoertuig"], (string)reader["Nummerplaat"]);
                            }
                            #endregion
                        }
                        if(brandstoffen != null && dbTankkaart != null) { dbTankkaart.zetBrandstoffen(brandstoffen); }
                        if(dbBestuurder != null && dbRijbewijzen != null) { dbRijbewijzen.ForEach(x => dbBestuurder.voegRijbewijsToe(x));}
                        if(dbBestuurder != null && dbVoertuig != null) { dbBestuurder.updateVoertuig(dbVoertuig); }
                        if(dbBestuurder != null && dbTankkaart != null) { dbTankkaart.updateInBezitVan(dbBestuurder); }
                        reader.Close();

                        #region Laatste ook nog toevoegen
                        if (dbTankkaart != null) kaarten.Add(dbTankkaart);
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

            SqlConnection conn = ConnectionClass.getConnection();
            string query = "DELETE FROM Tankkaart WHERE Id=@id";

            using(SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@id",id);
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new TankkaartRepositoryException("TankkaartRepository : verwijderTankkaart - Er is iets misgelopen!",ex);
                } finally { conn.Close(); }
            }
        }

        public void voegTankkaartToe(Tankkaart tankkaart) {
            int insertedId = 0;

            string inbezitvan = null;
            if(tankkaart.InBezitVan != null) {
                inbezitvan = tankkaart.InBezitVan.Rijksregisternummer;

                IEnumerable<Tankkaart> listje = geefTankkaarten(null, null, inbezitvan, null, null);

                if (listje.Count() > 0) throw new TankkaartRepositoryException("TankkaartRepositoryException : voegTankkaartToe - Opgegeven bestuurder heeft al een tankkaart!");
            }

            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder("INSERT INTO Tankkaart (GeldigDatum, Geblokkeerd");

            #region Optionele Te Vullen Velden
            if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                query.Append(", Pincode ");
            }

            if (!string.IsNullOrWhiteSpace(inbezitvan)) {
                query.Append(", BestuurderId ");
            }
            #endregion
            query.Append(") OUTPUT inserted.Id VALUES (@geldigdatum,@geblokkeerd");

            #region Opionele Te vullen Velden Parameter variabels
            if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                query.Append(",@pincode");
            }

            if (!string.IsNullOrWhiteSpace(inbezitvan)) {
                query.Append(",@bestuurderid");
            }
            #endregion
            query.Append(")");


            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();

                try {
                    cmd.CommandText = query.ToString();
                    Console.WriteLine(query.ToString());

                    cmd.Parameters.AddWithValue("@geldigdatum", tankkaart.GeldigheidsDatum.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@geblokkeerd", tankkaart.Geblokkeerd);

                    if (query.ToString().Contains("@pincode")) cmd.Parameters.AddWithValue("@pincode", tankkaart.Pincode);
                    if (query.ToString().Contains("@bestuurderid")) cmd.Parameters.AddWithValue("@bestuurderid", tankkaart.InBezitVan.Rijksregisternummer);

                   insertedId = (int)cmd.ExecuteScalar();
                } catch (Exception ex) {

                    throw new TankkaartRepositoryException("TankkaartRepository : voegTankkaartToe - Er heeft zich een fout voorgedaan!",ex);
                } finally { conn.Close(); }
            }

            //Als er brandstoffen zijn toegevoegd bij het aanmaken van de tankkaart!
            if(tankkaart.Brandstoffen != null) {

                string tankkaartBrandstofQuery = "INSERT INTO TankkaartBrandstof (TankkaartId,Brandstof) VALUES (@tankkaartid,@brandstof)";

                using(SqlCommand cmd2 = conn.CreateCommand()) {
                    conn.Open();
                    try {

                        cmd2.CommandText = tankkaartBrandstofQuery;
                        cmd2.Parameters.Add(new SqlParameter("@tankkaartId", SqlDbType.Int));
                        cmd2.Parameters.Add(new SqlParameter("@brandstof", SqlDbType.NVarChar));
                        cmd2.Parameters["@tankkaartId"].Value = insertedId;
                        foreach (string brandstof in tankkaart.Brandstoffen) {
                            cmd2.Parameters["@brandstof"].Value = brandstof;
                            cmd2.ExecuteNonQuery();
                        }
                    } catch (Exception ex) {
                        throw new TankkaartRepositoryException("TankkaartRepository : voegTankkaartToe - Er heeft zich een probleem voorgedaan!", ex);
                    } finally { conn.Close(); }
                }

            }
            



        }

        public void bewerkTankkaartBrandstof(int id,List<string> brandstof){

        }
    }
}
