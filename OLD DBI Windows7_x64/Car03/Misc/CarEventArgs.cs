using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
   public class CarEventArgs : EventArgs
    {
        public string Message { get; set; }
        public object Car { get; set; } //Object type because of circular dependency
        public CarEventArgs(string message, object car)
        {
            this.Message = message;
            this.Car = car;
        }
    }
}
