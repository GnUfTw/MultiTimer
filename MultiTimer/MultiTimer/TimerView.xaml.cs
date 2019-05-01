using System.Reactive.Disposables;
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
                this.BindCommand(ViewModel,
                        viewModel => viewModel.StopTimer,
                        view => view.StopTimer)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RestartTimer,
                        view => view.RestartTimer)
                    .DisposeWith(disposable);
            });
        }
    }
}
