using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Calculator
{
    public partial class MainWindow : Window
    {
        private double _memory = 0;
        private double _currentValue = 0;
        private string _currentOperation = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_MC_Click(object sender, RoutedEventArgs e) { _memory = 0; }
        private void Button_MR_Click(object sender, RoutedEventArgs e) { Display.Text = _memory.ToString(); }
        private void Button_MPlus_Click(object sender, RoutedEventArgs e) { _memory += _currentValue; }
        private void Button_MMinus_Click(object sender, RoutedEventArgs e) { _memory -= _currentValue; }
        private void Button_MS_Click(object sender, RoutedEventArgs e) { _memory = _currentValue; }
        private void Button_Mv_Click(object sender, RoutedEventArgs e) { Display.Text = _memory.ToString(); }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (Display.Text == "0")
                    Display.Text = button.Content.ToString();
                else
                    Display.Text += button.Content.ToString();
            }
        }

        private void Button_Operator_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                _currentValue = double.Parse(Display.Text);
                _currentOperation = button.Content.ToString();
                Display.Clear();
            }
        }

        private void Button_Equals_Click(object sender, RoutedEventArgs e)
        {
            double result = 0;
            double secondValue = double.Parse(Display.Text);

            switch (_currentOperation)
            {
                case "+":
                    result = _currentValue + secondValue;
                    break;
                case "-":
                    result = _currentValue - secondValue;
                    break;
                case "×":
                    result = _currentValue * secondValue;
                    break;
                case "÷":
                    result = _currentValue / secondValue;
                    break;
            }

            Display.Text = result.ToString();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            Display.Clear();
        }

        private void Button_ChangeSign_Click(object sender, RoutedEventArgs e)
        {
            double value = double.Parse(Display.Text);
            Display.Text = (-value).ToString();
        }

        private void Button_Dot_Click(object sender, RoutedEventArgs e)
        {
            if (!Display.Text.Contains("."))
            {
                Display.Text += ".";
            }
        }

    }
}
