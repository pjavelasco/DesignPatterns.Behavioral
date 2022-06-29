using System;
using System.Dynamic;
using ImpromptuInterface;
using static System.Console;

namespace Behavioral.NullObject
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            //var log = new ConsoleLog();
            //ILog log = null;
            //var log = new NullLog();
            var log = Null<ILog>.Instance;
            var ba = new BankAccount(log);
            ba.Deposit(100);
            ba.Withdraw(200);
        }
    }

    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
    }

    class ConsoleLog : ILog
    {
        public void Info(string msg) => WriteLine(msg);

        public void Warn(string msg) => WriteLine("WARNING: " + msg);
    }

    public class BankAccount
    {
        private readonly ILog _log;
        private int balance;

        public BankAccount(ILog log) => _log = log;

        public void Deposit(int amount)
        {
            balance += amount;
            // check for null everywhere
            _log?.Info($"Deposited ${amount}, balance is now {balance}");
        }

        public void Withdraw(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                _log?.Info($"Withdrew ${amount}, we have ${balance} left");
            }
            else
            {
                _log?.Warn($"Could not withdraw ${amount} because balance is only ${balance}");
            }
        }
    }

    public sealed class NullLog : ILog
    {
        public void Info(string msg)
        {

        }

        public void Warn(string msg)
        {

        }
    }

    public class Null<T> : DynamicObject where T : class
    {
        public static T Instance
        {
            get
            {
                if (!typeof(T).IsInterface)
                    throw new ArgumentException("I must be an interface type");

                return new Null<T>().ActLike<T>();
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Activator.CreateInstance(binder.ReturnType);
            return true;
        }

        private class Empty { }
    }
}
