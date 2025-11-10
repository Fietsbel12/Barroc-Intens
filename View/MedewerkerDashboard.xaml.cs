using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MedewerkerDashboard : Page
    {
        private string medewerkerRol; // veld op klassenniveau

        public MedewerkerDashboard()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // waarde opslaan in het veld
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            // Alleen zichtbaar maken als de rol "Eigenaar" is
            AdminPageButton.Visibility = medewerkerRol == "Eigenaar"
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private async void SalesPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer de rol voordat je navigeert
            if (medewerkerRol == "Sales" || medewerkerRol == "Eigenaar")
            {
                Frame.Navigate(typeof(SalesHomepage), medewerkerRol);
            }
            else
            {
                // Toon een melding als de medewerker geen toegang heeft
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Toegang geweigerd",
                    Content = "Je hebt geen toegang tot de Sales-pagina.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
        private async void PlanningPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer de rol voordat je navigeert
            if (medewerkerRol == "Planner" || medewerkerRol == "Eigenaar")
            {
                Frame.Navigate(typeof(PlannerHomepage), medewerkerRol);
            }
            else
            {
                // Toon een melding als de medewerker geen toegang heeft
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Toegang geweigerd",
                    Content = "Je hebt geen toegang tot de Planner-pagina.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
        private async void InkoopPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer de rol voordat je navigeert
            if (medewerkerRol == "Inkoop" || medewerkerRol == "Eigenaar")
            {
                Frame.Navigate(typeof(InkoopHomepage), medewerkerRol);
            }
            else
            {
                // Toon een melding als de medewerker geen toegang heeft
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Toegang geweigerd",
                    Content = "Je hebt geen toegang tot de Inkoop-pagina.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
        private async void MaintenancePageButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer de rol voordat je navigeert
            if (medewerkerRol == "Maintenance" || medewerkerRol == "Eigenaar")
            {
                Frame.Navigate(typeof(MaintenanceHomepage), medewerkerRol);
            }
            else
            {
                // Toon een melding als de medewerker geen toegang heeft
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Toegang geweigerd",
                    Content = "Je hebt geen toegang tot de Maintenance-pagina.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
        private async void FinancePageButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer de rol voordat je navigeert
            if (medewerkerRol == "Finance" || medewerkerRol == "Eigenaar")
            {
                Frame.Navigate(typeof(FinanceHomePage), medewerkerRol);
            }
            else
            {
                // Toon een melding als de medewerker geen toegang heeft
                ContentDialog dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Toegang geweigerd",
                    Content = "Je hebt geen toegang tot de Finance-pagina.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
            private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePageBarrocIntens));
        }

        private void AdminPageButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPanel), medewerkerRol);
        }

    }

}
