using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.CompositeComponentFactoryTests
{
    public class WhenFactoriesAreRegistered : SpecificationBase
    {
        private CompositeComponentFactory<object> componentFactory;
        private object intResult;
        private object stringResult;

        public override void Given()
        {
            componentFactory = new CompositeComponentFactory<object>();

            componentFactory.Factories.Add(CreateFactory(1));
            componentFactory.Factories.Add(CreateFactory("foo"));
            componentFactory.Factories.Add(CreateFactory(2));
        }

        private IComponentFactory<object> CreateFactory<T>(T returns) {
            var mock = new Moq.Mock<IComponentFactory<object>>();
            mock.Setup(cf => cf.CanHandle(It.IsAny<Type>())).Returns<Type>(type => type == typeof(T));
            mock.Setup(cf => cf.CreateComponent(It.Is<Type>(type => type == typeof (T)), It.IsAny<IDictionary<string, object>>())).Returns(returns);
            return mock.Object;
        }

        public override void When() {
            intResult = componentFactory.CreateComponent(typeof (int), null);
            stringResult = componentFactory.CreateComponent(typeof (string), null);
        }

        [Then]
        public void CreateComponent_Finds_Registered_Factories() {
            Assert.IsNotNull(intResult);
            Assert.IsNotNull(stringResult);
            Assert.IsInstanceOf<int>(intResult);
            Assert.IsInstanceOf<string>(stringResult);
        }

        [Then]
        public void CreateComponent_Searches_In_Reverse_Order() {
            object result = this.componentFactory.CreateComponent(typeof (int), null);
            Assert.AreEqual(2, result);
        }
    }
}
