using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    internal class ActivityRegistration
    {
        public ActivityRegistration() {
        }

        public ActivityRegistration(Type type, XmlElement configuration, string name) {
            if (type == null)
                throw new ArgumentNullException("type");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.Type = type;
            this.Configuration = configuration;
            this.Name = name;
        }

        
        public string Name { get; set; }
        public Type Type { get; set; }
        public XmlElement Configuration { get; set; }
    }
}
