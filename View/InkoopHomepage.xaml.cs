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
        private const string fotoFolder = @"C:\barroc intens\Barroc-Intens\FotoKoffiezetapparaatFolder";

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
            if (Directory.Exists(fotoFolder))
            {
                fotoBestanden = Directory.GetFiles(fotoFolder, "*.jp*g"); // jpg & jpeg
                if (fotoBestanden.Length > 0)
                {
                    huidigeFotoIndex = 0;
                    ToonHuidigeFoto();
                }
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

                var apparaat = new Koffiezetapparaat
                {
                    Naam = NaamBox.Text,
                    Merk = MerkBox.Text,
                    Prijs = float.Parse(PrijsBox.Text),
                    Voorraad = int.Parse(VoorraadBox.Text),
                    FotoPad = fotoBestanden[huidigeFotoIndex] // sla het pad van de geselecteerde foto op
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
                PrijsBox.Text = "";
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
