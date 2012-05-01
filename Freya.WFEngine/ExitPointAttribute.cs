using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ExitPointAttribute : Attribute
    {
        public ExitPointAttribute(string exitPointName) {
            if (exitPointName == null)
                throw new ArgumentNullException("exitPointName");

            if (exitPointName.Length == 0)
                throw new ArgumentException("Exit point name cannot be an empty string.");

            this.Name = exitPointName;
        }

        public string Name { get; private set; }
    }
}
