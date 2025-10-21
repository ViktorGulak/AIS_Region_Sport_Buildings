using AIS_Region_Sport_Buildings.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_Region_Sport_Buildings.Services
{
    class OrganizationService
    {
        private readonly string connStr = ConfigurationManager.ConnectionStrings["DB_Connection"].ConnectionString;

        public List<Organization> GetAll()
        {
            try
            {
                using (var connection = new NpgsqlConnection(connStr))
                {
                    connection.Open();

                    string sql = @"
                    SELECT 
                        o.id, 
                        o.org_number, 
                        o.org_name, 
                        o.org_short_name, 
                        o.org_phone, 
                        o.org_mail, 
                        o.address_id,
                        a.locality_name,
                        a.street, 
                        a.building_number, 
                        a.entrance_number,
                        lt.title
                    FROM organizations o
                    LEFT JOIN addresses a ON o.address_id = a.id
                    LEFT JOIN locality_type lt ON a.locality_id = lt.id
                    ORDER BY o.id";

                    using (var command = new NpgsqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        // Получаем индексы столбцов по именам
                        int idIndex = reader.GetOrdinal("id");
                        int numberIndex = reader.GetOrdinal("org_number");
                        int nameIndex = reader.GetOrdinal("org_name");
                        int shortNameIndex = reader.GetOrdinal("org_short_name");
                        int phoneIndex = reader.GetOrdinal("org_phone");
                        int emailIndex = reader.GetOrdinal("org_mail");
                        int addressIdIndex = reader.GetOrdinal("address_id");
                        int localityNameIndex = reader.GetOrdinal("locality_name");
                        int streetIndex = reader.GetOrdinal("street");
                        int buildingNumberIndex = reader.GetOrdinal("building_number");
                        int entranceNumberIndex = reader.GetOrdinal("entrance_number");
                        int localityTypeIndex = reader.GetOrdinal("title");

                        List<Organization> organizations = new List<Organization>();

                        while (reader.Read())
                        {
                            organizations.Add(new Organization
                            {
                                Id = reader.GetInt32(idIndex),
                                Number = reader.GetString(numberIndex),
                                Name = reader.GetString(nameIndex),
                                ShortName = reader.IsDBNull(shortNameIndex) ? string.Empty : reader.GetString(shortNameIndex),
                                Phone = reader.GetString(phoneIndex),
                                Email = reader.GetString(emailIndex),
                                AddressId = reader.IsDBNull(addressIdIndex) ? 0 : reader.GetInt32(addressIdIndex),
                                LocalityName = reader.IsDBNull(localityNameIndex) ? "Не указан" : reader.GetString(localityNameIndex),
                                Street = reader.IsDBNull(streetIndex) ? "Не указана" : reader.GetString(streetIndex),
                                BuildingNumber = reader.IsDBNull(buildingNumberIndex) ? "Не указан" : reader.GetString(buildingNumberIndex),
                                EntranceNumber = reader.IsDBNull(entranceNumberIndex) ? "Не указан" : reader.GetString(entranceNumberIndex),
                                LocalityType = reader.GetString(localityTypeIndex)
                            }); ;
                        }

                        return organizations;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки организаций: {ex.Message}");
                return new List<Organization>();
            }
        }
    }
}
