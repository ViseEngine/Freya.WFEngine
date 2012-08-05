using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    /// <summary>
    /// Represents a workflow state
    /// </summary>
    public class State
    {
        public State(string stateName) {
            if (stateName == null)
                throw new ArgumentNullException("stateName");

            this.Name = stateName;
            this.Activities = new List<ActivityDescription>();
        }

        /// <summary>
        /// Gets the state name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the list of activity descriptions
        /// </summary>
        public IList<ActivityDescription> Activities {
            get; private set;
        }  
    }
}
