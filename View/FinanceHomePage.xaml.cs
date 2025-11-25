using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.UniversalAccessibility.Drawing;
using System.IO;
using PdfSharp.Quality;

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
        private void offerteAanmaken_Click(object sender, RoutedEventArgs e)
        {
            var document = new PdfDocument();
            document.Info.Title = "Offerte";

            var page = document.AddPage();

            var logoPath = Path.Combine(AppContext.BaseDirectory, "Assets", "BarrocIntensHomePageImage.png");
            using var BarrocIntensHomePageImage = XImage.FromFile(logoPath);

            var gfx = XGraphics.FromPdfPage(page);

            // Fonts
            var titleFont = new XFont("Arial", 22, XFontStyleEx.Bold);
            var headerFont = new XFont("Arial", 14, XFontStyleEx.Bold);
            var regularFont = new XFont("Arial", 12);
            var smallFont = new XFont("Arial", 10);

            // Kleuren
            var geel = XBrushes.Gold;
            var zwart = new XSolidBrush(XColor.FromArgb(0x22, 0x20, 0x1D));
            var donker = XBrushes.Black;

            // Header
            gfx.DrawRectangle(geel, 0, 0, page.Width, 80);

            // Logo
            gfx.DrawImage(BarrocIntensHomePageImage, 30,20,100,40);

            // Titel rechtsboven
            gfx.DrawString("Offerte", titleFont, zwart,
                new XPoint(page.Width - 40, 45),
                XStringFormats.TopRight);

            // Datum
            gfx.DrawString($"Datum: {DateTime.Now:dd-MM-yyyy}",
                regularFont, donker, new XPoint(40, 120));

            // Beschrijving
            gfx.DrawString("Beschrijving", headerFont, donker, new XPoint(40, 170));

            // Lijn onder sectie
            gfx.DrawLine(new XPen(XColors.Gray, 1),
                new XPoint(40, 185), new XPoint(page.Width - 40, 185));

            // Inhoud
            gfx.DrawString("• Koffiezetapparaat", regularFont, donker, new XPoint(60, 210));
            gfx.DrawString("• Service en installatie inbegrepen", regularFont, donker, new XPoint(60, 230));

            // Footer
            gfx.DrawString("Barroc Intens",
                smallFont, XBrushes.Gray, new XPoint(40, page.Height - 40));

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
