using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrocIntens.View
{
    public sealed partial class PlannerHomepage : Page
    {
        private string medewerkerRol;

        // Takenlijst voor de agenda
        public ObservableCollection<Taken> TakenLijst { get; set; } = new();

        public PlannerHomepage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Rol opslaan en tonen
            medewerkerRol = e.Parameter as string;
            RolTextBlock.Text = $"Huidige rol: {medewerkerRol}";

            // Taken uit database ophalen
            using (var db = new AppDbContext())
            {
                var takenUitDB = await db.Taken.OrderBy(t => t.Tijd).ToListAsync();

                TakenLijst.Clear();
                foreach (var taak in takenUitDB)
                {
                    TakenLijst.Add(taak);
                }
            }

            DataContext = this;
        }

        // Klik op taak in de lijst
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

        // Sluit detailpaneel
        private void CloseDetail_Click(object sender, RoutedEventArgs e)
        {
            TaskDetailPanel.Visibility = Visibility.Collapsed;
        }

        // Back-knop
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
