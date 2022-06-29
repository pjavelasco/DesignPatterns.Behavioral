using System;
using System.Text;

namespace Behavioral.Visitor.Dynamic
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
            var sb = new StringBuilder();
            ep.Print((dynamic)e, sb);
            Console.WriteLine(sb);

            // disadvantages:

            // 1) Performance penalty
            // 2) Runtime error on missing visitor
            // 3) Problematic w.r.t. inheritance        }
        }

        public abstract class Expression
        {
        }

        public class DoubleExpression : Expression
        {
            public DoubleExpression(double value) => Value = value;

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

        public class ExpressionPrinter
        {
            public void Print(AdditionExpression ae, StringBuilder sb)
            {
                sb.Append("(");
                Print((dynamic)ae.Left, sb);
                sb.Append("+");
                Print((dynamic)ae.Right, sb);
                sb.Append(")");
            }

            public void Print(DoubleExpression de, StringBuilder sb) => sb.Append(de.Value);
        }
    }
}
