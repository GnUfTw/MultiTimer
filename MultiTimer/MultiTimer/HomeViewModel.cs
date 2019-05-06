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

            try
            {
                // Populate view with existing saved/persisted timers.
                using (var sr = new StreamReader(@"multitimer.config"))
                using (var reader = new JsonTextReader(sr))
                {
                    var deserializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
                    var timers = deserializer.Deserialize<List<Timer>>(reader);
                    if (timers != null)
                    {
                        // Create view model for each timer from config file & add it to view.
                        foreach (var timer in timers)
                        {
                            timer.IsPersisted = true;
                            var timerViewModel = new TimerViewModel(timer);
                            timerViewModel.DeleteTimer.Subscribe(unit => { Timers.Remove(timerViewModel); });
                            Timers.Add(timerViewModel);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // Create the application config file if it doesn't exist already.
                File.Create(@"multitimer.config");
            }
        }

        private readonly SourceList<TimerViewModel> _timers = new SourceList<TimerViewModel>();
        public IObservableCollection<TimerViewModel> Timers { get; } = new ObservableCollectionExtended<TimerViewModel>();

        public ReactiveCommand<Unit, Unit> AddTimer { get; }

        private async Task CreateNewTimer()
        {
            // Present user with dialog where they can define timer settings.
            var dialogView = new CreateNewTimerDialog { ViewModel = new CreateNewTimerViewModel() };
            var result = await DialogHost.Show(dialogView, "RootDialog");

            // Create the timer from defined settings if the user didn't cancel the dialog.
            if (!(bool)(result ?? "NULL")) return;
            var newTimer = new Timer
            {
                Name = dialogView.ViewModel.Name,
                Hours = dialogView.ViewModel.Hours,
                Minutes = dialogView.ViewModel.Minutes,
                Seconds = dialogView.ViewModel.Seconds
            };

            // Create a view model based on the new timer.
            var newTimerViewModel = new TimerViewModel(newTimer);
            newTimerViewModel.DeleteTimer.Subscribe(unit =>
            {
                Timers.Remove(newTimerViewModel);
            });

            // Present newly created timer to the user.
            Timers.Add(newTimerViewModel);
        }
    }
}