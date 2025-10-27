using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class Balance
    {
        public int Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; } // Может быть NULL
        public string OrganizationName { get; set; }
        public string BuildingName { get; set; }
        public int OrganizationId { get; set; }
        public int BuildingId { get; set; }

        // Вычисляемое свойство для периода баланса
        public string BalancePeriod
        {
            get
            {
                if (DateEnd.HasValue)
                    return $"С {DateStart:dd.MM.yyyy} по {DateEnd.Value:dd.MM.yyyy}";
                else
                    return $"С {DateStart:dd.MM.yyyy} по настоящее время";
            }
        }

        // Конструктор по умолчанию
        public Balance() { }

        public Balance(int id, DateTime dateStart, DateTime? dateEnd, int orgId, string orgName, int bldId, string bldName)
        {
            Id = id;
            DateStart = dateStart;
            DateEnd = dateEnd;
            OrganizationId = orgId;
            OrganizationName = orgName;
            BuildingId = bldId;
            BuildingName = bldName;
        }
    }
}
