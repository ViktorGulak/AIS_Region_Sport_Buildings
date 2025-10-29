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
    /// Логика взаимодействия для Reports.xaml
    /// </summary>
    public partial class Reports : Window
    {
        private ReportService _reportService = new ReportService();
        private BuildingTypeService _buildingTypeService = new BuildingTypeService();
        public Reports()
        {
            InitializeComponent();
            LoadBuildingType();
        }

        private void LoadBuildingType()
        {
            List<BuildingType> buildingType = _buildingTypeService.GetAll();
            BuildingTypeRepCB.ItemsSource = buildingType;
            BuildingTypeRepCB.DisplayMemberPath = "Title";  // Что показывать
            BuildingTypeRepCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (buildingType.Count > 0)
                BuildingTypeRepCB.SelectedIndex = -1;
        }

        private void GenerateSpBldRepBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (BuildingTypeRepCB.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип спортивного сооружения");
                return;
            }

            if (SpBldRepDP.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату");
                return;
            }

            try
            {
                string selectedBuildingType = ((BuildingType)BuildingTypeRepCB.SelectedItem).Title;
                DateTime selectedDate = SpBldRepDP.SelectedDate.Value;

                List<SportBuildingReport> report = _reportService.GetSportBuildingsByTypeAndDate(
                    selectedBuildingType,
                    selectedDate
                );

                SpBldRepDG.ItemsSource = report;

                // Показываем статистику
                QantitySpBldRepNodes.Text = report.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}");
            }
        }
    }
}
