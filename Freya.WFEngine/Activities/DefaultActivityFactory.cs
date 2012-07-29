using System;
using System.Collections.Generic;

namespace Freya.WFEngine
{
    public class DefaultActivityFactory : IComponentFactory<IActivity>
    {
        private readonly IDictionary<Type, bool> canHandleDict = new Dictionary<Type, bool>(); 

        public virtual bool CanHandle(Type componentType) {
            bool result;
            if (canHandleDict.TryGetValue(componentType, out result) == false) {
                return canHandleDict[componentType] = CanHandleInternal(componentType);
            }

            return result;
        }

        private bool CanHandleInternal(Type componentType) {
            if (componentType.IsInterface)
                return false;

            return componentType.GetConstructor(new Type[0]) != null;
        }

        public virtual IActivity CreateComponent(Type componentType, IDictionary<string, object> parameters) {
            if (componentType == null)
                throw new ArgumentNullException("componentType");

            if (parameters != null && parameters.Count > 0)
                throw new ArgumentException("This implementation does not support parameters. Use your own implementation of IComponentFactory<IActivity> instead.");

            return (IActivity) Activator.CreateInstance(componentType);
        }
    }
}
