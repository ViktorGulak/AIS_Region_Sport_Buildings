using AIS_Region_Sport_Buildings.Models;
using AIS_Region_Sport_Buildings.Services;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AIS_Region_Sport_Buildings
{
    /// <summary>
    /// Логика взаимодействия для EvtWindow.xaml
    /// </summary>
    public partial class EvtWindow : Window
    {
        private BalanceService _balanceService = new BalanceService();
        private EventTypeService _eventTypeService = new EventTypeService();
        private SportBuildingsService _buildingsService = new SportBuildingsService();

        private EventService _eventService = new EventService();
        private Event _selectedEvent;
        public EvtWindow()
        {
            InitializeComponent();
            LoadEvents();
            LoadBalances();
            LoadEventType();
        }

        private void LoadEvents()
        {
            List<Event> events = _eventService.GetAll();
            EventDG.ItemsSource = events;
        }
        private void LoadBalances()
        {
            List<Balance> balances = _balanceService.GetAll();
            EvtOrganaizerCB.ItemsSource = balances;
            EvtOrganaizerCB.DisplayMemberPath = "OrganizationName";  // Что показывать
            EvtOrganaizerCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (balances.Count > 0)
                EvtOrganaizerCB.SelectedIndex = -1;
        }

        private void LoadEventType()
        {
            List<EventType> eventType = _eventTypeService.GetAll();
            EvtTypeCB.ItemsSource = eventType;
            EvtTypeCB.DisplayMemberPath = "Title";  // Что показывать
            EvtTypeCB.SelectedValuePath = "Id";     // Что хранить как значение

            if (eventType.Count > 0)
                EvtTypeCB.SelectedIndex = -1;
        }

        private void ClearTextFields()
        {
            EvtTitleTB.Clear();
            EvtVisitorsCountTB.Clear();
            EvtPlaceTB.Clear();
            EvtStartDP.SelectedDate = null;
            EvtEndDP.SelectedDate = null;
            EvtOrganaizerCB.SelectedIndex = -1;
            EvtTypeCB.SelectedIndex = -1;
            _selectedEvent = null;
            EventDG.SelectedItem = null;
        }

        private void AddNewOrganaizerBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = EvtOrganaizerCB.Items.Count;
            BalanceWindow balanceWindow = new BalanceWindow();

            balanceWindow.Closed += (s, args) =>
            {
                LoadBalances();

                if (prevItemsCount == EvtOrganaizerCB.Items.Count || prevItemsCount > EvtOrganaizerCB.Items.Count)
                {
                    EvtOrganaizerCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < EvtOrganaizerCB.Items.Count)
                {
                    EvtOrganaizerCB.SelectedIndex = EvtOrganaizerCB.Items.Count - 1;
                }
            };

            balanceWindow.ShowDialog();
        }

        private void AddNewEvtTypeBtn_Click(object sender, RoutedEventArgs e)
        {
            int prevItemsCount = EvtTypeCB.Items.Count;
            ReferenceBooks refBooksWindow = new ReferenceBooks(2);

            refBooksWindow.Closed += (s, args) =>
            {
                LoadEventType();

                if (prevItemsCount == EvtTypeCB.Items.Count || prevItemsCount > EvtTypeCB.Items.Count)
                {
                    EvtTypeCB.SelectedIndex = -1;
                }
                else if (prevItemsCount < EvtTypeCB.Items.Count)
                {
                    EvtTypeCB.SelectedIndex = EvtTypeCB.Items.Count - 1;
                }
            };

            refBooksWindow.ShowDialog();
        }

        private void EventDG_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedEvent = EventDG.SelectedItem as Event;

            if (_selectedEvent != null)
            {
                // Заполняем поля данными выбранного мероприятия
                EvtTitleTB.Text = _selectedEvent.Title;
                EvtPlaceTB.Text = _selectedEvent.BuildingName;
                EvtVisitorsCountTB.Text = _selectedEvent.VisitorsCount.ToString();
                EvtStartDP.SelectedDate = _selectedEvent.DateStart;
                EvtEndDP.SelectedDate = _selectedEvent.DateEnd;
                EvtOrganaizerCB.SelectedValue = _selectedEvent.BalanceId;
                EvtTypeCB.SelectedValue = _selectedEvent.TypeId > 0 ? _selectedEvent.TypeId : (object)null;
            }
        }

        private void EvtOrganaizerCB_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            EvtPlaceTB.Text = string.Empty;
            if (EvtOrganaizerCB.SelectedItem is Balance selectedBalance)
            {
                // Заполняем поле места проведения
                EvtPlaceTB.Text = selectedBalance.BuildingName ?? string.Empty;
            }
            else
            {
                // Если ничего не выбрано, очищаем поле
                EvtPlaceTB.Text = string.Empty;
            }
        }

        private void EvtAddBtn_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(EvtTitleTB.Text) ||
                EvtStartDP.SelectedDate == null ||
                string.IsNullOrWhiteSpace(EvtVisitorsCountTB.Text) ||
                EvtOrganaizerCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля: название, дата начала, количество посетителей и организатор");
                return;
            }

            // Проверка дат
            if (EvtEndDP.SelectedDate.HasValue &&
                EvtEndDP.SelectedDate.Value < EvtStartDP.SelectedDate.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала");
                return;
            }

            // Проверка количества посетителей
            if (!int.TryParse(EvtVisitorsCountTB.Text, out int visitorsCount) || visitorsCount < 0)
            {
                MessageBox.Show("Количество посетителей должно быть положительным числом");
                return;
            }

            try
            {
                bool success = _eventService.Create(
                    title: EvtTitleTB.Text.Trim(),
                    dateStart: EvtStartDP.SelectedDate.Value,
                    dateEnd: EvtEndDP.SelectedDate.HasValue ? EvtEndDP.SelectedDate.Value : (DateTime?)null,
                    visitorsCount: visitorsCount,
                    balanceId: (int)EvtOrganaizerCB.SelectedValue,
                    typeId: EvtTypeCB.SelectedValue != null ? (int?)EvtTypeCB.SelectedValue : null
                );

                if (success)
                {
                    MessageBox.Show("Мероприятие успешно добавлено");
                    LoadEvents(); // Обновляем DataGrid
                    ClearTextFields(); // Очищаем поля
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении мероприятия");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void EvtClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearTextFields();
        }

        private void EvtUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Выберите мероприятие для изменения");
                return;
            }

            // Валидация данных (такая же как при добавлении)
            if (string.IsNullOrWhiteSpace(EvtTitleTB.Text) ||
                EvtStartDP.SelectedDate == null ||
                string.IsNullOrWhiteSpace(EvtVisitorsCountTB.Text) ||
                EvtOrganaizerCB.SelectedValue == null)
            {
                MessageBox.Show("Заполните все обязательные поля: название, дата начала, количество посетителей и организатор");
                return;
            }

            if (EvtEndDP.SelectedDate.HasValue &&
                EvtEndDP.SelectedDate.Value < EvtStartDP.SelectedDate.Value)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала");
                return;
            }

            if (!int.TryParse(EvtVisitorsCountTB.Text, out int visitorsCount) || visitorsCount < 0)
            {
                MessageBox.Show("Количество посетителей должно быть положительным числом");
                return;
            }

            try
            {
                bool success = _eventService.Update(
                    eventId: _selectedEvent.Id,
                    title: EvtTitleTB.Text.Trim(),
                    dateStart: EvtStartDP.SelectedDate.Value,
                    dateEnd: EvtEndDP.SelectedDate.HasValue ? EvtEndDP.SelectedDate.Value : (DateTime?)null,
                    visitorsCount: visitorsCount,
                    balanceId: (int)EvtOrganaizerCB.SelectedValue,
                    typeId: EvtTypeCB.SelectedValue != null ? (int?)EvtTypeCB.SelectedValue : null
                );

                if (success)
                {
                    MessageBox.Show("Мероприятие успешно обновлено");
                    LoadEvents();
                    ClearTextFields();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении мероприятия");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void EvtDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null)
            {
                MessageBox.Show("Выберите мероприятие для удаления");
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить мероприятие {_selectedEvent.Title}?",
                "Подтверждение удаления мероприятия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _eventService.Delete(_selectedEvent.Id);
                    if (success)
                    {
                        MessageBox.Show("Мероприятие успешно удалено");
                        LoadEvents();
                        ClearTextFields();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении мероприятия");
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
