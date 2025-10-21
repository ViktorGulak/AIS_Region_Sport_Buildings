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
    public partial class Tables : Window
    {
        private OrganizationService _organizationService = new OrganizationService();
        private Organization _selectedOrganization;

        private LocalityTypeService lts = new LocalityTypeService();
        public Tables()
        {
            InitializeComponent();

            LoadOrganizations();
            LoadLocalityTypes();
        }

        private void LoadLocalityTypes()
        {
            List<string> locTypeTitles = new List<string>();

            foreach(LocalityType locType in lts.GetAll())
            {
                locTypeTitles.Add(locType.Title);
            }
            LocTypeCB.ItemsSource = locTypeTitles;
            //LocTypeCB.SelectedItem = "Село";
        }
        private void LoadOrganizations()
        {
            List<Organization> organizations = _organizationService.GetAll();
            OrganizationDG.ItemsSource = organizations;
        }

        private void OrganizationDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
