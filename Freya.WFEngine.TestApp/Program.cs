using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            const string firstStateName = "first";
            const string secondStateName = "second";
            workflow.AddState(firstStateName);
            workflow.AddState(secondStateName);

            ActivityDescriptor toSecondAD = new ActivityDescriptor("toSecond", typeof(TransitionActivity));
            toSecondAD.ExitPointMapping.Add(Activity.DefaultExitPoint, secondStateName);
            workflow.AddActivity(firstStateName, toSecondAD);

            ActivityDescriptor toFirstAD = new ActivityDescriptor("toFirst", typeof(TransitionActivity));
            toFirstAD.ExitPointMapping.Add(Activity.DefaultExitPoint, firstStateName);
            workflow.AddActivity(secondStateName, toFirstAD);

            Item i1 = new Item() { ID = 1, CurrentState = firstStateName };
            IActivity[] activities = workflow.GetActivitiesForItem(i1).ToArray();
            workflow.GetActivitiesForItem(i1).ToArray();
            foreach (var act in activities) {
                Console.WriteLine("Available activity {0}", act.Name);
            }

            ITransitionActivity activity = activities.Cast<ITransitionActivity>().Single();
            activity.Invoke();
            
            Console.WriteLine("Current state: {0}", i1.CurrentState);

            activity.Invoke();
            
            Console.ReadKey();
        }
    }
}
