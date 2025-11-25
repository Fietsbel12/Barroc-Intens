using BarrocIntens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await AttemptLogin();
        }

        private async Task AttemptLogin()
        {
            string naam = NaamTextBox.Text.Trim();
            string wachtwoord = WachtwoordBox.Password;

            if (string.IsNullOrEmpty(naam) || string.IsNullOrEmpty(wachtwoord))
            {
                await ShowDialog("Login mislukt", "Vul zowel naam als wachtwoord in.");
                return;
            }

            using var db = new AppDbContext();

            // Medewerker ophalen op basis van naam
            var gebruiker = db.Medewerkers
                             .FirstOrDefault(m => m.Naam.ToLower() == naam.ToLower());

            if (gebruiker != null && BCrypt.Net.BCrypt.Verify(wachtwoord, gebruiker.Wachtwoord))
            {
                //await ShowDialog("Login geslaagd", $"Welkom {gebruiker.Naam}! Je rol is {gebruiker.MedewerkerRol}.");
                Frame.Navigate(typeof(MedewerkerDashboard), gebruiker.MedewerkerRol);
            }
            else
            {
                await ShowDialog("Login mislukt", "Naam of wachtwoord is onjuist.");
                WachtwoordBox.Password = string.Empty;
                WachtwoordBox.Focus(FocusState.Programmatic);
            }
        }

        private async Task ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = title,
                Content = content,
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void OwnerLoginButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MedewerkerDashboard), "Eigenaar");
        }
    }
}
