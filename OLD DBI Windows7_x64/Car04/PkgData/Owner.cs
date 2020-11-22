using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PkgData
{
    [Serializable]
    public class Owner
    {
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime OwnerFrom { get; set; }
        public DateTime OwnerTo { get; set; }
        public int CarId { get; set; }

        public Owner(int ownerId, string ownerName, DateTime ownerFrom, DateTime ownerTo)
        {
            OwnerId = ownerId;
            OwnerName = ownerName;
            OwnerFrom = ownerFrom;
            OwnerTo = ownerTo;
        }

        public Owner(int ownerId, string ownerName, DateTime ownerFrom, DateTime ownerTo, int carId) : this(ownerId, ownerName, ownerFrom, ownerTo)
        {
            CarId = carId;
        }
    }
}
