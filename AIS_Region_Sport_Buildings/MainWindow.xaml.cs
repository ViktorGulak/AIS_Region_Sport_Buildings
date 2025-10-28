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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefBooksBtn_Click(object sender, RoutedEventArgs e)
        {
            ReferenceBooks refBooksWindow = new ReferenceBooks();
            refBooksWindow.Show();
        }
        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            Reports reportsWindow = new Reports();
            reportsWindow.Show();
        }

        private void OrgBtn_Click(object sender, RoutedEventArgs e)
        {
            OrgWindow orgWindow = new OrgWindow();
            orgWindow.Show();
        }

        private void SpBldBtn_Click(object sender, RoutedEventArgs e)
        {
            SpBldWindow spBldWindow = new SpBldWindow();
            spBldWindow.Show();
        }

        private void BalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            BalanceWindow balanceWindow = new BalanceWindow();
            balanceWindow.Show();
        }

        private void EventBtn_Click(object sender, RoutedEventArgs e)
        {
            EvtWindow evtWindow = new EvtWindow();
            evtWindow.Show();
        }
    }
}
