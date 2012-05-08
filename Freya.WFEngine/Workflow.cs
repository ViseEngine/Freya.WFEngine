using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.DynamicProxy;
using Freya.WFEngine.Guards.Authorize;

namespace Freya.WFEngine
{
    public class Workflow<TItem>
    {
        public Workflow(IStateManager<TItem> stateManager) {
            if (stateManager == null)
                throw new ArgumentNullException("stateManager");

            this.StateManager = stateManager;

            CompositeXmlComponentFactory<IActivity> activityFactory = new CompositeXmlComponentFactory<IActivity>();
            activityFactory.Register(new TransitionActivityFactory());
            this.xmlActivityFactory = activityFactory;
            
            CompositeXmlComponentFactory<IGuard<TItem>> guardFactory = new CompositeXmlComponentFactory<IGuard<TItem>>();
            guardFactory.Register(new AuthorizeGuardFactory<TItem>());
            this.xmlGuardFactory = guardFactory;

            this.proxyGenerator = new ProxyGenerator();
        }

        readonly ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions(ActivityProxyGenerationHook.DefaultInstance);

        protected internal IStateManager<TItem> StateManager { get; private set; }

        private readonly ProxyGenerator proxyGenerator;

        private IXmlComponentFactory<IActivity> xmlActivityFactory;
        public IXmlComponentFactory<IActivity> XmlActivityFactory
        {
            get { return this.xmlActivityFactory; }

            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.xmlActivityFactory = value;
            }
        }

        private IXmlComponentFactory<IGuard<TItem>> xmlGuardFactory;
        public IXmlComponentFactory<IGuard<TItem>> XmlGuardFactory {
            get { return this.xmlGuardFactory; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.xmlGuardFactory = value;
            }
        }

        private readonly IDictionary<string, List<ActivityRegistration>> activities = new Dictionary<string, List<ActivityRegistration>>(StringComparer.InvariantCultureIgnoreCase);
        

        public bool AddState(string stateName) {
            if (this.activities.ContainsKey(stateName) == false) {
                this.activities.Add(stateName, new List<ActivityRegistration>());
                return true;
            }

            return false;
        }

        public void AddActivity(string state, Type activityType, XmlElement configuration, string activityName = null) {
            // check fox duplicit names
            if (this.activities[state].Any(r => r.Name == activityName))
#warning exception type fix
                throw new Exception();

            ActivityRegistration ar = new ActivityRegistration
            {
                Configuration = (XmlElement)configuration.CloneNode(true),
                Name = activityName,
                Type = activityType
            };

            this.activities[state].Add(ar);
        }

        public IEnumerable<IActivity> GetActivitiesForItem(TItem item) {
            string currentState = this.StateManager.GetCurrentState(item);
            return this.activities[currentState]
                .Where(ar => ar.Guards.All(gr => this.XmlGuardFactory.CreateComponent(gr.Type, gr.Configuration).Check(item)))
                .Select(activityRegistration => CreateActivity(activityRegistration, item, currentState));
        }

        private IActivity CreateActivity(ActivityRegistration activityRegistration, TItem item, string currentState) {
            IActivity baseActivity = this.XmlActivityFactory.CreateComponent(activityRegistration.Type, activityRegistration.Configuration);
            baseActivity.Context = new ActivityContext {
                                                              Item = item,
                                                              State = currentState,
                                                              Name = activityRegistration.Name
                                                          };

            var interceptor = new StateChangeActivityInterceptor<TItem>(this, item);
            Type[] additionalInterfaces = baseActivity.GetType().GetInterfaces();
            
            return (IActivity)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IActivity), additionalInterfaces, baseActivity, proxyGenerationOptions, new IInterceptor[] { interceptor });
        }

        

        
    }
}
