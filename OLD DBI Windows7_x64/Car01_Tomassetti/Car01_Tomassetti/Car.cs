using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Car01_Tomassetti
{
    [Serializable]
    public class Car : IComparable<Car>
    {
        public int CarId { get; set; }
        public string CarName { get; set; }

        public Car(int id, string name)
        {
            CarId = id;
            CarName = name;
        }

        public override string ToString()
        {
            return "ID: " +  CarId + ", Name: " + CarName;
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

        //TODO: sorten miasst i holt eben a noch ;-/
    }
}
