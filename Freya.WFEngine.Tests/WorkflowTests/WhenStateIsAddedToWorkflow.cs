using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.WorkflowTests
{
    public class WhenStateIsAddedToWorkflow : SpecificationBase
    {
        private Workflow<SpecificationBase> workflow;
        private string stateName;

        public override void Given() {
            this.workflow = new Workflow<SpecificationBase>(new Mock<IStateManager<SpecificationBase>>().Object);
            this.stateName = "firstState";
        }

        public override void When() {
            this.workflow.AddState(this.stateName);
        }

        [Then]
        public void Workflow_Contains_The_Added_State() {
            Assert.AreEqual(1, this.workflow.States.Count);
            Assert.AreEqual(this.stateName, this.workflow.States.First());
        }

        [Then]
        [ExpectedException(typeof(ArgumentException))]
        public void Attempt_To_Add_Same_State_Fails() {
            this.workflow.AddState(this.stateName);
        }

        [Then]
        public void Can_Add_State_With_Different_Casing() {
            string newStateName = this.stateName.ToUpperInvariant();
            this.workflow.AddState(newStateName);
            Assert.AreEqual(2, this.workflow.States.Count);
            Assert.IsTrue(this.workflow.States.Contains(newStateName));
        }


    }
}
