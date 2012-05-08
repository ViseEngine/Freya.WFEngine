using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine.Guards.Authorize
{
    public class AuthorizeGuardFactory<TItem> : IXmlComponentFactory<IGuard<TItem>>
    {
        private static readonly AuthorizeGuard<TItem> defaultInstance = new AuthorizeGuard<TItem>();

        public bool CanHandle(Type componentType) {
            return componentType == typeof (AuthorizeGuard<TItem>);
        }

        public IGuard<TItem> CreateComponent(Type componentType, XmlElement configuration) {
            return defaultInstance;
        }
    }
}
