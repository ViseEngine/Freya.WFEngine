using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.Xml.XmlParserTests
{
    public class WhenParsesGuardDescription : XmlParserTestsBase
    {
        private XmlParser parser;
        private GuardDescription result;
        private XmlElement xmlSample;
        private Type expectedGuardType;

        public override void Given() {
            xmlSample = ParseXml("<FooGuard par1=\"value\" />");
            var guardMock = new Moq.Mock<IGuard>();
            expectedGuardType = guardMock.Object.GetType();
            var guardTypes = new Dictionary<string, Type> { { "FooGuard", expectedGuardType }, { "AnotherGuard", typeof (IGuard) } };
            parser = new XmlParser(guardTypes, new Dictionary<string, Type>(), new Dictionary<string, IList<ActivityDescription>>());

        }

        public override void When() {
            result = parser.ParseGuardDescription(xmlSample);
        }

        [Then]
        public void Returns_GuardDescription() {
            Assert.IsNotNull(result);
        }

        [Then]
        public void Sets_Type() {
            Assert.AreEqual(expectedGuardType, result.Type);
        }

        [Then]
        public void Sets_Parameters() {

            Assert.IsNotNull(result.Parameters);
            Assert.AreEqual(1, result.Parameters.Count);
            Assert.IsTrue(result.Parameters.ContainsKey("par1"));
            Assert.AreEqual("value", result.Parameters["par1"]);
        }

    }
}
