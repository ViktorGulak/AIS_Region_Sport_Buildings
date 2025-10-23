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
    }
}
