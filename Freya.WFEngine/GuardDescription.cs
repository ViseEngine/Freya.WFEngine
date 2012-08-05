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
    /// Represents an activity guard description
    /// </summary>
    public class GuardDescription
    {
        public GuardDescription(Type type) : this(type, null) {
        }

        public GuardDescription(Type type, IDictionary<string, object> parameters) {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsGenericType == false || typeof(IGuard<>).IsAssignableFrom(type.GetGenericTypeDefinition()) == false)
                throw new ArgumentException("Parameter type must be a subtype of IGuard<>.", "type");
            
            this.Type = type;
            this.Parameters = parameters ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the guard type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the guard parameters
        /// </summary>
        public IDictionary<string, object> Parameters { get; private set; }
    }
}
