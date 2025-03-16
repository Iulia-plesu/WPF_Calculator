using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Calculator
{
    public partial class MemorySelectionWindow : Window
    {
        public double SelectedValue { get; private set; }

        public MemorySelectionWindow(List<double> memoryStack)
        {
            InitializeComponent();
            MemoryListBox.ItemsSource = memoryStack;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryListBox.SelectedItem is double selectedValue)
            {
                SelectedValue = selectedValue;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Selectați o valoare din listă.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}