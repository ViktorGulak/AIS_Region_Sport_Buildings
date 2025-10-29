using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class SportBuildingReport
    {
        public string BuildingName { get; set; }
        public string ShortName { get; set; }
        public int ArenaArea { get; set; }
        public int Capacity { get; set; }
        public string Locality { get; set; }
        public string LocalityTypeName { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string BalanceHolder { get; set; }
        public DateTime BalanceStartDate { get; set; }

        // Вычисляемое свойство для полного адреса
        public string FullAddress => $"{LocalityTypeName} {Locality}, ул. {Street}, д. {BuildingNumber}";

        // Вычисляемое свойство для информации о площади и вместимости
        public string ArenaInfo => $"Площадь: {ArenaArea} м², Вместимость: {Capacity} чел.";
    }
}
