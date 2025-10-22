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
                        a.locality_id,
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
                        int locTypeidIndex = reader.GetOrdinal("locality_id");
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
                                LocalityTypeId = reader.GetInt32(locTypeidIndex),
                                LocalityType = reader.GetString(localityTypeIndex)
                            });
                        }

                        return organizations;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки организаций: {ex.Message}");
                return new List<Organization>();
            }
        }

        public bool Create(string orgNumber, string orgName, string orgShortName, string orgPhone, string orgEmail,
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

                        // 2. Добавляем организацию с полученным address_id
                        string insertOrgSql = @"
                    INSERT INTO organizations (org_number, org_name, org_short_name, org_phone, org_mail, address_id) 
                    VALUES (@org_number, @org_name, @org_short_name, @org_phone, @org_mail, @address_id)";

                        using (var orgCommand = new NpgsqlCommand(insertOrgSql, connection, transaction))
                        {
                            orgCommand.Parameters.AddWithValue("@org_number", orgNumber);
                            orgCommand.Parameters.AddWithValue("@org_name", orgName);
                            orgCommand.Parameters.AddWithValue("@org_short_name", string.IsNullOrEmpty(orgShortName) ? (object)DBNull.Value : orgShortName);
                            orgCommand.Parameters.AddWithValue("@org_phone", orgPhone);
                            orgCommand.Parameters.AddWithValue("@org_mail", orgEmail);
                            orgCommand.Parameters.AddWithValue("@address_id", addressId);

                            int rowsAffected = orgCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
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
                        MessageBox.Show($"Ошибка при добавлении организации: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool Update(int orgId, string orgNumber, string orgName, string orgShortName, string orgPhone, string orgEmail,
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
                        // 1. Получаем address_id организации
                        int addressId;
                        string getAddressIdSql = "SELECT address_id FROM organizations WHERE id = @org_id";

                        using (var getAddressCommand = new NpgsqlCommand(getAddressIdSql, connection, transaction))
                        {
                            getAddressCommand.Parameters.AddWithValue("@org_id", orgId);
                            var result = getAddressCommand.ExecuteScalar();
                            addressId = result != null ? (int)result : 0;
                        }

                        if (addressId == 0)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Не удалось найти адрес организации");
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

                        // 3. Обновляем организацию
                        string updateOrgSql = @"
                    UPDATE organizations 
                    SET org_number = @org_number, 
                        org_name = @org_name, 
                        org_short_name = @org_short_name, 
                        org_phone = @org_phone, 
                        org_mail = @org_mail 
                    WHERE id = @org_id";

                        using (var orgCommand = new NpgsqlCommand(updateOrgSql, connection, transaction))
                        {
                            orgCommand.Parameters.AddWithValue("@org_number", orgNumber);
                            orgCommand.Parameters.AddWithValue("@org_name", orgName);
                            orgCommand.Parameters.AddWithValue("@org_short_name", string.IsNullOrEmpty(orgShortName) ? (object)DBNull.Value : orgShortName);
                            orgCommand.Parameters.AddWithValue("@org_phone", orgPhone);
                            orgCommand.Parameters.AddWithValue("@org_mail", orgEmail);
                            orgCommand.Parameters.AddWithValue("@org_id", orgId);

                            int orgRowsAffected = orgCommand.ExecuteNonQuery();

                            if (orgRowsAffected > 0)
                            {
                                // Если всё успешно - коммитим транзакцию
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
                        // При любой ошибке откатываем транзакцию
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при обновлении организации: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public bool Delete(int orgId)
        {
            using (var connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                // Начинаем транзакцию
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Получаем address_id организации
                        int addressId;
                        string getAddressIdSql = "SELECT address_id FROM organizations WHERE id = @org_id";

                        using (var getAddressCommand = new NpgsqlCommand(getAddressIdSql, connection, transaction))
                        {
                            getAddressCommand.Parameters.AddWithValue("@org_id", orgId);
                            var result = getAddressCommand.ExecuteScalar();
                            addressId = result != null ? (int)result : 0;
                        }

                        // 2. Удаляем организацию (внешний ключ address_id имеет on delete set null)
                        string deleteOrgSql = "DELETE FROM organizations WHERE id = @org_id";

                        using (var orgCommand = new NpgsqlCommand(deleteOrgSql, connection, transaction))
                        {
                            orgCommand.Parameters.AddWithValue("@org_id", orgId);
                            int orgRowsAffected = orgCommand.ExecuteNonQuery();

                            if (orgRowsAffected > 0)
                            {
                                // 3. Если адрес больше никем не используется - удаляем его
                                if (addressId > 0)
                                {
                                    string checkAddressUsageSql = "SELECT COUNT(*) FROM organizations WHERE address_id = @address_id";
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
                        MessageBox.Show($"Ошибка при удалении организации: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
