using ReactiveUI;

namespace MultiTimer
{
    public partial class ShellView : ReactiveWindow<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();
            var shellvm = new ShellViewModel();
            DataContext = shellvm;
        }
    }
}
