namespace momentum_app.Pages;

public partial class HomePage : ContentPage
{
    private enum PomodoroState
    {
        Focus,
        ShortBreak,
        LongBreak
    }

    private IDispatcherTimer timer;
    private TimeSpan timeRemaining;
    private PomodoroState currentState = PomodoroState.Focus;
    private bool isRunning = false;
    private int currentCycle = 0;

    private int focusDuration;
    private int shortBreakDuration;
    private int longBreakDuration;
    private int cyclesForLongBreak;

    public HomePage()
    {
        InitializeComponent();
        InitializeTimer();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSettings();

        if (!isRunning)
        {
            ResetTimerForCurrentState();
            UpdateUI();
        }
    }

    private void InitializeTimer()
    {
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += Timer_Tick;
    }

    private void LoadSettings()
    {
        focusDuration = Preferences.Get("FocusDuration", 25);
        shortBreakDuration = Preferences.Get("ShortBreakDuration", 5);
        longBreakDuration = Preferences.Get("LongBreakDuration", 15);
        cyclesForLongBreak = Preferences.Get("CyclesForLongBreak", 4);
    }

    private void StartTimer()
    {
        timer.Start();
        isRunning = true;
        btInicia.Source = "pausesolidfull.png";
    }

    private void StopTimer()
    {
        timer.Stop();
        isRunning = false;
        btInicia.Source = "playsolidfull.png";
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (timeRemaining.TotalSeconds > 0)
        {
            timeRemaining = timeRemaining.Add(TimeSpan.FromSeconds(-1));
            UpdateUI();
        }
        else
        {
            StopTimer();
            // adicioanr som de notificacao aqui
            GoToNextState();
        }
    }

    private void btInicia_Clicked(object sender, EventArgs e)
    {
        if (isRunning)
        {
            StopTimer();
        }
        else
        {
            StartTimer();
        }
    }

    private void btReinicia_Clicked(object sender, EventArgs e)
    {
        StopTimer();
        ResetTimerForCurrentState();
        UpdateUI();
    }

    private void btProx_Clicked(object sender, EventArgs e)
    {
        StopTimer();
        GoToNextState();
    }

    private void GoToNextState()
    {
        if (currentState == PomodoroState.Focus)
        {
            currentCycle++;
            if (currentCycle % cyclesForLongBreak == 0)
            {
                currentState = PomodoroState.LongBreak;
            }
            else
            {
                currentState = PomodoroState.ShortBreak;
            }
        }
        else
        {
            currentState = PomodoroState.Focus;
        }

        ResetTimerForCurrentState();
        UpdateUI();
        StartTimer();
    }

    private void ResetTimerForCurrentState()
    {
        switch (currentState)
        {
            case PomodoroState.Focus:
                timeRemaining = TimeSpan.FromMinutes(focusDuration);
                break;
            case PomodoroState.ShortBreak:
                timeRemaining = TimeSpan.FromMinutes(shortBreakDuration);
                break;
            case PomodoroState.LongBreak:
                timeRemaining = TimeSpan.FromMinutes(longBreakDuration);
                break;
        }
    }

    private void UpdateUI()
    {
        lbTempo.Text = timeRemaining.ToString(@"mm\:ss");

        switch (currentState)
        {
            case PomodoroState.Focus:
                lbFrase.Text = "Stay focused!";
                break;
            case PomodoroState.ShortBreak:
                lbFrase.Text = "Short break.";
                break;
            case PomodoroState.LongBreak:
                lbFrase.Text = "Long pause. Enjoy your rest!";
                break;
        }
    }
}