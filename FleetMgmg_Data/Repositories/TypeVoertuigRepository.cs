using FleetMgmt_Business.Enums;
using FleetMgmt_Business.Exceptions;
using FleetMgmt_Business.Objects;
using FleetMgmt_Business.Repos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetMgmg_Data.Repositories {
    public class TypeVoertuigRepository : ITypeVoertuigRepository {
        
        //Update vereistRijbewijs, verkrijgt via type
        public TypeVoertuig updateTypeVoertuig(TypeVoertuig type) {
            string query = "UPDATE TypeVoertuig SET Rijbewijs=@rijbewijs WHERE TypeVoertuig=@type";
            SqlConnection conn = ConnectionClass.getConnection();
            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@type", type.Type);
                cmd.Parameters.AddWithValue("@rijbewijs", type.vereistRijbewijs.ToString());

                try {
                    cmd.ExecuteNonQuery();
                    return type;
                } catch (Exception ex) {
                    throw new TypeVoertuigException("TypeVoertuigRepository : updateTypeVoertuig", ex);
                } finally { conn.Close(); }
            }
        }

        // Verkrijgt exact TypeVoertuig
        public TypeVoertuig verkrijgTypeVoertuig(string type, RijbewijsEnum rijbewijs) {
            string query = "SELECT * FROM TypeVoertuig WHERE TypeVoertuig=@type AND Rijbewijs=@rijbewijs";
            TypeVoertuig typeVoertuigDB = null;
            SqlConnection conn = ConnectionClass.getConnection();
            using(SqlCommand cmd = conn.CreateCommand()) {
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@rijbewijs", rijbewijs.ToString());

                using(SqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        typeVoertuigDB = new TypeVoertuig((string) reader["TypeVoertuig"], (RijbewijsEnum) Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]));
                    }
                    reader.Close();
                }
                return typeVoertuigDB;
            }
        }

        // Verkrijgt alle TypeVoertuigen afhankelijk van opgegeven parameters, null ook mogelijk
        public ICollection<TypeVoertuig> verkrijgTypeVoertuigen(string type, RijbewijsEnum? rijbewijs) {
            List<TypeVoertuig> dbObjects = new List<TypeVoertuig>();
            SqlConnection conn = ConnectionClass.getConnection();
            StringBuilder query = new StringBuilder("SELECT * FROM TypeVoertuig ");

            bool WHERE = false;
            bool AND = false;

            if (!string.IsNullOrWhiteSpace(type)) {
                if (!WHERE) { WHERE = true; query.Append(" WHERE "); }
                if (AND) { query.Append(" AND "); }
                query.Append(" TypeVoertuig=@type ");              
                if (!AND) { AND = true; }
            }
            if (rijbewijs.HasValue) {
                if (!WHERE) { WHERE = true; query.Append(" WHERE "); }
                if (AND) { query.Append(" AND "); }
                query.Append(" Rijbewijs=@rijbewijs ");
                if (!AND) { AND = true; }
            }

            try {
                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = query.ToString();
                    if (query.ToString().Contains("@type")) cmd.Parameters.AddWithValue("@type", type);
                    if (query.ToString().Contains("@rijbewijs")) cmd.Parameters.AddWithValue("@rijbewijs", rijbewijs.Value.ToString());

                    using(SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            TypeVoertuig verkregenDB = null;
                            verkregenDB = new TypeVoertuig((string)reader["TypeVoertuig"], (RijbewijsEnum)Enum.Parse(typeof(RijbewijsEnum), (string)reader["Rijbewijs"]));
                            dbObjects.Add(verkregenDB);
                        }
                        reader.Close();
                    }
                }
                return dbObjects;
            } catch (Exception ex) {
                throw new TypeVoertuigException("TypeVoertuigRepository : verkrijgVoertuigen", ex);
            } finally { conn.Close(); }

        }

        //Verwijder voertuig afhankeljk van typevoertuig en typerijbewijs
        public void verwijderTypeVoertuig(TypeVoertuig type) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "DELETE FROM TypeVoertuig TypeVoertuig=@type AND Rijbewijs=@rijbewijs";
            using(SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@type", type.Type);
                    cmd.Parameters.AddWithValue("@rijbewijs", type.vereistRijbewijs.ToString());

                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new TypeVoertuigException("TypeVoertuigRepository : verwijderTypeVoertuig",ex);
                }
            }
        }

        //Voeg nieuw typevoertuig toe afhankelijk van TypeVoertuig object
        public void voegTypeVoertuigToe(TypeVoertuig type) {
            SqlConnection conn = ConnectionClass.getConnection();
            string query = "INSERT INTO TypeVoertuig VALUES (@type,@rijbewijs)";
            using (SqlCommand cmd = conn.CreateCommand()) {
                conn.Open();
                try {
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@type", type.Type);
                    cmd.Parameters.AddWithValue("@rijbewijs", type.vereistRijbewijs.ToString());

                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new TypeVoertuigException("TypeVoertuigRepository : voegTypeVoertuigToe", ex);
                }
            }
        }
    }
}
