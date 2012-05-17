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
using System.Xml;
using NUnit.Framework;

namespace Freya.WFEngine.Tests
{
    [TestFixture]
    public class WorkflowTests
    {
        static WorkflowTests() {
            emptyXmlElement = ToXmlElement("<Empty />");
            transitionDefinitionToFirst = ToXmlElement("<Transition exitState=\"First\" />");
            transitionDefinitionToSecond = ToXmlElement("<Transition exitState=\"Second\" />");
        }

        #region helper classes
        public class Item
        {
            public int ID { get; set; }
            public string State { get; set; }
        }

        public class StateManager : IStateManager<Item>
        {
            public string GetCurrentState(Item item) {
                return item.State;
            }

            public void ChangeState(Item item, string newState) {
                item.State = newState;
            }
        }

        
        #endregion

        private Workflow<Item> emptyWorkflow;
        internal static XmlElement emptyXmlElement;
        internal static XmlElement transitionDefinitionToSecond;
        internal static XmlElement transitionDefinitionToFirst;

        [TestFixtureSetUp]
        public void SetUpFixture() {
           
        }

        private static XmlElement ToXmlElement(string xml) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc.DocumentElement;
        }

        [SetUp]
        public void SetUp() {
            this.emptyWorkflow = new Workflow<Item>(new StateManager());
        }
        
        [Test]
        public void New_Workflow_Has_No_States() {
            Assert.AreEqual(0, this.emptyWorkflow.States.Count);
        }

        [Test]
        public void AddState_Adds_State() {
            Assert.AreEqual(0, this.emptyWorkflow.States.Count);
            Assert.IsTrue(this.emptyWorkflow.AddState("state"));
            Assert.AreEqual(1, this.emptyWorkflow.States.Count);
            Assert.AreEqual("state", this.emptyWorkflow.States.First());
        }

        [Test]
        public void AddState_Doesnt_Add_Already_Existing_State() {
            Assert.IsTrue(this.emptyWorkflow.AddState("state"));
            Assert.IsFalse(this.emptyWorkflow.AddState("state"));
            Assert.AreEqual(1, this.emptyWorkflow.States.Count);
        }

        [Test]
        public void States_Are_Case_Sensitive() {
            Assert.IsTrue(this.emptyWorkflow.AddState("state"));
            Assert.IsTrue(this.emptyWorkflow.AddState("State"));
            Assert.AreEqual(2, this.emptyWorkflow.States.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_Throws_ArgumentException_For_Invalid_StateName() {
            this.emptyWorkflow.AddActivity("state", typeof(TransitionActivity), emptyXmlElement);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void AddActivity_Throws_ArgumentException_For_Invalid_Activity_Type() {
            this.emptyWorkflow.AddState("state");
            this.emptyWorkflow.AddActivity("state", typeof(Type), emptyXmlElement);
        }

        [Test]
        public void AddActivity_Adds_Activity() {
            this.emptyWorkflow.AddState("First");
            this.emptyWorkflow.AddState("Second");
            this.emptyWorkflow.AddActivity("First", typeof(TransitionActivity), transitionDefinitionToSecond);
            Item item = new Item { State = "First" };
            IEnumerable<IActivity> activities = this.emptyWorkflow.GetActivitiesForItem(item);
            Assert.AreEqual(1, activities.Count());
        }

        [Test]
        public void Activities_Are_Proxied() {
            this.emptyWorkflow.AddState("First");
            this.emptyWorkflow.AddState("Second");
            this.emptyWorkflow.AddActivity("First", typeof(TransitionActivity), transitionDefinitionToSecond);
            Item item = new Item { State = "First" };
            IEnumerable<IActivity> activities = this.emptyWorkflow.GetActivitiesForItem(item);
            IActivity activity = activities.Single();
            Assert.IsNotNull(activity.BaseActivity);
            Assert.AreEqual(typeof(TransitionActivity), activity.BaseActivity.GetType());
            Assert.AreNotEqual(typeof(TransitionActivity), activity.GetType());
            Assert.IsInstanceOf<ITransitionActivity>(activity);
        }
    }
}
