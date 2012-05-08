using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class CompositeXmlActivityFactory : IXmlActivityFactory
    {
        private readonly List<IXmlActivityFactory> factories = new List<IXmlActivityFactory>();

        public void Register(IXmlActivityFactory activityFactory) {
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");
            
            this.factories.Add(activityFactory);
        }

        public void RegisterAll(IEnumerable<IXmlActivityFactory> factoriesEnumeration) {
            this.factories.AddRange(factoriesEnumeration);
        }


        public bool CanHandle(Type activityType) {
            return this.factories.Any(f => f.CanHandle(activityType));
        }

        public IActivity CreateActivity(Type activityType, XmlElement configuration) {
            IXmlActivityFactory factory = this.factories.First(f => f.CanHandle(activityType));
            return factory.CreateActivity(activityType, configuration);
        }
    }
}
