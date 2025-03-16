using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Standard : ICommand, INotifyPropertyChanged
    {
        private void SwitchToStandard(object parameter)
        {
            Settings.Default.CalculatorMode = "Standard"; // Salvează modul Standard
            Settings.Default.Save(); // Salvează setările

            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Închide fereastra curentă după ce fereastra nouă a fost afișată
            if (parameter is Window currentWindow)
            {
                currentWindow.Close();
            }
        }

        private void SwitchToProgrammer(object parameter)
        {
            Settings.Default.CalculatorMode = "Programmer"; // Salvează modul Programmer
            Settings.Default.Save(); // Salvează setările

            var programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();

            // Închide fereastra curentă după ce fereastra nouă a fost afișată
            if (parameter is Window currentWindow)
            {
                currentWindow.Close();
            }
        }
        public event EventHandler CanExecuteChanged;
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

        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    OnPropertyChanged(nameof(CurrentValue));
                }
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

        public Standard()
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

            DigitGroupingEnabled = Settings.Default.DigitGroupingEnabled;
        }

        public void SaveSettings()
        {
            Settings.Default.DigitGroupingEnabled = DigitGroupingEnabled;
            Settings.Default.Save();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private bool _digitGroupingEnabled = false;
        public bool DigitGroupingEnabled
        {
            get => _digitGroupingEnabled;
            set
            {
                _digitGroupingEnabled = value;
                OnPropertyChanged(nameof(DigitGroupingEnabled));
                OnPropertyChanged(nameof(CurrentValueDisplay));
            }
        }

        public string CurrentValueDisplay => DigitGroupingEnabled ? FormatWithDigitGrouping(CurrentValue) : CurrentValue.ToString();

        private string FormatWithDigitGrouping(double value)
        {
            return value.ToString("N", CultureInfo.CurrentCulture);
        }

        private void About(object parameter)
        {
            MessageBox.Show("Nume: Pleșu Iulia\nGrupă: 10LF233", "About");
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
                if (_isNewNumber)
                {
                    CurrentValue = double.Parse(number, CultureInfo.InvariantCulture);
                    _isNewNumber = false;
                }
                else
                {
                    string currentValueStr = CurrentValue.ToString(CultureInfo.InvariantCulture);

                    if (number == "." && currentValueStr.Contains("."))
                    {
                        return; 
                    }

                    currentValueStr += number;
                    CurrentValue = double.Parse(currentValueStr, CultureInfo.InvariantCulture);
                }

                OperationString += number;
            }
        }

        private void SetOperation(object parameter)
        {
            if (parameter is string operation)
            {
                if (string.IsNullOrEmpty(_currentOperation) && _isNewNumber)
                {
                    // Dacă apăsăm un operator imediat după egal, începem un nou calcul
                    OperationString = $"{CurrentValue} {operation} ";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_currentOperation))
                    {
                        // Calculează rezultatul intermediar
                        Calculate(null);
                    }
                    // Setează primul operand și operatorul
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
                    }

                    // Actualizează rezultatul
                    CurrentValue = result;
                    _firstOperand = result; // Rezultatul devine primul termen al următoarei operații
                    secondValue = 0;
                    _currentOperation = string.Empty;
                    _isNewNumber = true;

                    // Actualizează afișajul operației
                    OperationString += $" = {result}";
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

        private void Reciprocal(object parameter) => CurrentValue = 1 / CurrentValue;
        private void Square(object parameter) => CurrentValue *= CurrentValue;
        private void SquareRoot(object parameter) => CurrentValue = Math.Sqrt(CurrentValue);

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => throw new NotImplementedException();
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
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