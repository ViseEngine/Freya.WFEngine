using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Freya.WFEngine;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.DefaultActivityFactoryTests
{
    public class WhenDefaultActivityFactoryIsCreated : SpecificationBase
    {
        private DefaultActivityFactory factory;

        #region Helper classes
        public class ClassWithDefaultCtor : IActivity
        {
            public IActivity BaseActivity {
                get { throw new NotSupportedException(); }
            }

            public ActivityContext Context {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
        }

        public class ClassWithNoDefaultCtor : IActivity
        {
            public ClassWithNoDefaultCtor(int val) {}

            public IActivity BaseActivity {
                get { throw new NotSupportedException(); }
            }

            public ActivityContext Context {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
        }

        public interface IInterface : IActivity
        {
        }
        #endregion

        public override void When()
        {
            factory = new DefaultActivityFactory();
        }

        [Then]
        public void Can_Handle_Types_With_Default_Ctor() {
            Assert.IsTrue(factory.CanHandle(typeof(ClassWithDefaultCtor)));
        }

        [Then]
        public void Cannot_Handle_Interfaces() {
            Assert.IsFalse(factory.CanHandle(typeof(IInterface)));
        }

        [Then]
        public void Cannot_Handle_Classes_With_No_Default_Ctor() {
            Assert.IsFalse(factory.CanHandle(typeof(ClassWithNoDefaultCtor)));
        }

        [Then]
        public void Can_Instantiate_Classes_With_Default_Ctor() {
            IActivity result = factory.CreateComponent(typeof (ClassWithDefaultCtor), new Dictionary<string, object>());
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ClassWithDefaultCtor>(result);
        }
    }
}
