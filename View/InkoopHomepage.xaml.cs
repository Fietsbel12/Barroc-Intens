using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;

namespace BarrocIntens.View
{
    public sealed partial class InkoopHomepage : Page
    {
        private string medewerkerRol;

        // FOTO-BROWSING
        private string[] fotoBestanden;
        private int huidigeFotoIndex = 0;

        // De foldernaam waarin de foto’s staan
        private const string fotoFolder = "FotoKoffiezetapparaatFolder";

        // Berekent het correcte pad naar de folder in de output
        private static readonly string FotoMapPad =
            Path.Combine(AppContext.BaseDirectory, fotoFolder);

        public InkoopHomepage()
        {
            InitializeComponent();
            LaadFotos();
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
                    FotoPad = $"{fotoFolder}/{bestandsNaam}"   // ✔️ RELATIEF PAD OPSLAAN
                };

                using (var db = new AppDbContext())
                {
                    db.Koffiezetapparaten.Add(apparaat);
                    await db.SaveChangesAsync();
                }

                FeedbackText.Text = "✅ Apparaat toegevoegd!";
                FeedbackText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                FeedbackText.Visibility = Visibility.Visible;

                // Formulier leegmaken
                NaamBox.Text = "";
                MerkBox.Text = "";
                VoorraadBox.Text = "";
            }
            catch (Exception ex)
            {
                FeedbackText.Text = $"❌ Fout: {ex.Message}";
                FeedbackText.Visibility = Visibility.Visible;
                FeedbackText.Foreground =
                    new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }
    }
}
