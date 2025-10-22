using AIS_Region_Sport_Buildings.Models;
using AIS_Region_Sport_Buildings.Services;
using System.Windows;
using System.Windows.Controls;

namespace AIS_Region_Sport_Buildings
{
    /// <summary>
    /// Логика взаимодействия для ReferenceBooks.xaml
    /// </summary>
    public partial class ReferenceBooks : Window
    {
        private LocalityTypeService _locTypeService = new LocalityTypeService();
        private LocalityType _selectedLocalityType;

        private BuildingTypeService _buildTypeService = new BuildingTypeService();
        private BuildingType _selectedBuildingType;

        private EventTypeService _eventTypeService = new EventTypeService();
        private EventType _selectedEventType;
        public ReferenceBooks(int selectedTabIdx = 0)
        {
            InitializeComponent();

            LoadLocalityTypes();
            LoadBuildingTypes();
            LoadEventTypes();

            TabMenu.SelectedIndex = selectedTabIdx;
        }

        private void LoadLocalityTypes()
        {
            LocalityTypeDG.ItemsSource = _locTypeService.GetAll();
        }

        private void LoadBuildingTypes()
        {
            SpBldTypeDG.ItemsSource = _buildTypeService.GetAll();
        }

        private void LoadEventTypes()
        {
            EvtTypeDG.ItemsSource = _eventTypeService.GetAll();
        }

        private void LocalityTypeDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLocalityType = LocalityTypeDG.SelectedItem as LocalityType;
            if (_selectedLocalityType != null)
            {
                LocTypeTB.Text = _selectedLocalityType.Title;
            }
        }

        private void SpBldTypeDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBuildingType = SpBldTypeDG.SelectedItem as BuildingType;
            if (_selectedBuildingType != null)
            {
                SpBldTB.Text = _selectedBuildingType.Title;
            }
        }

        private void EvtTypeDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEventType = EvtTypeDG.SelectedItem as EventType;
            if (_selectedEventType != null)
            {
                EvtTypeTB.Text = _selectedEventType.Title;
            }
        }

        private void AddLocTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            string title = LocTypeTB.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название типа населенного пункта");
                return;
            }

            bool success = _locTypeService.Create(title);
            if (success)
            {
                MessageBox.Show("Запись успешно добавлена");
                LoadLocalityTypes(); // Обновляем DataGrid
                LocTypeTB.Clear();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении записи");
            }
        }

        private void UpdateLocTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocalityType == null)
            {
                MessageBox.Show("Выберите запись для обновления");
                return;
            }

            string newTitle = LocTypeTB.Text.Trim();
            if (string.IsNullOrEmpty(newTitle))
            {
                MessageBox.Show("Введите новое название");
                return;
            }

            bool success = _locTypeService.Update(_selectedLocalityType.Id, newTitle);
            if (success)
            {
                MessageBox.Show("Запись успешно обновлена");
                LoadLocalityTypes(); // Обновляем DataGrid
                LocTypeTB.Clear();
                _selectedLocalityType = null;
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении записи");
            }
        }

        private void DeleteLocTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedLocalityType == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить '{_selectedLocalityType.Title}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _locTypeService.Delete(_selectedLocalityType.Id);
                if (success)
                {
                    MessageBox.Show("Запись успешно удалена");
                    LoadLocalityTypes(); // Обновляем DataGrid
                    LocTypeTB.Clear();
                    _selectedLocalityType = null;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении записи");
                }
            }
        }

        private void AddSpBldTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            string title = SpBldTB.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название типа населенного пункта");
                return;
            }

            bool success = _buildTypeService.Create(title);
            if (success)
            {
                MessageBox.Show("Запись успешно добавлена");
                LoadBuildingTypes(); // Обновляем DataGrid
                SpBldTB.Clear();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении записи");
            }
        }

        private void UpdateSpBldTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuildingType == null)
            {
                MessageBox.Show("Выберите запись для обновления");
                return;
            }

            string newTitle = SpBldTB.Text.Trim();
            if (string.IsNullOrEmpty(newTitle))
            {
                MessageBox.Show("Введите новое название");
                return;
            }

            bool success = _buildTypeService.Update(_selectedBuildingType.Id, newTitle);
            if (success)
            {
                MessageBox.Show("Запись успешно обновлена");
                LoadBuildingTypes(); // Обновляем DataGrid
                SpBldTB.Clear();
                _selectedBuildingType = null;
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении записи");
            }
        }

        private void DeleteSpBldTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBuildingType == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить '{_selectedBuildingType.Title}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _buildTypeService.Delete(_selectedBuildingType.Id);
                if (success)
                {
                    MessageBox.Show("Запись успешно удалена");
                    LoadBuildingTypes(); // Обновляем DataGrid
                    SpBldTB.Clear();
                    _selectedBuildingType = null;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении записи");
                }
            }
        }

        private void AddEvtTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            string title = EvtTypeTB.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название типа населенного пункта");
                return;
            }

            bool success = _eventTypeService.Create(title);
            if (success)
            {
                MessageBox.Show("Запись успешно добавлена");
                LoadEventTypes(); // Обновляем DataGrid
                EvtTypeTB.Clear();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении записи");
            }
        }

        private void UpdateEvtTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEventType == null)
            {
                MessageBox.Show("Выберите запись для обновления");
                return;
            }

            string newTitle = EvtTypeTB.Text.Trim();
            if (string.IsNullOrEmpty(newTitle))
            {
                MessageBox.Show("Введите новое название");
                return;
            }

            bool success = _eventTypeService.Update(_selectedEventType.Id, newTitle);
            if (success)
            {
                MessageBox.Show("Запись успешно обновлена");
                LoadEventTypes(); // Обновляем DataGrid
                EvtTypeTB.Clear();
                _selectedEventType = null;
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении записи");
            }
        }

        private void DeleteEvtTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEventType == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить '{_selectedEventType.Title}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                bool success = _eventTypeService.Delete(_selectedEventType.Id);
                if (success)
                {
                    MessageBox.Show("Запись успешно удалена");
                    LoadEventTypes(); // Обновляем DataGrid
                    EvtTypeTB.Clear();
                    _selectedEventType = null;
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении записи");
                }
            }
        }
    }
}
