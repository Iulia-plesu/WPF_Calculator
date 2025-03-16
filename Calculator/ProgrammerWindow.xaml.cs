using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Calculator
{
    public partial class ProgrammerWindow : Window
    {
        private Programmer _programmer;

        public ProgrammerWindow()
        {
            InitializeComponent();
            _programmer = new Programmer();
            this.DataContext = _programmer;
            DEC.Background = new SolidColorBrush(Color.FromRgb(102, 102, 102));
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
    }
}