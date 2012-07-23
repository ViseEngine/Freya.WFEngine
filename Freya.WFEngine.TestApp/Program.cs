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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.TestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Workflow<Item> workflow = new Workflow<Item>(new DefaultStateManager<Item>());
            XmlConfigurator xmlConfigurator = new XmlConfigurator();
            xmlConfigurator.AddXml(Resources.TypeRegistration);
            xmlConfigurator.AddXml(Resources.Sample);
            xmlConfigurator.Configure(workflow);
            workflow.ActivityFactory.Register(new BeepActivityFactory());
            workflow.GuardFactory.Register(new AskGuardFactory<Item>());
            workflow.PostInvoke += (sender, activity) => Console.WriteLine("PostInvoke: => {0}", activity.Context.State);
            workflow.PreInvoke += (sender, activity) => Console.WriteLine("PreInvoke ({0}, {1})", activity.GetType().FullName, activity.Context.Name);

            Item i1 = new Item() { ID = 1, CurrentState = "First" };
            while (true) {
                Console.WriteLine("Current state: {0}", i1.CurrentState);
                IActivity[] activities = workflow.GetActivitiesForItem(i1).ToArray();
                foreach (var activity in activities)
                    Console.WriteLine("Available activity: {0} ({1})", activity.Context.Name, activity.BaseActivity.GetType().FullName);
                
                if (activities.Length == 0) {
                    Console.WriteLine("No activities available!");
                    break;
                }

                Console.Write("Which to invoke (1..{0})?", activities.Length);
                int index = int.Parse(Console.ReadKey().KeyChar.ToString(CultureInfo.InvariantCulture)) - 1;
                Console.WriteLine();
                InvokeActivity(activities[index]);
                Console.WriteLine("----------");
            }

            Console.ReadKey();
        }

        private static void InvokeActivity(IActivity activity) {
            if (activity is ITransitionActivity) {
                ((ITransitionActivity) activity).Invoke();
            } else if (activity is IBeepActivity) {
                ((IBeepActivity) activity).Invoke();
            } else {
                throw new NotSupportedException();
                
            }
        }
    }
}
