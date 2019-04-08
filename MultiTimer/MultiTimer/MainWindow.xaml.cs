using ReactiveUI;

namespace MultiTimer
{
    public partial class MainWindow : ReactiveWindow<ShellViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            var shellvm = new ShellViewModel();
            DataContext = shellvm;
        }
    }
}
