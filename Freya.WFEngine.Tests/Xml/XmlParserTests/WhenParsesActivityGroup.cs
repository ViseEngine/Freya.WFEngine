using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Moq;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.Xml.XmlParserTests
{
    public class WhenParsesActivityGroup : XmlParserTestsBase
    {
        private XmlParser parser;
        private Type activityType;
        private Type localGuardType, sharedGuardType;
        private XmlElement xmlSample;
        private KeyValuePair<string, IList<ActivityDescription>> result;

        public override void Given() {
            this.activityType = new Moq.Mock<IActivity>().Object.GetType();
            this.sharedGuardType = new Moq.Mock<IGuard>().Object.GetType();
            this.localGuardType = typeof(IGuard);

            var activityTypes = new Dictionary<string, Type> { { "BarActivity", this.activityType } };
            var guardTypes = new Dictionary<string, Type> { { "SharedGuard", this.sharedGuardType }, { "LocalGuard", this.localGuardType } };

            this.parser = new XmlParser(guardTypes, activityTypes, new Dictionary<string, IList<ActivityDescription>>());

            this.xmlSample = ParseResourceXml("ActivityGroupDescriptionSample.xml");
        }

        public override void When() {
            this.result = this.parser.ParseActivityGroup(xmlSample);
        }

        [Then]
        public void Returns_Kvp() {
            Assert.IsNotNull(this.result);
        }

        [Then]
        public void Key_Corresponds_To_Group_Name() {
            Assert.AreEqual("FooGroup", this.result.Key);            
        }

        [Then]
        public void Value_Corresponds_To_Activities() {
            Assert.IsNotNull(this.result.Value);
            Assert.AreEqual(1, this.result.Value.Count);
        }

        [Then]
        public void Sets_Activity() {
            ActivityDescription activityDesc = this.result.Value[0];
            Assert.IsNotNull(activityDesc);
            Assert.AreEqual(this.activityType, activityDesc.Type);
            Assert.AreEqual("bar", activityDesc.Name);
        }

        [Then]
        public void Sets_Guards() {
            ActivityDescription activityDesc = this.result.Value[0];
            Assert.AreEqual(2, activityDesc.Guards.Count);
        }

        [Then]
        public void Sets_Shared_Guard() {
            ActivityDescription activityDesc = this.result.Value[0];
            GuardDescription guard = activityDesc.Guards.Single(g => g.Type == this.sharedGuardType);
        }

        [Then]
        public void Sets_Local_Guard() {
            ActivityDescription activityDesc = this.result.Value[0];
            GuardDescription guard = activityDesc.Guards.Single(g => g.Type == this.localGuardType);
        }
    }
}
