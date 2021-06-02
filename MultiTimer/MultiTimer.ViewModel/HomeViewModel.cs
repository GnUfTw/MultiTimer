using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Newtonsoft.Json;
using ReactiveUI;

namespace MultiTimer.ViewModel {
   public record TimerDialogOutput(string Name, int Hours, int Minutes, int Seconds);


   public class HomeViewModel : ReactiveObject, IObservable<Unit>, IObserver<TimerDialogOutput> {
      private Subject<Unit> subject = new();
      private Subject<TimerDialogOutput> subjectOutput = new();

      public HomeViewModel() {
         _timers.Connect().Bind(Timers).Subscribe();
         AddTimer = ReactiveCommand.CreateFromTask(OpenTimerDialogAndCreate, null, RxApp.MainThreadScheduler);
         AddTimer.ThrownExceptions.Subscribe(ex => Debug.WriteLine(ex));

         try {
            // Populate view with existing saved/persisted timers.
            using (var sr = new StreamReader(@"multitimer.config"))
            using (var reader = new JsonTextReader(sr)) {
               var deserializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
               var timers = deserializer.Deserialize<List<Timer>>(reader);
               if (timers != null) {
                  // Create view model for each timer from config file & add it to view.
                  foreach (var timer in timers) {
                     timer.IsPersisted = true;
                     var timerViewModel = new TimerViewModel(timer);
                     timerViewModel.DeleteTimer.Subscribe(unit => { Timers.Remove(timerViewModel); });
                     Timers.Add(timerViewModel);
                  }
               }
            }
         }
         catch (FileNotFoundException) {
            // Create the application config file if it doesn't exist already.
            File.Create(@"multitimer.config");
         }
      }

      private readonly SourceList<TimerViewModel> _timers = new SourceList<TimerViewModel>();
      public IObservableCollection<TimerViewModel> Timers { get; } = new ObservableCollectionExtended<TimerViewModel>();

      public ReactiveCommand<Unit, Unit> AddTimer { get; }



      private async Task OpenTimerDialogAndCreate() {
         subject.OnNext(Unit.Default);
         await subjectOutput.Take(1).Select(CreateNewTimer).ToTask();


         Unit CreateNewTimer(TimerDialogOutput value) {
            var newTimer = new Timer {
               Name = value.Name,
               Hours = value.Hours,
               Minutes = value.Minutes,
               Seconds = value.Seconds
            };

            // Create a view model based on the new timer.
            var newTimerViewModel = new TimerViewModel(newTimer);
            newTimerViewModel.DeleteTimer.Subscribe(unit => {
               Timers.Remove(newTimerViewModel);
            });

            // Present newly created timer to the user.
            Timers.Add(newTimerViewModel);
            return Unit.Default;
         }
      }

      public IDisposable Subscribe(IObserver<Unit> observer) {
         return subject.Subscribe(observer);
      }

      public void OnCompleted() {
         throw new NotImplementedException();
      }

      public void OnError(Exception error) {
         throw new NotImplementedException();
      }

      public void OnNext(TimerDialogOutput value) {
         subjectOutput.OnNext(value);

      }
   }
}