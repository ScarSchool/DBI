using pkgMiscellaneous;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PkgData
{
    public enum CarType
    {
        MICRO,
        SEDAN,
        SUV,
        Other
    }

    [Serializable]
    public class Car : IComparable<Car>
    {
        public delegate void CarChangedListener(object sender, EventArgsCar e);
        public static event CarChangedListener CollListenerMethods;

        public long SCN { get; set; }
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

        public double CarPrice { get; set; }
        public CarType Type { get; set; }

        public Owner[] CollOwners { get; set; }

        public Car() : this(-1, "?", CarType.Other, 0.0)
        {
        }

        public Car(int id, string name, string type, double price) : this(id, name, (CarType)Enum.Parse(typeof(CarType), type), price)
        {
        }

        public Car(int id, string name, CarType type, double price)
        {
            CarId = id;
            carName = name;
            Type = type;
            CarPrice = price;
        }

        public Car(int scn, int id, string name, CarType type, double price) : this(id, name, type, price)
        {
            SCN = scn;
        }

        public override string ToString()
        {
            return $"{CarId}, {CarName}, {Type}, {CarPrice} - {SCN}";
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
