using System;
using System.Collections.Generic;
using System.Text;

namespace Behavioral.Visitor.Reflective
{
    using DictType = Dictionary<Type, Action<Expression, StringBuilder>>;

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
            ExpressionPrinter.Print2(e, sb);
            Console.WriteLine(sb);
        }
    }


    public abstract class Expression
    {
    }

    public class DoubleExpression : Expression
    {
        public DoubleExpression(double value)
        {
            Value = value;
        }

        public double Value { get; set; }
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
    }

    public static class ExpressionPrinter
    {
        private static readonly DictType _actions = new()
        {
            [typeof(DoubleExpression)] = (e, sb) =>
            {
                var de = (DoubleExpression)e;
                sb.Append(de.Value);
            },
            [typeof(AdditionExpression)] = (e, sb) =>
            {
                var ae = (AdditionExpression)e;
                sb.Append("(");
                Print(ae.Left, sb);
                sb.Append("+");
                Print(ae.Right, sb);
                sb.Append(")");
            }
        };

        public static void Print2(Expression e, StringBuilder sb) => _actions[e.GetType()](e, sb);

        public static void Print(Expression e, StringBuilder sb)
        {
            if (e is DoubleExpression de)
            {
                sb.Append(de.Value);
            }
            else if (e is AdditionExpression ae)
            {
                sb.Append("(");
                Print(ae.Left, sb);
                sb.Append("+");
                Print(ae.Right, sb);
                sb.Append(")");
            }
            // breaks open-closed principle
            // will work incorrectly on missing case
        }
    }
}
