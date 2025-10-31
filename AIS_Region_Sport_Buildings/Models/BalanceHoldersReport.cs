using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Models
{
    class BalanceHoldersReport
    {
        public string OrgNumber { get; set; }
        public string OrgName { get; set; }
        public string OrgShortName { get; set; }
        public string OrgPhone { get; set; }
        public string OrgEmail { get; set; }
        public int BuildingsCount { get; set; }
        public string BuildingsList { get; set; }

        // Вычисляемые свойства для DataGrid
        public string OrgNameDisplay => OrgName;
        public string OrgShortNameDisplay => string.IsNullOrEmpty(OrgShortName) ? "Не указано" : OrgShortName;
        public string OrgPhoneDisplay => OrgPhone;
        public string OrgEmailDisplay => OrgEmail;
        public string BuildingsCountDisplay => BuildingsCount.ToString("N0");
        public string BuildingsListDisplay => BuildingsList ?? "Не указано";
    }
}
