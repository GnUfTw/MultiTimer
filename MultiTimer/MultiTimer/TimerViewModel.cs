using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using DynamicData;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ReactiveUI;

namespace MultiTimer
{
    public class TimerViewModel : ReactiveObject
    {
        private int _currentTotalSeconds;
        private int _currentTotalMilliseconds;
        private IDisposable _timerSubscription;
        private Timer _timer;
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();
        private IDisposable _progressUpdateSubscription;

        public TimerViewModel(Timer timer)
        {
            _timer = timer;
            _currentTotalSeconds = _timer.TotalSeconds;
            _currentTotalMilliseconds = _timer.TotalMilliseconds;

            Name = _timer.Name;
            FullTime = FormatTime(_currentTotalSeconds);
            CurrentTime = FormatTime(_currentTotalSeconds);

            // Read in existing list of saved/persisted timers.
            using (var sr = new StreamReader(@"multitimer.config"))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var timerList = serializer.Deserialize<List<Timer>>(reader);
                IsPersisted = timerList.Contains(_timer);
            }

            StartTimer = ReactiveCommand.Create(() =>
            {
                IsRunning = true;
                _progressUpdateSubscription = Observable
                    .Interval(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        _currentTotalMilliseconds -= 500;
                        SetTimerProgress();
                        if (_currentTotalMilliseconds > 0) return;
                        _progressUpdateSubscription.Dispose();
                    });
                _timerSubscription = Observable
                    .Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        // Update time being displayed to user.
                        _currentTotalSeconds--;
                        CurrentTime = FormatTime(_currentTotalSeconds);

                        if (_currentTotalSeconds > 0) return;

                        // Once the timer elapses to 0, play a single, short audio notification.
                        _mediaPlayer.Open(new Uri(@"long-expected.mp3", UriKind.RelativeOrAbsolute));
                        _mediaPlayer.Play();
                        _timerSubscription.Dispose();
                    });

            });

            StopTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription.Dispose();
                _progressUpdateSubscription.Dispose();
                IsRunning = false;
            });

            RestartTimer = ReactiveCommand.Create(() =>
            {
                // Set the timer back to it's original time.
                _timerSubscription.Dispose();
                IsRunning = false;
                _currentTotalSeconds = _timer.TotalSeconds;
                _currentTotalMilliseconds = _timer.TotalMilliseconds;
                SetTimerProgress();
                CurrentTime = FormatTime(_currentTotalSeconds);
            });

            SaveTimer = ReactiveCommand.Create(() =>
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Read in existing list of saved/persisted timers.
                List<Timer> timerList;
                using (var sr = new StreamReader(@"multitimer.config"))
                using (var reader = new JsonTextReader(sr))
                {
                    timerList = serializer.Deserialize<List<Timer>>(reader);
                }

                // Add given timer to existing list of saved/persisted timers.
                using (var sw = new StreamWriter(@"multitimer.config"))
                using (var writer = new JsonTextWriter(sw))
                {
                    if (timerList == null)
                    {
                        timerList = new List<Timer>();
                    }

                    timerList.Add(_timer);
                    serializer.Serialize(writer, timerList);
                    IsPersisted = true;
                }
            });

            DeleteTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription?.Dispose();

                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Read in existing list of saved/persisted timers.
                List<Timer> timerList;
                using (var sr = new StreamReader(@"multitimer.config"))
                using (var reader = new JsonTextReader(sr))
                {
                    timerList = serializer.Deserialize<List<Timer>>(reader);
                }

                // Remove given timer from list of saved/persisted timers.
                using (var sw = new StreamWriter(@"multitimer.config"))
                using (var writer = new JsonTextWriter(sw))
                {
                    if (timerList == null) return;
                    if (!timerList.Remove(_timer)) return;
                    serializer.Serialize(writer, timerList);
                    IsPersisted = false;
                }
            });

            EditTimer = ReactiveCommand.CreateFromTask(EditTimerSettings);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _fullTime;
        public string FullTime
        {
            get => _fullTime;
            set => this.RaiseAndSetIfChanged(ref _fullTime, value);
        }

        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set => this.RaiseAndSetIfChanged(ref _currentTime, value);
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                this.RaiseAndSetIfChanged(ref _isRunning, value);
                IsNotRunning = !value;
            }
        }

        private bool _isNotRunning = true;
        public bool IsNotRunning
        {
            get => _isNotRunning;
            set => this.RaiseAndSetIfChanged(ref _isNotRunning, value);
        }

        private bool _isPersisted;
        public bool IsPersisted
        {
            get => _isPersisted;
            set
            {
                _timer.IsPersisted = value;
                this.RaiseAndSetIfChanged(ref _isPersisted, value);
                IsNotPersisted = !value;
            }
        }

        private bool _isNotPersisted;
        public bool IsNotPersisted
        {
            get => _isNotPersisted;
            set => this.RaiseAndSetIfChanged(ref _isNotPersisted, value);
        }

        private int _timerProgress;
        public int TimerProgress
        {
            get => _timerProgress;
            set => this.RaiseAndSetIfChanged(ref _timerProgress, value);
        }

        private void SetTimerProgress()
        {
            TimerProgress = (_timer.TotalMilliseconds - _currentTotalMilliseconds) * 100 / _timer.TotalMilliseconds;
        }

        public ReactiveCommand<Unit, Unit> StartTimer { get; }

        public ReactiveCommand<Unit, Unit> StopTimer { get; }

        public ReactiveCommand<Unit, Unit> RestartTimer { get; }

        public ReactiveCommand<Unit, Unit> SaveTimer { get; }

        public ReactiveCommand<Unit, Unit> DeleteTimer { get; }

        public ReactiveCommand<Unit, Unit> EditTimer { get; }

        private string FormatTime(int seconds)
        {
            var currentHours = seconds / 3600;
            var currentMinutes = (seconds % 3600) / 60;
            var currentSeconds = (seconds % 3600) % 60;
            var isHourPadNecessary = currentHours < 10;
            var isMinutePadNecessary = currentMinutes < 10;
            var isSecondPadNecessary = currentSeconds < 10;

            return (isHourPadNecessary ? "0" : "") + currentHours + ":"
                    + (isMinutePadNecessary ? "0" : "") + currentMinutes + ":"
                    + (isSecondPadNecessary ? "0" : "") + currentSeconds;
        }

        private async Task EditTimerSettings()
        {
            // Present user with dialog where they can define timer settings.
            var dialogView = new EditTimerDialog { ViewModel = new TimerSettingsViewModel(_timer) };
            var result = await DialogHost.Show(dialogView, "RootDialog");

            // Create the timer from defined settings if the user didn't cancel the dialog.
            if (!(bool)(result ?? "NULL")) return;
            var originalTimer = _timer;
            _timer = new Timer
            {
                Name = dialogView.ViewModel.Name,
                Hours = dialogView.ViewModel.Hours,
                Minutes = dialogView.ViewModel.Minutes,
                Seconds = dialogView.ViewModel.Seconds
            };

            // Update timer view unconditionally.
            Name = _timer.Name;
            FullTime = FormatTime(_timer.TotalSeconds);

            // Update current time if timer is not running.
            if (!_isRunning)
            {
                CurrentTime = FullTime;
            }

            // Update serialized timer if persisted.
            if (!originalTimer.IsPersisted) return;
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            // Read in existing list of saved/persisted timers.
            List<Timer> timerList;
            using (var sr = new StreamReader(@"multitimer.config"))
            using (var reader = new JsonTextReader(sr))
            {
                timerList = serializer.Deserialize<List<Timer>>(reader);
            }

            // Replace given timer in list of saved/persisted timers.
            using (var sw = new StreamWriter(@"multitimer.config"))
            using (var writer = new JsonTextWriter(sw))
            {
                if (timerList == null) return;
                timerList.Replace(originalTimer, _timer);
                serializer.Serialize(writer, timerList);
            }
        }
    }
}