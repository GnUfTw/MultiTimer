using System.Reactive.Disposables;
using ReactiveUI;

namespace MultiTimer
{
    public partial class CreateNewTimerDialog : ReactiveUserControl<CreateNewTimerViewModel>
    {
        public CreateNewTimerDialog()
        {
            InitializeComponent();
            DataContext = ViewModel;

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel,
                        viewModel => viewModel.Name,
                        view => view.Name.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
