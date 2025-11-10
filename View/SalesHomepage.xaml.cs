using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Linq;

namespace BarrocIntens.View
{
    public sealed partial class SalesHomepage : Page
    {
        private string medewerkerRol;

        public SalesHomepage()
        {
            this.InitializeComponent();
            this.Loaded += SalesHomepage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchTextBox.Text.ToLower();

            using (var db = new AppDbContext())
            {
                var apparaten = db.Koffiezetapparaten
                    .Where(a => a.Naam.ToLower().Contains(query) || a.Merk.ToLower().Contains(query))
                    .OrderBy(a => a.Naam)
                    .ToList();

                ApparatenListView.ItemsSource = apparaten;

                GeenApparatenText.Visibility = apparaten.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        private async void SalesHomepage_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var apparaten = db.Koffiezetapparaten.ToList();

                // Controleer of er apparaten zijn
                if (apparaten.Any())
                {
                    ApparatenListView.ItemsSource = apparaten;
                }
                else
                {
                    GeenApparatenText.Visibility = Visibility.Visible;
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
