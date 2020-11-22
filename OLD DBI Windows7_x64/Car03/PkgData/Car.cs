using System;
using System.Collections.Generic;
using Misc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PkgData
{

    public enum CarType
    {
        SUV, ECar, LKW, Other
    }
    [Serializable]
    public class Car : IComparable<Car>
    {
        public delegate void CarChangedListener(object eventsource, EventArgs cea);//define the delete
        public static event CarChangedListener CollListenerMethods;//Holds all listener-methods; event makes compiler check signature of handlers

        public int CarId { get; set;}
        private string _carname;
        public string CarName { get {
                return _carname;
            } set {
                CollListenerMethods(this, new CarEventArgs("||carname changed from " + _carname + " to " + value, this));
                _carname = value;
            } }
        //[NonSerialized] fuer binary nicht fuer properties
        //[XmlIgnore] fuer xml
        public CarType Type { get; set; }

        public Car(int carId, string carName, CarType ct)
        {
            CarId = carId;
            _carname = carName; //private var because otherwise we would call the event
            Type = ct;
        }

        public Car(): this(0)
        {

        }

        public Car(int id) : this(id, "unknown", CarType.ECar)
        {
        }

        public override string ToString()
        {
            return CarId + ", " + CarName + ", " + Type;
        }

        public override bool Equals(object obj)
        {
            var car = obj as Car;
            return car != null &&
                   CarId == car.CarId;
        }

        public int CompareTo(Car car)
        {
            return CarName.CompareTo(car.CarName);
        }

        public override int GetHashCode()
        {
            var hashCode = 1068957387;
            hashCode = hashCode * -1521134295 + CarId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CarName);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }
    }
}
