using Npgsql;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIS_Region_Sport_Buildings
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connStr = "Host=localhost;Port=5432;Database=Sports_buildings_region;Username=postgres;Password=fuhetadb";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefBooksBtn_Click(object sender, RoutedEventArgs e)
        {
            ReferenceBooks refBooksWindow = new ReferenceBooks();
            refBooksWindow.Show();
        }

        private void TablesBtn_Click(object sender, RoutedEventArgs e)
        {
            Tables tablesWindow = new Tables();
            tablesWindow.Show();
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            Reports reportsWindow = new Reports();
            reportsWindow.Show();
        }
    }
}
