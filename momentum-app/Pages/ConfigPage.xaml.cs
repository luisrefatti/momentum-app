namespace momentum_app.Pages;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();
    }

    private void LoadSettings()
    {
        enFocusTime.Text = Preferences.Get("FocusDuration", 25).ToString();
        enShortBreak.Text = Preferences.Get("ShortBreakDuration", 5).ToString();
        enLongBreak.Text = Preferences.Get("LongBreakDuration", 15).ToString();
        enCycles.Text = Preferences.Get("CyclesForLongBreak", 4).ToString();
    }

    private void bt_save_Clicked(object sender, EventArgs e)
    {
        try
        {
            Preferences.Set("FocusDuration", Convert.ToDouble(enFocusTime.Text));
            Preferences.Set("ShortBreakDuration", Convert.ToDouble(enShortBreak.Text));
            Preferences.Set("LongBreakDuration", Convert.ToDouble(enLongBreak.Text));
            Preferences.Set("CyclesForLongBreak", Convert.ToDouble(enCycles.Text));

            DisplayAlert("Success", "Settings saved.", "OK");
            Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Please, insert valid numeric values.", "OK");
        }
    }
}