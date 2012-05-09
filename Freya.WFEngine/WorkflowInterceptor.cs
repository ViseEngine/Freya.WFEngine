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
