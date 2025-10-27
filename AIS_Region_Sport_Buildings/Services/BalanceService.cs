using AIS_Region_Sport_Buildings.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace AIS_Region_Sport_Buildings.Services
{
    class BalanceService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;

        public List<Balance> GetAll()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    bh.id,
                    bh.date_start,
                    bh.date_end,
                    bh.organization_id,
                    bh.building_id,
                    o.org_name as organization_name,
                    sb.sb_name as building_name
                FROM balance_history bh
                LEFT JOIN organizations o ON bh.organization_id = o.id
                LEFT JOIN sport_buildings sb ON bh.building_id = sb.id
                ORDER BY bh.id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        // Получаем индексы столбцов
                        int idIndex = reader.GetOrdinal("id");
                        int dateStartIndex = reader.GetOrdinal("date_start");
                        int dateEndIndex = reader.GetOrdinal("date_end");
                        int organizationIdIndex = reader.GetOrdinal("organization_id");
                        int buildingIdIndex = reader.GetOrdinal("building_id");
                        int organizationNameIndex = reader.GetOrdinal("organization_name");
                        int buildingNameIndex = reader.GetOrdinal("building_name");

                        List<Balance> balances = new List<Balance>();

                        while (reader.Read())
                        {
                            balances.Add(new Balance
                            {
                                Id = reader.GetInt32(idIndex),
                                DateStart = reader.GetDateTime(dateStartIndex),
                                DateEnd = reader.IsDBNull(dateEndIndex) ? (DateTime?)null : reader.GetDateTime(dateEndIndex),
                                OrganizationId = reader.GetInt32(organizationIdIndex),
                                BuildingId = reader.GetInt32(buildingIdIndex),
                                OrganizationName = reader.IsDBNull(organizationNameIndex) ? "Не указана" : reader.GetString(organizationNameIndex),
                                BuildingName = reader.IsDBNull(buildingNameIndex) ? "Не указано" : reader.GetString(buildingNameIndex)
                            });
                        }

                        return balances;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории баланса: {ex.Message}");
                return new List<Balance>();
            }
        }

        public bool Create(int organizationId, int buildingId, DateTime dateStart, DateTime? dateEnd)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string insertSql = @"
                INSERT INTO balance_history (
                    organization_id, 
                    building_id, 
                    date_start, 
                    date_end
                ) 
                VALUES (
                    @organization_id, 
                    @building_id, 
                    @date_start, 
                    @date_end
                )";

                    using (var command = new NpgsqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@organization_id", organizationId);
                        command.Parameters.AddWithValue("@building_id", buildingId);
                        command.Parameters.AddWithValue("@date_start", dateStart);

                        // Обрабатываем NULL для date_end
                        if (dateEnd.HasValue)
                            command.Parameters.AddWithValue("@date_end", dateEnd.Value);
                        else
                            command.Parameters.AddWithValue("@date_end", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении записи баланса: {ex.Message}");
                    return false;
                }
            }
        }

        public bool Update(int balanceId, int organizationId, int buildingId, DateTime dateStart, DateTime? dateEnd)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string updateSql = @"
                        UPDATE balance_history 
                        SET organization_id = @organization_id, 
                            building_id = @building_id, 
                            date_start = @date_start, 
                            date_end = @date_end
                        WHERE id = @id";

                    using (var command = new NpgsqlCommand(updateSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", balanceId);
                        command.Parameters.AddWithValue("@organization_id", organizationId);
                        command.Parameters.AddWithValue("@building_id", buildingId);
                        command.Parameters.AddWithValue("@date_start", dateStart);

                        // Обрабатываем NULL для date_end
                        if (dateEnd.HasValue)
                            command.Parameters.AddWithValue("@date_end", dateEnd.Value);
                        else
                            command.Parameters.AddWithValue("@date_end", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении записи баланса: {ex.Message}");
                    return false;
                }
            }
        }

        public bool Delete(int balanceId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string deleteSql = "DELETE FROM balance_history WHERE id = @id";

                    using (var command = new NpgsqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", balanceId);
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи баланса: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
