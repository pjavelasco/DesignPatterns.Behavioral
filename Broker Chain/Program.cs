using System;
using static System.Console;

namespace Behavioral.Broker_Chain
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            var goblin = new Creature(game, "Strong Goblin", 3, 3);
            WriteLine(goblin);

            using (new DoubleAttackModifier(game, goblin))
            {
                WriteLine(goblin);
                using (new IncreaseDefenseModifier(game, goblin))
                {
                    WriteLine(goblin);
                }
            }

            WriteLine(goblin);
        }
    }

    // command query separation is being used here
    public class Query
    {
        public enum Argument
        {
            Attack, Defense
        }

        public string CreatureName { get; set; }
        public Argument WhatToQuery { get; set; }
        public int Value { get; set; } // bidirectional

        public Query(string creatureName, Argument whatToQuery, int value)
        {
            CreatureName = creatureName ?? throw new ArgumentNullException(paramName: nameof(creatureName));
            WhatToQuery = whatToQuery;
            Value = value;
        }
    }

    public class Game // mediator pattern
    {
        public event EventHandler<Query> Queries; // effectively a chain

        public void PerformQuery(object sender, Query q)
        {
            Queries?.Invoke(sender, q);
        }
    }

    public class Creature
    {
        private readonly Game _game;
        private readonly int _attack;
        private readonly int _defense;

        public Creature(Game game, string name, int attack, int defense)
        {
            _game = game ?? throw new ArgumentNullException(paramName: nameof(game));
            Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
            _attack = attack;
            _defense = defense;
        }

        public int Attack
        {
            get
            {
                var q = new Query(Name, Query.Argument.Attack, _attack);
                _game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public int Defense
        {
            get
            {
                var q = new Query(Name, Query.Argument.Defense, _defense);
                _game.PerformQuery(this, q);
                return q.Value;
            }
        }

        public string Name { get; set; }

        public override string ToString() // no game
        {
            return $"{nameof(Name)}: {Name}, {nameof(_attack)}: {Attack}, {nameof(_defense)}: {Defense}";
            //                                                 ^^^^^^^^ using a property    ^^^^^^^^^
        }
    }

    public abstract class CreatureModifier : IDisposable
    {
        protected Game game;
        protected Creature creature;

        protected CreatureModifier(Game game, Creature creature)
        {
            this.game = game;
            this.creature = creature;
            game.Queries += Handle;
        }

        protected abstract void Handle(object sender, Query q);

        public void Dispose()
        {
            game.Queries -= Handle;
        }
    }

    public class DoubleAttackModifier : CreatureModifier
    {
        public DoubleAttackModifier(Game game, Creature creature) : base(game, creature)
        {
        }

        protected override void Handle(object sender, Query q)
        {
            if (q.CreatureName == creature.Name &&
                q.WhatToQuery == Query.Argument.Attack)
                q.Value *= 2;
        }
    }

    public class IncreaseDefenseModifier : CreatureModifier
    {
        public IncreaseDefenseModifier(Game game, Creature creature) : base(game, creature)
        {
        }

        protected override void Handle(object sender, Query q)
        {
            if (q.CreatureName == creature.Name &&
                q.WhatToQuery == Query.Argument.Defense)
                q.Value += 2;
        }
    }
}
