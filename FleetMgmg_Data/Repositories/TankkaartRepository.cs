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
            int kaartId = Int32.Parse(tankkaart.KaartNummer);
            Tankkaart huidigeKaart = geefTankkaarten(kaartId, null, null, null, null).ElementAt(0);

            bool ietsVerandert = false;

            StringBuilder query = new StringBuilder("UPDATE Tankkaart SET ");


            if(tankkaart.InBezitVan == null) {
                if(huidigeKaart.InBezitVan != null) {
                    ietsVerandert = true;
                    query.Append(" BestuurderId=@BestuurderIdLEEG ");
                }
            } else { // tankkaart.inbezitvan WEL een value heeft
                if(huidigeKaart.InBezitVan == null) {
                    ietsVerandert = true;
                    query.Append(" BestuurderId=@bestuurderId ");
                }
            }

            if(tankkaart.Geblokkeerd != huidigeKaart.Geblokkeerd) {
                ietsVerandert = true;
                query.Append(" Geblokkeerd=@geblokkeerd ");
            }

            if(tankkaart.Pincode != huidigeKaart.Pincode) {
                ietsVerandert = true;
                query.Append(" Pincode=@pincode ");
            }

            if(tankkaart.GeldigheidsDatum != huidigeKaart.GeldigheidsDatum) {
                ietsVerandert = true;
                query.Append(" GeldigDatum=@geldigdatum ");
            }

            query.Append(" WHERE id=@id");

            if (!ietsVerandert) throw new TankkaartRepositoryException("Er is niks verandert!");

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

        public IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurderId, bool? geblokkeerd, Brandstof? brandstof) {
            List<Tankkaart> kaarten = new List<Tankkaart>();
            SqlConnection conn = ConnectionClass.getConnection();

            #region TANKKAART AANMAKEN
            StringBuilder query = new StringBuilder("SELECT t.Id Id,t.Pincode Pincode,t.GeldigDatum GeldigDatum,t.BestuurderId BestuurderId,t.Geblokkeerd Geblokkeerd,tb.Brandstof Brandstof, b.Id BestuurderTabelId, b.Naam Naam, b.Achternaam Achternaam, b.Geboortedatum Geboortedatum, b.Rijksregisternummer Rijksregisternummer " +
                "FROM Tankkaart t " +
                "LEFT JOIN TankkaartBrandstof tb " +
                "ON t.Id = tb.TankkaartId " +
                "LEFT JOIN Bestuurder b "+
                "ON t.BestuurderId = b.Id");

            bool where = false;
            bool and = false;

            if(id != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.id=@id ");              
            }

            if(geldigheidsDatum != null) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.GeldigDatum=@geldigheidsDatum");
            }

            if (!string.IsNullOrWhiteSpace(bestuurderId)) {
                if (!where) query.Append(" WHERE "); where = true;
                if (and) query.Append(" AND "); else and = true;
                query.Append(" t.BestuurderId=@bestuurderId");
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
        
            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                if (query.ToString().Contains("@id")) cmd.Parameters.AddWithValue("@id", id.Value);
                if (query.ToString().Contains("@geldigheidsDatum")) cmd.Parameters.AddWithValue("@geldigheidsDatum", geldigheidsDatum.Value.ToString("yyyy-MM-dd"));
                if (query.ToString().Contains("@bestuurderId")) cmd.Parameters.AddWithValue("@bestuurderId", bestuurderId);
                if (query.ToString().Contains("@geblokkeerd")) cmd.Parameters.AddWithValue("@geblokkeerd", geblokkeerd.Value ? "1" : "0");
                if (query.ToString().Contains("@brandstof")) cmd.Parameters.AddWithValue("@brandstof", brandstof.ToString());

                conn.Open();
                try {
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                            int laatsGebruiktId = 0;
                            Tankkaart dbObject = null;
                            Bestuurder bestuurder = null;
                            while (reader.Read()) {

                                #region Als ID NIET hetzelfde is als vorige row
                                if ((int)reader["Id"] != laatsGebruiktId) {

                                    #region object waardes resetten
                                    //Checken of dbObject al een waarde heeft
                                    //Zoja die dan toevoegen aan list en terug op null plaatsen
                                    if (dbObject != null) {
                                        kaarten.Add(dbObject); //Toevoegen aan de te returnen list
                                        dbObject = null; //Terug op null zetten
                                        bestuurder = null;
                                    }
                                #endregion

                                if (reader["rijksregisternummer"] != DBNull.Value) {
                                    bestuurder = new Bestuurder(
                                   (int)reader["BestuurderTabelId"],
                                   (string)reader["rijksregisternummer"],
                                   (string)reader["naam"],
                                   (string)reader["achternaam"],
                                   (DateTime)reader["geboortedatum"]);
                                }

                                    laatsGebruiktId = (int)reader["Id"];
                                    dbObject = new Tankkaart(
                                        ((int)reader["Id"]).ToString(),
                                        (DateTime)reader["GeldigDatum"],
                                        (string)reader["Pincode"],
                                        bestuurder,
                                        new List<string>());
                                }
                                #endregion

                                #region Als ID hetzelfde is als vorige row
                                if ((int)reader["Id"] == laatsGebruiktId) {
                                if (reader.IsDBNull(reader.GetOrdinal("Brandstof"))) break;
                                    try {
                                        dbObject.Brandstoffen.Add((string)reader["Brandstof"]);
                                    } catch (Exception ex) {
                                        throw new TankkaartRepositoryException("TankkaartRepository: geefTankkaarten, Brandstoffen.Add : Er heeft zich een fout voorgedaan!", ex);
                                    }
                                }
                                #endregion
                            }
                            reader.Close();

                        #region Laatste ook nog toevoegen
                        if (dbObject != null) kaarten.Add(dbObject);
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

            string inbezitvanId = null;
            if(tankkaart.InBezitVan != null) {
                inbezitvanId = tankkaart.InBezitVan.Id.ToString();

                IEnumerable<Tankkaart> listje = geefTankkaarten(null, tankkaart.GeldigheidsDatum, inbezitvanId, tankkaart.Geblokkeerd, null);

                if (listje.Count() > 0) throw new TankkaartRepositoryException("TankkaartRepositoryException : voegTankkaartToe - Opgegeven bestuurder heeft al een tankkaart!");
            }

            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder("INSERT INTO Tankkaart (GeldigDatum, Geblokkeerd");

            #region Optionele Te Vullen Velden
            if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                query.Append(", Pincode ");
            }

            if (!string.IsNullOrWhiteSpace(inbezitvanId)) {
                query.Append(", BestuurderId ");
            }
            #endregion
            query.Append(") OUTPUT inserted.Id VALUES (@geldigdatum,@geblokkeerd");

            #region Opionele Te vullen Velden Parameter variabels
            if (!string.IsNullOrWhiteSpace(tankkaart.Pincode)) {
                query.Append(",@pincode");
            }

            if (!string.IsNullOrWhiteSpace(inbezitvanId)) {
                query.Append("@bestuurderid");
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
                    if (query.ToString().Contains("@bestuurderid")) cmd.Parameters.AddWithValue("@bestuurderid", tankkaart.InBezitVan.Id);

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

        //TODO
        //public void bewerkTankkaartBrandstof(List<string> brandstof){}
    }
}
