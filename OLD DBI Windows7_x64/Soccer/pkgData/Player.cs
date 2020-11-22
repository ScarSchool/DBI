using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgData
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public string BirthdateFormatted
        {
            get
            {
                return Birthdate.ToString("dd.MM.yyyy");
            }
            set
            {
                try
                {
                    Birthdate = DateTime.ParseExact(value, "dd.MM.yyyy", null);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public long SCN { get; set; }
        public bool Modified { get; set; }

        public Player()
        {
        }

        public Player(int id, string name, DateTime birthdate)
        {
            Id = id;
            Name = name;
            Birthdate = birthdate;
        }
    }
}
