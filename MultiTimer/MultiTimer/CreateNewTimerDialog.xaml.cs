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
                        view => view.Name)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        viewModel => viewModel.Hours,
                        view => view.Hours.Text,
                        IntToStringConverterFunc,
                        StringToIntConverterFunc)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        viewModel => viewModel.Minutes,
                        view => view.Minutes.Text,
                        IntToStringConverterFunc,
                        StringToIntConverterFunc)
                    .DisposeWith(disposable);
                this.Bind(ViewModel,
                        viewModel => viewModel.Seconds,
                        view => view.Seconds.Text,
                        IntToStringConverterFunc,
                        StringToIntConverterFunc)
                    .DisposeWith(disposable);
            });
        }

        private string IntToStringConverterFunc(int input)
        {
            return input.ToString();
        }

        private int StringToIntConverterFunc(string input)
        {
            int.TryParse(input, out var output);
            return output;
        }
    }
}
