using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _closeMenuTimer;
        private bool _isMouseOverMenu;

        public MainWindow()
        {
            InitializeComponent();

            string lastCalculatorMode = Settings.Default.CalculatorMode;

            if (lastCalculatorMode == "Programmer")
            {
                ProgrammerWindow programmerWindow = new ProgrammerWindow();
                programmerWindow.Show();
                this.Close();
            }
            else
            {
                this.DataContext = new Standard();
            }

            _closeMenuTimer = new DispatcherTimer();
            _closeMenuTimer.Interval = TimeSpan.FromMilliseconds(200);
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

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is Standard standard)
            {
                standard.SaveSettings();
            }
            base.OnClosed(e);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var standard = DataContext as Standard;
            if (standard == null)
                return;

            switch (e.Key)
            {
                case Key.D0: case Key.NumPad0: standard.AppendNumberCommand.Execute("0"); break;
                case Key.D1: case Key.NumPad1: standard.AppendNumberCommand.Execute("1"); break;
                case Key.D2: case Key.NumPad2: standard.AppendNumberCommand.Execute("2"); break;
                case Key.D3: case Key.NumPad3: standard.AppendNumberCommand.Execute("3"); break;
                case Key.D4: case Key.NumPad4: standard.AppendNumberCommand.Execute("4"); break;
                case Key.D5: case Key.NumPad5: standard.AppendNumberCommand.Execute("5"); break;
                case Key.D6: case Key.NumPad6: standard.AppendNumberCommand.Execute("6"); break;
                case Key.D7: case Key.NumPad7: standard.AppendNumberCommand.Execute("7"); break;
                case Key.D8: case Key.NumPad8: standard.AppendNumberCommand.Execute("8"); break;
                case Key.D9: case Key.NumPad9: standard.AppendNumberCommand.Execute("9"); break;

                case Key.OemPeriod:
                case Key.Decimal:
                    standard.AddDecimalPointCommand.Execute(null);
                    break;

                case Key.OemPlus: case Key.Add: standard.SetOperationCommand.Execute("+"); break;
                case Key.OemMinus: case Key.Subtract: standard.SetOperationCommand.Execute("-"); break;
                case Key.Multiply: standard.SetOperationCommand.Execute("×"); break;
                case Key.Divide: case Key.OemQuestion: standard.SetOperationCommand.Execute("÷"); break;
                case Key.Oem5: standard.SetOperationCommand.Execute("%"); break; 

                case Key.Enter: standard.CalculateCommand.Execute(null); break;

                case Key.Back: standard.BackspaceCommand.Execute(null); break;

                case Key.Escape: standard.ClearCommand.Execute(null); break;

                case Key.C when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                    standard.CopyCommand.Execute(null);
                    break;
                case Key.V when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                    standard.PasteCommand.Execute(null);
                    break;
                case Key.X when (e.KeyboardDevice.Modifiers == ModifierKeys.Control):
                    standard.CutCommand.Execute(null);
                    break;
            }
        }
    }
}
