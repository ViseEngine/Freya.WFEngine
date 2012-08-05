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
using Freya.WFEngine.Tests.WorkflowTests;
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
            Workflow<WorkflowTests_.Item> wf = new Workflow<WorkflowTests_.Item>(new WorkflowTests_.StateManager());
            State firstState = wf.States.Add("First");
            firstState.Activities.Add(new ActivityDescription(typeof(TransitionActivity), null, WorkflowTests_.transitionParametersToSecond));
            State secondState = wf.States.Add("Second");
            secondState.Activities.Add(new ActivityDescription(typeof(TransitionActivity), null, WorkflowTests_.transitionParametersToFirst));
            WorkflowTests_.Item item = new WorkflowTests_.Item() {
                                                                     State = "First"
                                                                 };

            var activities = wf.GetActivitiesForItem(item).ToArray();
            Assert.AreEqual(1, activities.Length);
            activities.Cast<ITransitionActivity>().Single().Invoke();

            Assert.AreEqual("Second", item.State);

            activities = wf.GetActivitiesForItem(item).ToArray();
            Assert.AreEqual(1, activities.Length);
            activities.Cast<ITransitionActivity>().Single().Invoke();
            Assert.AreEqual("First", item.State);
        }
    }
}
