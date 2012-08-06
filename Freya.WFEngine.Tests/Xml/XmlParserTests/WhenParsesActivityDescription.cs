using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.Xml.XmlParserTests
{
    public class WhenParsesActivityDescription : XmlParserTestsBase
    {
        private Type activityType;
        private XmlElement xmlSample;
        private XmlParser xmlParser;
        private IEnumerable<ActivityDescription> result;
        private Type guardType;

        public override void Given()
        {
            this.xmlSample = ParseResourceXml("ActivityDescriptionSample.xml");
            
            var activityMock = new Moq.Mock<IActivity>();
            this.activityType = activityMock.Object.GetType();

            var guardMock = new Moq.Mock<IGuard>();
            this.guardType = guardMock.Object.GetType();
            var activityTypes = new Dictionary<string, Type> { { "FooActivity", this.activityType } };
            var guardTypes = new Dictionary<string, Type> { { "BarGuard", this.guardType } };

            this.xmlParser = new XmlParser(guardTypes, activityTypes, new Dictionary<string, IList<ActivityDescription>>());
        }

        public override void When()
        {
            this.result = xmlParser.ParseActivityDescription(xmlSample);
        }

        [Then]
        public void Returns_ActivityDescription() {
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Then]
        public void Sets_Type() {
            ActivityDescription activity = result.First();
            Assert.AreEqual(activityType, activity.Type);
        }

        [Then]
        public void Sets_Parameters() {
            ActivityDescription activity = result.First();

            Assert.IsNotNull(activity.Parameters);
            Assert.AreEqual(1, activity.Parameters.Count);
            Assert.IsTrue(activity.Parameters.ContainsKey("par1"));
            Assert.AreEqual("value", activity.Parameters["par1"]);
        }

        [Then]
        public void Sets_Name() {
            ActivityDescription activity = result.First();
                
            Assert.AreEqual("someName", activity.Name);
        }

        [Then]
        public void Sets_Guard() {
            ActivityDescription activity = result.First();
            
            Assert.IsNotNull(activity.Guards);
            Assert.AreEqual(1, activity.Guards.Count);

            Assert.AreEqual(this.guardType, activity.Guards[0].Type);
        }
    }
}
