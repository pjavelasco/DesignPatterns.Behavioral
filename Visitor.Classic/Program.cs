using System;
using System.Text;

namespace Behavioral.Visitor.Classic
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var e = new AdditionExpression(
                    left: new DoubleExpression(1),
                    right: new AdditionExpression(
                      left: new DoubleExpression(2),
                      right: new DoubleExpression(3)));
            var ep = new ExpressionPrinter();
            ep.Visit(e);
            Console.WriteLine(ep.ToString());

            var calc = new ExpressionCalculator();
            calc.Visit(e);
            Console.WriteLine($"{ep} = {calc.Result}");
        }
    }

    public abstract class Expression
    {
        public abstract void Accept(IExpressionVisitor visitor);
    }

    public class DoubleExpression : Expression
    {
        public DoubleExpression(double value) => Value = value;

        public double Value { get; set; }

        public override void Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    public class AdditionExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public AdditionExpression(Expression left, Expression right)
        {
            Left = left ?? throw new ArgumentNullException(paramName: nameof(left));
            Right = right ?? throw new ArgumentNullException(paramName: nameof(right));
        }

        public override void Accept(IExpressionVisitor visitor) => visitor.Visit(this);
    }

    public interface IExpressionVisitor
    {
        void Visit(DoubleExpression de);
        void Visit(AdditionExpression ae);
    }

    public class ExpressionPrinter : IExpressionVisitor
    {
        private readonly StringBuilder _sb = new();

        public void Visit(DoubleExpression de) => _sb.Append(de.Value);

        public void Visit(AdditionExpression ae)
        {
            _sb.Append("(");
            ae.Left.Accept(this);
            _sb.Append("+");
            ae.Right.Accept(this);
            _sb.Append(")");
        }

        public override string ToString() => _sb.ToString();
    }

    public class ExpressionCalculator : IExpressionVisitor
    {
        public double Result { get; set; }

        // what you really want is int Visit(...)

        public void Visit(DoubleExpression de) => Result = de.Value;

        public void Visit(AdditionExpression ae)
        {
            ae.Left.Accept(this);
            var a = Result;
            ae.Right.Accept(this);
            var b = Result;
            Result = a + b;
        }
    }
}
