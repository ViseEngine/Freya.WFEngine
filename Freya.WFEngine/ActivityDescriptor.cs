using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class ActivityDescriptor
    {
        public ActivityDescriptor(string name, Type activityType) {
            if (activityType == null)
                throw new ArgumentNullException("activityType");

            if (name == null)
                throw new ArgumentNullException("name");

            if (typeof(Activity).IsAssignableFrom(activityType) == false)
                throw new ArgumentException(string.Format("Type {0} is not assignable from type {1}", typeof(Activity), activityType));

            if (name.Length == 0)
                throw new ArgumentException("Activity name cannot be an empty string.`");

            this.Type = activityType;
            this.Name = name;
            this.ExitPointMapping = new Dictionary<string, string>();
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public IDictionary<string, string> ExitPointMapping { get; private set; } 
    }
}
