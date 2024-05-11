using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyCalculator
{
    public partial class Form1 : Form
    {
        List<Button> buttons;
        StringBuilder txtSb=new StringBuilder();
        Stack<double> numbers = new Stack<double>();
        Stack<char> operators = new Stack<char>();
        public Form1()
        {
            InitializeComponent();
            buttons = new List<Button>
            {
                button1,
                button3,
                button4,
                button5,
                button6,
                button7,
                button8,
                button11,
                button10,
                button12,
                button13,
                button14,
                button15,
                button16,
                button17,
                button18,
                button19,
                button20,
                button21,
                button22,
                button23,
                button24,
                button25
            };
            foreach (var item in buttons)
            {
                
                item.Click += new EventHandler(button_Click);
                item.Font= Font = new Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            }

        }

        private void button_Click(object sender, EventArgs e)
        {
            Button bt = sender as Button;
            if(bt != null ) 
            {
                if (bt.Text.Length == 1 && bt.Text != "=")
                {
                    txtSb.Append(bt.Text);
                }
                else if (bt.Text == "删除")
                {
                    txtSb.Remove(txtSb.Length - 1, 1);
                }
                else if (bt.Text == "清除")
                {
                    txtSb.Clear();
                }
                else if (bt.Text == "平方")
                {

                    string valuetxt = textBox1.Text;
                    if (double.TryParse(valuetxt, out double result))
                    {
                        txtSb.Clear();
                        txtSb.Append(result * result);
                    }
                    else
                    {
                        MessageBox.Show("请先求值");
                    }
                }
                else if (bt.Text == "开方")
                {
                    string valuetxt = textBox1.Text;
                    if (double.TryParse(valuetxt, out double result))
                    {
                        txtSb.Clear();                  
                        txtSb.Append(Math.Sqrt(result));
                    }
                    else
                    {
                        MessageBox.Show("请先求值");
                    }
                }
                else if(bt.Text=="=")
                {
                    numbers.Clear();
                    operators.Clear();
                    try
                    {
                        var a = GetSum();
                        
                        numbers.Clear();
                        operators.Clear();
                        if (double.IsNaN(a)) throw new ArgumentException("非法运算，如除数为0或是负数开平方根");
                        txtSb.Clear();
                        txtSb.Append(a);
                    }
                    catch(Exception ee ){ MessageBox.Show(ee.Message); }
                }
            }
            textBox1.Text = txtSb.ToString();
        }

        bool IsOperator(char token)
        {
            return token == '+' || token == '-' || token == '*' || token == '/' || token == '^';
        }

        int Precedence(char operators)
        {
            if (operators == '+' || operators == '-') return 1;
            if (operators == '*' || operators == '/') return 2;
            if (operators == '^') return 3;
            return -1;
        }

        double ApplyOperation(char operation, double b, double a)
        {
            switch (operation)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/': { if (b == 0) { return double.NaN; } return a / b; }
                case '^': return Math.Pow(a, b); 
                default: throw new ArgumentException("Invalid operation");
            }
        }

        double GetResult(double num) 
        {
            double.TryParse(num.ToString("F7"), out double result);
            return result;
        }
        double GetSum() 
        {
            var list = txtSb.ToString();
            bool numFlag = false;
            for (int i = 0; i < list.Length; i++) 
            {
                char token = list[i];
                StringBuilder numsb = new StringBuilder();
                while (i < list.Length && (char.IsDigit(list[i]) || list[i] == '.'))
                {
                    numsb.Append(list[i]);
                    i++;
                }
                if (numsb.Length > 0)
                {
                    if (double.TryParse(numsb.ToString(), out double number))
                    {
                        numbers.Push(number);
                        numsb.Clear();
                        numFlag = true;
                        if (i < list.Length)
                        {
                            token = list[i];
                        }
                        else { break; }
                    }
                    else
                    {
                        throw new ArgumentException("表达式错误");
                    }
                }
                
                if (!numFlag && (token == '-'|| token == '+')) 
                {
                    int temp = i;
                    numsb.Append(token);
                    i++;
                    while (i < list.Length && (char.IsDigit(list[i]) || list[i] == '.'))
                    {
                        numsb.Append(list[i]);
                        i++;
                    }
                    if (numsb.Length > 1)
                    {
                        if (double.TryParse(numsb.ToString(), out double number))
                        {
                            numbers.Push(number);
                            numsb.Clear();
                            numFlag = true;
                            if (i < list.Length)
                            {
                                token = list[i];
                            }
                            else { break; }
                                
                        }
                        else
                        {
                            throw new ArgumentException("表达式错误");
                        }
                    }
                    else 
                    {
                        i = temp;
                    }
                }
                if (token == '(')
                {
                    operators.Push(token);
                    numFlag = false;
                }
                else if (token == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        numbers.Push(ApplyOperation(operators.Pop(), numbers.Pop(), numbers.Pop()));
                    }
                    operators.Pop();
                    numFlag = false;
                }
                else if (IsOperator(token))
                {
                    while (operators.Count > 0 && Precedence(token) <= Precedence(operators.Peek()))
                    {
                        numbers.Push(ApplyOperation(operators.Pop(), numbers.Pop(), numbers.Pop()));
                    }
                    operators.Push(token);
                    numFlag = false;
                }
                else if (i >= list.Length) { }
                else
                {
                    throw new ArgumentException("表达式错误");
                }
            }
            while (operators.Count > 0)
            {
                numbers.Push(ApplyOperation(operators.Pop(), numbers.Pop(), numbers.Pop()));
            }
            return GetResult(numbers.Peek());
        }
    }
}