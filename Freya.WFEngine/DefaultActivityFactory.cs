using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public class DefaultActivityFactory<TItem> : IActivityFactory<TItem>
    {
        public virtual IActivity CreateActivity(TItem item, ActivityDescriptor activityDescriptor) {
            Activity activity = CreateInstance(activityDescriptor);
            Initialize(activity, item, activityDescriptor);
            return activity;
        }

        protected virtual Activity CreateInstance(ActivityDescriptor activityDescriptor) {
            return (Activity)Activator.CreateInstance(activityDescriptor.Type);
        }

        protected virtual void Initialize(Activity activity, TItem item, ActivityDescriptor activityDescriptor) {
            activity.Name = activityDescriptor.Name;
            if (activity is Activity<TItem>) {
                ((Activity<TItem>) activity).Item = item;
            }
        }


    }
}
