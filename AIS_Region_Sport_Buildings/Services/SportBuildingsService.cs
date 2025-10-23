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
                        int organizationIdIndex = reader.GetOrdinal("organization_id"); // ← ДОБАВЛЕНО
                        int buildingIdIndex = reader.GetOrdinal("building_id");         // ← ДОБАВЛЕНО
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
                                OrganizationId = reader.IsDBNull(organizationIdIndex) ? 0 : reader.GetInt32(organizationIdIndex), // ← ДОБАВЛЕНО
                                BuildingTypeId = reader.IsDBNull(buildingIdIndex) ? 0 : reader.GetInt32(buildingIdIndex),         // ← ДОБАВЛЕНО
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
    }
}
