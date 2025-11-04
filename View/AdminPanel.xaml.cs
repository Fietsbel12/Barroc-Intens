using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class AdminPanel : Page
    {
        private string medewerkerRol;

        public AdminPanel()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            await LaadMedewerkersAsync();
        }

        private async Task LaadMedewerkersAsync()
        {
            using (var context = new AppDbContext())
            {
                var medewerkers = await context.Medewerkers
                    .OrderBy(m => m.Naam)
                    .ToListAsync();

                MedewerkersListView.ItemsSource = medewerkers;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void AdminCreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Hier kun je navigeren naar een pagina om een nieuwe medewerker aan te maken
            // Voor nu navigeren we tijdelijk naar SalesHomepage:
            Frame.Navigate(typeof(SalesHomepage), medewerkerRol);
        }
    }
}
