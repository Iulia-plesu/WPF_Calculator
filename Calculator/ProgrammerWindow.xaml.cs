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

            // Citește ultimul mod de calculator deschis din setări
            string lastCalculatorMode = Settings.Default.CalculatorMode;

            // Deschide fereastra corespunzătoare
            if (lastCalculatorMode == "Standard")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // Închide fereastra curentă (Programmer)
            }
            else
            {
                // Dacă modul este "Programmer" sau nu este setat, deschide fereastra Programmer
                _programmer = new Programmer();
                this.DataContext = _programmer;
                DEC.Background = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            }

            // Inițializează timer-ul pentru a închide meniul
            _closeMenuTimer = new DispatcherTimer();
            _closeMenuTimer.Interval = TimeSpan.FromMilliseconds(500); // Intervalul în care se va închide meniul
            _closeMenuTimer.Tick += CloseMenuTimer_Tick;

            // Abonare la evenimentele de mouse
            SideMenu.MouseEnter += SideMenu_MouseEnter;
            SideMenu.MouseLeave += SideMenu_MouseLeave;
        }

        private void SideMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverMenu = true;
            _closeMenuTimer.Stop(); // Opriți timerul dacă mouse-ul este pe meniu
        }

        private void SideMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverMenu = false;
            _closeMenuTimer.Start(); // Porniți timerul dacă mouse-ul a părăsit meniul
        }

        private void CloseMenuTimer_Tick(object sender, EventArgs e)
        {
            if (!_isMouseOverMenu)
            {
                // Închide meniul dacă mouse-ul nu este deasupra acestuia
                SideMenu.Margin = new Thickness(-150, 0, 0, 0); // Ascunde meniul lateral
                _closeMenuTimer.Stop();
            }
        }

        // Funcția care va comuta meniul lateral
        private void ToggleMenu()
        {
            if (SideMenu.Margin.Left == 0)
            {
                // Dacă meniul este deschis, îl închidem
                SideMenu.Margin = new Thickness(-150, 0, 0, 0);
            }
            else
            {
                // Dacă meniul este închis, îl deschidem
                SideMenu.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        // Metoda pentru a deschide/închide meniul cu ajutorul butonului
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
                //_programmer.HandleButtonClick(button.Content.ToString());
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

            // Convertim tastele numerice într-un string
            string keyPressed = e.Key.ToString();

            // Verificăm dacă tasta apăsată este un număr
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                programmer.AppendNumberCommand.Execute((e.Key - Key.D0).ToString());
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                // Verificăm dacă NumLock este activ pentru a permite tastarea numerelor
                if (Keyboard.IsKeyToggled(Key.NumLock))
                {
                    programmer.AppendNumberCommand.Execute((e.Key - Key.NumPad0).ToString());
                }
            }
            else
            {
                // Alte funcții, cum ar fi operatorii sau Enter
                switch (e.Key)
                {
                    case Key.OemPeriod:
                    case Key.Decimal:
                        programmer.AddDecimalPointCommand.Execute(null);
                        break;

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
