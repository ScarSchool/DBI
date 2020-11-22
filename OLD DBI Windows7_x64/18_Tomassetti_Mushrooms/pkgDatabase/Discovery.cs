using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgDatabase
{
    public class Discovery
    {
        public int Id { get; set; }
        public Mushroom Mushroom { get; set; }
        public DateTime? Date { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Remark { get; set; }

        public Discovery() { }

        public string ToString(bool showRemark = false) { return $"{Date}: {X}, {Y} - {Mushroom}" + ((showRemark) ? $" | {Remark}" : ""); }
    }
}
