using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Car01_Tomassetti
{
    public class DataBase
    {
        private ObservableCollection<Car> collCars = new ObservableCollection<Car>();

        private static readonly string FILENAME_BINARY = "..\\..\\..\\car.bin";

        public void AddCar(Car car)
        {
            if (collCars.Contains(car))
            {
                throw new Exception("car with id " + car.CarId + " already stored");
            }
            collCars.Add(car);
        }

        public void RemoveCar(Car car)
        {
            if (!collCars.Contains(car))
            {
                throw new Exception("car with this id not stored");
            }
            collCars.Remove(car);
        }
        public void UpdateCar(Car car)
        {
            if (!collCars.Contains(car))
            {
                throw new Exception("car with id: " + car.CarId + " is not stored");
            }
            RemoveCar(car);
            AddCar(car);
        }

        public void Sort()
        {
            List<Car> tmpCars = collCars.ToList();
            tmpCars.Sort();
            collCars.Clear();
            foreach (Car car in tmpCars)
            {
                AddCar(car);
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
            ObservableCollection<Car> tmpCars = (ObservableCollection<Car>)formatter.Deserialize(stream);
            stream.Close();
            collCars.Clear();
            foreach (Car car in tmpCars)
            {
                AddCar(car);
            }

        }
    }
}
