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
    /// Логика взаимодействия для Tables.xaml
    /// </summary>
    public partial class OrgWindow : Window
    {
        private OrganizationService _organizationService = new OrganizationService();
        private Organization _selectedOrganization;

        private LocalityTypeService _localityTypeService = new LocalityTypeService();
        public OrgWindow()
        {
            InitializeComponent();

            LoadOrganizations();
            LoadLocalityTypes();
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
        private void LoadOrganizations()
        {
            List<Organization> organizations = _organizationService.GetAll();
            OrganizationDG.ItemsSource = organizations;
        }

        private void OrganizationDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrganization = OrganizationDG.SelectedItem as Organization;  // ← Сначала устанавливаем!

            if (_selectedOrganization != null)  // ← Потом проверяем!
            {
                OrgTitleTB.Text = _selectedOrganization.Name;
                OrgNumberTB.Text = _selectedOrganization.Number;
                OrgShortTB.Text = _selectedOrganization.ShortName ?? string.Empty;
                OrgEmailTB.Text = _selectedOrganization.Email;
                OrgPhoneTB.Text = _selectedOrganization.Phone;

                // Используем LocalityTypeId если он есть
                if (_selectedOrganization.LocalityTypeId > 0)
                {
                    LocTypeCB.SelectedValue = _selectedOrganization.LocalityTypeId;
                }
                else
                {
                    LocTypeCB.SelectedIndex = -1;
                }

                LocNameTB.Text = _selectedOrganization.LocalityName ?? string.Empty;
                LocStreetTB.Text = _selectedOrganization.Street ?? string.Empty;
                BldNumberTB.Text = _selectedOrganization.BuildingNumber ?? string.Empty;
                EntrNumberTB.Text = _selectedOrganization.EntranceNumber ?? string.Empty;
            }
            else
            {
                // Очищаем поля если ничего не выбрано
                ClearTextFields();
            }
        }

        private void ClearTextFieldsBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }

        private void AddOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(OrgNumberTB.Text) ||
                string.IsNullOrWhiteSpace(OrgTitleTB.Text) ||
                string.IsNullOrWhiteSpace(OrgPhoneTB.Text) ||
                string.IsNullOrWhiteSpace(OrgEmailTB.Text) ||
                string.IsNullOrWhiteSpace(LocNameTB.Text) ||
                string.IsNullOrWhiteSpace(LocStreetTB.Text) ||
                string.IsNullOrWhiteSpace(BldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(EntrNumberTB.Text) ||
                LocTypeCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            try
            {

                bool success = _organizationService.Create(
                    orgNumber: OrgNumberTB.Text.Trim(),
                    orgName: OrgTitleTB.Text.Trim(),
                    orgShortName: OrgShortTB.Text.Trim(),
                    orgPhone: OrgPhoneTB.Text.Trim(),
                    orgEmail: OrgEmailTB.Text.Trim(),
                    localityName: LocNameTB.Text.Trim(),
                    street: LocStreetTB.Text.Trim(),
                    buildingNumber: BldNumberTB.Text.Trim(),
                    entranceNumber: EntrNumberTB.Text.Trim(),
                    localityTypeId: (int)LocTypeCB.SelectedValue  // передаем ID, а не строку
                );

                if (success)
                {
                    MessageBox.Show("Организация успешно добавлена");
                    LoadOrganizations();
                    ClearTextFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении организации");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Очистка полей
        private void ClearTextFields()
        {
            OrgNumberTB.Clear();
            OrgTitleTB.Clear();
            OrgShortTB.Clear();
            OrgPhoneTB.Clear();
            OrgEmailTB.Clear();
            LocNameTB.Clear();
            LocStreetTB.Clear();
            BldNumberTB.Clear();
            EntrNumberTB.Clear();
            LocTypeCB.SelectedItem = null;
        }

        private void UpdateOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrganization == null)
            {
                MessageBox.Show("Выберите организацию для изменения");
                return;
            }

            // Валидация данных
            if (string.IsNullOrWhiteSpace(OrgNumberTB.Text) ||
                string.IsNullOrWhiteSpace(OrgTitleTB.Text) ||
                string.IsNullOrWhiteSpace(OrgPhoneTB.Text) ||
                string.IsNullOrWhiteSpace(OrgEmailTB.Text) ||
                string.IsNullOrWhiteSpace(LocNameTB.Text) ||
                string.IsNullOrWhiteSpace(LocStreetTB.Text) ||
                string.IsNullOrWhiteSpace(BldNumberTB.Text) ||
                string.IsNullOrWhiteSpace(EntrNumberTB.Text) ||
                LocTypeCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            try
            {
                bool success = _organizationService.Update(
                    orgId: _selectedOrganization.Id,
                    orgNumber: OrgNumberTB.Text.Trim(),
                    orgName: OrgTitleTB.Text.Trim(),
                    orgShortName: OrgShortTB.Text.Trim(),
                    orgPhone: OrgPhoneTB.Text.Trim(),
                    orgEmail: OrgEmailTB.Text.Trim(),
                    localityName: LocNameTB.Text.Trim(),
                    street: LocStreetTB.Text.Trim(),
                    buildingNumber: BldNumberTB.Text.Trim(),
                    entranceNumber: EntrNumberTB.Text.Trim(),
                    localityTypeId: (int)LocTypeCB.SelectedValue
                );

                if (success)
                {
                    MessageBox.Show("Организация успешно обновлена");
                    LoadOrganizations(); // Обновляем DataGrid
                    ClearTextFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении организации");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DeleteOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrganization == null)
            {
                MessageBox.Show("Выберите организацию для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить организацию '{_selectedOrganization.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _organizationService.Delete(_selectedOrganization.Id);
                    if (success)
                    {
                        MessageBox.Show("Организация успешно удалена");
                        LoadOrganizations(); // Обновляем DataGrid
                        ClearTextFields(); // Очищаем поля
                        _selectedOrganization = null;
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении организации");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void OpenRefBookBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = LocTypeCB.Items.Count;
            ReferenceBooks refBook = new ReferenceBooks(0);
            // Подписываемся на событие закрытия refBook окна
            refBook.Closed += (s, args) =>
            {
                // Когда форма закрывается - обновляем ComboBox
                LoadLocalityTypes();

                if(prevItemsCount == LocTypeCB.Items.Count || prevItemsCount > LocTypeCB.Items.Count)
                {
                    LocTypeCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < LocTypeCB.Items.Count)
                {
                    LocTypeCB.SelectedIndex = LocTypeCB.Items.Count - 1;
                }
            };

            refBook.ShowDialog(); // ShowDialog блокирует родительскую форму
        }
    }
}
