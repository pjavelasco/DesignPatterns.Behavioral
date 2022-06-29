using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Behavioral.Mediator.ChatRoom
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var room = new ChatRoom();

            var john = new Person("John");
            var jane = new Person("Jane");

            room.Join(john);
            room.Join(jane);

            john.Say("hi room");
            jane.Say("oh, hey john");

            var simon = new Person("Simon");
            room.Join(simon);
            simon.Say("hi everyone!");

            jane.PrivateMessage("Simon", "glad you could join us!");
        }
    }

    public class Person
    {
        private readonly List<string> _chatLog = new();

        public string Name { get; set; }
        public ChatRoom Room { get; set; }

        public Person(string name) => Name = name;

        public void Receive(string sender, string message)
        {
            string s = $"{sender}: '{message}'";
            WriteLine($"[{Name}'s chat session] {s}");
            _chatLog.Add(s);
        }

        public void Say(string message) => Room.Broadcast(Name, message);

        public void PrivateMessage(string who, string message) => Room.Message(Name, who, message);
    }

    public class ChatRoom
    {
        private readonly List<Person> _people = new();

        public void Broadcast(string source, string message)
        {
            foreach (var p in _people)
                if (p.Name != source)
                    p.Receive(source, message);
        }

        public void Join(Person p)
        {
            string joinMsg = $"{p.Name} joins the chat";
            Broadcast("room", joinMsg);

            p.Room = this;
            _people.Add(p);
        }

        public void Message(string source, string destination, string message)
        {
            _people.FirstOrDefault(p => p.Name == destination)?.Receive(source, message);
        }
    }
}
