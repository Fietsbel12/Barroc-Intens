using BarrocIntens.Data;
using BarrocIntens.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarrocIntens.View
{
    public class CartItem
    {
        public Koffiezetapparaat Product { get; set; }
        public int Aantal { get; set; }
        public string TotaalPrijs => $"€ {(decimal)Product.Prijs * Aantal:N2}";
        public decimal TotaalBedragNumeriek => (decimal)Product.Prijs * Aantal;
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
                // We laden de lijst opnieuw in. 
                // Klanten blijven hetzelfde, maar producten krijgen de nieuwste voorraadstand.
                var producten = db.Koffiezetapparaten
                                  .Where(p => p.Voorraad > 0)
                                  .OrderBy(p => p.Naam).ToList();

                ProductComboBox.ItemsSource = producten;

                CustomerComboBox.ItemsSource = db.Klanten
                                                .OrderBy(k => k.KlantNaam).ToList();
            }
        }

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductComboBox.SelectedItem is Koffiezetapparaat selectedProduct)
            {
                // Toon voorraad met kleurindicatie
                StockStatusText.Text = $"Beschikbaar: {selectedProduct.Voorraad} stuks | Prijs: €{selectedProduct.Prijs:N2}";

                if (selectedProduct.Voorraad < 5)
                    StockStatusText.Foreground = new SolidColorBrush(Colors.Red);
                else
                    StockStatusText.Foreground = new SolidColorBrush(Colors.Green);

                // Beperk de NumberBox tot wat er echt is
                QuantityNumberBox.Maximum = selectedProduct.Voorraad;
                QuantityNumberBox.Value = 1;
            }
            else
            {
                StockStatusText.Text = "Voorraad: -";
                StockStatusText.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            var selectedProduct = ProductComboBox.SelectedItem as Koffiezetapparaat;
            int aantalWens = (int)QuantityNumberBox.Value;

            if (selectedProduct == null) { ShowError("Selecteer eerst een product."); return; }
            if (aantalWens <= 0) { ShowError("Aantal moet minimaal 1 zijn."); return; }

            var bestaandItem = winkelmandje.FirstOrDefault(i => i.Product.Id == selectedProduct.Id);
            int aantalAlInMandje = bestaandItem != null ? bestaandItem.Aantal : 0;

            if ((aantalAlInMandje + aantalWens) > selectedProduct.Voorraad)
            {
                ShowError($"Onvoldoende voorraad! Er zijn er nog {selectedProduct.Voorraad} over.");
                return;
            }

            if (bestaandItem != null)
            {
                int nieuweHoeveelheid = bestaandItem.Aantal + aantalWens;
                int index = winkelmandje.IndexOf(bestaandItem);
                winkelmandje[index] = new CartItem { Product = selectedProduct, Aantal = nieuweHoeveelheid };
            }
            else
            {
                winkelmandje.Add(new CartItem { Product = selectedProduct, Aantal = aantalWens });
            }

            CalculateTotal();
        }

        private void CalculateTotal()
        {
            decimal totaal = winkelmandje.Sum(item => item.TotaalBedragNumeriek);
            TotalPriceText.Text = $"€ {totaal:N2}";
        }

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;
            var selectedCustomer = CustomerComboBox.SelectedItem as Klant;

            if (selectedCustomer == null) { ShowError("Selecteer eerst een klant."); return; }
            if (winkelmandje.Count == 0) { ShowError("Winkelmandje is nog leeg."); return; }

            using (var db = new AppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    
                    foreach (var item in winkelmandje)
                    {
                        var dbProduct = db.Koffiezetapparaten.Find(item.Product.Id);

                        if (dbProduct == null || dbProduct.Voorraad < item.Aantal)
                        {
                            transaction.Rollback();
                            ShowError($"Product '{item.Product.Naam}' is in de tussentijd uitverkocht.");
                            return;
                        }

                        // Voorraad aftrekken
                        dbProduct.Voorraad -= item.Aantal;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // 1. Maak UI leeg
                    winkelmandje.Clear();
                    CalculateTotal();
                    CustomerComboBox.SelectedIndex = -1;
                    ProductComboBox.SelectedIndex = -1;

                    // 2. BELANGRIJK: Vernieuw de data uit de DB zodat de nieuwe voorraad zichtbaar is
                    LoadData();

                    ContentDialog successDialog = new ContentDialog
                    {
                        Title = "Order Voltooid",
                        Content = "De verkoop is opgeslagen en de voorraad is bijgewerkt.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await successDialog.ShowAsync();

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