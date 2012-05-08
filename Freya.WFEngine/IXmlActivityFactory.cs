using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public interface IXmlActivityFactory : IActivityFactory
    {
        IActivity CreateActivity(Type activityType, XmlElement configuration);
    }
}
