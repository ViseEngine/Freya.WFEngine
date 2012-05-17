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
    public class WorkflowContext<TItem>
    {
        public WorkflowContext(Workflow<TItem> workflow, TItem item, string activityName, Type activityType, string state) {
            this.Workflow = workflow;
            this.Item = item;
            this.ActivityName = activityName;
            this.ActivityType = activityType;
            this.State = state;
        }

        public string ActivityName { get; private set; }
        
        public Type ActivityType { get; private set; }
        
        public TItem Item { get; private set; }

        public string State { get; private set; }

        public Workflow<TItem> Workflow { get; private set; }
    }
}
