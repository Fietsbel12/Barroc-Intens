using BarrocIntens.Data;
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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }


        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string naam = NaamTextBox.Text;
            string wachtwoord = WachtwoordBox.Password;

            using var db = new AppDbContext();

            // Medewerker ophalen uit de database
            var gebruiker = db.Medewerkers
                             .FirstOrDefault(m => m.Naam == naam && m.Wachtwoord == wachtwoord);

            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                CloseButtonText = "OK"
            };

            if (gebruiker != null)
            {
                dialog.Title = "Login geslaagd";
                dialog.Content = $"Welkom {gebruiker.Naam}! Je rol is {gebruiker.MedewerkerRol}.";
                Frame.Navigate(typeof(MedewerkerDashboard), gebruiker.MedewerkerRol);
            }
            else
            {
                dialog.Title = "Login mislukt";
                dialog.Content = "Naam of wachtwoord is onjuist.";
            }

            await dialog.ShowAsync();
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
