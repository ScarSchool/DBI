using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkgMISC
{
    public class EventArgsCar : EventArgs
    {
        public string Message { get; set; }
        public object Car { get; set; } //type car would cause 'circualr dependency'

        public EventArgsCar(string message, object car) : base()
        {
            this.Message = message;
            this.Car = car;
        }
    }
}
