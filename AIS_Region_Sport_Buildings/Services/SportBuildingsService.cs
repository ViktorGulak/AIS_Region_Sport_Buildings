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
    class SportBuildingsService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;

        public List<SportBuildings> GetAll()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                                    SELECT 
                                        sb.id,
                                        sb.sb_number,
                                        sb.sb_name,
                                        sb.sb_short_name,
                                        sb.arena_area,
                                        sb.arena_capacity,
                                        sb.address_id,
                                        sb.organization_id,  -- ← ДОБАВЛЕНО
                                        sb.building_id,      -- ← ДОБАВЛЕНО
                                        -- Данные адреса
                                        a.locality_name,
                                        a.street,
                                        a.building_number,
                                        a.entrance_number,
                                        -- Тип населенного пункта
                                        lt.title as locality_type,
                                        lt.id as locality_type_id,
                                        -- Тип сооружения
                                        bt.title as building_type,
                                        -- Организация-информатор
                                        o.org_name as organization_name
                                    FROM sport_buildings sb
                                    LEFT JOIN addresses a ON sb.address_id = a.id
                                    LEFT JOIN locality_type lt ON a.locality_id = lt.id
                                    LEFT JOIN building_type bt ON sb.building_id = bt.id
                                    LEFT JOIN organizations o ON sb.organization_id = o.id
                                    ORDER BY sb.id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        // Получаем индексы столбцов
                        int idIndex = reader.GetOrdinal("id");
                        int numberIndex = reader.GetOrdinal("sb_number");
                        int nameIndex = reader.GetOrdinal("sb_name");
                        int shortNameIndex = reader.GetOrdinal("sb_short_name");
                        int areaIndex = reader.GetOrdinal("arena_area");
                        int capacityIndex = reader.GetOrdinal("arena_capacity");
                        int addressIdIndex = reader.GetOrdinal("address_id");
                        int organizationIdIndex = reader.GetOrdinal("organization_id"); 
                        int buildingIdIndex = reader.GetOrdinal("building_id");         
                        int localityNameIndex = reader.GetOrdinal("locality_name");
                        int streetIndex = reader.GetOrdinal("street");
                        int buildingNumberIndex = reader.GetOrdinal("building_number");
                        int entranceNumberIndex = reader.GetOrdinal("entrance_number");
                        int localityTypeIndex = reader.GetOrdinal("locality_type");
                        int localityTypeIdIndex = reader.GetOrdinal("locality_type_id");
                        int buildingTypeIndex = reader.GetOrdinal("building_type");
                        int organizationNameIndex = reader.GetOrdinal("organization_name");

                        List<SportBuildings> sportBuildings = new List<SportBuildings>();

                        while (reader.Read())
                        {
                            sportBuildings.Add(new SportBuildings
                            {
                                Id = reader.GetInt32(idIndex),
                                Number = reader.GetString(numberIndex),
                                Name = reader.GetString(nameIndex),
                                ShortName = reader.IsDBNull(shortNameIndex) ? string.Empty : reader.GetString(shortNameIndex),
                                ArenaArea = reader.GetInt32(areaIndex),
                                ArenaCapacity = reader.GetInt32(capacityIndex),
                                AddressId = reader.IsDBNull(addressIdIndex) ? 0 : reader.GetInt32(addressIdIndex),
                                OrganizationId = reader.IsDBNull(organizationIdIndex) ? 0 : reader.GetInt32(organizationIdIndex), 
                                BuildingTypeId = reader.IsDBNull(buildingIdIndex) ? 0 : reader.GetInt32(buildingIdIndex),         
                                LocalityName = reader.IsDBNull(localityNameIndex) ? "Не указан" : reader.GetString(localityNameIndex),
                                Street = reader.IsDBNull(streetIndex) ? "Не указана" : reader.GetString(streetIndex),
                                BuildingNumber = reader.IsDBNull(buildingNumberIndex) ? "Не указан" : reader.GetString(buildingNumberIndex),
                                EntranceNumber = reader.IsDBNull(entranceNumberIndex) ? "Не указан" : reader.GetString(entranceNumberIndex),
                                LocalityType = reader.IsDBNull(localityTypeIndex) ? "Не указан" : reader.GetString(localityTypeIndex),
                                LocalityTypeId = reader.IsDBNull(localityTypeIdIndex) ? 0 : reader.GetInt32(localityTypeIdIndex),
                                BuildingType = reader.IsDBNull(buildingTypeIndex) ? "Не указан" : reader.GetString(buildingTypeIndex),
                                OrganizationName = reader.IsDBNull(organizationNameIndex) ? "Не указана" : reader.GetString(organizationNameIndex)
                            });
                        }

                        return sportBuildings;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки спортивных сооружений: {ex.Message}");
                return new List<SportBuildings>();
            }
        }

        public bool Create(string sbNumber, string sbName, string sbShortName, int arenaArea, int arenaCapacity,
                      int organizationId, int buildingTypeId,
                      string localityName, string street, string buildingNumber, string entranceNumber, int localityTypeId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                // Начинаем транзакцию
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int addressId;

                        // 1. Добавляем адрес
                        string insertAddressSql = @"
                        INSERT INTO addresses (locality_name, street, building_number, entrance_number, locality_id) 
                        VALUES (@locality_name, @street, @building_number, @entrance_number, @locality_id) 
                        RETURNING id";

                        using (var addressCommand = new NpgsqlCommand(insertAddressSql, connection, transaction))
                        {
                            addressCommand.Parameters.AddWithValue("@locality_name", localityName);
                            addressCommand.Parameters.AddWithValue("@street", street);
                            addressCommand.Parameters.AddWithValue("@building_number", buildingNumber);
                            addressCommand.Parameters.AddWithValue("@entrance_number", entranceNumber);
                            addressCommand.Parameters.AddWithValue("@locality_id", localityTypeId);

                            // Получаем ID нового адреса
                            addressId = (int)addressCommand.ExecuteScalar();
                        }

                        // 2. Добавляем спортивное сооружение с полученным address_id
                        string insertBuildingSql = @"
                        INSERT INTO sport_buildings (
                            sb_number, sb_name, sb_short_name, arena_area, arena_capacity, 
                            address_id, organization_id, building_id
                        ) 
                        VALUES (
                            @sb_number, @sb_name, @sb_short_name, @arena_area, @arena_capacity,
                            @address_id, @organization_id, @building_id
                        )";

                        using (var buildingCommand = new NpgsqlCommand(insertBuildingSql, connection, transaction))
                        {
                            buildingCommand.Parameters.AddWithValue("@sb_number", sbNumber);
                            buildingCommand.Parameters.AddWithValue("@sb_name", sbName);
                            buildingCommand.Parameters.AddWithValue("@sb_short_name",
                                string.IsNullOrEmpty(sbShortName) ? (object)DBNull.Value : sbShortName);
                            buildingCommand.Parameters.AddWithValue("@arena_area", arenaArea);
                            buildingCommand.Parameters.AddWithValue("@arena_capacity", arenaCapacity);
                            buildingCommand.Parameters.AddWithValue("@address_id", addressId);
                            buildingCommand.Parameters.AddWithValue("@organization_id", organizationId);
                            buildingCommand.Parameters.AddWithValue("@building_id", buildingTypeId);

                            int rowsAffected = buildingCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // Если всё успешно - коммитим транзакцию
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                // Если что-то пошло не так - откатываем
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // При любой ошибке откатываем транзакцию
                        transaction.Rollback();
                        Console.WriteLine($"Ошибка при добавлении спортивного сооружения: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool Update(int buildingId, string sbNumber, string sbName, string sbShortName, int arenaArea, int arenaCapacity,
                  int organizationId, int buildingTypeId,
                  string localityName, string street, string buildingNumber, string entranceNumber, int localityTypeId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Получаем address_id спортивного сооружения
                        int addressId;
                        string getAddressIdSql = "SELECT address_id FROM sport_buildings WHERE id = @building_id";

                        using (var getAddressCommand = new NpgsqlCommand(getAddressIdSql, connection, transaction))
                        {
                            getAddressCommand.Parameters.AddWithValue("@building_id", buildingId);
                            var result = getAddressCommand.ExecuteScalar();
                            addressId = result != null ? (int)result : 0;
                        }

                        if (addressId == 0)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Не удалось найти адрес спортивного сооружения");
                            return false;
                        }

                        // 2. Обновляем адрес
                        string updateAddressSql = @"
                UPDATE addresses 
                SET locality_name = @locality_name, 
                    street = @street, 
                    building_number = @building_number, 
                    entrance_number = @entrance_number, 
                    locality_id = @locality_id 
                WHERE id = @address_id";

                        using (var addressCommand = new NpgsqlCommand(updateAddressSql, connection, transaction))
                        {
                            addressCommand.Parameters.AddWithValue("@locality_name", localityName);
                            addressCommand.Parameters.AddWithValue("@street", street);
                            addressCommand.Parameters.AddWithValue("@building_number", buildingNumber);
                            addressCommand.Parameters.AddWithValue("@entrance_number", entranceNumber);
                            addressCommand.Parameters.AddWithValue("@locality_id", localityTypeId);
                            addressCommand.Parameters.AddWithValue("@address_id", addressId);

                            int addressRowsAffected = addressCommand.ExecuteNonQuery();
                            if (addressRowsAffected == 0)
                            {
                                transaction.Rollback();
                                MessageBox.Show("Не удалось обновить адрес");
                                return false;
                            }
                        }

                        // 3. Обновляем спортивное сооружение - ИСПРАВЛЕННЫЙ ЗАПРОС
                        string updateBuildingSql = @"
                UPDATE sport_buildings 
                SET sb_number = @sb_number, 
                    sb_name = @sb_name, 
                    sb_short_name = @sb_short_name, 
                    arena_area = @arena_area, 
                    arena_capacity = @arena_capacity, 
                    organization_id = @organization_id, 
                    building_id = @building_type_id  -- ← изменил имя параметра для ясности
                WHERE id = @building_id"; 
        
                using (var buildingCommand = new NpgsqlCommand(updateBuildingSql, connection, transaction))
                        {
                            buildingCommand.Parameters.AddWithValue("@sb_number", sbNumber);
                            buildingCommand.Parameters.AddWithValue("@sb_name", sbName);
                            buildingCommand.Parameters.AddWithValue("@sb_short_name",
                                string.IsNullOrEmpty(sbShortName) ? (object)DBNull.Value : sbShortName);
                            buildingCommand.Parameters.AddWithValue("@arena_area", arenaArea);
                            buildingCommand.Parameters.AddWithValue("@arena_capacity", arenaCapacity);
                            buildingCommand.Parameters.AddWithValue("@organization_id", organizationId);
                            buildingCommand.Parameters.AddWithValue("@building_type_id", buildingTypeId);  // ← изменил имя параметра
                            buildingCommand.Parameters.AddWithValue("@building_id", buildingId);  // ← ИСПРАВЛЕНО!

                            int buildingRowsAffected = buildingCommand.ExecuteNonQuery();

                            if (buildingRowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("Не удалось обновить спортивное сооружение");
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при обновлении спортивного сооружения: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool Delete(int buildingId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Получаем address_id спортивного сооружения
                        int addressId;
                        string getAddressIdSql = "SELECT address_id FROM sport_buildings WHERE id = @building_id";

                        using (var getAddressCommand = new NpgsqlCommand(getAddressIdSql, connection, transaction))
                        {
                            getAddressCommand.Parameters.AddWithValue("@building_id", buildingId);
                            var result = getAddressCommand.ExecuteScalar();
                            addressId = result != DBNull.Value && result != null ? (int)result : 0; // ← ДОБАВЛЕНА ПРОВЕРКА на DBNull
                        }

                        // 2. Удаляем спортивное сооружение
                        string deleteBuildingSql = "DELETE FROM sport_buildings WHERE id = @building_id";

                        using (var buildingCommand = new NpgsqlCommand(deleteBuildingSql, connection, transaction))
                        {
                            buildingCommand.Parameters.AddWithValue("@building_id", buildingId);
                            int buildingRowsAffected = buildingCommand.ExecuteNonQuery();

                            if (buildingRowsAffected > 0)
                            {
                                // 3. Если адрес больше никем не используется - удаляем его
                                if (addressId > 0)
                                {
                                    // УЛУЧШЕННАЯ ПРОВЕРКА - учитываем NULL значения
                                    string checkAddressUsageSql = @"
                            SELECT COUNT(*) FROM (
                                SELECT address_id FROM sport_buildings WHERE address_id IS NOT NULL
                                UNION ALL 
                                SELECT address_id FROM organizations WHERE address_id IS NOT NULL
                            ) AS all_addresses 
                            WHERE address_id = @address_id";

                                    using (var checkCommand = new NpgsqlCommand(checkAddressUsageSql, connection, transaction))
                                    {
                                        checkCommand.Parameters.AddWithValue("@address_id", addressId);
                                        var usageCount = (long)checkCommand.ExecuteScalar();

                                        if (usageCount == 0)
                                        {
                                            string deleteAddressSql = "DELETE FROM addresses WHERE id = @address_id";
                                            using (var deleteAddressCommand = new NpgsqlCommand(deleteAddressSql, connection, transaction))
                                            {
                                                deleteAddressCommand.Parameters.AddWithValue("@address_id", addressId);
                                                deleteAddressCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при удалении спортивного сооружения: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
