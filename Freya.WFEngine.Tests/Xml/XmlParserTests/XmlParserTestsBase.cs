using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.Tests.Xml.XmlParserTests
{
    public class XmlParserTestsBase : SpecificationBase
    {
        protected XmlElement ParseXml(string xml)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml);
            return xmldoc.DocumentElement;
        }

        protected XmlElement ParseResourceXml(string resourceName) {
            XmlDocument xmlDoc = new XmlDocument();
            using (var stream = typeof (XmlParserTestsBase).Assembly.GetManifestResourceStream("Freya.WFEngine.Tests.Xml.XmlParserTests." + resourceName)) {
                xmlDoc.Load(stream);                
            };

            return xmlDoc.DocumentElement;
        }
    }
}
