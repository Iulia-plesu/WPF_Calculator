using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    public partial class MemorySelectionWindow : Window
    {
        public double SelectedValue { get; private set; }
        public bool ClearMemoryRequested { get; private set; } = false;


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemoryListBox.SelectedItem is double selectedValue)
            {
                SelectedValue = selectedValue;
                DialogResult = true;
            }
        }

        private void CleanMemoryButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMemoryRequested = true;
            DialogResult = true;
        }
        private void MemoryRecallButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is double selectedValue)
            {
                SelectedValue = selectedValue;
                DialogResult = true;
            }
        }

        private void MemoryDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is double selectedValue)
            {
                if (MemoryListBox.ItemsSource is List<double> memoryStack)
                {
                    memoryStack.Remove(selectedValue);
                    MemoryListBox.Items.Refresh(); 
                }
            }
        }
        private List<double> originalMemoryValues = new List<double>(); 

        public MemorySelectionWindow(List<double> memoryStack)
        {
            InitializeComponent();
            MemoryListBox.ItemsSource = memoryStack;

            originalMemoryValues = new List<double>(memoryStack);
        }

        private void MemoryAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is double selectedValue)
            {
                if (MemoryListBox.ItemsSource is List<double> memoryStack)
                {
                    int index = memoryStack.IndexOf(selectedValue);
                    if (index != -1)
                    {
                        memoryStack[index] += originalMemoryValues[index]; 
                        MemoryListBox.Items.Refresh();
                    }
                }
            }
        }

        private void MemorySubtractButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is double selectedValue)
            {
                if (MemoryListBox.ItemsSource is List<double> memoryStack)
                {
                    int index = memoryStack.IndexOf(selectedValue);
                    if (index != -1)
                    {
                        memoryStack[index] -= originalMemoryValues[index]; 
                        MemoryListBox.Items.Refresh();
                    }
                }
            }
        }
    }
}