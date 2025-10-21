using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class Addresses
    {
        public int Id { get; set; }
        public string LocalityName { get; set; }
        public string Street { get; set; }
        public string BuildNumber { get; set; }
        public string EntranceNumber { get; set; }
        public int LocalityId { get; set; }

        // Конструктор по умолчанию (обязательно для WPF)
        public Addresses() { }

        public Addresses(int id, string localityName, string street, string buildNumber, string entranceNumber, int localityId)
        {
            Id = id;
            LocalityName = localityName;
            Street = street;
            BuildNumber = buildNumber;
            EntranceNumber = entranceNumber;
            LocalityId = localityId;
        }
    }
}
