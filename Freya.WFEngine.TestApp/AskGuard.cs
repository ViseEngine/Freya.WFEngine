using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public class AskGuard<TItem> : IGuard<TItem>
    {
        public AskGuard(string question) {
            this.question = question;
        }

        private readonly string question;

        public bool Check(TItem item) {
            Console.WriteLine(question);
            while (true) {
                ConsoleKey consoleKey = Console.ReadKey(true).Key;
                switch (consoleKey) {
                    case ConsoleKey.Y:
                        return true;
                    case ConsoleKey.N:
                        return false;
                }
            }
        }
    }
}
