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
    class EventService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;
        public List<Event> GetAll()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                                SELECT 
                                    e.id,
                                    e.title,
                                    e.evt_start,
                                    e.evt_end,
                                    e.count_visitors,
                                    e.balance_id,
                                    e.type_id,
                                    -- Тип мероприятия
                                    et.title as event_type,
                                    -- Организатор (из balance_history -> organizations)
                                    o.org_name as organizer_name,
                                    -- Сооружение 
                                    sb.sb_name as building_name
                                FROM events e
                                LEFT JOIN event_type et ON e.type_id = et.id
                                LEFT JOIN balance_history bh ON e.balance_id = bh.id
                                LEFT JOIN organizations o ON bh.organization_id = o.id
                                LEFT JOIN sport_buildings sb ON bh.building_id = sb.id
                                ORDER BY e.id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        // Получаем индексы столбцов
                        int idIndex = reader.GetOrdinal("id");
                        int titleIndex = reader.GetOrdinal("title");
                        int dateStartIndex = reader.GetOrdinal("evt_start");
                        int dateEndIndex = reader.GetOrdinal("evt_end");
                        int visitorsCountIndex = reader.GetOrdinal("count_visitors");
                        int balanceIdIndex = reader.GetOrdinal("balance_id");
                        int typeIdIndex = reader.GetOrdinal("type_id");
                        int eventTypeIndex = reader.GetOrdinal("event_type");
                        int organizerNameIndex = reader.GetOrdinal("organizer_name");
                        int buildingNameIndex = reader.GetOrdinal("building_name");

                        List<Event> events = new List<Event>();

                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                Id = reader.GetInt32(idIndex),
                                Title = reader.GetString(titleIndex),
                                DateStart = reader.GetDateTime(dateStartIndex),
                                DateEnd = reader.IsDBNull(dateEndIndex) ? (DateTime?)null : reader.GetDateTime(dateEndIndex),
                                VisitorsCount = reader.GetInt32(visitorsCountIndex),
                                BalanceId = reader.GetInt32(balanceIdIndex),
                                TypeId = reader.IsDBNull(typeIdIndex) ? 0 : reader.GetInt32(typeIdIndex),
                                EventType = reader.IsDBNull(eventTypeIndex) ? "Не указан" : reader.GetString(eventTypeIndex),
                                OrganizerName = reader.IsDBNull(organizerNameIndex) ? "Не указан" : reader.GetString(organizerNameIndex),
                                BuildingName = reader.IsDBNull(buildingNameIndex) ? "Не указано" : reader.GetString(buildingNameIndex)
                            });
                        }

                        return events;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки мероприятий: {ex.Message}");
                return new List<Event>();
            }
        }

        public bool Create(string title, DateTime dateStart, DateTime? dateEnd, int visitorsCount, int balanceId, int? typeId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string insertSql = @"
                INSERT INTO events (
                    title, 
                    evt_start, 
                    evt_end, 
                    count_visitors, 
                    balance_id, 
                    type_id
                ) 
                VALUES (
                    @title, 
                    @evt_start, 
                    @evt_end, 
                    @count_visitors, 
                    @balance_id, 
                    @type_id
                )";

                    using (var command = new NpgsqlCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@evt_start", dateStart);

                        // Обрабатываем NULL для date_end
                        if (dateEnd.HasValue)
                            command.Parameters.AddWithValue("@evt_end", dateEnd.Value);
                        else
                            command.Parameters.AddWithValue("@evt_end", DBNull.Value);

                        command.Parameters.AddWithValue("@count_visitors", visitorsCount);
                        command.Parameters.AddWithValue("@balance_id", balanceId);

                        // Обрабатываем NULL для type_id
                        if (typeId.HasValue)
                            command.Parameters.AddWithValue("@type_id", typeId.Value);
                        else
                            command.Parameters.AddWithValue("@type_id", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении мероприятия: {ex.Message}");
                    return false;
                }
            }
        }

        public bool Update(int eventId, string title, DateTime dateStart, DateTime? dateEnd, int visitorsCount, int balanceId, int? typeId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string updateSql = @"
                                    UPDATE events 
                                    SET title = @title, 
                                        evt_start = @evt_start, 
                                        evt_end = @evt_end, 
                                        count_visitors = @count_visitors, 
                                        balance_id = @balance_id, 
                                        type_id = @type_id
                                    WHERE id = @id";

                    using (var command = new NpgsqlCommand(updateSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", eventId);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@evt_start", dateStart);

                        if (dateEnd.HasValue)
                            command.Parameters.AddWithValue("@evt_end", dateEnd.Value);
                        else
                            command.Parameters.AddWithValue("@evt_end", DBNull.Value);

                        command.Parameters.AddWithValue("@count_visitors", visitorsCount);
                        command.Parameters.AddWithValue("@balance_id", balanceId);

                        if (typeId.HasValue)
                            command.Parameters.AddWithValue("@type_id", typeId.Value);
                        else
                            command.Parameters.AddWithValue("@type_id", DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении мероприятия: {ex.Message}");
                    return false;
                }
            }
        }

        public bool Delete(int eventId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                try
                {
                    string deleteSql = "DELETE FROM events WHERE id = @id";

                    using (var command = new NpgsqlCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@id", eventId);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении мероприятия: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
