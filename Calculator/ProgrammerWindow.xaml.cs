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
using System.Windows;

namespace Calculator
{
    public partial class ProgrammerWindow : Window
    {
        public ProgrammerWindow()
        {
            InitializeComponent();
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
    }
}