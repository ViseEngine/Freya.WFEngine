using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class ActivityContext
    {
        public ActivityContext(string name, object item, string state) {
            this.Name = name;
            this.Item = item;
            this.State = state;
        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the current state of the item. Use the setter to set new state.
        /// </summary>
        public string State { get; set; }

        public object Item { get; private set; }
    }
}
