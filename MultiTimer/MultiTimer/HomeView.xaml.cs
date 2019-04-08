using System.Reactive.Disposables;
using ReactiveUI;

namespace MultiTimer
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
            });
        }
    }
}
