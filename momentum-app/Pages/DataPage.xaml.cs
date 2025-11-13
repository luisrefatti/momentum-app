namespace momentum_app.Pages
{
    public partial class DataPage : ContentPage
    {
        public DataPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStats();
        }

        private void LoadStats()
        {
            lbFocus.Text = Preferences.Get("TotalFocusSessions", 0).ToString();
            lbShortBreaks.Text = Preferences.Get("TotalShortBreaks", 0).ToString();
            lbLongBreaks.Text = Preferences.Get("TotalLongBreaks", 0).ToString();
            lbTasks.Text = Preferences.Get("TotalTasksCompleted", 0).ToString();
        }

        private async void btReset_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Reset?", "Are you sure you want to reset all statistics? This cannot be undone.", "Yes, Reset", "Cancel");

            if (answer)
            {
                Preferences.Remove("TotalFocusSessions");
                Preferences.Remove("TotalShortBreaks");
                Preferences.Remove("TotalLongBreaks");
                Preferences.Remove("TotalTasksCompleted");

                LoadStats();
            }
        }
    }
}