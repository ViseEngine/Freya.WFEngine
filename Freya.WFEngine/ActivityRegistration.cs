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
    internal class ActivityRegistration
    {
        public ActivityRegistration() {
            this.Guards = new List<GuardRegistration>();
        }

        public ActivityRegistration(Type type, IDictionary<string, object> parameters, string name) : this() {
            if (type == null)
                throw new ArgumentNullException("type");

            this.Type = type;
            this.Parameters = parameters;
            this.Name = name;
        }

        
        public string Name { get; set; }
        public Type Type { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
 
        public List<GuardRegistration> Guards { get; private set; } 
    }
}
