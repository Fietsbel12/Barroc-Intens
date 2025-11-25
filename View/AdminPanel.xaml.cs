using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class AdminPanel : Page
    {
        private string medewerkerRol;
        private readonly string[] Rollen = { "Finance", "Sales", "Inkoop", "Maintenance", "Planner" };

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
            using var context = new AppDbContext();
            var medewerkers = await context.Medewerkers.OrderBy(m => m.Naam).ToListAsync();
            MedewerkersListView.ItemsSource = medewerkers;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MedewerkerDashboard), medewerkerRol);
        }

        private void AdminCreateButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AdminPanelCreate), medewerkerRol);
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var medewerker = button.Tag as Medewerker;

            if (medewerker == null) return;

            var dialog = new ContentDialog
            {
                Title = "Bevestig verwijderen",
                Content = $"Weet je zeker dat je {medewerker.Naam} wilt verwijderen?",
                PrimaryButtonText = "Ja",
                CloseButtonText = "Nee",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                using var context = new AppDbContext();
                var dbMedewerker = await context.Medewerkers.FindAsync(medewerker.Id);
                if (dbMedewerker != null)
                {
                    context.Medewerkers.Remove(dbMedewerker);
                    await context.SaveChangesAsync();
                    await LaadMedewerkersAsync();
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var medewerker = button.Tag as Medewerker;

            if (medewerker == null) return;

            var editDialog = new ContentDialog
            {
                Title = $"Bewerk medewerker: {medewerker.Naam}",
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleer",
                XamlRoot = this.XamlRoot
            };

            var panel = new StackPanel { Spacing = 10 };

            var naamBox = new TextBox { Text = medewerker.Naam, PlaceholderText = "Naam" };
            var wachtwoordBox = new TextBox { PlaceholderText = "Nieuw wachtwoord" };
            var rolBox = new ComboBox { ItemsSource = Rollen, SelectedItem = medewerker.MedewerkerRol };
            var errorTextBlock = new TextBlock { Foreground = new SolidColorBrush(Colors.Red) };

            panel.Children.Add(new TextBlock { Text = "Naam:" });
            panel.Children.Add(naamBox);
            panel.Children.Add(new TextBlock { Text = "Wachtwoord:(laat leeg om niet te wijzigen)" });
            panel.Children.Add(wachtwoordBox);
            panel.Children.Add(new TextBlock { Text = "Rol:" });
            panel.Children.Add(rolBox);
            panel.Children.Add(errorTextBlock);

            editDialog.Content = panel;

            bool valid = false;

            while (!valid)
            {
                var result = await editDialog.ShowAsync();

                if (result != ContentDialogResult.Primary)
                    return; // Gebruiker annuleert

                string naam = naamBox.Text.Trim();
                string wachtwoord = wachtwoordBox.Text.Trim();
                string rol = rolBox.SelectedItem?.ToString();

                // Validatie
                if (string.IsNullOrWhiteSpace(naam))
                {
                    errorTextBlock.Text = "Naam is verplicht.";
                    continue;
                }
                if (naam.Length < 2)
                {
                    errorTextBlock.Text = "Naam moet minimaal 2 tekens bevatten.";
                    continue;
                }
                if (naam.Length > 50)
                {
                    errorTextBlock.Text = "Ongeldige naam.";
                    continue;
                }
                if (!Regex.IsMatch(naam, @"^[a-zA-Z\s]+$"))
                {
                    errorTextBlock.Text = "Naam mag alleen letters bevatten.";
                    continue;
                }

                // Alleen wachtwoord validatie als er een nieuw wachtwoord is ingevuld
                if (!string.IsNullOrWhiteSpace(wachtwoord))
                {
                    if (wachtwoord.Length < 6)
                    {
                        errorTextBlock.Text = "Wachtwoord moet minimaal 6 tekens bevatten.";
                        continue;
                    }
                    if (!Regex.IsMatch(wachtwoord, @"[0-9]"))
                    {
                        errorTextBlock.Text = "Wachtwoord moet minimaal één cijfer bevatten.";
                        continue;
                    }
                    if (!Regex.IsMatch(wachtwoord, @"[A-Z]"))
                    {
                        errorTextBlock.Text = "Wachtwoord moet minimaal één hoofdletter bevatten.";
                        continue;
                    }
                    if (!Regex.IsMatch(wachtwoord, @"[\W_]"))
                    {
                        errorTextBlock.Text = "Wachtwoord moet minimaal één speciaal teken bevatten.";
                        continue;
                    }
                }

                if (rolBox.SelectedItem == null)
                {
                    errorTextBlock.Text = "Selecteer een geldige rol.";
                    continue;
                }

                // Alles goed
                valid = true;

                try
                {
                    medewerker.Naam = naam;
                    medewerker.MedewerkerRol = rol;

                    // Als er een nieuw wachtwoord is ingevuld, hash het
                    if (!string.IsNullOrWhiteSpace(wachtwoord))
                    {
                        medewerker.Wachtwoord = BCrypt.Net.BCrypt.HashPassword(wachtwoord);
                    }

                    using var context = new AppDbContext();
                    context.Medewerkers.Update(medewerker);
                    await context.SaveChangesAsync();

                    await LaadMedewerkersAsync(); // Herlaad lijst
                }
                catch (Exception ex)
                {
                    errorTextBlock.Text = $"Fout bij opslaan: {ex.Message}";
                    valid = false;
                }
            }
        }
    }
}
