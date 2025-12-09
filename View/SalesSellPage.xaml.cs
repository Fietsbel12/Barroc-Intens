using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarrocIntens.View
{
    // Hulpklasse voor het mandje (MOET ZICH IN DEZELFDE NAMESPACE BEVINDEN of de XAML aanpassen)
    // De naam is aangepast naar 'CartItem' (hoofdletter I) voor consistentie met XAML/C# conventies
    public class CartItem
    {
        public Koffiezetapparaat Product { get; set; }
        public int Aantal { get; set; }
        public decimal TotaalPrijs => (decimal)Product.Prijs * Aantal;
    }

    public sealed partial class SalesSellPage : Page
    {
        private string medewerkerRol;
        private ObservableCollection<CartItem> winkelmandje = new ObservableCollection<CartItem>();

        public SalesSellPage()
        {
            this.InitializeComponent();
            CartListView.ItemsSource = winkelmandje;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new AppDbContext())
            {
                ProductComboBox.ItemsSource = db.Koffiezetapparaten
                                                .Where(p => p.Voorraad > 0)
                                                .OrderBy(p => p.Naam).ToList();

                CustomerComboBox.ItemsSource = db.Klanten
                                                .OrderBy(k => k.KlantNaam).ToList();
            }
        }

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Koffiezetapparaat selectedProduct)
            {
                StockStatusText.Text = $"Beschikbaar: {selectedProduct.Voorraad} | Prijs: €{selectedProduct.Prijs}";
                QuantityNumberBox.Value = 1;
            }
        }

        // TOEVOEGEN AAN MANDJE
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            var selectedProduct = ProductComboBox.SelectedItem as Koffiezetapparaat;
            // Zorg ervoor dat je de NumberBox waarde converteert naar int.
            int aantalWens = (int)QuantityNumberBox.Value;

            if (selectedProduct == null) { ShowError("Kies een product."); return; }
            if (aantalWens <= 0) { ShowError("Aantal moet minimaal 1 zijn."); return; }

            var bestaandItem = winkelmandje.FirstOrDefault(i => i.Product.Id == selectedProduct.Id);
            int aantalAlInMandje = bestaandItem != null ? bestaandItem.Aantal : 0;

            if ((aantalAlInMandje + aantalWens) > selectedProduct.Voorraad)
            {
                ShowError($"Niet genoeg voorraad! Je hebt er al {aantalAlInMandje} in het mandje, en er zijn er totaal maar {selectedProduct.Voorraad}.");
                return;
            }

            if (bestaandItem != null)
            {
                bestaandItem.Aantal += aantalWens;

                // Hack om de ListView te refreshen
                int index = winkelmandje.IndexOf(bestaandItem);
                winkelmandje[index] = new CartItem { Product = selectedProduct, Aantal = bestaandItem.Aantal, };
            }
            else
            {
                winkelmandje.Add(new CartItem
                {
                    Product = selectedProduct,
                    Aantal = aantalWens
                });
            }

            CalculateTotal();
        }

        // TOTAAL BEREKENEN
        private void CalculateTotal()
        {
            decimal totaal = winkelmandje.Sum(item => item.TotaalPrijs);
            TotalPriceText.Text = $"€ {totaal:N2}";
        }

        // AFREKENEN (Database update)
        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
            var selectedCustomer = CustomerComboBox.SelectedItem as Klant;

            if (selectedCustomer == null) { ShowError("Selecteer eerst een klant voor deze order."); return; }
            if (winkelmandje.Count == 0) { ShowError("Het winkelmandje is leeg."); return; }

            using (var db = new AppDbContext())
            {
                // De transactie zorgt dat alles in één keer wordt uitgevoerd.
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in winkelmandje)
                        {
                            var dbProduct = db.Koffiezetapparaten.Find(item.Product.Id);

                            if (dbProduct == null || dbProduct.Voorraad < item.Aantal)
                            {
                                throw new Exception($"Product '{item.Product.Naam}' is niet meer voldoende op voorraad.");
                            }

                            // Voorraad bijwerken
                            dbProduct.Voorraad -= item.Aantal;

                            // (Factuur/Bestelling aanmaken hier)
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        // UI Schoonmaken
                        winkelmandje.Clear();
                        CalculateTotal();
                        CustomerComboBox.SelectedIndex = -1;
                        ProductComboBox.SelectedIndex = -1;

                        ContentDialog successDialog = new ContentDialog
                        {
                            Title = "Verkoop Succesvol",
                            Content = "De order is verwerkt en de voorraad is bijgewerkt.",
                            CloseButtonText = "Ok",
                            XamlRoot = this.XamlRoot
                        };
                        await successDialog.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowError($"Fout tijdens verkoop: De transactie is ongedaan gemaakt. Details: {ex.Message}");
                    }
                }
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}