using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public interface IXmlComponentFactory<TComponent>
    {
        bool CanHandle(Type componentType);
        TComponent CreateComponent(Type componentType, XmlElement configuration);
    }
}
