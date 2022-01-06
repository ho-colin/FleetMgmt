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
    public class RijbewijsRepository : IRijbewijsRepository {
        public bool heeftRijbewijs(RijbewijsEnum r, Bestuurder b) {
            string query = "SELECT Count(*) FROM BestuurderRijbewijs WHERE Bestuurder=@bestuurder AND Categorie=@categorie";
            using(SqlConnection conn = ConnectionClass.getConnection()) {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("@bestuurder", b.Rijksregisternummer);
                    cmd.Parameters.AddWithValue("@categorie", r.ToString());

                    try {
                        int aantal = (int)cmd.ExecuteScalar();
                        if (aantal > 0) return true; return false;
                    } catch (Exception ex) {
                        throw new RijbewijsRepositoryException(ex.Message,ex);
                    } finally { conn.Close(); }
                }
            }
        }

        public Bestuurder toonRijbewijzen(Bestuurder b) {
            string query = "SELECT br.*,b.Naam,b.Achternaam,b.Geboortedatum FROM BestuurderRijbewijs br LEFT JOIN Bestuurder b ON br.Bestuurder=b.Rijksregisternummer WHERE Bestuurder=@bestuurder";
            using (SqlConnection conn = ConnectionClass.getConnection()) {
                using (SqlCommand cmd = new SqlCommand(query, conn)) {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@bestuurder", b.Rijksregisternummer);
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        Bestuurder bDb = null;
                        try {
                            while (reader.Read()) {
                                if (bDb == null) {
                                    string gevondenRijksregisternmr = (string)reader["Bestuurder"];
                                    string gevondenVoornaam = (string)reader["Naam"];
                                    string gevondenAchternaam = (string)reader["Achternaam"];
                                    DateTime gevondenGeboortedatum = (DateTime)reader["Geboortedatum"];
                                    bDb = new Bestuurder(gevondenRijksregisternmr, gevondenAchternaam, gevondenVoornaam, gevondenGeboortedatum);
                                }

                                DateTime gevondenBehaald = (DateTime)reader["Behaald"];
                                string gevondenCategorie = (string)reader["Categorie"];
                                bDb.voegRijbewijsToe(new Rijbewijs(gevondenCategorie, gevondenBehaald));
                            }
                            reader.Close();
                            return bDb;
                        } catch (Exception ex) {
                            throw new RijbewijsRepositoryException(ex.Message, ex);
                        } finally { conn.Close(); }
                    }
                }
            }
        }

        public void verwijderRijbewijs(RijbewijsEnum r, Bestuurder b) {
            if (!this.heeftRijbewijs(r, b)) throw new RijbewijsRepositoryException("Bestuurder heeft dit rijbewijs niet!");
            string query = "DELETE FROM BestuurderRijbewijs WHERE Categorie=@categorie AND Bestuurder=@bestuurder";
            using(SqlConnection conn = ConnectionClass.getConnection()) {
                try {
                    using (SqlCommand cmd = new SqlCommand(query, conn)) {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@categorie", r.ToString());
                        cmd.Parameters.AddWithValue("@bestuurder", b.Rijksregisternummer);
                        cmd.ExecuteNonQuery();
                    }
                } catch (Exception ex) {
                    throw new RijbewijsRepositoryException(ex.Message, ex);
                } finally { conn.Close(); }
            }
        }

        public void voegRijbewijsToe(Rijbewijs r, Bestuurder b) {
            if (this.heeftRijbewijs(r.Categorie, b)) throw new RijbewijsRepositoryException("Bestuurder heeft dit rijbewijs al!");
            string query = "INSERT INTO BestuurderRijbewijs (Bestuurder,Categorie,Behaald) VALUES (@bestuurder,@categorie,@behaald)";
            using(SqlConnection conn = ConnectionClass.getConnection()) {
                using(SqlCommand cmd = new SqlCommand(query, conn)) {
                    try {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@bestuurder", b.Rijksregisternummer);
                        cmd.Parameters.AddWithValue("@categorie", r.Categorie.ToString());
                        cmd.Parameters.AddWithValue("@behaald", r.BehaaldOp.ToString("yyyy-MM-dd"));
                        cmd.ExecuteNonQuery();
                    } catch (Exception ex) {
                        throw new RijbewijsRepositoryException(ex.Message, ex);
                    } finally { conn.Close(); }
                }
            }
        }
    }
}
