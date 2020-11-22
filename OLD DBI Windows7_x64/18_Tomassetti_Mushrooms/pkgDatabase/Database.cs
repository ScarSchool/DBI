using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace pkgDatabase
{
    public class Database
    {
        public static readonly Database Instance = new Database();

        private static readonly String FILENAME_XML = "../../../mushrooms.xml";
        private static readonly String CONNECTION_STRING =
            "Data Source=(localdb)\\MSSQLLocalDB;" +
            "Initial Catalog=schuelerdb;" +
            "Integrated Security=True;" +
            "Pooling=False";

        public readonly ObservableCollection<Mushroom> Mushrooms = new ObservableCollection<Mushroom>();
        public readonly ObservableCollection<Discovery> Discoveries = new ObservableCollection<Discovery>();

        private SqlConnection con;
        private SqlTransaction transaction;
        private IsolationLevel isolationLevel;

        private Database() { }

        public void Connect()
        {
            con = new SqlConnection(CONNECTION_STRING);
            con.Open();

            isolationLevel = IsolationLevel.ReadCommitted;
            transaction = con.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            transaction.Commit();
            transaction = con.BeginTransaction(isolationLevel);
        }

        public void Rollback()
        {
            transaction.Rollback();
            transaction = con.BeginTransaction(isolationLevel);
            RefreshMushrooms();
        }

        public void Add(Mushroom mushroom)
        {
            const string SQL =
                "INSERT INTO mushrooms(name, useful, cap_color, stem_color, descr, imagepath) " +
                "OUTPUT INSERTED.ID " +
                "VALUES(@name, @useful, @capColor, @stemColor, @description, @imagePath)";

            SqlCommand cmd = new SqlCommand(SQL, con, transaction);
            cmd.Parameters.Add("name", SqlDbType.VarChar).Value = mushroom.Name;
            cmd.Parameters.Add("useful", SqlDbType.VarChar).Value = mushroom.Useful.ToString();
            cmd.Parameters.Add("capColor", SqlDbType.VarChar).Value = mushroom.CapColor;
            cmd.Parameters.Add("stemColor", SqlDbType.VarChar).Value = mushroom.StemColor;
            cmd.Parameters.Add("description", SqlDbType.VarChar).Value = mushroom.Description;
            cmd.Parameters.Add("imagePath", SqlDbType.VarChar).Value = mushroom.ImagePath;

            int id = (int)cmd.ExecuteScalar();

            mushroom.Id = id;
            mushroom.Updated = false;
            Mushrooms.Add(mushroom);
        }

        public void Add(Discovery discovery)
        {
            const string SQL =
                "INSERT INTO discoveries(idMushroom, dateDiscovery, geopoint, x, y) " +
                "OUTPUT INSERTED.ID " +
                "VALUES(@idMushroom, @dateDiscovery, @geopoint, @x, @y)";

            SqlCommand cmd = new SqlCommand(SQL, con, transaction);
            cmd.Parameters.Add("idMushroom", SqlDbType.Int).Value = discovery.Mushroom.Id;
            cmd.Parameters.Add("dateDiscovery", SqlDbType.Date).Value = discovery.Date;

            SqlParameter geopoint = new SqlParameter
            {
                ParameterName = "geopoint",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.Point(discovery.Latitude, discovery.Longitude, 4326)
            };
            cmd.Parameters.Add(geopoint);

            cmd.Parameters.Add("x", SqlDbType.Int).Value = discovery.X;
            cmd.Parameters.Add("y", SqlDbType.Int).Value = discovery.Y;

            int id = (int)cmd.ExecuteScalar();

            discovery.Id = id;
            Discoveries.Add(discovery);
        }

        public void Update(Mushroom mushroom)
        {
            const string SQL =
                "UPDATE mushrooms SET name = @name, useful = @useful, cap_color = @capColor, stem_color = @stemColor, descr = @description, imagepath = @imagePath " +
                "WHERE id = @id";

            SqlCommand cmd = new SqlCommand(SQL, con, transaction);
            cmd.Parameters.Add("name", SqlDbType.VarChar).Value = mushroom.Name;
            cmd.Parameters.Add("useful", SqlDbType.VarChar).Value = mushroom.Useful.ToString();
            cmd.Parameters.Add("capColor", SqlDbType.VarChar).Value = mushroom.CapColor;
            cmd.Parameters.Add("stemColor", SqlDbType.VarChar).Value = mushroom.StemColor;
            cmd.Parameters.Add("description", SqlDbType.VarChar).Value = mushroom.Description;
            cmd.Parameters.Add("imagePath", SqlDbType.VarChar).Value = mushroom.ImagePath;
            cmd.Parameters.Add("id", SqlDbType.Int).Value = mushroom.Id;

            cmd.ExecuteNonQuery();
        }

        public void Remove(Mushroom mushroom)
        {
            const string SQL = "DELETE FROM mushrooms WHERE id = @id";
            SqlCommand cmd = new SqlCommand(SQL, con, transaction);
            cmd.Parameters.Add("id", SqlDbType.Int).Value = mushroom.Id;

            cmd.ExecuteNonQuery();
            Mushrooms.Remove(mushroom);
        }

        public void RefreshMushrooms()
        {
            const string SQL = "SELECT * FROM mushrooms ORDER BY id";
            SqlCommand query = new SqlCommand(SQL, con, transaction);
            SqlDataReader row = query.ExecuteReader();

            Mushrooms.Clear();
            while (row.Read())
            {
                Mushroom mushroom = new Mushroom
                {
                    Id = row.GetInt32(0),
                    Name = row.GetString(1),
                    Useful = (MushroomUseful)Enum.Parse(typeof(MushroomUseful), row.GetString(2)),
                    CapColor = row.GetString(3),
                    StemColor = row.GetString(4),
                    Description = row.GetString(5),
                    ImagePath = row.GetString(6),
                    Updated = false
                };

                Mushrooms.Add(mushroom);
            }

            row.Close();
        }

        public void RefreshDiscoveries()
        {
            const string SQL = "SELECT id, idMushroom, dateDiscovery, geopoint.Long, geopoint.Lat, x, y FROM discoveries ORDER BY id";
            SqlCommand query = new SqlCommand(SQL, con, transaction);
            SqlDataReader row = query.ExecuteReader();

            Discoveries.Clear();
            while (row.Read())
            {
                Discovery discovery = new Discovery
                {
                    Id = row.GetInt32(0)
                };
                int mushroomId = row.GetInt32(1);
                discovery.Mushroom = Mushrooms.Where((mushroom) => mushroom.Id == mushroomId).First();
                discovery.Date = row.GetDateTime(2);
                discovery.Longitude = row.GetDouble(3);
                discovery.Latitude = row.GetDouble(4);
                discovery.X = row.GetInt32(5);
                discovery.Y = row.GetInt32(6);

                Discoveries.Add(discovery);
            }

            row.Close();
        }

        public void CalculateDistances(double latitude, double longitude)
        {
            const string SQL = "SELECT id, geopoint.STDistance(@hunter) FROM discoveries ORDER BY id";
            SqlCommand query = new SqlCommand(SQL, con, transaction);

            
            query.Parameters.Add(GetGeopoint(longitude, latitude));

            SqlDataReader row = query.ExecuteReader();
            while (row.Read())
            {
                int id = row.GetInt32(0);
                double distance = row.GetDouble(1);

                Discovery discovery = Discoveries.Where((_discovery) => _discovery.Id == id).First();
                discovery.Remark = $"Distance: {distance}";
            }

            row.Close();
        }

        public void StoreAsXml()
        {
            TextWriter writer = new StreamWriter(FILENAME_XML);
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public void GetNeighbours(double longitude, double latitude, int neighbourAmount)
        {
            const string SQL = "SELECT TOP(@amount) id, idMushroom, dateDiscovery, geopoint.Long, geopoint.Lat, x, y, geopoint.STDistance(@hunter) " +
                "FROM discoveries ORDER BY geopoint.STDistance(@hunter);";

            SqlCommand query = new SqlCommand(SQL, con, transaction);

            SqlParameter amount = new SqlParameter
            {
                ParameterName = "amount",
                SqlDbType = SqlDbType.Int,
                Value = neighbourAmount
            };
            query.Parameters.Add(amount);
            query.Parameters.Add(GetGeopoint(longitude, latitude));

            List<Discovery> neighbours = new List<Discovery>();

            RefreshDiscoveries();

            Discoveries.Clear();

            SqlDataReader row = query.ExecuteReader();
            try
            {
                while (row.Read())
                {
                    int size = row.FieldCount;
                    Discoveries.Add(new Discovery
                    {
                        Id = row.GetInt32(0),
                        Mushroom = Mushrooms.Where((mushroom) => mushroom.Id == row.GetInt32(1)).First(),
                        Date = row.GetDateTime(2),
                        Longitude = row.GetDouble(3),
                        Latitude = row.GetDouble(4),
                        X = row.GetInt32(5),
                        Y = row.GetInt32(6),
                        Remark = "Nr. " + (Discoveries.Count() + 1) + "  Distance to Hunter: " + row.GetDouble(7)
                    });
                }

                row.Close();
            }
            catch (Exception) { throw; } finally { row.Close(); }
        }

        public bool IfSelectRectCollide(string r1, string r2)
        {
            const string SQL = "SELECT @FirstNote.MakeValid().STOverlaps(@SecondNote.MakeValid());";

            SqlCommand query = new SqlCommand(SQL, con, transaction);

            SqlParameter FirstNote = new SqlParameter
            {
                ParameterName = "FirstNote",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.STPolyFromText(new System.Data.SqlTypes.SqlChars(r1), 4326)
            };

            SqlParameter SecondNote = new SqlParameter
            {
                ParameterName = "SecondNote",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.STPolyFromText(new System.Data.SqlTypes.SqlChars(r2), 4326)
            };

            query.Parameters.Add(FirstNote);
            query.Parameters.Add(SecondNote);


            bool returnBool = false;
            SqlDataReader row = query.ExecuteReader();
            try
            {
                if (row.Read())
                {
                    returnBool = row.GetBoolean(0);
                }
                
            }
            catch (Exception) { throw; } finally { row.Close(); }

            return returnBool;
        }        

        private SqlParameter GetGeopoint (double longitude, double latitude)
        {
            return new SqlParameter
            {
                ParameterName = "hunter",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.Point(latitude, longitude, 4326)
            };
        }
    }
}
