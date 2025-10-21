using AIS_Region_Sport_Buildings.Models;
using AIS_Region_Sport_Buildings.Types;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace AIS_Region_Sport_Buildings.Services
{
    class EventTypeService : IRefBookService<EventType>
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;

        public List<EventType> GetAll()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = "SELECT id, title FROM event_type ORDER BY id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        List<EventType> eventTypes = new List<EventType>();

                        while (reader.Read())
                        {
                            eventTypes.Add(new EventType
                            {
                                Id = (int)reader["id"],
                                Title = (string)reader["title"]
                            });
                        }

                        return eventTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                return new List<EventType>();
            }
        }

        // CREATE - добавление новой записи
        public bool Create(string title)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = "INSERT INTO event_type (title) VALUES (@title)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", title);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления записи: {ex.Message}");
                return false;
            }
        }

        // UPDATE - обновление существующей записи
        public bool Update(int id, string newTitle)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = "UPDATE event_type SET title = @title WHERE id = @id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@title", newTitle);
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления записи: {ex.Message}");
                return false;
            }
        }

        // DELETE - удаление записи
        public bool Delete(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = "DELETE FROM event_type WHERE id = @id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления записи: {ex.Message}");
                return false;
            }
        }
    }
}

