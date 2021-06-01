using System;
using System.Reactive.Disposables;
using MaterialDesignThemes.Wpf;
using ReactiveUI;


namespace MultiTimer
{
    public partial class TimerView : ReactiveUserControl<TimerViewModel>
    {
        public TimerView()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Name,
                        view => view.Name.Text)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.FullTime,
                        view => view.FullTime.Text)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.CurrentTime,
                        view => view.CurrentTime.Text)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.StartTimer,
                        view => view.StartTimer)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsNotRunning,
                        view => view.StartTimer.IsEnabled)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.StopTimer,
                        view => view.StopTimer)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsStopEnabled,
                        view => view.StopTimer.IsEnabled)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RestartTimer,
                        view => view.RestartTimer)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsRunning,
                        view => view.RestartTimer.IsEnabled)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.SaveTimer,
                        view => view.SaveTimer)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsNotPersisted,
                        view => view.SaveTimer.IsEnabled)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.DeleteTimer,
                        view => view.DeleteTimer)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.EditTimer,
                        view => view.EditTimer)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsNotRunning,
                        view => view.EditTimer.IsEnabled)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.TimerProgress,
                        view => view.TimerProgress.Value)
                    .DisposeWith(disposable);


                this.ViewModel.Subscribe(async a =>
                {
                   var viewModel = new TimerSettingsViewModel
                   {
                      Hours=a.Hours, Minutes = a.Minutes, Name = a.Name, Seconds =  a.Seconds
                         
                   };
                   // Present user with dialog where they can define timer settings.
                   var dialogView = new CreateNewTimerDialog { ViewModel = viewModel };
                   var result = await DialogHost.Show(dialogView, "RootDialog");

                   // Create the timer from defined settings if the user didn't cancel the dialog.
                   if (!(bool)(result ?? "NULL"))
                      return;
                   else
                      ViewModel.OnNext(new TimerDialogOutput(viewModel.Name, viewModel.Hours, viewModel.Minutes, viewModel.Seconds));
                })
                   .DisposeWith(disposable);
            });
        }
    }
}
