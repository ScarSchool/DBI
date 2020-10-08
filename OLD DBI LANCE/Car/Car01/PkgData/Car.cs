using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkgData
{
    public enum CarType
    {
        LKW,
        eCar,
        Bus
    }

    [Serializable]
    public class Car : IComparable<Car>
    {
        public delegate void CarChangedListener(object sender, EventArgsCar e);

        public int CarId { get; set; }
        public string CarName { get; set; }
        public CarType Type { get; set; }

        public Car()
        {
            CarId = -1;
            CarName = "undefined";
            Type = CarType.LKW;
        }

        public Car(int id, string name, CarType type)
        {
            CarId = id;
            CarName = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"{CarId}, {CarName}, {Type}";
        }

        public override bool Equals(object obj)
        {
            var car = obj as Car;
            return car != null &&
                   CarId == car.CarId;
        }

        public int CompareTo(Car other)
        {
            return CarName.CompareTo(other.CarName);
        }
    }
}
