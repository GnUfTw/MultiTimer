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
                        view => view.Name)
                    .DisposeWith(disposable);
            });
        }
    }
}
