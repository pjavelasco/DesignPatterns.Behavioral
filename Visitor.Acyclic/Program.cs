using System;
using System.Text;

namespace Behavioral.Visitor.Acyclic
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
        }
    }

    public interface IVisitor<TVisitable>
    {
        void Visit(TVisitable obj);
    }

    public interface IVisitor { } // marker interface

    public abstract class Expression
    {
        public virtual void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<Expression> typed)
            {
                typed.Visit(this);
            }
        }
    }

    public class DoubleExpression : Expression
    {
        public DoubleExpression(double value) => Value = value;

        public double Value { get; set; }

        public override void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<DoubleExpression> typed)
            {
                typed.Visit(this);
            }
        }
    }

    public class AdditionExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public AdditionExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public override void Accept(IVisitor visitor)
        {
            if (visitor is IVisitor<AdditionExpression> typed)
            {
                typed.Visit(this);
            }
        }
    }

    public class ExpressionPrinter : IVisitor,
      IVisitor<Expression>,
      IVisitor<DoubleExpression>,
      IVisitor<AdditionExpression>
    {
        readonly StringBuilder _sb = new();

        public void Visit(DoubleExpression de)
        {
            _sb.Append(de.Value);
        }

        public void Visit(AdditionExpression ae)
        {
            _sb.Append("(");
            ae.Left.Accept(this);
            _sb.Append("+");
            ae.Right.Accept(this);
            _sb.Append(")");
        }

        public void Visit(Expression obj)
        {
            // default handler?
        }

        public override string ToString() => _sb.ToString();
    }
}
