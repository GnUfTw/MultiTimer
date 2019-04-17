using System.ComponentModel;
using System.Runtime.CompilerServices;
using DynamicData.Annotations;
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
    }
}