using System;
using System.Reactive.Disposables;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using MultiTimer.ViewModel;
using ReactiveUI;

namespace MultiTimer.View
{
    public partial class HomeView : ReactiveUserControl<HomeViewModel>
    {
        public HomeView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.BindCommand(ViewModel, 
                        viewModel => viewModel.AddTimer, 
                        view => view.CreateNewTimerButton)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Timers,
                        view => view.Timers.ItemsSource)
                    .DisposeWith(disposable);

                this.ViewModel.Subscribe(async a =>
                {
                   var settings = new TimerSettingsViewModel();
                   // Present user with dialog where they can define timer settings.
                   var dialogView = new CreateNewTimerDialog { ViewModel = settings };
                   var result = await DialogHost.Show(dialogView, "RootDialog");

                   // Create the timer from defined settings if the user didn't cancel the dialog.
                   if (!(bool)(result ?? "NULL")) 
                      return;
                   else
                      ViewModel.OnNext(new TimerDialogOutput(settings.Name, settings.Hours, settings.Minutes, settings.Seconds));
                })
                   .DisposeWith(disposable);
            });
        }
    }
}
