using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class Organization
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }

        // Новые свойства для адреса
        public string LocalityName { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string EntranceNumber { get; set; }

        public string LocalityType { get; set; }

        // Вычисляемое свойство для полного адреса
        public string FullAddress
        {
            get
            {
                return $"{LocalityType} {LocalityName}, ул. {Street}, д. {BuildingNumber}, вход: {EntranceNumber}";
            }
        }

        // Конструктор по умолчанию
        public Organization() { }

        public Organization(int id, string number, string name, string shortName, string phone, string email,
                           int addressId, string localityName, string street, string buildingNumber, 
                           string entranceNumber, string localityType)
        {
            Id = id;
            Number = number;
            Name = name;
            ShortName = shortName;
            Phone = phone;
            Email = email;
            AddressId = addressId;
            LocalityName = localityName;
            Street = street;
            BuildingNumber = buildingNumber;
            EntranceNumber = entranceNumber;
            LocalityType = localityType;
        }
    }
}
