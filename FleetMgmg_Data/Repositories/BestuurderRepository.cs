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
                    throw new BestuurderRepositoryException("BestuurderRepository: bestaatBestuurder - Er werd geen bestuurder gevonden!");
                }
                finally {
                    conn.Close();
                }
            }
        }

        public void bewerkBestuurder(Bestuurder bestuurder) {  //NULLABLE, NIETS INVULLEN IS ALLEMAAL!!! VOORBEELD? = LIJN 28!
            throw new NotImplementedException();
        }

        public void geefBestuurder(int id) { // LEFT JOIN 2 maal
            throw new NotImplementedException();
        }

        public IEnumerable<Bestuurder> toonBestuurders() { //NULLABLE, NIETS INVULLEN IS ALLEMAAL!!! VOORBEELD? = LIJN 28!
            throw new NotImplementedException();
        }

        public void verwijderBestuurder(int id) {
            SqlConnection conn = ConnectionClass.getConnection();
            try {
                conn.Open();
                string sql = $"DELETE FROM Bestuurder WHERE Id = " + id;
                var updateCommand = new SqlCommand(sql, conn);
                updateCommand.ExecuteNonQuery();
            }
            catch (Exception e) {
                throw new BestuurderRepositoryException("BestuurdersRepository: verwijderBestuurder - Er werd geen bestuurder verwijderd!");
            }
            finally {
                conn.Close();
                conn.Dispose();
            }
        }

        public void voegBestuurderToe(Bestuurder bestuurder) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "INSERT INTO Bestuurder(Naam, Achternaam, Geboortedatum, Rijksregisternummer) VALUES(@Naam, @Achternaam, @Geboortedatum, @Rijksregisternummer, " +
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
                    throw new BestuurderRepositoryException("BestuurderRepository: voegBestuurderToe - Geen bestuurder werd toegevoegd!");
                }
                finally {
                    conn.Close();
                }
            }

        }
    }
}
