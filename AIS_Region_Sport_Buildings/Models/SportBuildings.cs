using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class SportBuildings
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int ArenaArea { get; set; }
        public int ArenaCapacity { get; set; }
        public int AddressId { get; set; }
        public int OrganizationId { get; set; } // Добавил для связей
        public int BuildingTypeId { get; set; } // Добавил для связей

        // Свойства адреса
        public string LocalityName { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string EntranceNumber { get; set; }

        public string LocalityType { get; set; }
        public int LocalityTypeId { get; set; }

        public string BuildingType { get; set; }

        public string OrganizationName { get; set; }

        // Вычисляемое свойство для полного адреса
        public string FullAddress
        {
            get
            {
                return $"{LocalityType} {LocalityName}, ул. {Street}, д. {BuildingNumber}, вход: {EntranceNumber}";
            }
        }

        // Вычисляемое свойство для информации о площади и вместимости
        public string ArenaInfo
        {
            get
            {
                return $"Площадь: {ArenaArea} м²; Вместимость: {ArenaCapacity} чел.";
            }
        }

        public SportBuildings() { }

        public SportBuildings(int id, string number, string name, string shortName, int area, int capacity,
                           int addressId, string localityName, string street, string buildingNumber,
                           string entranceNumber, string localityType, string buildingType, string organizationName)
        {
            Id = id;
            Number = number;
            Name = name;
            ShortName = shortName;
            ArenaArea = area;
            ArenaCapacity = capacity;
            AddressId = addressId;
            LocalityName = localityName;
            Street = street;
            BuildingNumber = buildingNumber;
            EntranceNumber = entranceNumber;
            LocalityType = localityType;
            BuildingType = buildingType;
            OrganizationName = organizationName;
        }
    }
}
