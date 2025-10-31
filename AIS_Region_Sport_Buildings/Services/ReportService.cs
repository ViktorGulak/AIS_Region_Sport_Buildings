using AIS_Region_Sport_Buildings.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AIS_Region_Sport_Buildings.Services
{
    class ReportService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;

        public List<SportBuildingReport> GetSportBuildingsByTypeAndDate(string buildingType, DateTime date)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    sb.sb_name AS ""BuildingName"",
                    sb.sb_short_name AS ""ShortName"", 
                    sb.arena_area AS ""ArenaArea"",
                    sb.arena_capacity AS ""Capacity"",
                    a.locality_name AS ""Locality"",
                    lt.title AS ""LocalityTypeName"",  -- Добавлен тип населенного пункта
                    a.street AS ""Street"",
                    a.building_number AS ""BuildingNumber"",
                    o.org_name AS ""BalanceHolder"",
                    bh.date_start AS ""BalanceStartDate""
                FROM sport_buildings sb
                JOIN building_type bt ON sb.building_id = bt.id
                JOIN balance_history bh ON sb.id = bh.building_id
                JOIN organizations o ON bh.organization_id = o.id
                JOIN addresses a ON sb.address_id = a.id
                JOIN locality_type lt ON a.locality_id = lt.id  -- Добавлен JOIN
                WHERE bt.title = @buildingType
                  AND bh.date_start <= @reportDate
                  AND (bh.date_end >= @reportDate OR bh.date_end IS NULL)
                ORDER BY a.locality_name, sb.sb_name";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@buildingType", buildingType);
                        command.Parameters.AddWithValue("@reportDate", date);

                        using (var reader = command.ExecuteReader())
                        {
                            List<SportBuildingReport> report = new List<SportBuildingReport>();

                            while (reader.Read())
                            {
                                report.Add(new SportBuildingReport
                                {
                                    BuildingName = reader.GetString(reader.GetOrdinal("BuildingName")),
                                    ShortName = reader.IsDBNull(reader.GetOrdinal("ShortName")) ? "" : reader.GetString(reader.GetOrdinal("ShortName")),
                                    ArenaArea = reader.GetInt32(reader.GetOrdinal("ArenaArea")),
                                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                                    Locality = reader.GetString(reader.GetOrdinal("Locality")),
                                    LocalityTypeName = reader.GetString(reader.GetOrdinal("LocalityTypeName")), // Новое поле
                                    Street = reader.GetString(reader.GetOrdinal("Street")),
                                    BuildingNumber = reader.GetString(reader.GetOrdinal("BuildingNumber")),
                                    BalanceHolder = reader.GetString(reader.GetOrdinal("BalanceHolder")),
                                    BalanceStartDate = reader.GetDateTime(reader.GetOrdinal("BalanceStartDate"))
                                });
                            }

                            return report;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}");
                return new List<SportBuildingReport>();
            }
        }

        public List<DynamicsReport> GetDynamicsReport(string buildingName, string eventType, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    CASE EXTRACT(MONTH FROM e.evt_start)
                        WHEN 1 THEN 'Январь'
                        WHEN 2 THEN 'Февраль'
                        WHEN 3 THEN 'Март'
                        WHEN 4 THEN 'Апрель'
                        WHEN 5 THEN 'Май'
                        WHEN 6 THEN 'Июнь'
                        WHEN 7 THEN 'Июль'
                        WHEN 8 THEN 'Август'
                        WHEN 9 THEN 'Сентябрь'
                        WHEN 10 THEN 'Октябрь'
                        WHEN 11 THEN 'Ноябрь'
                        WHEN 12 THEN 'Декабрь'
                    END AS ""Month"",
                    EXTRACT(YEAR FROM e.evt_start) AS ""Year"",
                    COUNT(e.id) AS ""EventsCount"",
                    SUM(e.count_visitors) AS ""TotalVisitors"",
                    AVG(e.count_visitors) AS ""AverageVisitors""
                FROM events e
                JOIN balance_history bh ON e.balance_id = bh.id
                JOIN sport_buildings sb ON bh.building_id = sb.id
                JOIN event_type et ON e.type_id = et.id
                WHERE sb.sb_name = @buildingName
                  AND et.title = @eventType
                  AND e.evt_start >= @startDate 
                  AND e.evt_start <= @endDate
                GROUP BY EXTRACT(MONTH FROM e.evt_start), EXTRACT(YEAR FROM e.evt_start)
                ORDER BY EXTRACT(YEAR FROM e.evt_start), EXTRACT(MONTH FROM e.evt_start)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@buildingName", buildingName);
                        command.Parameters.AddWithValue("@eventType", eventType);
                        command.Parameters.AddWithValue("@startDate", startDate.Date);
                        command.Parameters.AddWithValue("@endDate", endDate.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            List<DynamicsReport> report = new List<DynamicsReport>();

                            // Получаем индексы столбцов
                            int monthIndex = reader.GetOrdinal("Month");
                            int yearIndex = reader.GetOrdinal("Year");
                            int eventsCountIndex = reader.GetOrdinal("EventsCount");
                            int totalVisitorsIndex = reader.GetOrdinal("TotalVisitors");
                            int averageVisitorsIndex = reader.GetOrdinal("AverageVisitors");

                            while (reader.Read())
                            {
                                report.Add(new DynamicsReport
                                {
                                    Month = reader.GetString(monthIndex),
                                    Year = (int)reader.GetDouble(yearIndex), // EXTRACT возвращает double
                                    EventsCount = reader.GetInt32(eventsCountIndex),
                                    TotalVisitors = reader.GetInt32(totalVisitorsIndex),
                                    AverageVisitors = reader.GetDouble(averageVisitorsIndex)
                                });
                            }

                            return report;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета по динамике: {ex.Message}");
                return new List<DynamicsReport>();
            }
        }

        public List<BalanceHoldersReport> GetBalanceHoldersReport(DateTime date)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    o.org_number AS ""OrgNumber"",
                    o.org_name AS ""OrgName"",
                    o.org_short_name AS ""OrgShortName"",
                    o.org_phone AS ""OrgPhone"",
                    o.org_mail AS ""OrgEmail"",
                    COUNT(DISTINCT bh.building_id) AS ""BuildingsCount"",
                    STRING_AGG(sb.sb_name, ', ') AS ""BuildingsList""
                FROM organizations o
                LEFT JOIN balance_history bh ON o.id = bh.organization_id
                LEFT JOIN sport_buildings sb ON bh.building_id = sb.id
                WHERE bh.date_start <= @reportDate
                    AND (bh.date_end IS NULL OR bh.date_end >= @reportDate)
                    AND bh.building_id IS NOT NULL
                GROUP BY 
                    o.id, o.org_number, o.org_name, o.org_short_name, 
                    o.org_phone, o.org_mail
                HAVING COUNT(DISTINCT bh.building_id) > 0
                ORDER BY COUNT(DISTINCT bh.building_id) DESC, o.org_name";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@reportDate", date.Date);

                        using (var reader = command.ExecuteReader())
                        {
                            List<BalanceHoldersReport> report = new List<BalanceHoldersReport>();

                            // Получаем индексы столбцов
                            int orgNumberIndex = reader.GetOrdinal("OrgNumber");
                            int orgNameIndex = reader.GetOrdinal("OrgName");
                            int orgShortNameIndex = reader.GetOrdinal("OrgShortName");
                            int orgPhoneIndex = reader.GetOrdinal("OrgPhone");
                            int orgEmailIndex = reader.GetOrdinal("OrgEmail");
                            int buildingsCountIndex = reader.GetOrdinal("BuildingsCount");
                            int buildingsListIndex = reader.GetOrdinal("BuildingsList");

                            while (reader.Read())
                            {
                                report.Add(new BalanceHoldersReport
                                {
                                    OrgNumber = reader.GetString(orgNumberIndex),
                                    OrgName = reader.GetString(orgNameIndex),
                                    OrgShortName = reader.IsDBNull(orgShortNameIndex) ? "" : reader.GetString(orgShortNameIndex),
                                    OrgPhone = reader.GetString(orgPhoneIndex),
                                    OrgEmail = reader.GetString(orgEmailIndex),
                                    BuildingsCount = reader.GetInt32(buildingsCountIndex),
                                    BuildingsList = reader.IsDBNull(buildingsListIndex) ? "" : reader.GetString(buildingsListIndex)
                                });
                            }

                            return report;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета по балансодержателям: {ex.Message}");
                return new List<BalanceHoldersReport>();
            }
        }
    }
}
