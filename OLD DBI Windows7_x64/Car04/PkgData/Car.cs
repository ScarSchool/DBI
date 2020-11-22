using PkgMISC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PkgData
{
    [Serializable]
    public class Car : IComparable<Car>
    {
        #region Listeners
        public delegate void CarChangedListener(object eventsource, EventArgsCar ea);
        public static event CarChangedListener CollListenerMethods; //holds all listener methods; 'event' makes check signature of handlers
        #endregion
        #region Properties
        public enum CarType
        {
            SUV,
            Electro,
            City,
            Semi,
            SEDAN,
            MICRO
        }

        private string name;
        public int CarId { get; set; }
        public string CarName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

                if (CollListenerMethods != null)
                {
                    CollListenerMethods(this, new EventArgsCar("carname changed", this));
                }
            }
        }
        public CarType Type { get; set; }
        public int CarPrice { get; set; }
        #endregion
        #region Constructors
        public Car() { }
        public Car(int carId, string carName, CarType type, int carPrice)
        {
            CarId = carId;
            name = carName; //avoid firing changed event when new car
            Type = type;
            CarPrice = carPrice;
        }
        public Car(int carId, string carName, string type, int carPrice) : this(carId, carName, (CarType)Enum.Parse(typeof(CarType), type), carPrice) { }
        #endregion
        #region Methods
        public override string ToString()
        {
            return CarId + " " + CarName + " " + Type;
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
        #endregion
    }
}
