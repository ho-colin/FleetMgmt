using FleetMgmg_Data.Exceptions;
using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public Tankkaart geefTankkaart(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT t.Id,t.GeldigDatum,t.Pincode,t.BestuurderId,t.Geblokkeerd,tb.Brandstof FROM Tankkaart t LEFT JOIN TankkaartBrandstof tb ON t.Id = tb.TankkaartId WHERE Id=@id";
            return null;

        }

        public IEnumerable<Tankkaart> geefTankkaarten(int? id, DateTime? geldigheidsDatum, string bestuurderId, bool? geblokkeerd, Brandstof? brandstof) {
            List<Tankkaart> kaarten = new List<Tankkaart>();
            SqlConnection conn = ConnectionClass.getConnection();

            StringBuilder query = new StringBuilder("SELECT t.Id Id,t.Pincode Pincode,t.GeldigDatum GeldigDatum,t.BestuurderId BestuurderId,t.Geblokkeerd Geblokkeerd,tb.Brandstof Brandstof " +
                "FROM Tankkaart t " +
                "LEFT JOIN TankkaartBrandstof tb " +
                "ON t.Id = tb.TankkaartId");

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
                            while (reader.Read()) {

                                #region Als ID NIET hetzelfde is als vorige row
                                if ((int)reader["Id"] != laatsGebruiktId) {

                                    #region object waarde resetten
                                    //Checken of dbObject al een waarde heeft
                                    //Zoja die dan toevoegen aan list en terug op null plaatsen
                                    if (dbObject != null) {
                                        kaarten.Add(dbObject); //Toevoegen aan de te returnen list
                                        dbObject = null; //Terug op null zetten
                                    }
                                    #endregion

                                    laatsGebruiktId = (int)reader["Id"];
                                    dbObject = new Tankkaart(
                                        ((int)reader["Id"]).ToString(),
                                        (DateTime)reader["GeldigDatum"],
                                        (string)reader["Pincode"],
                                        null,
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
            return kaarten;
        }

        public void verwijderTankkaart(int id) {
            throw new NotImplementedException();
        }

        public void voegTankkaartToe(Tankkaart tankkaart) {
            throw new NotImplementedException();
        }
    }
}
