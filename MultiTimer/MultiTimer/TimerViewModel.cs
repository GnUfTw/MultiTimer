using ReactiveUI;

namespace MultiTimer
{
    public class TimerViewModel : ReactiveObject
    {
        public TimerViewModel(Timer timer)
        {
            Name = timer.Name;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set => this.RaiseAndSetIfChanged(ref _hours, value);
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set => this.RaiseAndSetIfChanged(ref _minutes, value);
        }

        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set => this.RaiseAndSetIfChanged(ref _seconds, value);
        }
    }
}