using Kalkulator;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace Kalkulator
{
    public partial class Form1 : Form
    {
        private double firstNumber = 0;
        private double secondNumber = 0;
        private double result = 0;
        private string operation = "";
        private bool isNewCalculation = false;

        private readonly CalculatorService _calcService = new CalculatorService();

        public Form1()
        {
            InitializeComponent();
        }

        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (txtDisplay.Text == "0" || isNewCalculation)
            {
                txtDisplay.Text = button.Text;
                isNewCalculation = false;
            }
            else
            {
                txtDisplay.Text += button.Text;
            }
        }

        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                firstNumber = parsed;
            else
                firstNumber = double.Parse(txtDisplay.Text);

            operation = button.Text;
            lblHistoryPreview.Text = $"{firstNumber} {operation}";
            txtDisplay.Clear();
            isNewCalculation = false;
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(operation))
                    return;

                if (!double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out secondNumber))
                    secondNumber = double.Parse(txtDisplay.Text);

                result = _calcService.Calculate(firstNumber, secondNumber, operation);

                string record = $"{firstNumber} {operation} {secondNumber} = {result}";
                lblHistoryPreview.Text = record;
                lstHistory.Items.Insert(0, record);

                txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
                operation = "";
                isNewCalculation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (isNewCalculation)
                return;

            if (txtDisplay.Text.Length > 0 && txtDisplay.Text != "0")
            {
                txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
                if (string.IsNullOrEmpty(txtDisplay.Text) || txtDisplay.Text == "-")
                    txtDisplay.Text = "0";
            }
        }

        private void btnPlusMinus_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                val = -val;
                txtDisplay.Text = val.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                val /= 100.0;
                txtDisplay.Text = val.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void ScientificButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            try
            {
                if (!double.TryParse(txtDisplay.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double inputVal))
                    inputVal = double.Parse(txtDisplay.Text);

                double res = 0;
                string formula = "";

                switch (button.Text)
                {
                    case "√x":
                        res = _calcService.SquareRoot(inputVal);
                        formula = $"√({inputVal}) = {res}";
                        break;
                    case "x²":
                        res = _calcService.Square(inputVal);
                        formula = $"sqr({inputVal}) = {res}";
                        break;
                    case "sin":
                        res = _calcService.Sin(inputVal);
                        formula = $"sin({inputVal}°) = {res}";
                        break;
                    case "cos":
                        res = _calcService.Cos(inputVal);
                        formula = $"cos({inputVal}°) = {res}";
                        break;
                    case "tan":
                        res = _calcService.Tan(inputVal);
                        formula = $"tan({inputVal}°) = {res}";
                        break;
                    case "log":
                        res = _calcService.Log(inputVal);
                        formula = $"log({inputVal}) = {res}";
                        break;
                }

                lblHistoryPreview.Text = formula;
                lstHistory.Items.Insert(0, formula);
                txtDisplay.Text = res.ToString(CultureInfo.InvariantCulture);
                isNewCalculation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void btnPi_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = Math.PI.ToString(CultureInfo.InvariantCulture);
            isNewCalculation = false;
        }

        // 8. Clear & Decimal
        private void btnClear_Click(object sender, EventArgs e)
        {
            firstNumber = 0;
            secondNumber = 0;
            result = 0;
            operation = "";
            txtDisplay.Text = "0";
            lblHistoryPreview.Text = "";
            isNewCalculation = false;
        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            if (isNewCalculation)
            {
                txtDisplay.Text = "0.";
                isNewCalculation = false;
                return;
            }

            if (!txtDisplay.Text.Contains("."))
                txtDisplay.Text += ".";
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            lstHistory.Items.Clear();
        }
    }
}