using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace PowerCalc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MathDecoratorAttribute : Attribute
    {
        public string Description { get; }
        public MathDecoratorAttribute(string desc) => Description = desc;
    }

    public class Calculator : Form
    {
        TextBox display = new TextBox();
        PrivateFontCollection fontCollection = new PrivateFontCollection();
        Font sourceCodeFont = SystemFonts.DefaultFont;
        Font vt323Font = SystemFonts.DefaultFont;
        Font buttonFont = SystemFonts.DefaultFont;

        string input = "", operand1 = "", operand2 = "";
        char operation;

        public Calculator()
        {
            this.Text = "PowerCalc";
            this.Size = new Size(400, 540);
            this.BackColor = Color.Black;

            LoadCustomFonts();
            SetupDisplay();
            SetupCalcButtons();
        }

        void LoadCustomFonts()
        {
            try
            {
                fontCollection.AddFontFile("Fonts/SourceCodePro-Regular.ttf");
                fontCollection.AddFontFile("Fonts/VT323-Regular.ttf");
                sourceCodeFont = new Font(fontCollection.Families[0], 20);
                vt323Font = new Font(fontCollection.Families[1], 24);
                buttonFont = vt323Font;
            }
            catch
            {
                MessageBox.Show("Custom fonts not found. Using system defaults.");
            }
        }

        void SetupDisplay()
        {
            display.ReadOnly = true;
            display.Dock = DockStyle.Top;
            display.Font = vt323Font;
            display.BackColor = Color.Black;
            display.ForeColor = Color.LimeGreen;
            display.Height = 50;
            this.Controls.Add(display);
        }

        void SetupCalcButtons()
        {
            string[] buttons = { "7", "8", "9", "/", "4", "5", "6", "*", "1", "2", "3", "-", "0", "C", "=", "+" };
            int x = 10, y = 70;

            foreach (string b in buttons)
            {
                var btn = new Button
                {
                    Text = b,
                    Size = new Size(60, 40),
                    Location = new Point(x, y),
                    Font = buttonFont,
                    BackColor = Color.Black,
                    ForeColor = Color.LimeGreen
                };
                btn.Click += (s, e) => HandleClick(b);
                this.Controls.Add(btn);

                x += 70;
                if (x > 220) { x = 10; y += 50; }
            }
        }

        void HandleClick(string value)
        {
            if (value == "C")
            {
                input = operand1 = operand2 = "";
                display.Text = "";
            }
            else if (value == "=")
            {
                operand2 = input;
                if (double.TryParse(operand1, out double num1) && double.TryParse(operand2, out double num2))
                {
                    double result = operation switch
                    {
                        '+' => Add(num1, num2),
                        '-' => Subtract(num1, num2),
                        '*' => Multiply(num1, num2),
                        '/' => num2 != 0 ? Divide(num1, num2) : 0,
                        _ => 0
                    };
                    display.Text = result.ToString();
                    input = result.ToString();
                }
                else
                {
                    display.Text = "Error";
                }
            }
            else if ("+-*/".Contains(value))
            {
                operand1 = input;
                operation = value[0];
                input = "";
            }
            else
            {
                input += value;
                display.Text = input;
            }
        }

        [MathDecorator("Adds two numbers.")]
        public double Add(double a, double b) => a + b;
        [MathDecorator("Subtracts second number from first.")]
        public double Subtract(double a, double b) => a - b;
        [MathDecorator("Multiplies two numbers.")]
        public double Multiply(double a, double b) => a * b;
        [MathDecorator("Divides first number by second.")]
        public double Divide(double a, double b) => b != 0 ? a / b : 0;
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Calculator());
        }
    }
}
