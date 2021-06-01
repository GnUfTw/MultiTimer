using System.Reflection;
using System.Windows;
using ReactiveUI;
using Splat;

namespace MultiTimer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            Locator.CurrentMutable.Register<IViewFor<HomeViewModel>>(()=> new HomeView());
            Locator.CurrentMutable.Register<IViewFor<AboutViewModel>>(()=> new About());
            Locator.CurrentMutable.Register<IViewFor<TimerSettingsViewModel>>(()=> new EditTimerDialog());
            Locator.CurrentMutable.Register<IViewFor<TimerViewModel>>(()=> new TimerView ());
        }
    }
}
