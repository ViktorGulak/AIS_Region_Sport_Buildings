using AIS_Region_Sport_Buildings.Models;
using AIS_Region_Sport_Buildings.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIS_Region_Sport_Buildings
{
    /// <summary>
    /// Логика взаимодействия для SpBldWindow.xaml
    /// </summary>
    public partial class SpBldWindow : Window
    {
        private LocalityTypeService _localityTypeService = new LocalityTypeService();
        private BuildingTypeService _buildingTypeService = new BuildingTypeService();
        private OrganizationService _organizationService = new OrganizationService();
        private SportBuildingsService _sportBuildingsService = new SportBuildingsService();

        private SportBuildings _selectedBuilding;
        public SpBldWindow()
        {
            InitializeComponent();

            LoadSportBuildings();
            // Заполняем ComboBox-ы
            LoadLocalityTypes();
            LoadBuildingsTypes();
            LoadOrganizations();
        }

        private void LoadLocalityTypes()
        {
            List<LocalityType> localityTypes = _localityTypeService.GetAll();
            LocTypeCB.ItemsSource = localityTypes;
            LocTypeCB.DisplayMemberPath = "Title";  // Что показывать
            LocTypeCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (localityTypes.Count > 0)
                LocTypeCB.SelectedIndex = -1;
        }

        private void LoadBuildingsTypes()
        {
            List<BuildingType> buildingsTypes = _buildingTypeService.GetAll();
            SpBldTypeCB.ItemsSource = buildingsTypes;
            SpBldTypeCB.DisplayMemberPath = "Title";  // Что показывать
            SpBldTypeCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (buildingsTypes.Count > 0)
                SpBldTypeCB.SelectedIndex = -1;
        }

        private void LoadOrganizations()
        {
            List<Organization> organizations = _organizationService.GetAll();
            OrgCB.ItemsSource = organizations;
            OrgCB.DisplayMemberPath = "Name";  // Что показывать
            OrgCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (organizations.Count > 0)
                OrgCB.SelectedIndex = -1;
        }

        private void LoadSportBuildings()
        {
            
            List<SportBuildings> buildings = _sportBuildingsService.GetAll();
            SpBldDG.ItemsSource = buildings;
        }

        private void SpBldDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBuilding = SpBldDG.SelectedItem as SportBuildings;  // ← Сначала устанавливаем!

            if (_selectedBuilding != null)  // ← Потом проверяем!
            {
                SpBldTitleTB.Text = _selectedBuilding.Name;
                SpBldNumberTB.Text = _selectedBuilding.Number;
                SpBldShortTB.Text = _selectedBuilding.ShortName ?? string.Empty;
                SpBldAreaTB.Text = _selectedBuilding.ArenaArea.ToString();
                SpBldCapacityTB.Text = _selectedBuilding.ArenaCapacity.ToString();

                LocNameTB.Text = _selectedBuilding.LocalityName ?? string.Empty;
                LocStreetTB.Text = _selectedBuilding.Street ?? string.Empty;
                BldNumberTB.Text = _selectedBuilding.BuildingNumber ?? string.Empty;
                EntrNumberTB.Text = _selectedBuilding.EntranceNumber ?? string.Empty;

                // Используем LocalityTypeId если он есть
                
                LocTypeCB.SelectedValue = _selectedBuilding.LocalityTypeId;
                SpBldTypeCB.SelectedValue = _selectedBuilding.BuildingTypeId;
                OrgCB.SelectedValue = _selectedBuilding.OrganizationId;

            }
            else
            {
                // Очищаем поля если ничего не выбрано
                ClearTextFields();
            }
        }

        private void ClearTextFields()
        {
            SpBldTitleTB.Clear();
            SpBldNumberTB.Clear();
            SpBldShortTB.Clear();
            SpBldAreaTB.Clear();
            SpBldCapacityTB.Clear();
            LocNameTB.Clear();
            LocStreetTB.Clear();
            BldNumberTB.Clear();
            EntrNumberTB.Clear();
            LocTypeCB.SelectedIndex = -1;
            SpBldTypeCB.SelectedIndex = -1;
            OrgCB.SelectedIndex = -1;
            _selectedBuilding = null;
        }

        private void ClearTextFildsBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }

        private void AddSpBlgTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = SpBldTypeCB.Items.Count;
            ReferenceBooks refBook = new ReferenceBooks(1);
            // Подписываемся на событие закрытия refBook окна
            refBook.Closed += (s, args) =>
            {
                // Когда форма закрывается - обновляем ComboBox
                LoadBuildingsTypes();

                if (prevItemsCount == SpBldTypeCB.Items.Count || prevItemsCount > SpBldTypeCB.Items.Count)
                {
                    SpBldTypeCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < SpBldTypeCB.Items.Count)
                {
                    SpBldTypeCB.SelectedIndex = SpBldTypeCB.Items.Count - 1;
                }
            };

            refBook.ShowDialog();
        }

        private void AddOrganizationBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = OrgCB.Items.Count;
            OrgWindow orgWin = new OrgWindow();
            orgWin.Closed += (s, args) =>
            {
                LoadOrganizations();

                if (prevItemsCount == OrgCB.Items.Count || prevItemsCount > OrgCB.Items.Count)
                {
                    OrgCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < OrgCB.Items.Count)
                {
                    OrgCB.SelectedIndex = OrgCB.Items.Count - 1;
                }
            };

            orgWin.ShowDialog();
        }

        private void OpenRefBookBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = LocTypeCB.Items.Count;
            ReferenceBooks refBook = new ReferenceBooks(0);
            refBook.Closed += (s, args) =>
            {
                // Когда форма закрывается - обновляем ComboBox
                LoadLocalityTypes();

                if (prevItemsCount == LocTypeCB.Items.Count || prevItemsCount > LocTypeCB.Items.Count)
                {
                    LocTypeCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < LocTypeCB.Items.Count)
                {
                    LocTypeCB.SelectedIndex = LocTypeCB.Items.Count - 1;
                }
            };

            refBook.ShowDialog();
        }

        private void AddOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(SpBldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldTitleTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldAreaTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldCapacityTB.Text) ||
                OrgCB.SelectedValue == null ||
                SpBldTypeCB.SelectedValue == null ||
                string.IsNullOrWhiteSpace(LocNameTB.Text) ||
                string.IsNullOrWhiteSpace(LocStreetTB.Text) ||
                string.IsNullOrWhiteSpace(BldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(EntrNumberTB.Text) ||
                LocTypeCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            // Проверка числовых полей
            if (!int.TryParse(SpBldAreaTB.Text, out int area) || area <= 0)
            {
                MessageBox.Show("Площадь арены должна быть положительным числом");
                return;
            }

            if (!int.TryParse(SpBldCapacityTB.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Вместимость должна быть положительным числом");
                return;
            }

            try
            {
                bool success = _sportBuildingsService.Create(
                    sbNumber: SpBldNumberTB.Text.Trim(),
                    sbName: SpBldTitleTB.Text.Trim(),
                    sbShortName: SpBldShortTB.Text.Trim(),
                    arenaArea: area,
                    arenaCapacity: capacity,
                    organizationId: (int)OrgCB.SelectedValue,
                    buildingTypeId: (int)SpBldTypeCB.SelectedValue,
                    localityName: LocNameTB.Text.Trim(),
                    street: LocStreetTB.Text.Trim(),
                    buildingNumber: BldNumberTB.Text.Trim(),
                    entranceNumber: EntrNumberTB.Text.Trim(),
                    localityTypeId: (int)LocTypeCB.SelectedValue
                );

                if (success)
                {
                    MessageBox.Show("Спортивное сооружение успешно добавлено");
                    LoadSportBuildings(); // Обновляем DataGrid
                    ClearTextFields(); // Очищаем поля
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении спортивного сооружения");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void UpdateOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuilding == null)
            {
                MessageBox.Show("Выберите спортивное сооружение для изменения");
                return;
            }

            // Валидация данных
            if (string.IsNullOrWhiteSpace(SpBldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldTitleTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldAreaTB.Text) ||
                string.IsNullOrWhiteSpace(SpBldCapacityTB.Text) ||
                OrgCB.SelectedValue == null ||
                SpBldTypeCB.SelectedValue == null ||
                string.IsNullOrWhiteSpace(LocNameTB.Text) ||
                string.IsNullOrWhiteSpace(LocStreetTB.Text) ||
                string.IsNullOrWhiteSpace(BldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(EntrNumberTB.Text) ||
                LocTypeCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            // Проверка числовых полей
            if (!int.TryParse(SpBldAreaTB.Text, out int area) || area <= 0)
            {
                MessageBox.Show("Площадь арены должна быть положительным числом");
                return;
            }

            if (!int.TryParse(SpBldCapacityTB.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Вместимость должна быть положительным числом");
                return;
            }

            try
            {
                bool success = _sportBuildingsService.Update(
                    buildingId: _selectedBuilding.Id,
                    sbNumber: SpBldNumberTB.Text.Trim(),
                    sbName: SpBldTitleTB.Text.Trim(),
                    sbShortName: SpBldShortTB.Text.Trim(),
                    arenaArea: area,
                    arenaCapacity: capacity,
                    organizationId: (int)OrgCB.SelectedValue,
                    buildingTypeId: (int)SpBldTypeCB.SelectedValue,
                    localityName: LocNameTB.Text.Trim(),
                    street: LocStreetTB.Text.Trim(),
                    buildingNumber: BldNumberTB.Text.Trim(),
                    entranceNumber: EntrNumberTB.Text.Trim(),
                    localityTypeId: (int)LocTypeCB.SelectedValue
                );

                if (success)
                {
                    MessageBox.Show("Спортивное сооружение успешно обновлено");
                    LoadSportBuildings();
                    ClearTextFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении спортивного сооружения");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DeleteOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuilding == null)
            {
                MessageBox.Show("Выберите спортивное сооружение для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить спортивное сооружение '{_selectedBuilding.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _sportBuildingsService.Delete(_selectedBuilding.Id);
                    if (success)
                    {
                        MessageBox.Show("Спортивное сооружение успешно удалено");
                        LoadSportBuildings();
                        ClearTextFields();
                        _selectedBuilding = null;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении спортивного сооружения");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}
