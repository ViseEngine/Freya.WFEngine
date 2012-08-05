using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.StateSetTests
{
    public class WhenStateIsAdded : SpecificationBase
    {
        private StateSet stateSet;
        private string stateName;

        public override void Given() {
            this.stateSet = new StateSet();
            this.stateName = "firstState";
        }

        public override void When() {
            this.stateSet.Add(this.stateName);
        }

        [Then]
        public void Contains_The_Added_State() {
            Assert.AreEqual(1, this.stateSet.Count);
            Assert.AreEqual(this.stateName, this.stateSet.First().Name);
        }

        [Then]
        public void Attempt_To_Add_Same_State_Doesnt_Add_New_State() {
            State currentState = this.stateSet.Single();
            State returnedState = this.stateSet.Add(this.stateName);
            Assert.AreEqual(1, this.stateSet.Count);
            Assert.IsTrue(object.ReferenceEquals(currentState, returnedState));
        }

        [Then]
        public void Can_Add_State_With_Different_Casing() {
            string newStateName = this.stateName.ToUpperInvariant();
            this.stateSet.Add(newStateName);
            Assert.AreEqual(2, this.stateSet.Count);
            State state = this.stateSet[newStateName];
            Assert.IsNotNull(state);
            Assert.AreEqual(newStateName, state.Name);
        }


    }
}
