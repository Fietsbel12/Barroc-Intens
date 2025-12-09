using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.UniversalAccessibility.Drawing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

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

        // Offerte buttton
        private async void offerteAanmaken_Click(object sender, RoutedEventArgs e)
        {
            var companyBox = new TextBox { Header = "Naam bedrijf" };
            var customerBox = new TextBox { Header = "Naam contact persoon" };
            var adressBox = new TextBox { Header = "Adres" };
            var emailBox = new TextBox { Header = "E-mail" };

            var customerDialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = "Vul klantgegevens in",
                PrimaryButtonText = "Offerte aanmaken",
                CloseButtonText = "Annuleren",
                DefaultButton = ContentDialogButton.Primary,
                Content = new StackPanel
                {
                    Spacing = 10,
                    Children =
            {
                companyBox,
                customerBox,
                adressBox,
                emailBox
            }
                }
            };

            var result = await customerDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Haal de waarden nu op uit de variabelen
                string company = companyBox.Text;
                string customer = customerBox.Text;
                string adress = adressBox.Text;
                string email = emailBox.Text;

                GeneratePdfWithCustomerData(company, customer, adress, email);
            }
        }

        private void GeneratePdfWithCustomerData(string company, string customer, string adress, string email)
        {
            var document = new PdfDocument();
            document.Info.Title = "Offerte";

            var page = document.AddPage();

            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "BarrocIntensHomePageImage.png");
            using var BarrocIntensHomePageImage = XImage.FromFile(logoPath); // Logo 

            var gfx = XGraphics.FromPdfPage(page);

            // Fonts
            var titleFont = new XFont("Arial", 22, XFontStyleEx.Bold);
            var headerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            var regularFont = new XFont("Arial", 12);
            var smallFont = new XFont("Arial", 10);

            // Kleuren
            var yellow = XBrushes.Gold;
            var barrocBlack = new XSolidBrush(XColor.FromArgb(34, 32, 29));
            var black = XBrushes.Black;
            var lightGray = new XSolidBrush(XColor.FromArgb(240, 240, 240));

            // =======================
            // 1) HEADER GEEL VLAK
            // =======================

            gfx.DrawRectangle(yellow, 0, 0, page.Width, 110);

            // Logo path
            var logo = XImage.FromFile(logoPath);

            // =======================
            // WIT VLAK LINKS IN DE HOEK
            // =======================

            double cardX = 0;       // helemaal links
            double cardY = 0;       // helemaal bovenaan
            double cardWidth = 200; // breedte wit vlak
            double cardHeight = 110; // zelfde hoogte als header

            // Witte achtergrond
            gfx.DrawRectangle(XBrushes.White, cardX, cardY, cardWidth, cardHeight);

            // Logo schalen binnen het witte vlak
            double maxWidth = cardWidth - 20;
            double maxHeight = cardHeight - 20;

            double ratioX = maxWidth / logo.PixelWidth;
            double ratioY = maxHeight / logo.PixelHeight;
            double ratio = Math.Min(ratioX, ratioY);

            double logoWidth = logo.PixelWidth * ratio;
            double logoHeight = logo.PixelHeight * ratio;

            // Logo centreren in het witte vlak
            double logoX = cardX + (cardWidth - logoWidth) / 2;
            double logoY = cardY + (cardHeight - logoHeight) / 2;

            // Logo plaatsen in het witte vlak met juiste afmetingn
            gfx.DrawImage(logo, logoX, logoY, logoWidth, logoHeight);


            // =======================
            // TITEL EN DATUM RECHTS
            // =======================

            gfx.DrawString("Offerte", titleFont, black,
                new XPoint(page.Width - 40, 45),
                XStringFormats.TopRight);

            gfx.DrawString($"Datum: {DateTime.Now:dd-MM-yyyy}", regularFont, barrocBlack,
                new XPoint(page.Width - 40, 65),
                XStringFormats.TopRight);


            // =======================
            // LIJN ONDER HEADER
            // =======================

            gfx.DrawLine(new XPen(XColors.Black, 1.5), 0, 110, page.Width, 110);

            // Startpositie volgende content
            double y = 140;

            // =======================
            //  KLANTGEGEVENS BLOK
            // =======================


            gfx.DrawString("Klantgegevens", headerFont, black, 40, y);
            y += 25;

            gfx.DrawString($"Naam bedrijf: {company}", regularFont, black, 40, y); y += 18;
            gfx.DrawString($"Naam klant: {customer}", regularFont, black, 40, y); y += 18;
            gfx.DrawString($"Adres: {adress}", regularFont, black, 40, y); y += 18;
            gfx.DrawString($"E-mail: {email}", regularFont, black, 40, y); y += 35;

            // =======================
            // TABELLAYOUT
            // =======================

            gfx.DrawString("Artikeloverzicht", headerFont, black, 40, y);
            y += 20;

            double tableStart = y;

            // Tabel header achtergrond
            gfx.DrawRectangle(lightGray, 40, y, page.Width - 80, 25);

            gfx.DrawString("Omschrijving", regularFont, black, 50, y + 17);
            gfx.DrawString("Aantal", regularFont, black, 350, y + 17);
            gfx.DrawString("Prijs", regularFont, black, 430, y + 17);

            y += 30;

            // =======================
            // HANDTEKENINGEN BLOK
            // =======================

            gfx.DrawString("Handtekening opdrachtgever:", regularFont, black, 40, y);
            gfx.DrawLine(new XPen(XColors.Black, 1), 40, y + 20, 250, y + 20);

            gfx.DrawString("Handtekening leverancier:", regularFont, black, 330, y);
            gfx.DrawLine(new XPen(XColors.Black, 1), 330, y + 20, 540, y + 20);

            y += 80;

            // =======================
            // FOOTER
            // =======================

            gfx.DrawLine(new XPen(XColors.Gray, 1), 0, page.Height - 60, page.Width, page.Height - 60);

            gfx.DrawString("Barroc Intens B.V.", smallFont, black, 40, page.Height - 40);
            gfx.DrawString("info@barrocintens.nl | www.barrocintens.nl", smallFont, black, 40, page.Height - 28);
            gfx.DrawString("KvK: 12345678 | BTW: NL001234567B01", smallFont, black, 40, page.Height - 16);

            // =======================
            // OPSLAAN & OPENEN
            // =======================
            var filename = Path.Combine(Path.GetTempPath(), $"Offerte_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            document.Save(filename);

            PdfFileUtility.ShowDocument(filename);
        }

        private void factuurAanmaken_Click(object sender, RoutedEventArgs e)
        {

        }

        private void contractAanmaken_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
