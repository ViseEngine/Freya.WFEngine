#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public class AskGuard : IGuard
    {
        public AskGuard(string question) {
            this.question = question;
        }

        private readonly string question;

        public bool Check(WorkflowContext context) {
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
