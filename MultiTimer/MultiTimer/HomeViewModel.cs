using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace MultiTimer
{
    public class HomeViewModel : ReactiveObject
    {
        public HomeViewModel()
        {
            _timers.Connect().Bind(Timers).Subscribe();
            AddTimer = ReactiveCommand.CreateFromTask(CreateNewTimer, null, RxApp.MainThreadScheduler);
            AddTimer.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex));
        }

        private readonly SourceList<TimerViewModel> _timers = new SourceList<TimerViewModel>();
        public IObservableCollection<TimerViewModel> Timers { get; } = new ObservableCollectionExtended<TimerViewModel>();

        public ReactiveCommand<Unit, Unit> AddTimer { get; }

        private async Task CreateNewTimer()
        {
            var dialogView = new CreateNewTimerDialog { ViewModel = new CreateNewTimerViewModel() };
            var result = await DialogHost.Show(dialogView, "RootDialog");
            if (!(bool) (result ?? "NULL")) return;
            var newTimer = new Timer { Name = dialogView.ViewModel.Name };
            var newTimerViewModel = new TimerViewModel(newTimer);
            Debug.WriteLine("Created new timer named " + newTimer.Name);
            Timers.Add(newTimerViewModel);
            Debug.WriteLine("Timer count: " + Timers.Count);
        }
    }
}