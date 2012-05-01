using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    public class Workflow<TItem>
    {
        public Workflow(IStateManager<TItem> stateManager) {
            if (stateManager == null)
                throw new ArgumentNullException("stateManager");

            this.StateManager = stateManager;
            this.activityFactory = new DefaultActivityFactory<TItem>();
            this.proxyGenerator = new ProxyGenerator();
        }

        protected internal IStateManager<TItem> StateManager { get; private set; }

        private readonly ProxyGenerator proxyGenerator;

        private IActivityFactory<TItem> activityFactory;
        public IActivityFactory<TItem> ActivityFactory
        {
            get { return this.activityFactory; }

            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.activityFactory = value;
            }
        }

        private readonly IDictionary<string, List<ActivityDescriptor>> activities = new Dictionary<string, List<ActivityDescriptor>>(StringComparer.InvariantCultureIgnoreCase);

        public void AddState(string stateName) {
            activities.Add(stateName, new List<ActivityDescriptor>());
        }

        public void AddActivity(string stateName, ActivityDescriptor activity) {
            if (activities.ContainsKey(stateName) == false)
                AddState(stateName);

            activities[stateName].Add(activity);
        }

        public IEnumerable<IActivity> GetActivitiesForItem(TItem item) {
            string currentState = this.StateManager.GetCurrentState(item);
            IEnumerable<ActivityDescriptor> activityDescriptors = this.activities[currentState];
            ProxyGenerationOptions options = new ProxyGenerationOptions(ActivityProxyGenerationHook.DefaultInstance);

            foreach (var ad in activityDescriptors) {
                IActivity baseActivity = this.ActivityFactory.CreateActivity(item, ad);
                var interceptor = new StateChangeActivityInterceptor<TItem>(this, ad.ExitPointMapping, item);
                Type[] additionalInterfaces = baseActivity.GetType().GetInterfaces();
                yield return (IActivity) proxyGenerator.CreateInterfaceProxyWithTarget(typeof (IActivity), additionalInterfaces, baseActivity, options, new IInterceptor[] { interceptor });
            }
        }
    }
}
