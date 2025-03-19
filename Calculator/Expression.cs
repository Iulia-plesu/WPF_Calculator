using Calculator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    public partial class Expression : INotifyPropertyChanged

    {
        private string _expressionStr;

        public string ExpressionStr
        {
            get => _expressionStr ?? "";
            set
            {
                if (Regex.IsMatch(value, @"^[0-9+\-*/.=]*$"))
                {
                    _expressionStr = value;
                    OnPropertyChanged(nameof(ExpressionStr));
                }
                else
                {
                    MessageBox.Show("Caractere invalide. Te rog introdu numere și operatori validi.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public ICommand CalculateCommand { get; }
        public ICommand CloseCommand { get; }

        public event Action<string> CalculationCompleted;

        public Expression()
        {
            CalculateCommand = new RelayCommand(param => CalculateExpression());
            CloseCommand = new RelayCommand(param => CloseWindow());
        }

        

        private void CalculateExpression()
        {
            if (string.IsNullOrWhiteSpace(ExpressionStr) || !ExpressionStr.EndsWith("="))
            {
                MessageBox.Show("Expresia trebuie să conțină '=' la final", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string infix = ExpressionStr.TrimEnd('=');
                string rpn = ConvertToRPN(infix);
                MessageBox.Show($"RPN: {rpn}");

                double result = EvaluateRPN(rpn);
                CalculationCompleted?.Invoke(result.ToString());

                MessageBox.Show($"Rezultat: {result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la evaluare: {ex.Message}\n{ex.StackTrace}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            CalculationCompleted?.Invoke(null);
        }

        private string ConvertToRPN(string infix)
        {
            Dictionary<char, int> precedence = new()
    {
        { '+', 1 }, { '-', 1 },
        { '*', 2 }, { '/', 2 }
    };

            Stack<char> operators = new();
            List<string> output = new();
            string number = "";

            foreach (char c in infix)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    number += c;
                }
                else
                {
                    if (!string.IsNullOrEmpty(number))
                    {
                        output.Add(number);
                        number = "";
                    }

                    if (!precedence.ContainsKey(c))
                    {
                        throw new InvalidOperationException($"Caracter necunoscut: {c}");
                    }

                    while (operators.Count > 0 && precedence[operators.Peek()] >= precedence[c])
                    {
                        output.Add(operators.Pop().ToString());
                    }

                    operators.Push(c);
                }
            }

            if (!string.IsNullOrEmpty(number))
                output.Add(number);

            while (operators.Count > 0)
                output.Add(operators.Pop().ToString());

            return string.Join(" ", output);
        }

        private double EvaluateRPN(string rpn)
        {
            Stack<double> stack = new();
            string[] tokens = rpn.Split(' ');

            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double num))
                {
                    stack.Push(num);
                }
                else
                {
                    if (stack.Count < 2)
                        throw new InvalidOperationException($"Expresie invalidă la token: {token}");

                    double b = stack.Pop();
                    double a = stack.Pop();

                    double result = token switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => b != 0 ? a / b : throw new DivideByZeroException("Împărțire la zero"),
                        _ => throw new InvalidOperationException($"Operator necunoscut: {token}")
                    };
                    stack.Push(result);
                }
            }

            if (stack.Count != 1)
                throw new InvalidOperationException("Expresia RPN este incorectă");

            return stack.Pop();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}