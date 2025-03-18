using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Calculator
{
    public partial class ProgrammerWindow : Window
    {
        private Programmer _programmer;
        private DispatcherTimer _closeMenuTimer;
        private bool _isMouseOverMenu;

        public ProgrammerWindow()
        {
            InitializeComponent();

            string lastCalculatorMode = Settings.Default.CalculatorMode;

            if (lastCalculatorMode == "Standard")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); 
            }
            else
            {
               _programmer = new Programmer();
                this.DataContext = _programmer;
                DEC.Background = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            }

            _closeMenuTimer = new DispatcherTimer();
            _closeMenuTimer.Interval = TimeSpan.FromMilliseconds(500); 
            _closeMenuTimer.Tick += CloseMenuTimer_Tick;

            SideMenu.MouseEnter += SideMenu_MouseEnter;
            SideMenu.MouseLeave += SideMenu_MouseLeave;
        }

        private void SideMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverMenu = true;
            _closeMenuTimer.Stop(); 
        }

        private void SideMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverMenu = false;
            _closeMenuTimer.Start();
        }

        private void CloseMenuTimer_Tick(object sender, EventArgs e)
        {
            if (!_isMouseOverMenu)
            {
                SideMenu.Margin = new Thickness(-150, 0, 0, 0); 
                _closeMenuTimer.Stop();
            }
        }

        private void ToggleMenu()
        {
            if (SideMenu.Margin.Left == 0)
            {
                SideMenu.Margin = new Thickness(-150, 0, 0, 0);
            }
            else
            {
                SideMenu.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu();
        }

        private void StandardButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ProgrammerButton_Click(object sender, RoutedEventArgs e)
        {
            ProgrammerWindow programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                _programmer.HandleButtonClick(button.Content.ToString());
            }
        }

        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {
            HEX.Background = Brushes.Transparent;
            DEC.Background = Brushes.Transparent;
            OCT.Background = Brushes.Transparent;
            BIN.Background = Brushes.Transparent;

            var clickedButton = sender as Button;
            if (clickedButton != null)
            {
                clickedButton.Background = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            }
            Button_Click(sender, e);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var programmer = DataContext as Programmer;
            if (programmer == null) return;

            string keyPressed = e.Key.ToString();

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                programmer.AppendNumberCommand.Execute((e.Key - Key.D0).ToString());
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                if (Keyboard.IsKeyToggled(Key.NumLock))
                {
                    programmer.AppendNumberCommand.Execute((e.Key - Key.NumPad0).ToString());
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.OemPlus:
                    case Key.Add:
                        programmer.SetOperationCommand.Execute("+");
                        break;
                    case Key.OemMinus:
                    case Key.Subtract:
                        programmer.SetOperationCommand.Execute("-");
                        break;
                    case Key.Multiply:
                        programmer.SetOperationCommand.Execute("×");
                        break;
                    case Key.Divide:
                    case Key.OemQuestion:
                        programmer.SetOperationCommand.Execute("÷");
                        break;

                    case Key.Enter:
                        programmer.CalculateCommand.Execute(null);
                        break;

                    case Key.Back:
                        programmer.BackspaceCommand.Execute(null);
                        break;

                    case Key.Escape:
                        programmer.ClearCommand.Execute(null);
                        break;

                    case Key.C when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                        programmer.CopyCommand.Execute(null);
                        break;
                    case Key.V when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                        programmer.PasteCommand.Execute(null);
                        break;
                    case Key.X when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                        programmer.CutCommand.Execute(null);
                        break;
                }
            }
        }
    }
}
