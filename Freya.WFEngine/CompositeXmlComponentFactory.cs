using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    public class CompositeXmlComponentFactory<TComponent> : IXmlComponentFactory<TComponent>
    {
        private readonly List<IXmlComponentFactory<TComponent>> factories = new List<IXmlComponentFactory<TComponent>>();

        public void Register(IXmlComponentFactory<TComponent> activityFactory) {
            if (activityFactory == null)
                throw new ArgumentNullException("activityFactory");

            this.factories.Add(activityFactory);
        }

        public void RegisterAll(IEnumerable<IXmlComponentFactory<TComponent>> factoriesEnumeration) {
            this.factories.AddRange(factoriesEnumeration);
        }

        public bool CanHandle(Type activityType) {
            return this.factories.Any(f => f.CanHandle(activityType));
        }

        public TComponent CreateComponent(Type activityType, XmlElement configuration) {
            IXmlComponentFactory<TComponent> factory = this.factories.First(f => f.CanHandle(activityType));
            return factory.CreateComponent(activityType, configuration);
        }
    }
}
