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

namespace Freya.WFEngine
{
    public abstract class Activity : IActivity
    {
        private ActivityContext context;

        public ActivityContext Context {
            get { return this.context; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.context = value;
            }
        }

        public virtual IActivity BaseActivity {
            get { return this; }
        }
    }
}
