using System;
using System.Text;

namespace Behavioral.Visitor.Intrusive
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
            var sb = new StringBuilder();
            e.Print(sb);
            Console.WriteLine(sb);
        }
    }

    public abstract class Expression
    {
        // adding a new operation
        public abstract void Print(StringBuilder sb);
    }

    public class DoubleExpression : Expression
    {
        private readonly double _value;

        public DoubleExpression(double value) => _value = value;

        public override void Print(StringBuilder sb) => sb.Append(_value);
    }

    public class AdditionExpression : Expression
    {
        private readonly Expression _left;
        private readonly Expression _right;

        public AdditionExpression(Expression left, Expression right)
        {
            _left = left ?? throw new ArgumentNullException(paramName: nameof(left));
            _right = right ?? throw new ArgumentNullException(paramName: nameof(right));
        }

        public override void Print(StringBuilder sb)
        {
            sb.Append(value: "(");
            _left.Print(sb);
            sb.Append(value: "+");
            _right.Print(sb);
            sb.Append(value: ")");
        }
    }
}
