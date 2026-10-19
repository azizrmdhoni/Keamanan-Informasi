using System;

namespace Kalkulator
{
    public class CalculatorService
    {
        public double Calculate(double firstNumber, double secondNumber, string operation)
        {
            switch (operation)
            {
                case "+":
                    return firstNumber + secondNumber;
                case "−":
                case "-":
                    return firstNumber - secondNumber;
                case "×":
                case "*":
                case "x":
                    return firstNumber * secondNumber;
                case "÷":
                case "/":
                    if (secondNumber == 0)
                        throw new DivideByZeroException("Cannot divide by zero.");
                    return firstNumber / secondNumber;
                default:
                    return secondNumber;
            }
        }

        public double SquareRoot(double value)
        {
            if (value < 0)
                throw new InvalidOperationException("Invalid input for square root.");
            return Math.Sqrt(value);
        }

        public double Square(double value) => Math.Pow(value, 2);

        public double Sin(double value) => Math.Sin(value * Math.PI / 180.0);

        public double Cos(double value) => Math.Cos(value * Math.PI / 180.0);

        public double Tan(double value)
        {
            if (Math.Abs(value % 180) == 90)
                throw new InvalidOperationException("Invalid input for tangent.");
            return Math.Tan(value * Math.PI / 180.0);
        }

        public double Log(double value)
        {
            if (value <= 0)
                throw new InvalidOperationException("Invalid input for log.");
            return Math.Log10(value);
        }
    }
}