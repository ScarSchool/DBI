using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const String FILENAME_BINARY = "../../../car.bin";
        private const String FILENAME_XML = "../../../car.xml";
        public ObservableCollection<Car> collCars = new ObservableCollection<Car>();
        public readonly static Database Instance = new Database();

        public ObservableCollection<Car> GetAllCars()
        {
            return collCars;
        }

        public void AddCar(Car car)
        {
            if(collCars.Contains(car))
            {
                throw new Exception($"car with id {car.CarId} already stored");
            }
            else
            {
                collCars.Add(car);
            }
        }

        public void  UpdateCar(Car car)
        {
            int idx = collCars.IndexOf(car);
            DeleteCar(car);
            collCars.Insert(idx, car);
        }

        public void DeleteCar(Car car)
        {
            bool removed = collCars.Remove(car);

            if (!removed)
            {
                throw new Exception($"car with id {car.CarId} does not exist");
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
    }
}
