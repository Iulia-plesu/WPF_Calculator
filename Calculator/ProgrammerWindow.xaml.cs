using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System;
using System.Windows.Media.Animation;

namespace Calculator
{
    public partial class ProgrammerWindow : Window
    {
        private double _memory = 0;
        private double _currentValue = 0;
        private string _currentOperation = string.Empty;
        public ProgrammerWindow()
        {
            InitializeComponent();
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2)
            };

            if (SideMenu.Margin.Left < 0)
            {
                animation.To = new Thickness(0, 0, 0, 0);
            }
            else
            {
                animation.To = new Thickness(-150, 0, 0, 0);
            }

            SideMenu.BeginAnimation(MarginProperty, animation);
        }

        private void StandardButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void ProgrammerButton_Click(object sender, RoutedEventArgs e)
        {
            ProgrammerWindow programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key >= Key.D0 && e.Key <= Key.D9)
            //{
            //    Display.Text = Display.Text == "0" ? ((int)e.Key - (int)Key.D0).ToString() : Display.Text + ((int)e.Key - (int)Key.D0).ToString();
            //}
            //else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            //{
            //    Display.Text = Display.Text == "0" ? (e.Key.ToString().Substring(e.Key.ToString().Length - 1)) : Display.Text + (e.Key.ToString().Substring(e.Key.ToString().Length - 1));
            //}
            //else if (e.Key == Key.Add || e.Key == Key.Subtract || e.Key == Key.Multiply || e.Key == Key.Divide || e.Key == Key.OemPlus || e.Key == Key.OemMinus || e.Key == Key.OemQuestion)
            //{
            //    string operatorSymbol = string.Empty;

            //    if (e.Key == Key.Add || e.Key == Key.OemPlus) operatorSymbol = "+";
            //    if (e.Key == Key.Subtract || e.Key == Key.OemMinus) operatorSymbol = "-";
            //    if (e.Key == Key.Multiply) operatorSymbol = "×";
            //    if (e.Key == Key.Divide) operatorSymbol = "÷";
            //    if (e.Key == Key.OemQuestion) operatorSymbol = "%";

            //    _currentValue = double.Parse(Display.Text);
            //    _currentOperation = operatorSymbol;
            //    Display.Clear();
            //}
            //else if (e.Key == Key.Enter || e.Key == Key.Return)
            //{
            //    Button_Equals_Click(this.FindName("Button_Equals") as Button, null);
            //}
            //else if (e.Key == Key.Back)
            //{
            //    if (Display.Text.Length > 1)
            //        Display.Text = Display.Text.Substring(0, Display.Text.Length - 1);
            //    else
            //        Display.Text = "0";
            //}
            //else if (e.Key == Key.Escape)
            //{
            //    Button_Clear_Click(this.FindName("Button_Clear") as Button, null);
            //}
        }

        private void Button_Logic_Click(object sender, RoutedEventArgs e)
        {
            // TODO: AND, OR, XOR, NOT
        }

        private void Button_Shift_Click(object sender, RoutedEventArgs e)
        {
            // TODO: <<, >>, ROL, ROR
        }

        private void Button_Hex_Click(object sender, RoutedEventArgs e)
        {
            // TODO: HEXA A-F, 0-5
        }

        private void Button_Base_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string baseType = button.Content.ToString();
                int value = int.Parse(Display.Text);

                switch (baseType)
                {
                    case "HEX":
                        HEXDisplay.Text = Convert.ToString(value, 16).ToUpper();
                        DECDisplay.Text = value.ToString();
                        OCTDisplay.Text = Convert.ToString(value, 8);
                        BINDisplay.Text = Convert.ToString(value, 2);
                        break;
                    case "DEC":
                        HEXDisplay.Text = Convert.ToString(value, 16).ToUpper();
                        DECDisplay.Text = value.ToString();
                        OCTDisplay.Text = Convert.ToString(value, 8);
                        BINDisplay.Text = Convert.ToString(value, 2);
                        break;
                    case "OCT":
                        HEXDisplay.Text = Convert.ToString(value, 16).ToUpper();
                        DECDisplay.Text = value.ToString();
                        OCTDisplay.Text = Convert.ToString(value, 8);
                        BINDisplay.Text = Convert.ToString(value, 2);
                        break;
                    case "BIN":
                        HEXDisplay.Text = Convert.ToString(value, 16).ToUpper();
                        DECDisplay.Text = value.ToString();
                        OCTDisplay.Text = Convert.ToString(value, 8);
                        BINDisplay.Text = Convert.ToString(value, 2);
                        break;
                }
            }
        }
    }
}