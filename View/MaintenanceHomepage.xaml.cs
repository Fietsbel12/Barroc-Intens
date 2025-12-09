using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class MaintenanceHomepage : Page
    {
        private string medewerkerRol;

        public ObservableCollection<Taken> TakenLijst { get; set; } = new();

        public MaintenanceHomepage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            await LoadTakenAsync();
            DataContext = this;
        }

        private async Task LoadTakenAsync()
        {
            using var db = new AppDbContext();
            var taken = await db.Taken
                                .Include(t => t.Medewerker)
                                .ToListAsync();

            // Filter alleen taken van medewerkers met rol "Maintenance"
            var maintenanceTaken = taken
                .Where(t => t.Medewerker != null && t.Medewerker.MedewerkerRol == "Maintenance")
                .OrderBy(t => t.Tijd);

            TakenLijst.Clear();
            foreach (var taak in maintenanceTaken)
            {
                TakenLijst.Add(taak);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
