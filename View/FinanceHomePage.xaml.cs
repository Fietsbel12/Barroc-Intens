using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class FinanceHomePage : Page
    {
        private string medewerkerRol;

        public FinanceHomePage()
        {
            this.InitializeComponent();
            this.Loaded += FinanceHomePage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";
        }

        private async void FinanceHomePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LaadApparaten();
        }

        private async Task LaadApparaten()
        {
            using (var db = new AppDbContext())
            {
                var apparaten = db.Koffiezetapparaten
                    .Where(a => a.Prijs == 0)
                    .OrderBy(a => a.Naam)
                    .ToList();

                ApparatenListView.ItemsSource = apparaten;
                GeenApparatenText.Visibility = apparaten.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private async void OpslaanPrijs_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag is Koffiezetapparaat apparaat)
            {
                try
                {
                    // Zoek TextBox in dezelfde StackPanel
                    StackPanel sp = btn.Parent as StackPanel;
                    TextBox prijsBox = sp.Children.OfType<TextBox>().FirstOrDefault();
                    if (prijsBox == null) return;

                    if (!float.TryParse(prijsBox.Text, out float nieuwePrijs))
                    {
                        FeedbackText.Text = "❌ Ongeldige prijs!";
                        FeedbackText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                        FeedbackText.Visibility = Visibility.Visible;
                        return;
                    }

                    using (var db = new AppDbContext())
                    {
                        var dbApparaat = db.Koffiezetapparaten.FirstOrDefault(a => a.Id == apparaat.Id);
                        if (dbApparaat != null)
                        {
                            dbApparaat.Prijs = nieuwePrijs;
                            await db.SaveChangesAsync();
                        }
                    }

                    FeedbackText.Text = $"✅ Prijs van {apparaat.Naam} opgeslagen!";
                    FeedbackText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Green);
                    FeedbackText.Visibility = Visibility.Visible;

                    await LaadApparaten(); // Lijst vernieuwen
                }
                catch (Exception ex)
                {
                    FeedbackText.Text = $"❌ Fout: {ex.Message}";
                    FeedbackText.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Red);
                    FeedbackText.Visibility = Visibility.Visible;
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
