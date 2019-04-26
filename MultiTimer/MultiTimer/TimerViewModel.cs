using ReactiveUI;

namespace MultiTimer
{
    public class TimerViewModel : ReactiveObject
    {
        public TimerViewModel(Timer timer)
        {
            Name = timer.Name;
            var timeString = timer.Hours + ":" + timer.Minutes + ":" + timer.Seconds;
            FullTime = timeString;
            CurrentTime = timeString;
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
    }
}