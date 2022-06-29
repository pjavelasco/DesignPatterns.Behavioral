using System.Collections.Generic;
using static System.Console;

namespace Behavioral.State.Handmade
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var state = State.OffHook;
            while (true)
            {
                WriteLine($"The phone is currently {state}");
                WriteLine("Select a trigger:");

                // foreach to for
                for (var i = 0; i < _rules[state].Count; i++)
                {
                    var (t, _) = _rules[state][i];
                    WriteLine($"{i}. {t}");
                }

                var input = int.Parse(ReadLine());

                var (_, s) = _rules[state][input];
                state = s;
            }
        }

        private static readonly Dictionary<State, List<(Trigger, State)>> _rules = new()
        {
            [State.OffHook] = new List<(Trigger, State)>
            {
                (Trigger.CallDialed, State.Connecting)
            },
            [State.Connecting] = new List<(Trigger, State)>
            {
                (Trigger.HungUp, State.OffHook),
                (Trigger.CallConnected, State.Connected)
            },
            [State.Connected] = new List<(Trigger, State)>
            {
                (Trigger.LeftMessage, State.OffHook),
                (Trigger.HungUp, State.OffHook),
                (Trigger.PlacedOnHold, State.OnHold)
            },
            [State.OnHold] = new List<(Trigger, State)>
            {
                (Trigger.TakenOffHold, State.Connected),
                (Trigger.HungUp, State.OffHook)
            }
        };
    }

    public enum State
    {
        OffHook,
        Connecting,
        Connected,
        OnHold
    }

    public enum Trigger
    {
        CallDialed,
        HungUp,
        CallConnected,
        PlacedOnHold,
        TakenOffHold,
        LeftMessage
    }
}
