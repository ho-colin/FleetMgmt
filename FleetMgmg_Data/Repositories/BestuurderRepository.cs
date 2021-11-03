using FleetMgmg_Data.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Repositories {
    public class BestuurderRepository : IBestuurderRepository {
        public bool bestaatBestuurder(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT COUNT(1) FROM [Bestuurder] WHERE Id = @Id";
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
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "UPDATE Bestuurder SET Rijksregisternummer=@Rijksregisternummer, Naam=@Naam, Voornaam=@Voornaam, Geboortedatum=@Geboortedatum WHERE Id=Id";
            using(SqlCommand comm = conn.CreateCommand()) {
                conn.Open();
                try {
                    comm.Parameters.Add(new SqlParameter("@Rijksregisternummer", System.Data.SqlDbType.NVarChar));
                    comm.Parameters.Add(new SqlParameter("@Naam", System.Data.SqlDbType.NVarChar));
                    comm.Parameters.Add(new SqlParameter("@Voornaam", System.Data.SqlDbType.NVarChar));
                    comm.Parameters.Add(new SqlParameter("@Geboortedatum", System.Data.SqlDbType.Date));
                    comm.CommandText = query;
                    comm.Parameters["@Naam"].Value = bestuurder.Voornaam;
                    comm.Parameters["@Achternaam"].Value = bestuurder.Naam;
                    comm.Parameters["@Geboortedatum"].Value = bestuurder.GeboorteDatum;
                    comm.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                }catch(Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: bewerkBestuurder - Bestuurder werd niet bewerkt!", ex);
                }
                finally {
                    conn.Close();
                }
            }

        }

        public void geefBestuurder(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT b.id,b.naam,b.achternaamb.rijksregisternummer FROM Bestuurder b LEFT JOIN BestuurderRijbewijs br ON b.id = br.Id WHERE Id=@Id";
            using(SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.NVarChar));
                    cmd.CommandText = query;
                    cmd.Parameters["@Id"].Value = id;
                }
                catch(Exception ex) {
                    throw new BestuurderRepositoryException("BestuurderRepository: geefBestuurder - Bestuurder werd niet gevonden!", ex);
                }
                finally {
                    conn.Close();
                }
            }
        }

        public IEnumerable<Bestuurder> toonBestuurders(string rijksregisternummer, string naam, string voornamam, DateTime geboortedatum) {
            List<Bestuurder> lijstbestuurder = new List<Bestuurder>();
            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder("SELECT * FROM Bestuurder");

            bool and = false;
            bool where = false;

            if (!string.IsNullOrWhiteSpace(rijksregisternummer)) {
                if (!where) query.Append(" WHERE "); where = true;
                query.Append(" rijksregisternummer=@Rijksregisternummer");
                and = true;
            }

            if (!string.IsNullOrWhiteSpace(naam)) {
                if (!where) query.Append(" WHERE "); where = true;
                query.Append(" naam=@Naam");
                and = true;
            }

            if (!string.IsNullOrWhiteSpace(voornamam)) {
                if (!where) query.Append(" WHERE "); where = true;
                query.Append(" voornaam=@Voornaam");
                and = true;
            }

            if (geboortedatum.GetHashCode() == 0) {
                if (!where) query.Append(" WHERE "); where = true;
                query.Append(" geboortedatum=@Geboortedatum");
                and = true;
            }


            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query.ToString();

                if (query.ToString().Contains("@Rijksregisternummer")) cmd.Parameters.AddWithValue("@Rijksregisternummer", rijksregisternummer);
                if (query.ToString().Contains("@Naam")) cmd.Parameters.AddWithValue("@Naam", naam);
                if (query.ToString().Contains("@Voornaam")) cmd.Parameters.AddWithValue("@Voornaam", voornamam);
                if (query.ToString().Contains("@Geboortedatum")) cmd.Parameters.AddWithValue("@Geboortedatum", geboortedatum);

                using (SqlDataReader reader = cmd.ExecuteReader()) {

                    if (reader.HasRows) {
                        while (reader.Read()) {
                            lijstbestuurder.Add(new Bestuurder((string)reader["Rijksregisternummer"], (string)reader["Naam"], (string)reader["Voornaam"], Convert.ToDateTime(reader["Geboortedatum"])));
                        }
                        conn.Close();
                    }
                    else throw new BestuurderRepositoryException("BestuurderRepository : toonBestuurder - Geen bestuurders gevonden!");
                }
            }

            return lijstbestuurder;

        }

        public void verwijderBestuurder(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            try {
                conn.Open();
                string sql = $"DELETE FROM Bestuurder WHERE Id = " + id;
                var updateCommand = new SqlCommand(sql, conn);
                updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex) {
                throw new BestuurderRepositoryException("BestuurdersRepository: verwijderBestuurder - Er werd geen bestuurder verwijderd!", ex);
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "INSERT INTO Bestuurder(Naam, Achternaam, Geboortedatum, Rijksregisternummer) VALUES(@Naam, @Achternaam, @Geboortedatum, " +
                "@Rijksregisternummer, " +
                ")";
            using(SqlCommand cmd = conn.CreateCommand()) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@Naam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@Achternaam", System.Data.SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@Geboortedatum", System.Data.SqlDbType.Date));
                    cmd.Parameters.Add(new SqlParameter("@Rijksregisternummer", System.Data.SqlDbType.NVarChar));
                    cmd.CommandText = query;
                    cmd.Parameters["@Naam"].Value = bestuurder.Voornaam;
                    cmd.Parameters["@Achternaam"].Value = bestuurder.Naam;
                    cmd.Parameters["@Geboortedatum"].Value = bestuurder.GeboorteDatum;
                    cmd.Parameters["@Rijksregisternummer"].Value = bestuurder.Rijksregisternummer;
                    cmd.ExecuteNonQuery();
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
