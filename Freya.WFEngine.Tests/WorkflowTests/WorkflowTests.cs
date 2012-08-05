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
using NUnit.Framework;

namespace Freya.WFEngine.Tests.WorkflowTests
{
    [TestFixture]
    public class WorkflowTests_
    {

        #region helper classes
        public class Item : IStatefulItem
        {
            public int ID { get; set; }

            public string CurrentState {
                get; set;
            }
        }

        #endregion

        private Workflow<Item> emptyWorkflow;
        internal static IDictionary<string, object> transitionParametersToFirst = new Dictionary<string, object> { { SingleExitPointActivity.ExitPointParameterName, "First" } };
        internal static IDictionary<string, object> transitionParametersToSecond = new Dictionary<string, object> { { SingleExitPointActivity.ExitPointParameterName, "Second" } };
        
        [SetUp]
        public void SetUp() {
            this.emptyWorkflow = new Workflow<Item>(new DefaultStateManager<Item>());
        }
        
        [Test]
        public void Activities_Are_Proxied() {
            
            State firstState = this.emptyWorkflow.States.Add("First");
            firstState.Activities.Add(new ActivityDescription(typeof(TransitionActivity), null, transitionParametersToSecond));
            
            Item item = new Item { CurrentState= "First" };
            IEnumerable<IActivity> activities = this.emptyWorkflow.GetActivitiesForItem(item);
            IActivity activity = activities.Single();
            Assert.IsNotNull(activity.BaseActivity);
            Assert.AreEqual(typeof(TransitionActivity), activity.BaseActivity.GetType());
            Assert.AreNotEqual(typeof(TransitionActivity), activity.GetType());
            Assert.IsInstanceOf<ITransitionActivity>(activity);
        }
    }
}
