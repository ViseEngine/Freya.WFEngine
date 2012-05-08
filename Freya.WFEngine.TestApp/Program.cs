using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.TestApp
{
    public class Item
    {
        public int ID { get; set; }
        public string CurrentState { get; set; }
    }

    public class DummyStateManager : IStateManager<Item>
    {
        public string GetCurrentState(Item item) {
            return item.CurrentState;
        }

        public void ChangeState(Item item, string newState) {
            item.CurrentState = newState;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            Workflow<Item> workflow = new Workflow<Item>(new DummyStateManager());
            XmlConfigurator xmlConfigurator = new XmlConfigurator();
            xmlConfigurator.AddXml(Resources.Sample);
            xmlConfigurator.Configure(workflow);


            Item i1 = new Item() { ID = 1, CurrentState = "First" };
            IActivity[] activities = workflow.GetActivitiesForItem(i1).ToArray();
            workflow.GetActivitiesForItem(i1).ToArray();
            foreach (var act in activities) {
                Console.WriteLine("Available activity {0}", act.Context.Name);
            }

            ITransitionActivity activity = activities.Cast<ITransitionActivity>().Single();
            activity.Invoke();
            
            Console.WriteLine("Current state: {0}", i1.CurrentState);

            activity.Invoke();
            
            Console.ReadKey();
        }
    }
}
