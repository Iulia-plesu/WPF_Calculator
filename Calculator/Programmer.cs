using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Programmer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SwitchToStandardCommand { get; }
        public ICommand SwitchToProgrammerCommand { get; }
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

        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue != value)
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
        }

        public string CurrentValueDisplay => ConvertFromDecimal(_currentValue, CurrentBase);

        public string OperationString
        {
            get => _operationString;
            set
            {
                if (_operationString != value)
                {
                    _operationString = value;
                    OnPropertyChanged(nameof(OperationString));
                }
            }
        }

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
            AddDecimalPointCommand = new RelayCommand(AddDecimalPoint);
            ClearCommand = new RelayCommand(Clear);
            BackspaceCommand = new RelayCommand(Backspace);
            ReciprocalCommand = new RelayCommand(Reciprocal);
            SquareCommand = new RelayCommand(Square);
            SquareRootCommand = new RelayCommand(SquareRoot);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            AboutCommand = new RelayCommand(About);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private void MemoryClear(object parameter) => _memory = 0;
        private void MemoryRecall(object parameter) => CurrentValue = _memory;
        private void MemoryAdd(object parameter) => _memory += CurrentValue;
        private void MemorySubtract(object parameter) => _memory -= CurrentValue;
        private void MemoryStore(object parameter) => _memory = CurrentValue;

        private void AppendNumber(object parameter)
        {
            if (parameter is string number)
            {
                if (!IsValidInput(number))
                    return;

                if (_isNewNumber)
                {
                    CurrentValue = ParseNumber(number, CurrentBase);
                    _isNewNumber = false;
                }
                else
                {
                    string currentValueStr = ConvertFromDecimal(CurrentValue, CurrentBase);
                    currentValueStr += number;
                    CurrentValue = ParseNumber(currentValueStr, CurrentBase);
                }
                OperationString += number;
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

        private void AddDecimalPoint(object parameter)
        {
            string currentValueStr = ConvertFromDecimal(CurrentValue, CurrentBase);
            if (!currentValueStr.Contains("."))
            {
                currentValueStr += ".";
                CurrentValue = ParseNumber(currentValueStr, CurrentBase);
                OperationString += ".";
            }
        }

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
                    return input == "0" || input == "1" || input == ".";
                case "OCT":
                    return int.TryParse(input, out int oct) && oct >= 0 && oct <= 7 || input == ".";
                case "DEC":
                    return int.TryParse(input, out int dec) || input == ".";
                case "HEX":
                    return int.TryParse(input, out int hex) || (input.Length == 1 && "ABCDEF.".Contains(input.ToUpper()));
                default:
                    return false;
            }
        }

        private double ConvertFractionalPart(string fractionalPart, string fromBase)
        {
            double result = 0;
            int baseValue = GetBase(fromBase);
            for (int i = 0; i < fractionalPart.Length; i++)
            {
                int digit = Convert.ToInt32(fractionalPart[i].ToString(), baseValue);
                result += digit / Math.Pow(baseValue, i + 1);
            }
            return result;
        }

        private string ConvertFromDecimal(double number, string toBase)
        {
            if (toBase == "DEC")
            {
                return number.ToString();
            }

            int integerPart = (int)number;
            double fractionalPart = number - integerPart;

            string integerResult = Convert.ToString(integerPart, GetBase(toBase));
            string fractionalResult = ConvertFractionalPartToBase(fractionalPart, toBase);

            if (toBase == "HEX")
            {
                integerResult = integerResult.ToUpper();
                fractionalResult = fractionalResult.ToUpper();
            }

            return fractionalResult == "" ? integerResult : $"{integerResult}.{fractionalResult}";
        }

        private string ConvertFractionalPartToBase(double fractionalPart, string toBase)
        {
            if (fractionalPart == 0)
                return "";

            int baseValue = GetBase(toBase);
            string result = "";
            for (int i = 0; i < 10; i++)
            {
                fractionalPart *= baseValue;
                int digit = (int)fractionalPart;
                result += digit.ToString("X");
                fractionalPart -= digit;
                if (fractionalPart == 0)
                    break;
            }
            return result;
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
            if (number.Contains("."))
            {
                string[] parts = number.Split('.');
                double integerPart = Convert.ToInt32(parts[0], GetBase(fromBase));
                double fractionalPart = ConvertFractionalPart(parts[1], fromBase);
                return integerPart + fractionalPart;
            }
            else
            {
                return Convert.ToInt32(number, GetBase(fromBase));
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
                case ".":
                    AddDecimalPoint(null);
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