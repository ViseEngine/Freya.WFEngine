using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Castle.DynamicProxy;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;

namespace Freya.WFEngine.NinjectAdapter
{
    public class NinjectActivityFactory<TItem> : DefaultActivityFactory<TItem>
    {
        public NinjectActivityFactory(IKernel kernel) {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.Kernel = kernel;
        }

        public IKernel Kernel { get; private set; }

        protected override Activity CreateInstance(ActivityDescriptor activityDescriptor) {
            return (Activity) this.Kernel.Get(activityDescriptor.Type);
        }
    }
}
