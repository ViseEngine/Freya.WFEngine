﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Freya.WFEngine.Tests
{
    [TestFixture]
    public class WorkflowSimpleScenarioTests
    {
        /// <summary>
        /// Transitions from state First to Second and back again.
        /// </summary>
        [Test]
        public void SimpleScenario1() {
            Workflow<WorkflowTests.Item> wf = new Workflow<WorkflowTests.Item>(new WorkflowTests.StateManager());
            Assert.IsTrue(wf.AddState("First"));
            Assert.IsTrue(wf.AddState("Second"));
            wf.AddActivity("First", typeof(TransitionActivity), WorkflowTests.transitionDefinitionToSecond);
            wf.AddActivity("Second", typeof(TransitionActivity), WorkflowTests.transitionDefinitionToFirst);
            WorkflowTests.Item item = new WorkflowTests.Item() {
                                                                   State = "First"
                                                               };

            var activities = wf.GetActivitiesForItem(item).ToArray();
            Assert.AreEqual(1, activities.Length);
            Assert.AreEqual("Second", ((ITransitionActivity) activities.Single()).Invoke());

            Assert.AreEqual("Second", item.State);

            activities = wf.GetActivitiesForItem(item).ToArray();
            Assert.AreEqual(1, activities.Length);
            Assert.AreEqual("First", ((ITransitionActivity)activities.Single()).Invoke());

            Assert.AreEqual("First", item.State);
        }
    }
}