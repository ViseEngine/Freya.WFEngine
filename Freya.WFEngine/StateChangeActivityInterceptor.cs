using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public class StateChangeActivityInterceptor<TItem>: IInterceptor
    {
        public StateChangeActivityInterceptor(Workflow<TItem> workflow, IDictionary<string, string> exitPointMapping, TItem item) {
            if (workflow    == null)
                throw new ArgumentNullException("workflow");
            
            if (exitPointMapping == null)
                throw new ArgumentNullException("exitPointMapping");

            this.exitPointMapping = exitPointMapping;
            this.workflow = workflow;
            this.item = item;
        }

        private readonly Workflow<TItem> workflow;
        private readonly TItem item;
        private readonly IDictionary<string, string> exitPointMapping; 

        public void Intercept(IInvocation invocation) {
            invocation.Proceed();
            string result = (string) invocation.ReturnValue;
            string exitState = exitPointMapping[result];
            workflow.StateManager.ChangeState(item, exitState);
            invocation.ReturnValue = exitState;
        }
    }
}
