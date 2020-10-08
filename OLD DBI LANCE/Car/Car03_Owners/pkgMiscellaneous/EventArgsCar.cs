using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgMiscellaneous
{
    public class EventArgsCar : EventArgs
    {
        public string Message { get; set; }
        public object Car { get; set; } // type Car would cause 'circular dependency'
        public EventArgsCar(string message, object car) : base()
        {
            Message = message;
            Car = car;
        }
    }
}
