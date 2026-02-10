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
    // Pagina voor het plannen en bekijken van taken via een kalender
    public sealed partial class PlannerHomepage : Page
    {
        // Houdt de rol van de ingelogde medewerker bij (bijv. Planner of Eigenaar)
        private string medewerkerRol;

        // Bevat alle taken die uit de database worden opgehaald
        public ObservableCollection<Taken> TakenLijst { get; set; } = new();

        // Bevat alleen de taken van de geselecteerde dag
        public ObservableCollection<Taken> TakenVoorGeselecteerdeDag { get; set; } = new();

        // Constructor van de pagina
        // Stelt de DataContext in voor databinding met XAML
        public PlannerHomepage()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Wordt aangeroepen wanneer naar deze pagina wordt genavigeerd
        // Ontvangt de rol van de medewerker en initialiseert de pagina
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Haal de rol op uit de navigatieparameter
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
            =
            await LoadTakenAsync();

            CalendarView.SelectedDates.Clear();
            CalendarView.SelectedDates.Add(DateTime.Today);
        }

        // Haalt alle taken inclusief gekoppelde medewerker op uit de database
        // en vult de TakenLijst ObservableCollection
        private async Task LoadTakenAsync()
        {
            
            using var db = new AppDbContext();

            
            var items = await db.Taken.Include(t => t.Medewerker).ToListAsync();

            
            TakenLijst.Clear();

            
            foreach (var taak in items)
                TakenLijst.Add(taak);
        }

        // Wordt gebruikt om kalenderdagen dynamisch aan te passen
        // (bijvoorbeeld kleuren voor vandaag, taken en geselecteerde dag)
        private void CalendarView_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            
            if (args.Item.Date.Year < 1900) return;

            // Fase 0 registreert een callback voor UI-updates
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

                if (args.Item.Date.Date == selectedDate)
                {
                    args.Item.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
                    args.Item.BorderThickness = new Thickness(3);
                }
                else
                {
                    args.Item.BorderBrush = null;
                    args.Item.BorderThickness = new Thickness(0);
                }
            }
        }

        // Wordt aangeroepen wanneer de geselecteerde datum in de kalender verandert
        // Filtert taken op basis van de gekozen dag
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

            // Werk de takenlijst bij
            TakenVoorGeselecteerdeDag.Clear();
            foreach (var taak in filtered)
                TakenVoorGeselecteerdeDag.Add(taak);

            // Toon de geselecteerde datum boven de takenlijst
            SelectedDateTextBlock.Text = $"Taken voor {day:dd-MM-yyyy}";

            // Verberg het detailpaneel
            TaskDetailPanel.Visibility = Visibility.Collapsed;
        }

        // Wordt aangeroepen wanneer een taak wordt aangeklikt
        // Toont de details van de geselecteerde taak
        private void TaskList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Controleer of het aangeklikte item een taak is
            if (e.ClickedItem is not Taken taak) return;

            // Vul het detailpaneel met taakgegevens
            TaskDetailName.Text = taak.Name;
            TaskDetailDescription.Text = taak.Description;
            TaskDetailTime.Text = taak.Tijd.ToString("dd-MM-yyyy HH:mm");
            TaskDetailAssignedTo.Text = $"Toegewezen aan: {taak.Medewerker?.Naam}";

            
            TaskDetailPanel.Visibility = Visibility.Visible;
        }

        // Navigatie buttons
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
