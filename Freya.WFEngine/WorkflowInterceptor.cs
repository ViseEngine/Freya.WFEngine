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
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    internal class WorkflowInterceptor<TItem> : IInterceptor
    {

        public WorkflowInterceptor(Workflow<TItem> workflow) {
            if (workflow == null)
                throw new ArgumentNullException("workflow");

            this.workflow = workflow;
        }

        private readonly Workflow<TItem> workflow; 


        public void Intercept(IInvocation invocation) {
            workflow.Intercept(invocation);
        }
    }
}
