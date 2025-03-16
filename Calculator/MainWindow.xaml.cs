using System.Windows;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new Standard();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is Standard standard)
            {
                standard.SaveSettings();
            }
            base.OnClosed(e);
        }
    }
}