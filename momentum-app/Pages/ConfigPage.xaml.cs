using Microsoft.Maui.Storage;

namespace momentum_app.Pages;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        enFocusTime.Text = Preferences.Get("FocusTime", 25).ToString();
        enShortBreak.Text = Preferences.Get("ShortBreak", 5).ToString();
        enLngBreak.Text = Preferences.Get("LongBreak", 10).ToString();
        enCycles.Text = Preferences.Get("Cycles", 4).ToString();
    }

    private async void bt_save_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(enFocusTime.Text, out int focusTime))
        {
            await DisplayAlert("Error", "Invalid Focus Time", "OK");
            return;
        }

        if (!int.TryParse(enShortBreak.Text, out int shortBreak))
        {
            await DisplayAlert("Error", "Invalid Short Break Time", "OK");
            return;
        }

        if (!int.TryParse(enLngBreak.Text, out int longBreak))
        {
            await DisplayAlert("Error", "Invalid Long Break Time", "OK");
            return;
        }

        if (!int.TryParse(enCycles.Text, out int cycles))
        {
            await DisplayAlert("Error", "Invalid Cycles Count", "OK");
            return;
        }

        Preferences.Set("FocusTime", focusTime);
        Preferences.Set("ShortBreak", shortBreak);
        Preferences.Set("LongBreak", longBreak);
        Preferences.Set("Cycles", cycles);

        await DisplayAlert("Success", "Settings saved successfully", "OK");
    }
}