using System;
using System.Linq;
using Moq;
using NUnit.Framework;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace Freya.WFEngine.Tests.WorkflowTests
{
    public class WhenNewWorkflowIsCreated : SpecificationBase
    {
        private Workflow<SpecificationBase> workflow; 
        public override void When()
        {
            this.workflow = new Workflow<SpecificationBase>(new Mock<IStateManager<SpecificationBase>>().Object);
        }

        [Then]
        public void Has_No_States() {
            Assert.AreEqual(0, this.workflow.States.Count);
        }
        
        [Then]
        public void ActivityFactory_Is_Set_To_Composite_Factory() {
            Assert.IsNotNull(this.workflow.ActivityFactory);
            Assert.IsInstanceOf<CompositeComponentFactory<IActivity>>(this.workflow.ActivityFactory);
        }

        [Then]
        public void ActivityFactory_Contains_DefaultActivityFactory() {
            this.workflow.ActivityFactory.Factories.Single(f => f.GetType() == typeof (DefaultActivityFactory));
        }

        [Then]
        public void ActivityFactory_Contains_TransitionActivityFactory() {
            this.workflow.ActivityFactory.Factories.Single(f => f.GetType() == typeof (TransitionActivityFactory));
        }
    }
}
