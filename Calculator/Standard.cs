using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Standard : ICommand, INotifyPropertyChanged
    {

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

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
        public ICommand PercentageCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand MemoryStackCommand { get; }
        public ICommand ClearEntireMemoryCommand { get; }



        private List<double> _memoryStack = new List<double>();
        private double _memory = 0;
        private double _currentValue = 0;
        private double _firstOperand = 0;
        private string _currentOperation = string.Empty;
        private string _operationString = string.Empty;
        private bool _isNewNumber = true;


        public Standard()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");

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
            ClearEntryCommand = new RelayCommand(ClearEntry);
            BackspaceCommand = new RelayCommand(Backspace);
            ReciprocalCommand = new RelayCommand(Reciprocal);
            SquareCommand = new RelayCommand(Square);
            SquareRootCommand = new RelayCommand(SquareRoot);
            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);
            AboutCommand = new RelayCommand(About);
            PercentageCommand = new RelayCommand(CalculatePercentage);
            MemoryStackCommand = new RelayCommand(MemoryStack);
            SwitchToExpressionCommand = new RelayCommand(SwitchToExpression);
            ClearEntireMemoryCommand = new RelayCommand(ClearEntireMemory);

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
                }
            }
        }
        
        public string[] CurrentValueDisplay
        {
            get
            {
                string formattedValue = EnableDigitGrouping
                    ? CurrentValue.ToString("N0", CultureInfo.CurrentCulture)
                    : CurrentValue.ToString(CultureInfo.InvariantCulture);
                return formattedValue.ToCharArray().Select(c => c.ToString()).ToArray();
            }
        }

        public bool EnableDigitGrouping
        {
            get => Settings.Default.EnableDigitGrouping;
            set
            {
                if (Settings.Default.EnableDigitGrouping != value)
                {
                    Settings.Default.EnableDigitGrouping = value;
                    OnPropertyChanged(nameof(EnableDigitGrouping));
                    Settings.Default.Save();
                }
            }
        }



        private string FormatWithDigitGrouping(double value)
        {
            if (EnableDigitGrouping) 
            {
                return value.ToString("N0", CultureInfo.CurrentCulture); 
            }
            else
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private string FormatOperationStringWithDigitGrouping(string operationString)
        {
            var parts = operationString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var formattedParts = new List<string>();

            foreach (var part in parts)
            {
                if (double.TryParse(part, out double number))
                {
                    formattedParts.Add(FormatWithDigitGrouping(number));
                }
                else
                {
                    formattedParts.Add(part);
                }
            }

            return string.Join(" ", formattedParts);
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

                if (number == ".")
                {
                    string currentValueStr = CurrentValue.ToString(CultureInfo.InvariantCulture);
                    if (!currentValueStr.Contains("."))
                    {
                        currentValueStr += ".";
                        CurrentValue = double.Parse(currentValueStr, CultureInfo.InvariantCulture);
                        OperationString += ".";
                    }
                }
                else
                {
                    string currentValueStr = CurrentValue.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", "");
                    currentValueStr += number;
                    CurrentValue = double.Parse(currentValueStr, CultureInfo.InvariantCulture);

                    OperationString = FormatOperationStringWithDigitGrouping(OperationString + number);
                }

                OnPropertyChanged(nameof(CurrentValueDisplay));
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
        
        private void ClearEntireMemory()
        {
            _memoryStack.Clear();
            OnPropertyChanged(nameof(MemoryStack)); 
        }
    

        private void SetOperation(object parameter)
        {
            if (parameter is string operation)
            {
                if (operation == "%")
                {
                    CalculatePercentage(null);
                    return;
                }

                if (string.IsNullOrEmpty(_currentOperation) && _isNewNumber)
                {
                    OperationString = $"{CurrentValue} {operation} ";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_currentOperation))
                    {
                        Calculate(null);
                    }
                    _firstOperand = CurrentValue;
                    OperationString = $"{_firstOperand} {operation} ";
                }

                _currentOperation = operation;
                _isNewNumber = true;
            }
        }

        private void Calculate(object parameter)
        {
            if (!string.IsNullOrEmpty(_currentOperation))
            {
                double secondValue = CurrentValue;
                double result = 0;

                try
                {
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
                            if (secondValue == 0)
                            {
                                MessageBox.Show("Cannot divide by zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            result = _firstOperand / secondValue;
                            break;
                        case "%":
                            result = _firstOperand * (secondValue / 100);
                            break;
                    }

                    CurrentValue = result;
                    _firstOperand = result;
                    secondValue = 0;
                    _currentOperation = string.Empty;
                    _isNewNumber = true;

                    OperationString += $" = {FormatWithDigitGrouping(result)}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangeSign(object parameter) => CurrentValue = -CurrentValue;

        private void AddDecimalPoint(object parameter)
        {
            if (!CurrentValue.ToString(CultureInfo.InvariantCulture).Contains("."))
            {
                CurrentValue = double.Parse(CurrentValue.ToString(CultureInfo.InvariantCulture) + ".");
                OperationString += ".";
            }
        }



        private void Backspace(object parameter)
        {
            if (CurrentValue.ToString().Length > 1)
            {
                CurrentValue = double.Parse(CurrentValue.ToString().Substring(0, CurrentValue.ToString().Length - 1));
                OperationString = OperationString.Substring(0, OperationString.Length - 1);
            }
            else
            {
                CurrentValue = 0;
                OperationString = string.Empty;
            }
        }

        private void ClearEntry(object parameter)
        {
            CurrentValue = 0;
            OperationString = string.Empty;
            _isNewNumber = true;
        }

        private void Clear(object parameter)
        {
            CurrentValue = 0;
            _firstOperand = 0;
            _currentOperation = string.Empty;
            OperationString = string.Empty;
            _isNewNumber = true;
        }

        private void Reciprocal(object parameter) => CurrentValue = 1 / CurrentValue;

        private void Square(object parameter) => CurrentValue *= CurrentValue;

        private void SquareRoot(object parameter) => CurrentValue = Math.Sqrt(CurrentValue);

        private void CalculatePercentage(object parameter)
        {
            if (_currentOperation == "+" || _currentOperation == "-")
            {
                double percentageValue = _firstOperand * (CurrentValue / 100);
                CurrentValue = percentageValue;
            }
            else
            {
                CurrentValue = CurrentValue / 100;
            }

            _isNewNumber = true;
        }

        

        public void SaveSettings()
        {
            Settings.Default.Save();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                if (double.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out double value))
                {
                    CurrentValue = value;
                }
            }
        }

        private void About(object parameter)
        {
            MessageBox.Show("Nume: Iulia\nGrupă: 233", "About");
        }
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => throw new NotImplementedException();
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private Action memoryClear;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Action memoryClear)
        {
            this.memoryClear = memoryClear;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}