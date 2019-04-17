using ReactiveUI;

namespace MultiTimer
{
    public class CreateNewTimerViewModel : ReactiveObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}