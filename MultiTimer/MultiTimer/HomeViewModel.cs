using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
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

            // Populate view with saved timers.
            var deserializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            try
            {
                using (var sr = new StreamReader(@"multitimer.config"))
                using (var reader = new JsonTextReader(sr))
                {
                    var timers = deserializer.Deserialize<List<Timer>>(reader);

                    // Create view model for each timer from config file & add it to view.
                    foreach (var timer in timers)
                    {
                        var timerViewModel = new TimerViewModel(timer);
                        Timers.Add(timerViewModel);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(@"multitimer.config");
            }
        }

        private readonly SourceList<TimerViewModel> _timers = new SourceList<TimerViewModel>();
        public IObservableCollection<TimerViewModel> Timers { get; } = new ObservableCollectionExtended<TimerViewModel>();

        public ReactiveCommand<Unit, Unit> AddTimer { get; }

        private async Task CreateNewTimer()
        {
            var dialogView = new CreateNewTimerDialog { ViewModel = new CreateNewTimerViewModel() };
            var result = await DialogHost.Show(dialogView, "RootDialog");
            if (!(bool) (result ?? "NULL")) return;
            var newTimer = new Timer
            {
                Name = dialogView.ViewModel.Name,
                Hours = dialogView.ViewModel.Hours,
                Minutes = dialogView.ViewModel.Minutes,
                Seconds = dialogView.ViewModel.Seconds
            };
            var newTimerViewModel = new TimerViewModel(newTimer);
            Timers.Add(newTimerViewModel);
        }
    }
}