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
    /// Логика взаимодействия для BalanceWindow.xaml
    /// </summary>
    public partial class BalanceWindow : Window
    {
        private BalanceService _balanceService = new BalanceService();
        private OrganizationService _organizationService = new OrganizationService();
        private SportBuildingsService _sportBuildingsService = new SportBuildingsService();

        private Balance _selectedBalance;
        public BalanceWindow()
        {
            InitializeComponent();
            LoadBalanceHistory();
            LoadOrganizations();
            LoadSportBuildings();
        }

        private void LoadOrganizations()
        {
            List<Organization> organizations = _organizationService.GetAll();
            BalanceOrgCB.ItemsSource = organizations;
            BalanceOrgCB.DisplayMemberPath = "Name";  // Что показывать
            BalanceOrgCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (organizations.Count > 0)
                BalanceOrgCB.SelectedIndex = -1;
        }

        private void LoadSportBuildings()
        {
            List<SportBuildings> sportBuildings = _sportBuildingsService.GetAll();
            BalanceBldCB.ItemsSource = sportBuildings;
            BalanceBldCB.DisplayMemberPath = "Name";  
            BalanceBldCB.SelectedValuePath = "Id";    

            if (sportBuildings.Count > 0)
                BalanceBldCB.SelectedIndex = -1;
        }

        private void LoadBalanceHistory()
        {
            List<Balance> balances = _balanceService.GetAll();
            BalanceDG.ItemsSource = balances;
        }

        private void BalanceDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBalance = BalanceDG.SelectedItem as Balance;

            if (_selectedBalance != null)
            {
                // Заполняем поля данными выбранной записи
                BalanceOrgCB.SelectedValue = _selectedBalance.OrganizationId;
                BalanceBldCB.SelectedValue = _selectedBalance.BuildingId;
                BalanceStartDP.SelectedDate = _selectedBalance.DateStart;
                BalanceEndDP.SelectedDate = _selectedBalance.DateEnd;
            }
        }

        private void ClearTextFields()
        {
            BalanceOrgCB.SelectedIndex = -1;
            BalanceBldCB.SelectedIndex = -1;
            BalanceStartDP.SelectedDate = null;
            BalanceEndDP.SelectedDate = null;
            _selectedBalance = null;
        }

        private void ClearBlcTextFieldBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }

        private void AddNewOrgBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = BalanceOrgCB.Items.Count;
            OrgWindow orgWindow = new OrgWindow();
            
            orgWindow.Closed += (s, args) =>
            {
                LoadOrganizations();

                if (prevItemsCount == BalanceOrgCB.Items.Count || prevItemsCount > BalanceOrgCB.Items.Count)
                {
                    BalanceOrgCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < BalanceOrgCB.Items.Count)
                {
                    BalanceOrgCB.SelectedIndex = BalanceOrgCB.Items.Count - 1;
                }
            };

            orgWindow.ShowDialog();
        }

        private void AddNewSpBldBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = BalanceBldCB.Items.Count;
            SpBldWindow spBldWindow = new SpBldWindow();

            spBldWindow.Closed += (s, args) =>
            {
                LoadSportBuildings();

                if (prevItemsCount == BalanceBldCB.Items.Count || prevItemsCount > BalanceBldCB.Items.Count)
                {
                    BalanceBldCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < BalanceBldCB.Items.Count)
                {
                    BalanceBldCB.SelectedIndex = BalanceBldCB.Items.Count - 1;
                }
            };

            spBldWindow.ShowDialog();
        }

        private void AddBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (BalanceOrgCB.SelectedValue == null ||
                BalanceBldCB.SelectedValue == null ||
                BalanceStartDP.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля: организация, сооружение и дата начала");
                return;
            }

            // Проверка дат
            if (BalanceEndDP.SelectedDate.HasValue &&
                BalanceEndDP.SelectedDate.Value < BalanceStartDP.SelectedDate.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала");
                return;
            }

            try
            {
                bool success = _balanceService.Create(
                    organizationId: (int)BalanceOrgCB.SelectedValue,
                    buildingId: (int)BalanceBldCB.SelectedValue,
                    dateStart: BalanceStartDP.SelectedDate.Value,
                    dateEnd: BalanceEndDP.SelectedDate.HasValue ? BalanceEndDP.SelectedDate.Value : (DateTime?)null
                );

                if (success)
                {
                    MessageBox.Show("Запись баланса успешно добавлена");
                    LoadBalanceHistory(); // Обновляем DataGrid
                    ClearTextFields(); // Очищаем поля
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении записи баланса");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void UpdateBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBalance == null)
            {
                MessageBox.Show("Выберите запись баланса для изменения");
                return;
            }

            // Валидация данных
            if (BalanceOrgCB.SelectedValue == null ||
                BalanceBldCB.SelectedValue == null ||
                BalanceStartDP.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля: организация, сооружение и дата начала");
                return;
            }

            // Проверка дат
            if (BalanceEndDP.SelectedDate.HasValue &&
                BalanceEndDP.SelectedDate.Value < BalanceStartDP.SelectedDate.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала");
                return;
            }

            try
            {
                bool success = _balanceService.Update(
                    balanceId: _selectedBalance.Id,
                    organizationId: (int)BalanceOrgCB.SelectedValue,
                    buildingId: (int)BalanceBldCB.SelectedValue,
                    dateStart: BalanceStartDP.SelectedDate.Value,
                    dateEnd: BalanceEndDP.SelectedDate.HasValue ? BalanceEndDP.SelectedDate.Value : (DateTime?)null
                );

                if (success)
                {
                    MessageBox.Show("Запись баланса успешно обновлена");
                    LoadBalanceHistory(); // Обновляем DataGrid
                    ClearTextFields(); // Очищаем поля
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении записи баланса");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void DeleteBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBalance == null)
            {
                MessageBox.Show("Выберите запись баланса для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить запись баланса?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _balanceService.Delete(_selectedBalance.Id);
                    if (success)
                    {
                        MessageBox.Show("Запись баланса успешно удалена");
                        LoadBalanceHistory(); // Обновляем DataGrid
                        ClearTextFields(); // Очищаем поля
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении записи баланса");
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
