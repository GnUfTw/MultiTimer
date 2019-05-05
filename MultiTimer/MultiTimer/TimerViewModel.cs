using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Media;
using Newtonsoft.Json;
using ReactiveUI;

namespace MultiTimer
{
    public class TimerViewModel : ReactiveObject
    {
        private int _currentTotalSeconds;
        private IDisposable _timerSubscription;
        private readonly Timer _timer;
        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();

        public TimerViewModel(Timer timer)
        {
            _timer = timer;
            _currentTotalSeconds = _timer.TotalSeconds;

            Name = _timer.Name;
            FullTime = GetCurrentTimeFormatted();
            CurrentTime = GetCurrentTimeFormatted();

            StartTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        // Update time being displayed to user.
                        _currentTotalSeconds--;
                        CurrentTime = GetCurrentTimeFormatted();

                        if (_currentTotalSeconds != 0) return;
                        
                        // Once the timer elapses to 0, play a single, short audio notification.
                        _mediaPlayer.Open(new Uri(@"long-expected.mp3", UriKind.RelativeOrAbsolute));
                        _mediaPlayer.Play();
                        _timerSubscription.Dispose();
                    });
            });

            StopTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription.Dispose();
            });

            RestartTimer = ReactiveCommand.Create(() =>
            {
                // Set the timer back to it's original time.
                _timerSubscription.Dispose();
                _currentTotalSeconds = _timer.TotalSeconds;
                CurrentTime = GetCurrentTimeFormatted();
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
                }
            });
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

        public ReactiveCommand<Unit, Unit> StartTimer { get; }

        public ReactiveCommand<Unit, Unit> StopTimer { get; }

        public ReactiveCommand<Unit, Unit> RestartTimer { get; }

        public ReactiveCommand<Unit, Unit> SaveTimer { get; }

        public ReactiveCommand<Unit, Unit> DeleteTimer { get; }

        private string GetCurrentTimeFormatted()
        {
            var currentHours = _currentTotalSeconds / 3600;
            var currentMinutes = (_currentTotalSeconds % 3600) / 60;
            var currentSeconds = (_currentTotalSeconds % 3600) % 60;
            var isHourPadNecessary = currentHours < 10;
            var isMinutePadNecessary = currentMinutes < 10;
            var isSecondPadNecessary = currentSeconds < 10;

            return (isHourPadNecessary ? "0" : "") + currentHours + ":"
                    + (isMinutePadNecessary ? "0" : "") + currentMinutes + ":"
                    + (isSecondPadNecessary ? "0" : "") + currentSeconds;
        }
    }
}