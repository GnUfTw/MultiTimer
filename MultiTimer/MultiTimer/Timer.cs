namespace MultiTimer
{
    public class Timer
    {
        public string Name { get; set; }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                UpdateTotalSeconds();
            }
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                UpdateTotalSeconds();
            }
        }

        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                UpdateTotalSeconds();
            }
        }

        public int TotalSeconds { get; private set; }

        private void UpdateTotalSeconds()
        {
            TotalSeconds = Hours * 60 * 60 + Minutes * 60 + Seconds;
        }
    }
}