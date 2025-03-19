using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Programmer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<double> _memoryStack = new List<double>();
        public ICommand SwitchToStandardCommand { get; }
        public ICommand SwitchToProgrammerCommand { get; }
        public ICommand SwitchToExpressionCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand MemoryClearCommand { get; }
        public ICommand MemoryRecallCommand { get; }
        public ICommand MemoryAddCommand { get; }
        public ICommand MemorySubtractCommand { get; }
        public ICommand MemoryStoreCommand { get; }
        public ICommand AppendNumberCommand { get; }
        public ICommand SetOperationCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ChangeSignCommand { get; }
        public ICommand AddDecimalPointCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ReciprocalCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand SquareRootCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand AboutCommand { get; }

        public ICommand MemoryStackCommand { get; }

        private double _memory = 0;
        private double _currentValue = 0;
        private double _firstOperand = 0;
        private string _currentOperation = string.Empty;
        private string _operationString = string.Empty;
        private bool _isNewNumber = true;
        private string _currentBase = "DEC";

        public string HexDisplay => ConvertFromDecimal(CurrentValue, "HEX");
        public string DecDisplay => ConvertFromDecimal(CurrentValue, "DEC");
        public string OctDisplay => ConvertFromDecimal(CurrentValue, "OCT");
        public string BinDisplay => ConvertFromDecimal(CurrentValue, "BIN");

        public string CurrentBase
        {
            get => _currentBase;
            set
            {
                if (_currentBase != value)
                {
                    _currentBase = value;
                    OnPropertyChanged(nameof(CurrentBase));
                    OnPropertyChanged(nameof(CurrentValueDisplay));  
                    Clear(null);  
                }
            }
        }


        public Programmer()
        {
            SwitchToStandardCommand = new RelayCommand(SwitchToStandard);
            SwitchToProgrammerCommand = new RelayCommand(SwitchToProgrammer);
            SwitchToExpressionCommand = new RelayCommand(SwitchToExpression);
            ToggleMenuCommand = new RelayCommand(ToggleMenu);
            MemoryClearCommand = new RelayCommand(MemoryClear);
            MemoryRecallCommand = new RelayCommand(MemoryRecall);
            MemoryAddCommand = new RelayCommand(MemoryAdd);
            MemorySubtractCommand = new RelayCommand(MemorySubtract);
            MemoryStoreCommand = new RelayCommand(MemoryStore);
            AppendNumberCommand = new RelayCommand(AppendNumber);
            SetOperationCommand = new RelayCommand(SetOperation);
            CalculateCommand = new RelayCommand(Calculate);
            ChangeSignCommand = new RelayCommand(ChangeSign);
            ClearCommand = new RelayCommand(Clear);
            BackspaceCommand = new RelayCommand(Backspace);
            ReciprocalCommand = new RelayCommand(Reciprocal);
            SquareCommand = new RelayCommand(Square);
            SquareRootCommand = new RelayCommand(SquareRoot);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            AboutCommand = new RelayCommand(About);
            MemoryStackCommand = new RelayCommand(MemoryStack);

            CultureInfo.CurrentCulture = new CultureInfo("en-GB");
        }

        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged(nameof(CurrentValue));
                OnPropertyChanged(nameof(CurrentValueDisplay));

                OnPropertyChanged(nameof(HexDisplay));
                OnPropertyChanged(nameof(DecDisplay));
                OnPropertyChanged(nameof(OctDisplay));
                OnPropertyChanged(nameof(BinDisplay));
            }
        }

        public string CurrentValueDisplay
        {
            get
            {
                return ConvertFromDecimal(CurrentValue, CurrentBase); // Ensure CurrentBase is passed here
            }
        }



        public string OperationString
        {
            get => _operationString;
            set
            {
                _operationString = value;
                OnPropertyChanged(nameof(OperationString));
            }
        }

        private void AppendNumber(object parameter)
        {
            if (parameter is string number)
            {
                if (_isNewNumber)
                {
                    CurrentValue = 0;
                    _isNewNumber = false;
                }

                if (!IsValidInput(number))
                {
                    return; 
                }

                //string currentValueStr = CurrentValue.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", "") + number;
                string currentValueStr = ConvertFromDecimal(CurrentValue, CurrentBase) + number;

                CurrentValue = ParseNumber(currentValueStr, CurrentBase);

                OperationString = FormatOperationStringWithDigitGrouping(OperationString + number);
                OnPropertyChanged(nameof(CurrentValueDisplay));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string FormatWithDigitGrouping(string value, string baseName)
        {
            int groupSize = 0;

            switch (baseName)
            {
                case "BIN":
                    groupSize = 4;
                    break;
                case "OCT":
                    groupSize = 3; 
                    break;
                case "DEC":
                    groupSize = 3; 
                    break;
                case "HEX":
                    groupSize = 4;
                    break;
            }

            if (string.IsNullOrEmpty(value)) return value;

            string cleanValue = value.Replace(".", "");

            string integerPart = cleanValue.Split('.')[0];
            var grouped = new List<string>();
            for (int i = integerPart.Length; i > 0; i -= groupSize)
            {
                int groupStart = Math.Max(i - groupSize, 0);
                grouped.Add(integerPart.Substring(groupStart, i - groupStart));
            }
            grouped.Reverse(); 
            string groupedIntegerPart = string.Join(",", grouped);

            return groupedIntegerPart;
        }


        private string FormatOperationStringWithDigitGrouping(string operationString)
        {
            var parts = operationString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var formattedParts = new List<string>();

            foreach (var part in parts)
            {
                if (double.TryParse(part, out double number))
                {
                    formattedParts.Add(FormatWithDigitGrouping(number.ToString(), CurrentBase));
                }
                else
                {
                    formattedParts.Add(part);
                }
            }

            return string.Join(" ", formattedParts);
        }

        private void SwitchToStandard(object parameter)
        {
            Settings.Default.CalculatorMode = "Standard"; 
            Settings.Default.Save();

            var mainWindow = new MainWindow();
            mainWindow.Show();

            if (parameter is Window currentWindow)
            {
                currentWindow.Close();
            }
        }

        private void SwitchToProgrammer(object parameter)
        {
            Settings.Default.CalculatorMode = "Programmer"; 
            Settings.Default.Save(); 

            var programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();

            if (parameter is Window currentWindow)
            {
                currentWindow.Close();
            }
        }
        private void SwitchToExpression(object parameter)
        {

            var expressionWindow = new ExpressionWindow();
            expressionWindow.Show();

        }
        private void ToggleMenu(object parameter)
        {
            if (parameter is Window window && window.FindName("SideMenu") is Border sideMenu)
            {
                sideMenu.Margin = sideMenu.Margin.Left < 0 ? new Thickness(0, 0, 0, 0) : new Thickness(-150, 0, 0, 0);
            }
        }

        private void Cut(object parameter)
        {
            Clipboard.SetText(CurrentValue.ToString());
            CurrentValue = 0;
        }

        private void Copy(object parameter)
        {
            Clipboard.SetText(CurrentValue.ToString());
        }

        private void Paste(object parameter)
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                if (double.TryParse(text, out double value))
                {
                    CurrentValue = value;
                }
            }
        }
        private void MemoryClear() => _memory = 0;

        private void MemoryRecall(object parameter)
        {
            if (_memoryStack.Count == 0)
            {
                MessageBox.Show("Stiva de memorie este goală.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectionWindow = new MemorySelectionWindow(_memoryStack);
            if (selectionWindow.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(_currentOperation))
                {
                    CurrentValue = selectionWindow.SelectedValue;
                    OperationString = selectionWindow.SelectedValue.ToString();
                }
                else
                {
                    CurrentValue = selectionWindow.SelectedValue;
                    OperationString += selectionWindow.SelectedValue.ToString();
                }
            }
        }

        private void MemoryAdd(object parameter)
        {
            _memory += CurrentValue;
            _memoryStack.Add(CurrentValue);
        }

        private void MemorySubtract(object parameter) => _memory -= CurrentValue;

        private void MemoryStore(object parameter) => _memory = CurrentValue;

        private void MemoryStack(object parameter)
        {
            if (_memoryStack.Count == 0)
            {
                MessageBox.Show("Stiva de memorie este goală.", "Stiva de Memorie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var selectionWindow = new MemorySelectionWindow(_memoryStack);
            if (selectionWindow.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(_currentOperation))
                {
                    CurrentValue = selectionWindow.SelectedValue;
                    OperationString = selectionWindow.SelectedValue.ToString();
                }
                else
                {
                    CurrentValue = selectionWindow.SelectedValue;
                    OperationString += selectionWindow.SelectedValue.ToString();
                }
            }
        }
        
        private void SetOperation(object parameter)
        {
            if (parameter is string operation)
            {
                if (!_isNewNumber)
                {
                    if (!string.IsNullOrEmpty(_currentOperation))
                    {
                        Calculate(null);
                    }
                    _firstOperand = CurrentValue;
                    _currentOperation = operation;
                    _isNewNumber = true;
                    OperationString += $" {operation} ";
                }
            }
        }

        private void Calculate(object parameter)
        {
            if (!string.IsNullOrEmpty(_currentOperation))
            {
                double secondValue = CurrentValue;
                double result = 0;

                switch (_currentOperation)
                {
                    case "+":
                        result = _firstOperand + secondValue;
                        break;
                    case "-":
                        result = _firstOperand - secondValue;
                        break;
                    case "×":
                        result = _firstOperand * secondValue;
                        break;
                    case "÷":
                        result = _firstOperand / secondValue;
                        break;
                }

                CurrentValue = result;
                _firstOperand = result;
                _currentOperation = string.Empty;
                _isNewNumber = true;
                OperationString += $" = {ConvertFromDecimal(result, CurrentBase)}";
            }
        }

        private void ChangeSign(object parameter) => CurrentValue = -CurrentValue;


        private void Clear(object parameter)
        {
            CurrentValue = 0;
            _firstOperand = 0;
            _currentOperation = string.Empty;
            OperationString = string.Empty;
            _isNewNumber = true;
        }

        private void Backspace(object parameter)
        {
            string currentValueStr = ConvertFromDecimal(CurrentValue, CurrentBase);
            if (currentValueStr.Length > 1)
            {
                currentValueStr = currentValueStr.Substring(0, currentValueStr.Length - 1);
                CurrentValue = ParseNumber(currentValueStr, CurrentBase);
                OperationString = OperationString.Substring(0, OperationString.Length - 1);
            }
            else
            {
                CurrentValue = 0;
                OperationString = string.Empty;
            }
        }


        private void Reciprocal(object parameter) => CurrentValue = 1 / CurrentValue;
        private void Square(object parameter) => CurrentValue *= CurrentValue;
        private void SquareRoot(object parameter) => CurrentValue = Math.Sqrt(CurrentValue);

        private bool IsValidInput(string input)
        {
            switch (CurrentBase)
            {
                case "BIN":
                    return "01".Contains(input) || input == ".";
                case "OCT":
                    return "01234567".Contains(input) || input == ".";  
                case "DEC":
                    return "0123456789".Contains(input) || input == "."; 
                case "HEX":
                    return "0123456789ABCDEF".Contains(input.ToUpper()) || input == "."; 
                default:
                    return false;
            }
        }

        private string ConvertFromDecimal(double number, string toBase)
        {
            if (toBase == "DEC")
            {
                return FormatWithDigitGrouping(number.ToString(), "DEC");  
            }

            int integerPart = (int)number;

            string integerResult = Convert.ToString(integerPart, GetBase(toBase));

            if (toBase == "HEX")
            {
                integerResult = integerResult.ToUpper();
            }

            // Pass the current base name to the formatting function
            return FormatWithDigitGrouping(integerResult, toBase);
        }


        private int GetBase(string baseName)
        {
            switch (baseName)
            {
                case "BIN":
                    return 2;
                case "OCT":
                    return 8;
                case "DEC":
                    return 10;
                case "HEX":
                    return 16;
                default:
                    return 10;
            }
        }

        private double ParseNumber(string number, string fromBase)
        {
            int baseValue = GetBase(fromBase);
            
            return ConvertIntegerPart(number, baseValue);
            
        }

        private double ConvertIntegerPart(string integerPart, int baseValue)
        {
            double result = 0;
            int length = integerPart.Length;

            for (int i = 0; i < length; i++)
            {
                char digitChar = integerPart[i];
                int digitValue = GetDigitValue(digitChar);

                result += digitValue * Math.Pow(baseValue, length - 1 - i);
            }

            return result;
        }


        private int GetDigitValue(char digitChar)
        {
            // Handle digit values (0-9)
            if (char.IsDigit(digitChar))
            {
                return digitChar - '0';
            }
            // Handle hexadecimal letters (A-F)
            else if (char.IsLetter(digitChar))
            {
                char upperChar = char.ToUpper(digitChar); // Make sure it's uppercase for consistency
                if (upperChar >= 'A' && upperChar <= 'F')
                {
                    return upperChar - 'A' + 10;
                }
                else
                {
                    throw new FormatException($"Invalid hexadecimal character '{digitChar}' in number.");
                }
            }
            else
            {
                throw new FormatException($"Invalid character '{digitChar}' in number.");
            }
        }




        public void HandleButtonClick(string buttonContent)
        {
            switch (buttonContent)
            {
                case "HEX":
                case "DEC":
                case "OCT":
                case "BIN":
                    CurrentBase = buttonContent;
                    break;
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "F":
                    if (CurrentBase == "HEX")
                        AppendNumber(buttonContent);
                    break;
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (IsValidInput(buttonContent))
                        AppendNumber(buttonContent);
                    break;
                case "+":
                case "-":
                case "×":
                case "÷":
                    SetOperation(buttonContent);
                    break;
                case "=":
                    Calculate(null);
                    break;
                case "CE":
                    Clear(null);
                    break;
                case "⌫":
                    Backspace(null);
                    break;
                case "±":
                    ChangeSign(null);
                    break;
                default:
                    break;
            }
        }

        private void About(object parameter)
        {
            MessageBox.Show("Nume: Iulia\nGrupă: 233", "About");
        }
    }
}