using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgData
{
    public class Team
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string Coach { get; set; }
        public long SCN { get; set; }
        public bool Modified { get; set; }

        public Team()
        {
        }

        public Team(int id, string country, string coach)
        {
            Id = id;
            Country = country;
            Coach = coach;
        }
    }
}
