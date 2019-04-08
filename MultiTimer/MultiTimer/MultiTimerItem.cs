using System.ComponentModel;
using System.Runtime.CompilerServices;
using DynamicData.Annotations;

namespace MultiTimer
{
    public class MultiTimerItem : INotifyPropertyChanged
    {
        private string _name;
        private object _content;

        public MultiTimerItem(string name, object content)
        {
            _name = name;
            _content = content;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}