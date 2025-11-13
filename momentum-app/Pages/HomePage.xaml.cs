using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.Maui.Audio;
using System;


namespace momentum_app.Pages
{
    public partial class HomePage : ContentPage
    {

        AudioPlayerViewModel MediaPlayer = new AudioPlayerViewModel();
        private enum TimerState
        {
            Focus,
            ShortBreak,
            LongBreak
        }

        private IDispatcherTimer _timer;
        private TimerState _currentState;
        private int _totalSeconds;
        private int _cycleCount;
        private bool _isRunning;

        private int _focusTime;
        private int _shortBreakTime;
        private int _longBreakTime;
        private int _cyclesForLongBreak;

        private bool _isFirstAppearance = true;

        public HomePage()
        {
            InitializeComponent();
            InitializeTimer();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
                {
                    await LocalNotificationCenter.Current.RequestNotificationPermission();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao pedir permissão: {ex.Message}");
            }

            LoadSettings();

            if (_isFirstAppearance)
            {
                ResetTimer(TimerState.Focus);
                _cycleCount = 0;
                _isFirstAppearance = false;
            }

            try
            {
                await MediaPlayer.LoadAudioAsync("music.wav");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }

        private void InitializeTimer()
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }
        private void LoadSettings()
        {
            _focusTime = Preferences.Get("FocusTime", 25);
            _shortBreakTime = Preferences.Get("ShortBreak", 5);
            _longBreakTime = Preferences.Get("LongBreak", 10);
            _cyclesForLongBreak = Preferences.Get("Cycles", 4);
        }

        private void SendNotification(string title, string message)
        {
            var request = new NotificationRequest
            {
                NotificationId = 1337,
                Title = title,
                Description = message,
                BadgeNumber = 42,

                //Android = new AndroidOptions
                //{
                //    IconSmallName = new AndroidIcon { ResourceName = "appicon" }
                //}
            };

            LocalNotificationCenter.Current.Show(request);
        }

        private void ResetTimer(TimerState newState)
        {
            StopTimer();
            _currentState = newState;

            switch (_currentState)
            {
                case TimerState.Focus:
                    _totalSeconds = _focusTime * 60;
                    lbFrase.Text = "Stay focused!";
                    break;
                case TimerState.ShortBreak:
                    _totalSeconds = _shortBreakTime * 60;
                    lbFrase.Text = "Time for a short break!";
                    break;
                case TimerState.LongBreak:
                    _totalSeconds = _longBreakTime * 60;
                    lbFrase.Text = "Take a long break!";
                    _cycleCount = 0;
                    break;
            }
            UpdateTimerDisplay();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            _totalSeconds--;
            UpdateTimerDisplay();

            if (_totalSeconds <= 0)
            {
                StopTimer();

                try
                {
                    await MediaPlayer.PlayAudioAsync(false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
                }

                SendNotification("Hey!", "Time is over!");
                GoToNextState();
            }
        }

        private void GoToNextState()
        {
            if (_currentState == TimerState.Focus)
            {
                var totalFocus = Preferences.Get("TotalFocusSessions", 0);
                Preferences.Set("TotalFocusSessions", totalFocus + 1);

                _cycleCount++;
                if (_cycleCount >= _cyclesForLongBreak)
                {
                    var totalLong = Preferences.Get("TotalLongBreaks", 0);
                    Preferences.Set("TotalLongBreaks", totalLong + 1);

                    ResetTimer(TimerState.LongBreak);
                }
                else
                {
                    var totalShort = Preferences.Get("TotalShortBreaks", 0);
                    Preferences.Set("TotalShortBreaks", totalShort + 1);

                    ResetTimer(TimerState.ShortBreak);
                }
            }
            else
            {
                ResetTimer(TimerState.Focus);
            }
        }

        private void UpdateTimerDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(_totalSeconds);
            lbTempo.Text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }

        private void StartTimer()
        {
            _timer.Start();
            _isRunning = true;
            btInicia.Source = "pausesolidfull.png";
        }

        private void StopTimer()
        {
            _timer.Stop();
            _isRunning = false;
            btInicia.Source = "playsolidfull.png";
        }

        private void btInicia_Clicked(object sender, EventArgs e)
        {
            if (_isRunning)
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
            ResetTimer(_currentState);
        }

        private async void btProx_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Skip?", "Are you sure you want to skip to the next session?", "Yes", "No");
            if (answer)
            {
                StopTimer();
                GoToNextState();
            }
        }
    }

    public class AudioPlayerViewModel
    {
        IAudioPlayer audioPlayer;

        public AudioPlayerViewModel()
        {
            audioPlayer = null;
        }

        public async Task LoadAudioAsync(string music)
        {
            if (audioPlayer == null)
            {
                audioPlayer = AudioManager.Current.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(music));
                audioPlayer.Loop = false;
            }
        }

        public async Task PlayAudioAsync(bool loop)
        {
            if (audioPlayer == null) return;

            audioPlayer.Loop = loop;

            if (audioPlayer.IsPlaying)
            {
                audioPlayer.Stop();
            }

            audioPlayer.Play();

            await Task.CompletedTask;
        }

        public async Task StopAudioAsync()
        {
            if (audioPlayer != null && audioPlayer.IsPlaying)
            {
                audioPlayer.Stop();
            }
            await Task.CompletedTask;
        }

        public async Task setAudioLoop(bool loop)
        {
            if (audioPlayer != null)
            {
                audioPlayer.Loop = loop;
            }
            await Task.CompletedTask;
        }
    }
}