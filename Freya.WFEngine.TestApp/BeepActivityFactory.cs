using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.TestApp
{
    public class BeepActivityFactory : IXmlComponentFactory<IActivity>
    {
        public bool CanHandle(Type componentType) {
            return componentType == typeof (BeepActivity);
        }

        public IActivity CreateComponent(Type componentType, XmlElement configuration) {
            return new BeepActivity(configuration.GetAttribute("exitState"));
        }
    }
}
