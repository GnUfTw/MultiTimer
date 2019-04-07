using ReactiveUI;

namespace MultiTimer
{
    public partial class MainWindow : ReactiveWindow<ShellViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new ShellViewModel();

            this.WhenActivated(disposableRegistration => { });
        }
    }
}
