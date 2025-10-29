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
    }
}
