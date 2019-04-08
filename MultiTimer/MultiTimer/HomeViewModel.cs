using System.Diagnostics;
using System.Reactive;
using ReactiveUI;

namespace MultiTimer
{
    public class HomeViewModel : ReactiveObject
    {
        public HomeViewModel()
        {
            AddTimer = ReactiveCommand.Create(() => Debug.WriteLine("Timer created!"));
        }

        public ReactiveCommand<Unit, Unit> AddTimer { get; }
    }
}