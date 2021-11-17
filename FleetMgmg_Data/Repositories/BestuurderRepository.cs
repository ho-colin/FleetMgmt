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


        public IEnumerable<(Bestuurder, Tankkaart, Voertuig)> toonBestuurders(string rijksregisternummer, string naam, string voornamam, DateTime geboortedatum, bool strikt = true) {
            List<(Bestuurder, Tankkaart, Voertuig)> lijstbestuurder = new List<(Bestuurder, Tankkaart, Voertuig)>();
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "SELECT b.Rijksregisternummer, b.Naam, b.Voornaam, b.Geboortedatum FROM BESTUURDER b " +
                           "v.Brandstof, v.Chassisnummer, v.Kleur, v.AantalDeuren, v.Merk, v.Model, v.TypeVoertuig, v.Nummerplaat " +
                           "t.KaartNummer, t.GeldigheidsDatum, t.Pincode, t.Geblokkeerd " +
                            "LEFT JOIN Voertuig v on b.id = v.BestuurderId " +
                            "LEFT JOIN Tankkaart t ON b.TankkaartId = t.Id ";

            bool AND = false;
            bool WHERE = false;

            if (!string.IsNullOrWhiteSpace(rijksregisternummer)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                AND = true;
                if (strikt)
                    query += "Rijksregisternummer=@rijksregisternummer";
                else
                    query += " UPPER(Rijksregisternummer)=UPPER(rijksregisternummer)";
            }

            if (!string.IsNullOrWhiteSpace(naam)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += "Naam=@naam";
                else
                    query += " UPPER(Naam)=UPPER(@naam)";
            }

            if (!string.IsNullOrWhiteSpace(voornamam)) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += "Voornaam=@voornamam";
                else
                    query += " UPPER(voornamam)=UPPER(@voornamam)";
            }

            if (geboortedatum.GetHashCode() != 0) {
                if (!WHERE) query += "WHERE "; WHERE = true;
                if (AND) query += " AND "; else AND = true;
                if (strikt)
                    query += "geboortedatum=@geboortedatum";
                else
                    query += " UPPER(geboortedatum)=UPPER(@geboortedatum)";
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
                            string rijksregisternummerdb = (string)reader["Rijksregisternummer"];
                            string naamdb = (string)reader["Naam"];
                            string voornaamdb = (string)reader["Voornaam"];
                            DateTime geboortedatumdb = (DateTime)reader["Geboortedatum"];
                            int kaartnummerdb = (int)reader["Id"];
                            DateTime geldigheidsdatumdb = (DateTime)reader["Geldigheidsdatum"];
                            string pincodedb = (string)reader["Pincode"];
                            List<string> _brandstoffen = new List<string>();
                            string chassisnummerdb = (string)reader["Chassisnummer"];
                            string merkDb = (string)reader["Merk"];
                            string modeldb = (string)reader["Model"];
                            string nummerplaatdb = (string)reader["Nummerplaat"];
                            string brandstofdb = (string)reader["TypeVoertuig"];
                            TypeVoertuig typeVoertuigdb = new TypeVoertuig((string) reader["TypeRijbewijs"], RijbewijsEnum.B); //HIER NOG FIXEN DAT ER EEN RIJBEWIJS ENUM WORDT INGELEZEN
                            string kleurdb = (string)reader["Kleur"];
                            int aantalDeurendb = (int)reader["AantalDeuren"];

                            Bestuurder bestuurder = new Bestuurder(rijksregisternummerdb, naamdb, voornaamdb, geboortedatumdb);

                            Tankkaart tankkaart = new Tankkaart(kaartnummerdb, geldigheidsdatumdb, pincodedb);

                            Voertuig voertuig = new Voertuig((BrandstofEnum)Enum.Parse(typeof(Enum), brandstofdb), chassisnummerdb, kleurdb, aantalDeurendb,merkDb, modeldb, typeVoertuigdb, nummerplaatdb);

                            lijstbestuurder.Add((bestuurder, tankkaart, voertuig));
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
