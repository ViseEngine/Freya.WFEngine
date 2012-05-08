using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    internal class GuardRegistration
    {
        public Type Type { get; set; }
        public XmlElement Configuration { get; set; }
    }
}
