using Microsoft.Maui.Storage;

namespace momentum_app.Pages;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
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
        bool allValid = true;

        if (int.TryParse(enFocusTime.Text, out int focusTime))
        {
            Preferences.Set("FocusTime", focusTime);
        }
        else
        {
            allValid = false;
            await DisplayAlert("Error", "Invalid Focus Time", "OK");
        }

        if (int.TryParse(enShortBreak.Text, out int shortBreak))
        {
            Preferences.Set("ShortBreak", shortBreak);
        }
        else
        {
            allValid = false;
            await DisplayAlert("Error", "Invalid Short Break Time", "OK");
        }

        if (int.TryParse(enLngBreak.Text, out int longBreak))
        {
            Preferences.Set("LongBreak", longBreak);
        }
        else
        {
            allValid = false;
            await DisplayAlert("Error", "Invalid Long Break Time", "OK");
        }

        if (int.TryParse(enCycles.Text, out int cycles))
        {
            Preferences.Set("Cycles", cycles);
        }
        else
        {
            allValid = false;
            await DisplayAlert("Error", "Invalid Cycles Time", "OK");
        }

        if (allValid)
        {
            await DisplayAlert("Success", "Settings saved successfully", "OK");
        }

        else
        {
            await DisplayAlert("Error", "Something went wrong, contact the administrator", "All right");
        }

    }
}