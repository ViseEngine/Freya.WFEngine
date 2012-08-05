#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Freya.WFEngine
{
    /// <summary>
    /// Represents a description of a workflow activity
    /// </summary>
    public class ActivityDescription
    {
        public ActivityDescription(Type activityType)
            : this(activityType, null) {
        }

        public ActivityDescription(Type activityType, string name)
            : this(activityType, name, null) {
        }

        public ActivityDescription(Type activityType, string name, IDictionary<string, object> parameters) {
            if (activityType == null)
                throw new ArgumentNullException("activityType");

            if (typeof (IActivity).IsAssignableFrom(activityType) == false)
                throw new ArgumentException("Parameter activityType must be a subtype of IActivity.", "activityType");

            this.Type = activityType;
            this.Name = name;
            this.Parameters = parameters ?? new Dictionary<string, object>();

            this.Guards = new List<GuardDescription>();
        }

        /// <summary>
        /// Gets the list of activity guards
        /// </summary>
        public List<GuardDescription> Guards { get; private set; }

        /// <summary>
        /// Gets the activity name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the activity parameters
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the activity type
        /// </summary>
        public Type Type { get; private set; }
    }
}
