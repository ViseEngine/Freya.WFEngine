using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.TestApp
{
    public class AskGuardFactory<TItem> : IXmlComponentFactory<IGuard<TItem>>
    {
        public bool CanHandle(Type componentType) {
            return componentType == typeof (AskGuard<TItem>);
        }

        public IGuard<TItem> CreateComponent(Type componentType, XmlElement configuration) {
            return new AskGuard<TItem>(configuration.GetAttribute("question"));
        }
    }
}
