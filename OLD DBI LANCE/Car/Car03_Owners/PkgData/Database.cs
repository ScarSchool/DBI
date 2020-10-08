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
        private static readonly string FILENAME_OWNERS_XML = "../../../carWithOwners.xml";

        private static readonly String SELECT_CARS = "SELECT NVL(ORA_ROWSCN, -1), carid, carname, cartype, carprice FROM cars ORDER BY carname";
        private static readonly String INSERT_CAR = "INSERT INTO cars VALUES(:carid, :carname, :cartype, :carprice)";
        private static readonly String DELETE_CAR = "DELETE FROM cars WHERE carid = :carid";
        private static readonly String UPDATE_CAR = "UPDATE cars SET carname = :carname, cartype = :cartype, carprice = :carprice WHERE carid = :carid";
        private static readonly String SELECT_CAR_VERSION = "SELECT NVL(ORA_ROWSCN, -1) from cars WHERE carid = :carid FOR UPDATE";
        private static readonly String SELECT_CAR_OWNERS = "SELECT NVL(ORA_ROWSCN, -1), ownerid, ownername, NVL(ownerfrom, SYSDATE), NVL(ownerto, SYSDATE) FROM owners WHERE carid = :carid ORDER BY ownerfrom";
        private static readonly String INSERT_CAR_OWNER = "INSERT INTO owners VALUES(seqOwner.NEXTVAL, :ownername, TO_DATE(:ownerfrom,'DD.MM.YYYY'), TO_DATE(:ownerto,'DD.MM.YYYY'), :carid)";
        private static readonly String DELETE_CAR_OWNER = "DELETE FROM owners WHERE ownerid = :ownerid";
        private static readonly String UPDATE_CAR_OWNER = "UPDATE owners SET ownername = :ownername, ownerfrom = TO_DATE(:ownerfrom,'DD.MM.YYYY'), ownerto = TO_DATE(:ownerto,'DD.MM.YYYY') WHERE ownerid = :ownerid";
        private static readonly String SELECT_CAR_OWNER_VERSION = "SELECT NVL(ORA_ROWSCN, -1) from owners WHERE ownerid = :ownerid FOR UPDATE";
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
        public ObservableCollection<Owner> collOwners = new ObservableCollection<Owner>();
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
                Car car = new Car(reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetDouble(4));
                car.SCN = reader.GetInt64(0);
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
            cmd.Parameters.Add("carprice", OracleDbType.Double).Value = car.CarPrice;

            cmd.ExecuteNonQuery();
            FillAllCars();
        }

        private bool lockCar(Car car)
        {
            OracleCommand cmd = new OracleCommand(SELECT_CAR_VERSION, conn);
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;
            OracleDataReader reader = cmd.ExecuteReader();
            reader.Read();
            long currSCN = reader.GetInt64(0);
            reader.Close();

            return (currSCN == car.SCN);
        }

        public void UpdateCar(Car car)
        {
            if (lockCar(car))
            {
                OracleCommand cmd = new OracleCommand(UPDATE_CAR, conn);

                cmd.Parameters.Add("carname", OracleDbType.Varchar2).Value = car.CarName;
                cmd.Parameters.Add("cartype", OracleDbType.Varchar2).Value = car.Type.ToString();
                cmd.Parameters.Add("carprice", OracleDbType.Double).Value = car.CarPrice;
                cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

                cmd.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Car was changed by another client, update was canceled");
            }
        }

        public void DeleteCar(Car car)
        {
            if (lockCar(car))
            {
                OracleCommand cmd = new OracleCommand(DELETE_CAR, conn);
                cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

                cmd.ExecuteNonQuery();
                FillAllCars();
            }
            else
            {
                throw new Exception("Car was changed by another client, delete was canceled");
            }
        }

        public void GetCarOwners(Car car)
        {
            collOwners.Clear();
            OracleCommand cmd = new OracleCommand(SELECT_CAR_OWNERS, conn);
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;
            OracleDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Owner owner = new Owner(reader.GetInt32(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4));
                owner.SCN = reader.GetInt32(0);
                collOwners.Add(owner);
            }

            reader.Close();
        }

        public void AddCarOwner(Car car, Owner owner)
        {
            OracleCommand cmd = new OracleCommand(INSERT_CAR_OWNER, conn);

            cmd.Parameters.Add("ownername", OracleDbType.Varchar2).Value = owner.OwnerName;
            cmd.Parameters.Add("ownerfrom", OracleDbType.Varchar2).Value = owner.OwnerFromFormatted;
            cmd.Parameters.Add("ownerto", OracleDbType.Varchar2).Value = owner.OwnerTillFormatted;
            cmd.Parameters.Add("carid", OracleDbType.Int32).Value = car.CarId;

            cmd.ExecuteNonQuery();
            GetCarOwners(car);
        }

        private bool lockCarOwner(Owner owner)
        {
            OracleCommand cmd = new OracleCommand(SELECT_CAR_OWNER_VERSION, conn);
            cmd.Parameters.Add("ownerid", OracleDbType.Int32).Value = owner.OwnerId;
            OracleDataReader reader = cmd.ExecuteReader();
            long currSCN = 0;
            reader.Read();
            currSCN = reader.GetInt64(0);
            reader.Close();

            return (currSCN == owner.SCN);
        }

        public void UpdateCarOwner(Car car, Owner owner)
        {
            if (lockCarOwner(owner))
            {
                OracleCommand cmd = new OracleCommand(UPDATE_CAR_OWNER, conn);

                cmd.Parameters.Add("ownername", OracleDbType.Varchar2).Value = owner.OwnerName;
                cmd.Parameters.Add("ownerfrom", OracleDbType.Varchar2).Value = owner.OwnerFromFormatted;
                cmd.Parameters.Add("ownerto", OracleDbType.Varchar2).Value = owner.OwnerTillFormatted;
                cmd.Parameters.Add("ownerid", OracleDbType.Int32).Value = owner.OwnerId;

                cmd.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Owner was changed by another client, update was canceled");
            }
        }

        public void DeleteCarOwner(Car car, Owner owner)
        {
            if (lockCarOwner(owner))
            {
                OracleCommand cmd = new OracleCommand(DELETE_CAR_OWNER, conn);
                cmd.Parameters.Add("ownerid", OracleDbType.Int32).Value = owner.OwnerId;

                cmd.ExecuteNonQuery();
                GetCarOwners(car);
            }
            else
            {
                throw new Exception("Owner was changed by another client, delete was canceled");
            }
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

        public void StoreAllXml()
        {
            Car[] arrayOfCars = new Car[collCars.Count];

            for (int idx = 0; idx < collCars.Count; idx++)
            {
                Car car = collCars[idx];
                GetCarOwners(car);
                car.CollOwners = collOwners.ToArray();
                arrayOfCars[idx] = car;
            }

            collOwners.Clear();
            
            TextWriter tw = new StreamWriter(FILENAME_OWNERS_XML);
            XmlSerializer ser = new XmlSerializer(typeof(Car[]));
            ser.Serialize(tw, arrayOfCars);
            tw.Close();
        }
    }
}
