using System;

namespace MultiTimer
{
    public class Timer : IEquatable<Timer>
    {
        public string Name { get; set; }
        public bool IsPersisted { get; set; } = false;

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

        public bool Equals(Timer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Timer) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}