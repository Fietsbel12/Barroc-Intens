using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using BarrocIntens.Data;
using System.Linq;

namespace BarrocIntens.View
{
    public sealed partial class CreateTaskpage : Page
    {
        private string medewerkerRol;
        private readonly AppDbContext _context = new AppDbContext();

        public CreateTaskpage()
        {
            this.InitializeComponent();
            LoadMedewerkers();
        }

        private void LoadMedewerkers()
        {
            // Haal medewerkers uit database
            var medewerkers = _context.Medewerkers.ToList();

            // Zet in ComboBox
            MedewerkerCombo.ItemsSource = medewerkers;
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
            if (MedewerkerCombo.SelectedItem == null)
            {
                ContentDialog err = new ContentDialog
                {
                    Title = "Geen medewerker geselecteerd",
                    Content = "Selecteer een medewerker voordat je de taak opslaat.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await err.ShowAsync();
                return;
            }

            string name = NameInput.Text;
            string description = DescriptionInput.Text;

            DateTime tijd = DateInput.Date.DateTime + TimeInput.Time;

            // Haal medewerker ID
            int medewerkerId = (int)MedewerkerCombo.SelectedValue;

            var taak = new Taken
            {
                Name = name,
                Description = description,
                Tijd = tijd,
                MedewerkerId = medewerkerId
            };

            _context.Taken.Add(taak);
            _context.SaveChanges();

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
