using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace PkgData
{
    public class Database
    {
        public static Database instance { get; private set; } = new Database();
        public ObservableCollection<Car> collCars = new ObservableCollection<Car>();
        private static readonly string FILENAME_BINARY = "..\\..\\..\\car.bin";
        private static readonly string FILENAME_XML = "..\\..\\..\\car.xml";


        public Database() {}

        public void AddCar(Car car)
        {
            if (collCars.Contains(car))
            {
                throw new Exception("Car already in collection. CarId: " + car.CarId);
            }

            collCars.Add(car);
        }

        public void DelCar(Car car)
        {
            if (!collCars.Contains(car))
            {
                throw new Exception("Car does not exist in collection. CarId: " + car.CarId);
            }
            collCars.Remove(car);
        }

        public void UpdateCar(Car car)
        {
            if (!collCars.Contains(car))
            {
                throw new Exception("Car does not exist in collection. CarId: " + car.CarId);
            }
            int idx = collCars.IndexOf(car);
            Car tmpCar = collCars.ElementAt(idx);
            tmpCar.CarName = car.CarName;
            /*DelCar(car);
            collCars.Insert(idx, car);*/
        }

        public void Sort()
        {
            List<Car> sortableList = collCars.ToList();
            sortableList.Sort();

            for (int i = 0; i < sortableList.Count; i++)
            {
                collCars.Move(collCars.IndexOf(sortableList[i]), i);
            }
        }

        public ObservableCollection<Car> GetAllCars()
        {
            return collCars;
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
            ObservableCollection<Car> tempCollCars = (ObservableCollection<Car>)formatter.Deserialize(stream);
            stream.Close();
            collCars.Clear();
            foreach (Car car in tempCollCars)
            {
                AddCar(car);
            }
        }

        public void StoreXML()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            TextWriter writer = new StreamWriter(FILENAME_XML);
            ser.Serialize(writer, this);
            writer.Close();
        }

        public void StoreXML(string filepath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            TextWriter writer = new StreamWriter(filepath);
            ser.Serialize(writer, this);
            writer.Close();
        }

        public void LoadXML()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            TextReader reader = new StreamReader(FILENAME_XML);
            Database dbt = (Database)ser.Deserialize(reader);
            reader.Close();
            collCars.Clear();
            foreach (Car car in dbt.collCars)
            {
                AddCar(car);
            }
        }

        public void LoadXML(string filepath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Database));
            TextReader reader = new StreamReader(filepath);
            Database dbt = (Database)ser.Deserialize(reader);
            reader.Close();
            collCars.Clear();
            foreach (Car car in dbt.collCars)
            {
                AddCar(car);
            }
        }
    }
}
