using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace MultiTimer
{
    public class TimerViewModel : ReactiveObject
    {
        private int _currentTotalSeconds;
        private IDisposable _timerSubscription;
        private Timer _timer;

        public TimerViewModel(Timer timer)
        {
            _timer = timer;
            Name = _timer.Name;
            var timeString = 
                (_timer.Hours < 10 ? "0" : "") + _timer.Hours + ":" +
                (_timer.Minutes < 10 ? "0" : "") + _timer.Minutes + ":" + 
                (_timer.Seconds < 10 ? "0" : "") + _timer.Seconds;
            FullTime = timeString;
            CurrentTime = timeString;
            _currentTotalSeconds = _timer.TotalSeconds;
            StartTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                    .Subscribe(_ =>
                    {
                        _currentTotalSeconds--;
                        UpdateCurrentTime();
                    });
            });
            StopTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription.Dispose();
            });
            RestartTimer = ReactiveCommand.Create(() =>
            {
                _timerSubscription.Dispose();
                _currentTotalSeconds = _timer.TotalSeconds;
                UpdateCurrentTime();
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

        private void UpdateCurrentTime()
        {
            var currentHours = _currentTotalSeconds / 3600;
            var currentMinutes = (_currentTotalSeconds % 3600) / 60;
            var currentSeconds = (_currentTotalSeconds % 3600) % 60;
            var isHourPadNecessary = currentHours < 10;
            var isMinutePadNecessary = currentMinutes < 10;
            var isSecondPadNecessary = currentSeconds < 10;
            CurrentTime = (isHourPadNecessary ? "0" : "") + currentHours + ":"
                          + (isMinutePadNecessary ? "0" : "") + currentMinutes + ":"
                          + (isSecondPadNecessary ? "0" : "") + currentSeconds;
        }
    }
}