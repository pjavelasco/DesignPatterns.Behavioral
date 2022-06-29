using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Behavioral.Command.Composite
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var ba = new BankAccount();
            var cmdDeposit = new BankAccountCommand(ba, BankAccountCommand.Action.Deposit, 100);
            var cmdWithdraw = new BankAccountCommand(ba, BankAccountCommand.Action.Withdraw, 1000);
            cmdDeposit.Call();
            cmdWithdraw.Call();
            WriteLine(ba);
            cmdWithdraw.Undo();
            cmdDeposit.Undo();
            WriteLine(ba);

            var from = new BankAccount();
            from.Deposit(100);
            var to = new BankAccount();

            var mtc = new MoneyTransferCommand(from, to, 1000);
            mtc.Call();

            // Deposited $100, balance is now 100
            // balance: 100
            // balance: 0

            WriteLine(from);
            WriteLine(to);
        }
    }

    public class BankAccount
    {
        private int _balance;
        private readonly int _overdraftLimit = -500;

        public BankAccount(int balance = 0) => _balance = balance;

        public void Deposit(int amount)
        {
            _balance += amount;
            Console.WriteLine($"Deposited ${amount}, balance is now {_balance}");
        }

        public bool Withdraw(int amount)
        {
            if (_balance - amount >= _overdraftLimit)
            {
                _balance -= amount;
                Console.WriteLine($"Withdrew ${amount}, balance is now {_balance}");
                return true;
            }
            return false;
        }

        public override string ToString() => $"{nameof(_balance)}: {_balance}";
    }

    public abstract class Command
    {
        public abstract void Call();
        public abstract void Undo();
        public bool Success { get; set; }
    }

    public class BankAccountCommand : Command
    {
        private readonly BankAccount _account;
        private readonly Action _action;
        private readonly int _amount;
        private bool _succeeded;

        public enum Action
        {
            Deposit, Withdraw
        }

        public BankAccountCommand(BankAccount account, Action action, int amount)
        {
            _account = account;
            _action = action;
            _amount = amount;
        }

        public override void Call()
        {
            switch (_action)
            {
                case Action.Deposit:
                    _account.Deposit(_amount);
                    _succeeded = true;
                    break;
                case Action.Withdraw:
                    _succeeded = _account.Withdraw(_amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Undo()
        {
            if (!_succeeded) return;
            switch (_action)
            {
                case Action.Deposit:
                    _account.Withdraw(_amount);
                    break;
                case Action.Withdraw:
                    _account.Deposit(_amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    abstract class CompositeBankAccountCommand : List<BankAccountCommand>//, ICommand
    {
        public virtual void Call()
        {
            ForEach(cmd => cmd.Call());
        }

        public virtual void Undo()
        {
            foreach (var cmd in ((IEnumerable<BankAccountCommand>)this).Reverse())
            {
                cmd.Undo();
            }
        }

    }

    class MoneyTransferCommand : CompositeBankAccountCommand
    {
        public MoneyTransferCommand(BankAccount from, BankAccount to, int amount)
        {
            AddRange(new[]
            {
                new BankAccountCommand(from, BankAccountCommand.Action.Withdraw, amount),
                new BankAccountCommand(to, BankAccountCommand.Action.Deposit, amount)
            });
        }

        public override void Call()
        {
            bool ok = true;
            foreach (var cmd in this)
            {
                if (ok)
                {
                    cmd.Call();
                    ok = cmd.Success;
                }
                else
                {
                    cmd.Success = false;
                }
            }
        }
    }

}
