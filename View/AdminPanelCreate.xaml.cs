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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace BarrocIntens.View
{
    public sealed partial class AdminPanelCreate : Page
    {
        private string medewerkerRol;

        private readonly List<string> Rollen = new List<string>
        {
            "Finance", "Sales", "Inkoop", "Maintenance", "Planner"
        };

        public AdminPanelCreate()
        {
            this.InitializeComponent();
            MedewerkerRolComboBox.ItemsSource = Rollen;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            medewerkerRol = e.Parameter as string;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPanel), medewerkerRol);
            Frame.BackStack.Clear();
        }

        private async void OpslaanButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorsTextblock.Text = "";
            SuccessTextblock.Text = "";

            // Validatie van rol
            if (MedewerkerRolComboBox.SelectedItem == null)
            {
                ErrorsTextblock.Text = "Selecteer een geldige rol.";
                return;
            }

            string geselecteerdeRol = MedewerkerRolComboBox.SelectedItem.ToString();

            // NAAM VALIDATIE
            string naam = NaamTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(naam))
            {
                ErrorsTextblock.Text = "Naam is verplicht.";
                return;
            }
            if (naam.Length > 50)
            {
                ErrorsTextblock.Text = "Ongeldige naam.";
                return;
            }
            if (naam.Length < 2)
            {
                ErrorsTextblock.Text = "Naam moet minimaal 2 tekens bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(naam, @"^[a-zA-Z\s]+$"))
            {
                ErrorsTextblock.Text = "Naam mag alleen letters bevatten.";
                return;
            }

            // WACHTWOORD VALIDATIE
            string wachtwoord = WachtwoordTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(wachtwoord))
            {
                ErrorsTextblock.Text = "Wachtwoord is verplicht.";
                return;
            }
            if (wachtwoord.Length > 50)
            {
                ErrorsTextblock.Text = "Ongeldig wachtwoord.";
                return;
            }
            if (wachtwoord.Length < 6)
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal 6 tekens bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(wachtwoord, @"[0-9]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één cijfer bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(wachtwoord, @"[A-Z]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één hoofdletter bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(wachtwoord, @"[\W_]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één speciaal teken bevatten.";
                return;
            }

            // HASH HET WACHTWOORD
            string gehashtWachtwoord = BCrypt.Net.BCrypt.HashPassword(wachtwoord);

            // AANGEMAAKT OBJECT NA VALIDATIES
            var medewerker = new Medewerker
            {
                Naam = naam,
                Wachtwoord = gehashtWachtwoord,
                MedewerkerRol = geselecteerdeRol
            };

            try
            {
                using var db = new AppDbContext();
                db.Medewerkers.Add(medewerker);
                db.SaveChanges();

                SuccessTextblock.Text = "Medewerker succesvol toegevoegd!";

                await Task.Delay(1000);

                Frame.Navigate(typeof(AdminPanel), medewerkerRol);
                Frame.BackStack.Clear();
            }
            catch (Exception ex)
            {
                ErrorsTextblock.Text = $"Fout bij opslaan: {ex.Message}";
            }
        }
    }
}
