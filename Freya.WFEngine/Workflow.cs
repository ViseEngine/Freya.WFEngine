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
        #region ctor
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
        #endregion

        #region Fields
        private readonly IDictionary<string, List<ActivityRegistration>> activities = new Dictionary<string, List<ActivityRegistration>>(StringComparer.InvariantCulture);
        private readonly ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions(ActivityProxyGenerationHook.DefaultInstance);
        private readonly ProxyGenerator proxyGenerator;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the item state manager.
        /// </summary>
        public IStateManager<TItem> StateManager { get; private set; }

        /// <summary>
        /// Gets registered workflow states.
        /// </summary>
        public ICollection<string> States {
            get { return this.activities.Keys; }
        }

        private IXmlComponentFactory<IActivity> xmlActivityFactory;
        /// <summary>
        /// Gets or sets the activity factory.
        /// </summary>
        public IXmlComponentFactory<IActivity> XmlActivityFactory {
            get { return this.xmlActivityFactory; }

            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.xmlActivityFactory = value;
            }
        }

        private IXmlComponentFactory<IGuard<TItem>> xmlGuardFactory;
        /// <summary>
        /// Gets or sets the activity guard factory.
        /// </summary>
        public IXmlComponentFactory<IGuard<TItem>> XmlGuardFactory {
            get { return this.xmlGuardFactory; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.xmlGuardFactory = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Adds a state to the workflow.
        /// </summary>
        /// <returns><c>true</c> when the state is added, <c>false</c> when the state has been already registered.</returns>
        public bool AddState(string stateName) {
            if (this.activities.ContainsKey(stateName) == false) {
                this.activities.Add(stateName, new List<ActivityRegistration>());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an activity for the specified <paramref name="state"/>.
        /// </summary>
        public void AddActivity(string state, Type activityType, XmlElement configuration, string activityName = null)
        {
            #region parameter check
            if (this.States.Contains(state) == false)
                throw new ArgumentException(string.Format("State '{0}' has not been registered yet.", state), "state");

            if (this.activities[state].Any(r => r.Name == activityName && r.Type == activityType))
                throw new ArgumentException(string.Format("Activity with same name and type has been already registered for state {0}", state), "activityName");

            if (activityType == null)
                throw new ArgumentNullException("activityType");
            
            if (typeof(IActivity).IsAssignableFrom(activityType) == false)
                throw new ArgumentException(string.Format("Parameter activityType ({0}) must be a subtype of IActivity interface", activityType.FullName), "activityType");

            if (configuration == null)
                throw new ArgumentNullException("configuration");
            #endregion

            ActivityRegistration ar = new ActivityRegistration
            {
                Configuration = (XmlElement)configuration.CloneNode(true),
                Name = activityName,
                Type = activityType
            };

            this.activities[state].Add(ar);
        }

        /// <summary>
        /// Returns all invokable activities for the specified <paramref name="item"/>.
        /// </summary>
        public IEnumerable<IActivity> GetActivitiesForItem(TItem item) {
            string currentState = this.StateManager.GetCurrentState(item);

            IEnumerable<ActivityRegistration> activityRegistrations = this.activities[currentState];
            
            // check all guards
            activityRegistrations = activityRegistrations.Where(ar => ar.Guards.All(gr => this.XmlGuardFactory.CreateComponent(gr.Type, gr.Configuration).Check(item)));

            return activityRegistrations.Select(activityRegistration => CreateActivity(activityRegistration, item, currentState));
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Creates the activity and wraps it in a proxy
        /// </summary>
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
        #endregion




    }
}
