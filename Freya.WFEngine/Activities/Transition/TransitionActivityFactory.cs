using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class TransitionActivityFactory : IXmlComponentFactory<IActivity>
    {
        public IActivity CreateComponent(Type activityType, System.Xml.XmlElement configuration)
        {
            if (activityType != typeof(TransitionActivity))
                throw new NotSupportedException();

            return new TransitionActivity(configuration.GetAttribute(SR.DefaultExitStateAttributeName));
        }

        public bool CanHandle(Type activityType) {
            return activityType == typeof (TransitionActivity);
        }
    }
}
