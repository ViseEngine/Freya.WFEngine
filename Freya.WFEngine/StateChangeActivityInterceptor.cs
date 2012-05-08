using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public class StateChangeActivityInterceptor<TItem>: IInterceptor
    {
        public StateChangeActivityInterceptor(Workflow<TItem> workflow, TItem item) {
            if (workflow    == null)
                throw new ArgumentNullException("workflow");
            
            this.workflow = workflow;
            this.item = item;
        }

        private readonly Workflow<TItem> workflow;
        private readonly TItem item;

        public void Intercept(IInvocation invocation) {
            invocation.Proceed();
            string exitState = (string) invocation.ReturnValue;
            workflow.StateManager.ChangeState(item, exitState);
        }
    }
}
