using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class DynamicsReport
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public int EventsCount { get; set; }
        public int TotalVisitors { get; set; }
        public double AverageVisitors { get; set; }

        // Вычисляемое свойство для периода
        public string Period => $"{Month} {Year}";

        // Свойства для DataGrid
        public string PeriodDisplay => Period;
        public string EventsCountDisplay => EventsCount.ToString("N0");
        public string TotalVisitorsDisplay => TotalVisitors.ToString("N0");
        public string AverageVisitorsDisplay => Math.Round(AverageVisitors, 1).ToString("N1");
    }
}
