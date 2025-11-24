using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using System.Linq;

namespace BarrocIntens.View
{
    public sealed partial class InkoopHomepage : Page
    {
        private string medewerkerRol;

        // FOTO-BROWSING
        private string[] fotoBestanden;
        private int huidigeFotoIndex = 0;

        // Zoekterm onthouden
        private string huidigeZoekterm = "";

        // De foldernaam waarin de foto’s staan
        private const string fotoFolder = "FotoKoffiezetapparaatFolder";

        // Berekent het correcte pad naar de folder in de output
        private static readonly string FotoMapPad =
            Path.Combine(AppContext.BaseDirectory, fotoFolder);

        public InkoopHomepage()
        {
            InitializeComponent();
            LaadFotos();
            UpdateLijst(); // init lijst
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        // ---------------- FOTO LADEN ----------------
        private void LaadFotos()
        {
            if (Directory.Exists(FotoMapPad))
            {
                fotoBestanden = Directory.GetFiles(FotoMapPad, "*.jp*g");

                if (fotoBestanden.Length > 0)
                {
                    huidigeFotoIndex = 0;
                    ToonHuidigeFoto();
                }
                else
                {
                    FeedbackText.Text = "❌ Geen foto's gevonden in de map!";
                    FeedbackText.Visibility = Visibility.Visible;
                }
            }
            else
            {
                FeedbackText.Text = $"❌ Map niet gevonden: {FotoMapPad}";
                FeedbackText.Visibility = Visibility.Visible;
            }
        }

        private void ToonHuidigeFoto()
        {
            if (fotoBestanden == null || fotoBestanden.Length == 0) return;

            BitmapImage bitmap = new BitmapImage(new Uri(fotoBestanden[huidigeFotoIndex]));
            ApparaatFoto.Source = bitmap;
        }

        private void VorigeFoto_Click(object sender, RoutedEventArgs e)
        {
            if (fotoBestanden == null || fotoBestanden.Length == 0) return;

            huidigeFotoIndex--;
            if (huidigeFotoIndex < 0) huidigeFotoIndex = fotoBestanden.Length - 1;

            ToonHuidigeFoto();
        }

        private void VolgendeFoto_Click(object sender, RoutedEventArgs e)
        {
            if (fotoBestanden == null || fotoBestanden.Length == 0) return;

            huidigeFotoIndex++;
            if (huidigeFotoIndex >= fotoBestanden.Length) huidigeFotoIndex = 0;

            ToonHuidigeFoto();
        }

        // ---------------- OPSLAAN ----------------
        private async void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (fotoBestanden == null || fotoBestanden.Length == 0)
                {
                    FeedbackText.Text = "❌ Geen foto geselecteerd!";
                    FeedbackText.Visibility = Visibility.Visible;
                    return;
                }

                string volledigeFoto = fotoBestanden[huidigeFotoIndex];
                string bestandsNaam = Path.GetFileName(volledigeFoto);

                var apparaat = new Koffiezetapparaat
                {
                    Naam = NaamBox.Text,
                    Merk = MerkBox.Text,
                    Prijs = 0,
                    Voorraad = int.Parse(VoorraadBox.Text),
                    FotoPad = $"{fotoFolder}/{bestandsNaam}"
                };

                using (var db = new AppDbContext())
                {
                    db.Koffiezetapparaten.Add(apparaat);
                    await db.SaveChangesAsync();
                }

                FeedbackText.Text = "✅ Apparaat toegevoegd!";
                FeedbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                FeedbackText.Visibility = Visibility.Visible;

                NaamBox.Text = "";
                MerkBox.Text = "";
                VoorraadBox.Text = "";

                UpdateLijst(); // refresh lijst
            }
            catch (Exception ex)
            {
                FeedbackText.Text = $"❌ Fout: {ex.Message}";
                FeedbackText.Visibility = Visibility.Visible;
                FeedbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }

        // ---------------- ZOEKEN ----------------
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            huidigeZoekterm = SearchTextBox.Text.ToLower();
            UpdateLijst();
        }

        // ---------------- VOORRAAD BUTTONS ----------------
        private void VoorraadPlus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int id))
            {
                using var db = new AppDbContext();
                var apparaat = db.Koffiezetapparaten.FirstOrDefault(a => a.Id == id);
                if (apparaat != null)
                {
                    apparaat.Voorraad++;
                    db.SaveChanges();
                    UpdateLijst(); // 🔥 filter blijft
                }
            }
        }

        private void VoorraadMin_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag.ToString(), out int id))
            {
                using var db = new AppDbContext();
                var apparaat = db.Koffiezetapparaten.FirstOrDefault(a => a.Id == id);
                if (apparaat != null && apparaat.Voorraad > 0)
                {
                    apparaat.Voorraad--;
                    db.SaveChanges();
                    UpdateLijst();
                }
            }
        }

        private void VoorraadTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textbox &&
                int.TryParse(textbox.Text, out int nieuweVoorraad) &&
                int.TryParse(textbox.Tag.ToString(), out int id))
            {
                using var db = new AppDbContext();
                var apparaat = db.Koffiezetapparaten.FirstOrDefault(a => a.Id == id);

                if (apparaat != null)
                {
                    if (nieuweVoorraad < 0) nieuweVoorraad = 0;
                    apparaat.Voorraad = nieuweVoorraad;
                    db.SaveChanges();
                    UpdateLijst();
                }
            }
        }

        // ---------------- UPDATE LIJST ----------------
        private void UpdateLijst()
        {
            using (var db = new AppDbContext())
            {
                var apparaten = db.Koffiezetapparaten
                    .Where(a => a.Prijs > 0 &&
                                (a.Naam.ToLower().Contains(huidigeZoekterm) ||
                                 a.Merk.ToLower().Contains(huidigeZoekterm)))
                    .OrderBy(a => a.Naam)
                    .ToList();

                ApparatenListView.ItemsSource = apparaten;
                GeenApparatenText.Visibility = apparaten.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
