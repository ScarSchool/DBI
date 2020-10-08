using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PkgData
{
    public class Database
    {
        private static readonly string FILENAME_BINARY = "../../../car.bin";
        private static readonly string FILENAME_XML = "../../../car.xml";

        private static readonly String SELECT_CARS = "SELECT carid, carname, cartype FROM cars ORDER BY carname";
        private static readonly String INSERT_CAR = "INSERT INTO cars VALUES(:carid, :carname, :cartype)";
        private static readonly String DELETE_CAR = "DELETE FROM cars WHERE carid = :carid";
        private static readonly String UPDATE_CAR = "UPDATE cars SET carname = :carname, cartype = :cartype WHERE carid = :carid";
        private OracleConnection conn;
        private OracleTransaction trx;
        private readonly string CONNSTRING = @"user id={0};password={1};data source=" +
                                             "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                                             "(HOST={2})(PORT=1521))(CONNECT_DATA=" +
                                             "(SERVICE_NAME=ora11g)))";
        public string Db_Host { get; set; }
        public string Db_User { get; set; }
        public string Db_Password { get; set; }

        public ObservableCollection<Car> collCars = new ObservableCollection<Car>();
        public readonly static Database Instance = new Database();

        public ObservableCollection<Car> GetAllCars()
        {
            return collCars;
        }

        public void BeginTrx()
        {
            if (conn == null)
            {
                conn = new OracleConnection(String.Format(CONNSTRING, Db_User, Db_Password, Db_Host));
            }

            conn.Open();
            trx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTrx()
        {
            trx.Commit();
            conn.Close();
        }

        public void RollbackTrx()
        {
            trx.Rollback();
            conn.Close();
        }

        public void FillAllCars()
        {
            collCars.Clear();
            OracleCommand cmd = new OracleCommand(SELECT_CARS, conn);
            OracleDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                Car car = new Car(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                collCars.Add(car);
            }

            reader.Close();
        }

        public void AddCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(INSERT_CAR, conn);

            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;
            cmd.Parameters.Add("carname", OracleDbType.Varchar2).Value = car.CarName;
            cmd.Parameters.Add("cartype", OracleDbType.Varchar2).Value = car.Type.ToString();

            cmd.ExecuteNonQuery();
            FillAllCars();
        }

        public void UpdateCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(UPDATE_CAR, conn);

            cmd.Parameters.Add("carname", OracleDbType.Varchar2).Value = car.CarName;
            cmd.Parameters.Add("cartype", OracleDbType.Varchar2).Value = car.Type.ToString();
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

            cmd.ExecuteNonQuery();
            FillAllCars();
        }

        public void DeleteCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(DELETE_CAR, conn);
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

            cmd.ExecuteNonQuery();
            FillAllCars();
        }

        public void Sort()
        {
            List<Car> sortedCars = collCars.ToList();
            sortedCars.Sort();
            collCars.Clear();
            
            foreach(Car car in sortedCars)
            {
                AddCar(car);
            }
        }

        public void Store()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(FILENAME_BINARY, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, collCars);
            stream.Close();
        }

        public void Load()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(FILENAME_BINARY, FileMode.Open, FileAccess.Read);
            ObservableCollection<Car> loadedCars = formatter.Deserialize(stream) as ObservableCollection<Car>;
            stream.Close();

            collCars.Clear();
            foreach(Car car in loadedCars)
            {
                AddCar(car);
            }
        }

        public void StoreXml()
        {
            
            TextWriter tw = new StreamWriter(FILENAME_XML);
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            ser.Serialize(tw, this);
            tw.Close();
        }

        public void LoadXml(String filename)
        {
            TextReader tr = new StreamReader(filename);
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            Database loadedDb = (Database)ser.Deserialize(tr);
            tr.Close();

            collCars.Clear();
            foreach (Car car in loadedDb.GetAllCars())
            {
                AddCar(car);
            }
        }
    }
}
