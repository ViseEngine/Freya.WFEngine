using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    /// <summary>
    /// Interface for items tracking their own state
    /// </summary>
    public interface IStatefulItem
    {
        /// <summary>
        /// Gets or sets item's current state
        /// </summary>
        string CurrentState { get; set; }
    }
}
