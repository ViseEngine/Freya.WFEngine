using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.CompositeComponentFactoryTests
{
    public class WhenNewCompositeCompomentFactoryIsCreated : SpecificationBase
    {
        private CompositeComponentFactory<object> factory; 

        public override void When()
        {
            factory = new CompositeComponentFactory<object>();
        }

        [Then]
        public void No_Underlying_Factories_Are_Registered() {
            Assert.AreEqual(0, factory.Factories.Count);
        }

        [Then]
        public void Can_Add_A_Factory() {
            factory.Factories.Add(new CompositeComponentFactory<object>());
        }

        [Then]
        [ExpectedException(typeof(ArgumentException))]
        public void Cannot_AddRange_Self() {
            factory.Factories.AddRange(new[] { factory });
        }

        [Then]
        [ExpectedException(typeof(ArgumentException))]
        public void Cannot_Add_Self() {
            factory.Factories.Add(factory);
        }

        [Then]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cannot_Add_Null_Factory() {
            factory.Factories.Add(null);
        }

        [Then]
        public void Cannot_Addrange_Null() {
            try {
                factory.Factories.AddRange(new IComponentFactory<object>[] { null });
                throw new NUnit.Framework.AssertionException("Expected ArgumentException");
            }
            catch (ArgumentException) {
            }
        }
    }
}
