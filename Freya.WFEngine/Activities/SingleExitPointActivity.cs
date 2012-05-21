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
    public abstract class SingleExitPointActivity : Activity
    {
        public const string ExitPointParameterName = SR.DefaultExitStateAttributeName;

        protected SingleExitPointActivity(string exitState)
        {
            this.ExitState = exitState;
        }

        public virtual string ExitState {
            get;
            private set;
        }

        protected virtual void UpdateState() {
            if (this.ExitState != null)
                this.Context.State = this.ExitState;
        }
    }
}
