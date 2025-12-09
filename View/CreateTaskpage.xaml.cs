using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using BarrocIntens.Data;

namespace BarrocIntens.View
{
    public sealed partial class CreateTaskpage : Page
    {
        private string medewerkerRol;
        private readonly AppDbContext _context = new AppDbContext(); // <-- BELANGRIJK
        // test
        public CreateTaskpage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlannerHomepage), medewerkerRol);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text;
            string description = DescriptionInput.Text;

            // Combineer Datum + Tijd
            DateTime tijd = DateInput.Date.DateTime + TimeInput.Time;

            var taak = new Taken
            {
                Name = name,
                Description = description,
                Tijd = tijd
            };

            // ------------------ DATABASE OPSLAAN ------------------
            _context.Taken.Add(taak);
            _context.SaveChanges();
            // -------------------------------------------------------

            ContentDialog dialog = new ContentDialog
            {
                Title = "Taak opgeslagen",
                Content = "De taak is succesvol aangemaakt!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();

            Frame.Navigate(typeof(PlannerHomepage), medewerkerRol);
        }
    }
}
