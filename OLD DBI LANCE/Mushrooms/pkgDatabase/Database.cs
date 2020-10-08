using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private SqlConnection db;
        private SqlTransaction trx;
        private IsolationLevel isolationLevel;

        private Database()
        {
            
        }

        public void Connect()
        {
            db = new SqlConnection(CONNECTION_STRING);
            db.Open();

            isolationLevel = IsolationLevel.ReadCommitted;
            trx = db.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            trx.Commit();
            trx = db.BeginTransaction(isolationLevel);
        }

        public void Rollback()
        {
            trx.Rollback();
            trx = db.BeginTransaction(isolationLevel);
            RefreshMushrooms();
        }

        public void Add(Mushroom mushroom)
        {
            const string SQL = 
                "INSERT INTO mushrooms(name, useful, cap_color, stem_color, descr, imagepath) " +
                "OUTPUT INSERTED.ID " +
                "VALUES(@name, @useful, @capColor, @stemColor, @description, @imagePath)";

            SqlCommand cmd = new SqlCommand(SQL, db, trx);
            cmd.Parameters.Add("name", SqlDbType.VarChar).Value = mushroom.Name;
            cmd.Parameters.Add("useful", SqlDbType.VarChar).Value = mushroom.Useful.ToString();
            cmd.Parameters.Add("capColor", SqlDbType.VarChar).Value = mushroom.CapColor;
            cmd.Parameters.Add("stemColor", SqlDbType.VarChar).Value = mushroom.StemColor;
            cmd.Parameters.Add("description", SqlDbType.VarChar).Value = mushroom.Description;
            cmd.Parameters.Add("imagePath", SqlDbType.VarChar).Value = mushroom.ImagePath;

            int id = (int)cmd.ExecuteScalar();

            mushroom.Id = id;
            mushroom.Modified = false;
            Mushrooms.Add(mushroom);
        }

        public void Add(Discovery discovery)
        {
            const string SQL =
                "INSERT INTO discoveries(idMushroom, dateDiscovery, geopoint, x, y) " +
                "OUTPUT INSERTED.ID " +
                "VALUES(@idMushroom, @dateDiscovery, @geopoint, @x, @y)";

            SqlCommand cmd = new SqlCommand(SQL, db, trx);
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

            SqlCommand cmd = new SqlCommand(SQL, db, trx);
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
            SqlCommand cmd = new SqlCommand(SQL, db, trx);
            cmd.Parameters.Add("id", SqlDbType.Int).Value = mushroom.Id;

            cmd.ExecuteNonQuery();
            Mushrooms.Remove(mushroom);
        }

        public void RefreshMushrooms()
        {
            const string SQL = "SELECT * FROM mushrooms ORDER BY id";
            SqlCommand query = new SqlCommand(SQL, db, trx);
            SqlDataReader row = query.ExecuteReader();

            Mushrooms.Clear();
            while (row.Read())
            {
                Mushroom mushroom = new Mushroom();
                mushroom.Id = row.GetInt32(0);
                mushroom.Name = row.GetString(1);
                mushroom.Useful = (MushroomUseful)Enum.Parse(typeof(MushroomUseful), row.GetString(2));
                mushroom.CapColor = row.GetString(3);
                mushroom.StemColor = row.GetString(4);
                mushroom.Description = row.GetString(5);
                mushroom.ImagePath = row.GetString(6);
                mushroom.Modified = false;

                Mushrooms.Add(mushroom);
            }

            row.Close();
        }

        public void RefreshDiscoveries()
        {
            const string SQL = "SELECT id, idMushroom, dateDiscovery, geopoint.Long, geopoint.Lat, x, y FROM discoveries ORDER BY id";
            SqlCommand query = new SqlCommand(SQL, db, trx);
            SqlDataReader row = query.ExecuteReader();

            Discoveries.Clear();
            while (row.Read())
            {
                Discovery discovery = new Discovery();
                discovery.Id = row.GetInt32(0);
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
            SqlCommand query = new SqlCommand(SQL, db, trx);

            SqlParameter geopoint = new SqlParameter
            {
                ParameterName = "hunter",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.Point(latitude, longitude, 4326)
            };
            query.Parameters.Add(geopoint);

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

        public List<Discovery> GetNeighbors(double latitude, double longitude, int amount)
        {
            const string SQL = "SELECT TOP(@amount) id, idMushroom, dateDiscovery, geopoint.Long, geopoint.Lat, x, y, geopoint.STDistance(@hunter) " +
                               "FROM discoveries ORDER BY geopoint.STDistance(@hunter)";

            SqlCommand query = new SqlCommand(SQL, db, trx);
            query.Parameters.Add("amount", SqlDbType.Int).Value = amount;
            SqlParameter geopoint = new SqlParameter
            {
                ParameterName = "hunter",
                SqlDbType = SqlDbType.Udt,
                UdtTypeName = "geography",
                Value = SqlGeography.Point(latitude, longitude, 4326)
            };
            query.Parameters.Add(geopoint);

            SqlDataReader row = query.ExecuteReader();
            List<Discovery> neighbors = new List<Discovery>();
            int counter = 1;

            while (row.Read())
            {
                Discovery discovery = new Discovery();
                discovery.Id = row.GetInt32(0);
                int mushroomId = row.GetInt32(1);
                discovery.Mushroom = Mushrooms.Where((mushroom) => mushroom.Id == mushroomId).First();
                discovery.Date = row.GetDateTime(2);
                discovery.Longitude = row.GetDouble(3);
                discovery.Latitude = row.GetDouble(4);
                discovery.X = row.GetInt32(5);
                discovery.Y = row.GetInt32(6);
                discovery.Remark = $"Distance: {row.GetDouble(7)} ({counter++} seq)";

                neighbors.Add(discovery);
            }

            row.Close();
            return neighbors;
        }

        public void StoreAsXml()
        {

            TextWriter writer = new StreamWriter(FILENAME_XML);
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            serializer.Serialize(writer, this);
            writer.Close();
        }
    }
}
