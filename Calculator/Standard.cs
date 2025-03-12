using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Standard : ICommand, INotifyPropertyChanged
    {
        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        // Comenzi pentru meniul lateral
        public ICommand SwitchToStandardCommand { get; }
        public ICommand SwitchToProgrammerCommand { get; }
        public ICommand ToggleMenuCommand { get; }

        // Comenzi pentru butoanele calculatorului
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

        // Câmpuri și proprietăți
        private double _memory = 0;
        private double _currentValue = 0;
        private double _firstOperand = 0;
        private string _currentOperation = string.Empty;
        private string _operationString = string.Empty;
        private bool _isNewNumber = true;

        // Proprietatea CurrentValue
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

        // Proprietatea OperationString
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
            // Inițializăm comenzile
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
        }

        // Metoda pentru notificarea schimbării proprietății
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Metode pentru comenzile meniului lateral
        private void SwitchToStandard(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void SwitchToProgrammer(object parameter)
        {
            var programmerWindow = new ProgrammerWindow();
            programmerWindow.Show();
            Application.Current.Windows[0]?.Close();
        }

        private void ToggleMenu(object parameter)
        {
            if (parameter is Window window && window.FindName("SideMenu") is Border sideMenu)
            {
                sideMenu.Margin = sideMenu.Margin.Left < 0 ? new Thickness(0, 0, 0, 0) : new Thickness(-150, 0, 0, 0);
            }
        }

        // Metode pentru comenzile calculatorului
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
                    CurrentValue = double.Parse(number);
                    _isNewNumber = false;
                }
                else
                {
                    CurrentValue = double.Parse(CurrentValue.ToString() + number);
                }
                OperationString += number; // Adăugăm cifra la operație
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
                        Calculate(null); // Calculează rezultatul intermediar
                    }
                    _firstOperand = CurrentValue;
                    _currentOperation = operation;
                    _isNewNumber = true;
                    OperationString += $" {operation} "; // Adăugăm operatorul la operație
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
                OperationString += $" = {result}"; // Adăugăm rezultatul final la operație
            }
        }

        private void ChangeSign(object parameter) => CurrentValue = -CurrentValue;
        private void AddDecimalPoint(object parameter)
        {
            if (!CurrentValue.ToString().Contains("."))
            {
                CurrentValue = double.Parse(CurrentValue.ToString() + ".");
                OperationString += "."; // Adăugăm punctul zecimal la operație
            }
        }

        private void Clear(object parameter)
        {
            CurrentValue = 0;
            _firstOperand = 0;
            _currentOperation = string.Empty;
            OperationString = string.Empty; // Resetăm operația
            _isNewNumber = true;
        }

        private void Backspace(object parameter)
        {
            if (CurrentValue.ToString().Length > 1)
            {
                CurrentValue = double.Parse(CurrentValue.ToString().Substring(0, CurrentValue.ToString().Length - 1));
                OperationString = OperationString.Substring(0, OperationString.Length - 1); // Ștergem ultimul caracter din operație
            }
            else
            {
                CurrentValue = 0;
                OperationString = string.Empty; // Resetăm operația dacă nu mai sunt cifre
            }
        }

        private void Reciprocal(object parameter) => CurrentValue = 1 / CurrentValue;
        private void Square(object parameter) => CurrentValue *= CurrentValue;
        private void SquareRoot(object parameter) => CurrentValue = Math.Sqrt(CurrentValue);

        // Implementarea ICommand
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => throw new NotImplementedException();
    }

    // Implementare simplă a RelayCommand
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