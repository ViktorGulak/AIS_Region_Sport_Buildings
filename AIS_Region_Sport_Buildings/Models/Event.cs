using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EventType { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; } // Может быть NULL
        public int VisitorsCount { get; set; }
        public int BalanceId { get; set; }
        public int TypeId { get; set; }

        // Новые свойства для организатора
        public string OrganizerName { get; set; }
        public string BuildingName { get; set; }

        // Вычисляемое свойство для периода мероприятия
        public string EventPeriod
        {
            get
            {
                if (DateEnd.HasValue)
                    return $"С {DateStart:dd.MM.yyyy} по {DateEnd.Value:dd.MM.yyyy}";
                else
                    return $"{DateStart:dd.MM.yyyy}";
            }
        }

        // Конструктор по умолчанию
        public Event() { }

        public Event(int id, string title, string eventType, DateTime dateStart, DateTime? dateEnd,
                     int visitorsCount, int balanceId, int typeId, string organizerName, string buildingName)
        {
            Id = id;
            Title = title;
            EventType = eventType;
            DateStart = dateStart;
            DateEnd = dateEnd;
            VisitorsCount = visitorsCount;
            BalanceId = balanceId;
            TypeId = typeId;
            OrganizerName = organizerName;
            BuildingName = buildingName;
        }
    }
}
