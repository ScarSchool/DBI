using pkgMiscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkgData
{
    public enum CarType
    {
        MICRO,
        SEDAN,
        Other
    }

    [Serializable]
    public class Car : IComparable<Car>
    {
        public delegate void CarChangedListener(object sender, EventArgsCar e);
        public static event CarChangedListener CollListenerMethods;

        public int CarId { get; set; }

        private string carName;
        public string CarName
        {
            get { return carName; }
            set
            {
                carName = value;
                CollListenerMethods?.Invoke(this, new EventArgsCar("carname changed", this));
            }
        }
        public CarType Type { get; set; }

        public Car() : this(-1, "?", CarType.Other)
        {
        }

        public Car(int id, string name, string type) : this(id, name, (CarType)Enum.Parse(typeof(CarType), type))
        {
        }

        public Car(int id, string name, CarType type)
        {
            CarId = id;
            carName = name;
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
