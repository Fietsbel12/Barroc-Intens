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
            DataContext = this;
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

            // Kalender vernieuwen zodat de dagen correct gekleurd worden
            CalendarView.SelectedDates.Clear();
            CalendarView.SelectedDates.Add(DateTime.Today);
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
            if (args.Item.Date.Year < 1900) return;

            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(CalendarView_CalendarViewDayItemChanging);
            }
            else
            {
                var today = DateTime.Today;
                var selectedDate = sender.SelectedDates.FirstOrDefault().Date;

                bool hasTasks = TakenLijst.Any(t => t.Tijd.Date == args.Item.Date.Date);

                // Achtergrondkleur instellen
                if (args.Item.Date.Date == today)
                {
                    args.Item.Background = new SolidColorBrush(Colors.Blue);
                    args.Item.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (hasTasks)
                {
                    args.Item.Background = new SolidColorBrush(Colors.Yellow);
                    args.Item.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    args.Item.Background = new SolidColorBrush(Colors.Transparent);
                    args.Item.Foreground = new SolidColorBrush(Colors.Black);
                }

                // Border voor geselecteerde dag
                if (args.Item.Date.Date == selectedDate)
                {
                    args.Item.BorderBrush = new SolidColorBrush(Colors.OrangeRed); // kies gewenste kleur
                    args.Item.BorderThickness = new Thickness(3);
                }
                else
                {
                    args.Item.BorderBrush = null;
                    args.Item.BorderThickness = new Thickness(0);
                }
            }
        }



        private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (!sender.SelectedDates.Any())
            {
                SelectedDateTextBlock.Text = "Selecteer een dag";
                TakenVoorGeselecteerdeDag.Clear();
                return;
            }

            var day = sender.SelectedDates.First().Date;

            var filtered = TakenLijst
                .Where(t => t.Tijd.Date == day)
                .OrderBy(t => t.Tijd)
                .ToList();

            TakenVoorGeselecteerdeDag.Clear();
            foreach (var taak in filtered)
                TakenVoorGeselecteerdeDag.Add(taak);

            SelectedDateTextBlock.Text = $"Taken voor {day:dd-MM-yyyy}";
            TaskDetailPanel.Visibility = Visibility.Collapsed;
        }

        private void TaskList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not Taken taak) return;

            TaskDetailName.Text = taak.Name;
            TaskDetailDescription.Text = taak.Description;
            TaskDetailTime.Text = taak.Tijd.ToString("dd-MM-yyyy HH:mm");
            TaskDetailAssignedTo.Text = $"Toegewezen aan: {taak.Medewerker?.Naam}";

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
