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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            // ComboBox vullen met rollen
            MedewerkerRolComboBox.ItemsSource = Rollen;
        }

        // Ontvang de ingelogde rol bij navigatie naar deze pagina
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


            string geselecteerdeRol = MedewerkerRolComboBox.SelectedItem.ToString();

            var medewerker = new Medewerker
            {
                Naam = NaamTextBox.Text.Trim(),
                Wachtwoord = WachtwoordTextBox.Text.Trim(),
                MedewerkerRol = geselecteerdeRol
            };

            // Naam validatie
            if (string.IsNullOrWhiteSpace(medewerker.Naam))
            {
                ErrorsTextblock.Text = "Naam is verplicht.";
                return;
            }
            if (medewerker.Naam.Length > 50)
            {
                ErrorsTextblock.Text = "Ongeldige naam.";
                return;
            }
            if (medewerker.Naam.Length < 2)
            {
                ErrorsTextblock.Text = "Naam moet minimaal 2 tekens bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(medewerker.Naam, @"^[a-zA-Z\s]+$"))
            {
                ErrorsTextblock.Text = "Naam mag alleen letters bevatten.";
                return;
            }

            // Wachtwoord validatie
            if (string.IsNullOrWhiteSpace(medewerker.Wachtwoord))
            {
                ErrorsTextblock.Text = "Wachtwoord is verplicht.";
                return;
            }
            if (medewerker.Wachtwoord.Length > 50)
            {
                ErrorsTextblock.Text = "Ongeldig wachtwoord.";
                return;
            }
            if (medewerker.Wachtwoord.Length < 6)
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal 6 tekens bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(medewerker.Wachtwoord, @"[0-9]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één cijfer bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(medewerker.Wachtwoord, @"[A-Z]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één hoofdletter bevatten.";
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(medewerker.Wachtwoord, @"[\W_]"))
            {
                ErrorsTextblock.Text = "Wachtwoord moet minimaal één speciaal teken bevatten.";
                return;
            }

            // Rol validatie
            if (MedewerkerRolComboBox.SelectedItem == null)
            {
                ErrorsTextblock.Text = "Selecteer een geldige rol.";
                return;
            }

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
