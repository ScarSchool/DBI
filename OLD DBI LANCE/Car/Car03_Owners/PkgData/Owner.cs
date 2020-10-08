using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PkgData
{
    [Serializable]
    public class Owner
    {
        public long SCN { get; set; }

        [XmlIgnore]
        public bool OwnerUpdated { get; set; }
        public int OwnerId { get; set; }

        private string ownerName;
        public string OwnerName
        {
            get
            {
                return ownerName;
            }

            set
            {
                ownerName = value;
                OwnerUpdated = true;
            }
        }

        public DateTime OwnerFrom { get; set; }

        public DateTime OwnerTill { get; set; }

        [XmlIgnore]
        public string OwnerFromFormatted
        {
            get
            {
                return OwnerFrom.ToString("dd.MM.yyyy");
            }

            set
            {
                OwnerFrom = DateTime.ParseExact(value, "dd.MM.yyyy", null);
                OwnerUpdated = true;
            }
        }
        [XmlIgnore]
        public string OwnerTillFormatted
        {
            get
            {
                return OwnerTill.ToString("dd.MM.yyyy");
            }

            set
            {
                OwnerTill = DateTime.ParseExact(value, "dd.MM.yyyy", null);
                OwnerUpdated = true;
            }
        }

        public Owner() : this(-1, "?", DateTime.Now, DateTime.Now)
        {
        }

        public Owner(int id, string name, DateTime from, DateTime till)
        {
            OwnerId = id;
            ownerName = name;
            OwnerFrom = from;
            OwnerTill = till;
            OwnerUpdated = false;
        }
    }
}
