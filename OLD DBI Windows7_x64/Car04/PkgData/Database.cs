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

        private static readonly string FILENAME_BINARY = @"..\..\..\cars.bin";
        private static readonly string FILENAME_XML = @"..\..\..\cars.xml";
        public ObservableCollection<Car> collCars = new ObservableCollection<Car>();

        public ObservableCollection<Owner> collOwners = new ObservableCollection<Owner>();

        private static readonly string SELECT_CARS = "SELECT carid, carname, cartype, carprice FROM cars ORDER BY carname";
        private static readonly string INSERT_CAR = "INSERT INTO cars VALUES(:carid, :carname, :cartype, :carprice)";
        private static readonly string DELETE_CAR = "DELETE cars WHERE carid = :carid";
        private static readonly string UPDATE_CAR = "UPDATE cars SET carname = :carname, cartype = :cartype WHERE carid = :carid";

        private static readonly string SELECT_OWNERS = "SELECT ownerid, ownername, ownerfrom, ownerto FROM owners WHERE carid = :carid ORDER BY ownername";
        private static readonly string INSERT_OWNER = "INSERT INTO owners VALUES(seqOwner.NEXTVAL,:ownername, :ownerfrom, :ownerto, :carid)";

        private OracleConnection conn;
        private OracleTransaction trx;

        public string DB_Host { get; set; }
        public string DB_Username { get; set; }
        public string DB_Password { get; set; }

        private readonly string CONNSTRING = @"user id={0};password={1};data source=" +
                         "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                         "(HOST={2})(PORT=1521))(CONNECT_DATA=" +
                         "(SERVICE_NAME=ora11g)))";


        public readonly static Database Instance = new Database();

        public void BeginnTrx()
        {
            if (conn == null)
            {
                conn = new OracleConnection(string.Format(CONNSTRING, DB_Username, DB_Password, DB_Host));
            }

            conn.Open();
            trx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTrx()
        {
            //if (trx != null)
            //{
            trx.Commit();
            conn.Close();
            //}
        }

        public void Rollback()
        {
            //if (trx != null)
            //{
            trx.Rollback();
            conn.Close();
            FillAllCars();
            //}
        }
        #region Cars Operations
        public void AddCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(INSERT_CAR, conn);
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;
            cmd.Parameters.Add("carname", OracleDbType.Varchar2).Value = car.CarName;
            cmd.Parameters.Add("cartype", OracleDbType.Varchar2).Value = car.Type.ToString();
            cmd.Parameters.Add("carprice", OracleDbType.Int32).Value = car.CarPrice;
            cmd.ExecuteNonQuery();

            FillAllCars();
        }

        public void FillAllCars()
        {
            collCars.Clear();
            OracleCommand cmd = new OracleCommand(SELECT_CARS, conn);
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Car car = new Car(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
                collCars.Add(car);
            }

            reader.Close();
        }

        public void DeleteCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(DELETE_CAR, conn);
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;
            cmd.ExecuteNonQuery();

            FillAllCars();
        }

        public void UpdateCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(UPDATE_CAR, conn);

            cmd.Parameters.Add("carname", OracleDbType.Varchar2).Value = car.CarName;
            cmd.Parameters.Add("cartype", OracleDbType.Varchar2).Value = car.Type.ToString();
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

            if (cmd.ExecuteNonQuery() != 1)
                throw new Exception("car with id not found");
            else
                FillAllCars();
        }

        public void SortCars()
        {
            List<Car> tmpCars = collCars.ToList();

            tmpCars.Sort();
            collCars.Clear();

            foreach (Car c in tmpCars)
            {
                AddCar(c);
            }
        }

        public ObservableCollection<Car> GetAllCars()
        {
            return collCars;
        }

        public void LoadCars()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(
                FILENAME_BINARY,
                FileMode.Open,
                FileAccess.Read
                );

            ObservableCollection<Car> tmpCollCars = (ObservableCollection<Car>)formatter.Deserialize(stream);
            stream.Close();
            collCars.Clear();

            foreach (Car car in tmpCollCars)
            {
                collCars.Add(car);
            }
        }

        public void LoadXmlCars()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            TextReader txtReader = new StreamReader(FILENAME_XML);

            Database tmpDB = (Database)serializer.Deserialize(txtReader);
            txtReader.Close();
            collCars.Clear();

            foreach (Car car in tmpDB.GetAllCars())
            {
                AddCar(car);
            }
        }

        public void StoreXmlCars()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            TextWriter txtWriter = new StreamWriter(FILENAME_XML);
            serializer.Serialize(txtWriter, this);
            txtWriter.Close();
        }
        public void StoreCars()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(
                FILENAME_BINARY,
                FileMode.Create,
                FileAccess.Write
                );

            formatter.Serialize(stream, collCars);
            stream.Close();
        }
        #endregion
        #region Owners Operations
        public void FillAllOwnersOfCar(int carId)
        {
            try
            {
                collOwners.Clear();
                OracleCommand cmd = new OracleCommand(SELECT_OWNERS, conn);
                cmd.Parameters.Add("carid", OracleDbType.Int32).Value = carId;

                OracleDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    Owner newOwner = new Owner(reader.GetInt32(0), reader.GetString(1), reader.GetDateTime(2), reader.GetDateTime(3));
                    collOwners.Add(newOwner);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<Owner> GetAllOwners()
        {
            return collOwners;
        }
        public void AddOwner(Owner owner)
        {
            OracleCommand cmd = new OracleCommand(INSERT_OWNER, conn);
            cmd.Parameters.Add("ownername", OracleDbType.Varchar2).Value = owner.OwnerName;
            cmd.Parameters.Add("ownerfrom", OracleDbType.Date).Value = owner.OwnerFrom;
            cmd.Parameters.Add("ownerto", OracleDbType.Date).Value = owner.OwnerTo;
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = owner.CarId;
            cmd.ExecuteNonQuery();

            FillAllOwnersOfCar(owner.CarId);
        }
        #endregion
    }
}