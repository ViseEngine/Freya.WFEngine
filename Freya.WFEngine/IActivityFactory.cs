using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public interface IActivityFactory<TItem>
    {
        IActivity CreateActivity(TItem item, ActivityDescriptor activityDescriptor);
    }
}
