using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BarrocIntens.View
{
    public sealed partial class PlannerHomepage : Page
    {
        private string medewerkerRol;

        public ObservableCollection<Taken> TakenLijst { get; set; } = new();
        public ObservableCollection<Taken> TakenVoorGeselecteerdeDag { get; set; } = new();

        public PlannerHomepage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            medewerkerRol = e.Parameter as string;
            if (string.IsNullOrWhiteSpace(medewerkerRol))
            {
                Frame.GoBack();
                return;
            }

            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            CreatetaskButton.Visibility =
                (medewerkerRol == "Planner" || medewerkerRol == "Eigenaar")
                ? Visibility.Visible
                : Visibility.Collapsed;

            await LoadTakenAsync();

            DataContext = this;

            // Trigger CalendarViewDayItemChanging om dagen met taken te markeren
            // Dit werkt door tijdelijk een selectie te maken
            CalendarView.SelectedDates.Add(DateTime.Today);
            CalendarView.SelectedDates.Clear();
        }

        private async Task LoadTakenAsync()
        {
            using var db = new AppDbContext();
            var items = await db.Taken.Include(t => t.Medewerker).ToListAsync();

            TakenLijst.Clear();
            foreach (var taak in items)
                TakenLijst.Add(taak);
        }

        private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            // Als er taken zijn op deze dag, markeer met goud
            bool hasTasks = TakenLijst.Any(t => t.Tijd.Date == args.Item.Date.Date);
            args.Item.Background = hasTasks ? new SolidColorBrush(Colors.Gold) : new SolidColorBrush(Colors.Transparent);
        }

        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (!sender.SelectedDates.Any()) return;

            var day = sender.SelectedDates.First().Date;

            var filtered = TakenLijst
                .Where(t => t.Tijd.Date == day)
                .OrderBy(t => t.Tijd)
                .ToList();

            TakenVoorGeselecteerdeDag.Clear();
            foreach (var taak in filtered)
                TakenVoorGeselecteerdeDag.Add(taak);

            SelectedDateTextBlock.Text = $"Taken voor {day:dd-MM-yyyy}";
        }

        private void TaskList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not Taken taak) return;

            TaskDetailName.Text = taak.Name;
            TaskDetailDescription.Text = taak.Description;
            TaskDetailTime.Text = taak.Tijd.ToString("dd-MM-yyyy HH:mm");
            TaskDetailAssignedTo.Text = $"Toegewezen aan: {taak.Medewerker.Naam}";

            TaskDetailPanel.Visibility = Visibility.Visible;
        }

        private void CreatetaskButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateTaskpage), medewerkerRol);
        }

        private void CloseDetail_Click(object sender, RoutedEventArgs e)
        {
            TaskDetailPanel.Visibility = Visibility.Collapsed;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MedewerkerDashboard), medewerkerRol);
        }
    }
}
