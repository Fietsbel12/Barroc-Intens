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

        private DateTime _currentDisplayDate = DateTime.Today;

        public PlannerHomepage()
        {
            InitializeComponent();

            CalendarView.Language = "nl-NL";
            CalendarView.SelectedDates.Add(DateTime.Today);
            CalendarView.SetDisplayDate(_currentDisplayDate);
            UpdateCurrentMonthDisplay(_currentDisplayDate);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            if (medewerkerRol == "Planner" || medewerkerRol == "Eigenaar")
            {
                backButton.Visibility = Visibility.Visible;
                CreatetaskButton.Visibility = Visibility.Visible;
            }


            // Taken opnieuw laden zodra we op deze pagina komen
            await LoadTakenAsync();

            DataContext = this;
        }


        private void FilterTasksForSelectedDate(DateTime selectedDate)
        {
            var startOfDay = selectedDate.Date;
            var endOfDay = startOfDay.AddDays(1);

            var filteredTasks = TakenLijst
                .Where(t => t.Tijd >= startOfDay && t.Tijd < endOfDay)
                .OrderBy(t => t.Tijd)
                .ToList();

            TakenVoorGeselecteerdeDag.Clear();
            foreach (var taak in filteredTasks)
                TakenVoorGeselecteerdeDag.Add(taak);

            SelectedDateTextBlock.Text = $"Taken voor: {selectedDate:dd MMMM yyyy}";
            TaskDetailPanel.Visibility = Visibility.Collapsed;
        }

        private void UpdateCurrentMonthDisplay(DateTime date)
        {
            CurrentMonthTextBlock.Text = date.ToString("MMMM yyyy");
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDisplayDate = _currentDisplayDate.AddMonths(-1);
            CalendarView.SetDisplayDate(_currentDisplayDate);
            UpdateCurrentMonthDisplay(_currentDisplayDate);
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDisplayDate = _currentDisplayDate.AddMonths(1);
            CalendarView.SetDisplayDate(_currentDisplayDate);
            UpdateCurrentMonthDisplay(_currentDisplayDate);
        }

        private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (args.Phase == 0)
                args.RegisterUpdateCallback(CalendarView_CalendarViewDayItemChanging);
            else if (args.Phase == 1)
            {
                var hasTasks = TakenLijst.Any(t => t.Tijd.Date == args.Item.Date.Date);
                args.Item.Background = hasTasks
                    ? new SolidColorBrush(Colors.Gold)
                    : new SolidColorBrush(Colors.Transparent);
            }
        }

        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (!sender.SelectedDates.Any()) return;

            var selectedDate = sender.SelectedDates.First().Date;
            while (sender.SelectedDates.Count > 1)
                sender.SelectedDates.RemoveAt(1);

            FilterTasksForSelectedDate(selectedDate);
        }

        private void TaskList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Taken taak)
            {
                TaskDetailName.Text = taak.Name;
                TaskDetailDescription.Text = taak.Description;
                TaskDetailTime.Text = taak.Tijd.ToString("dd-MM-yyyy HH:mm");
                TaskDetailPanel.Visibility = Visibility.Visible;
            }
        }
        private async Task LoadTakenAsync()
        {
            using (var db = new AppDbContext())
            {
                var takenUitDB = await db.Taken
                    .OrderBy(t => t.Tijd)
                    .ToListAsync();

                TakenLijst.Clear();
                foreach (var taak in takenUitDB)
                    TakenLijst.Add(taak);
            }

            if (CalendarView.SelectedDates.Any())
            {
                FilterTasksForSelectedDate(CalendarView.SelectedDates.First().Date);
            }
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
