using ReactiveUI;

namespace MultiTimer
{
    public class ShellViewModel : ReactiveObject
    {
        public ShellViewModel()
        {
            MultiTimerItems = new[]
            {
                new MultiTimerItem("HomeView", new HomeView { ViewModel = new HomeViewModel()}),
                new MultiTimerItem("About", new About { ViewModel = new AboutViewModel() }), 
            };
        }

        public MultiTimerItem[] MultiTimerItems { get; }
    }
}