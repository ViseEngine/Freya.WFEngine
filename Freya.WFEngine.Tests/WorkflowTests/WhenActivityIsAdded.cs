using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// ReSharper disable PossibleMultipleEnumeration

namespace Freya.WFEngine.Tests.WorkflowTests
{
    public class WhenActivityIsAdded : SpecificationBase
    {
        #region Helper classes
        public interface ITestActivity : IActivity
        {
        }

        public class TestActivity : ITestActivity
        {
            public IActivity BaseActivity {
                get { return null; }
            }

            public ActivityContext Context {
                get; set; 
            }
        }

        public class Item : IStatefulItem
        {
            public string CurrentState { get; set; }
        }

        #endregion

        private Workflow<Item> workflow;
        private const string stateName = "firstState";
        private string activityName;
        public override void Given() {
            this.workflow = new Workflow<Item>(new DefaultStateManager<Item>());
            this.workflow.AddState(stateName);
            this.activityName = "firstActivity";
        }

        public override void When() {
            this.workflow.AddActivity(stateName, typeof (TestActivity), null, this.activityName);
        }

        [Then]
        public void Workflow_Contains_The_Activity_For_State() {
            IEnumerable<IActivity> activities = this.workflow.GetActivitiesForItem(new Item { CurrentState = stateName });
            Assert.IsNotNull(activities);
            Assert.AreEqual(1, activities.Count());
            Assert.IsInstanceOf<ITestActivity>(activities.First());
        }
    }
}
