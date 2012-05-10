using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            this.XmlActivityFactory = new CompositeXmlComponentFactory<IActivity>();
            this.XmlActivityFactory.Register(new TransitionActivityFactory());
            
            this.XmlGuardFactory = new CompositeXmlComponentFactory<IGuard<TItem>>();
            this.XmlGuardFactory.Register(new AuthorizeGuardFactory<TItem>());

            this.proxyGenerator = new ProxyGenerator();
            this.interceptors = new IInterceptor[] { new WorkflowInterceptor<TItem>(this) };
        }
        #endregion

        #region Fields
        private readonly IDictionary<string, List<ActivityRegistration>> activities = new Dictionary<string, List<ActivityRegistration>>(StringComparer.InvariantCulture);
        private readonly ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions(ActivityProxyGenerationHook.DefaultInstance);
        private readonly ProxyGenerator proxyGenerator;
        private readonly IInterceptor[] interceptors;
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

        /// <summary>
        /// Gets the activity factory.
        /// </summary>
        public CompositeXmlComponentFactory<IActivity> XmlActivityFactory {
            get; private set;
        }

        /// <summary>
        /// Gets the activity guard factory.
        /// </summary>
        public CompositeXmlComponentFactory<IGuard<TItem>> XmlGuardFactory {
            get; private set;
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
        /// Adds an activity guard to the workflow.
        /// </summary>
        /// <param name="state">state name</param>
        /// <param name="activityType">activity type</param>
        /// <param name="activityName">activity name</param>
        /// <param name="guardType">guard type</param>
        /// <param name="configuration">guard configuration</param>
        public void AddGuard(string state, Type activityType, string activityName, Type guardType, XmlElement configuration)
        {
            #region parameter check

            if (state == null)
                throw new ArgumentNullException("state");

            if (activityType == null)
                throw new ArgumentNullException("activityType");

            if (guardType == null)
                throw new ArgumentNullException("guardType");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            // autofill generic type arguments
            if (guardType.IsGenericTypeDefinition && guardType.GetGenericArguments().Length == 1) {
                guardType = guardType.MakeGenericType(typeof (TItem));
            }
            
            if (typeof(IGuard<TItem>).IsAssignableFrom(guardType) == false) {
                throw new ArgumentException(string.Format("Guard type must be subtype of IGuard<{0}>", typeof(TItem).FullName));
            }
                

            if (this.activities.ContainsKey(state) == false)
                throw new ArgumentException(string.Format("State '{0}' has not been registered.", state), "state");

            ActivityRegistration activityRegistration = activities[state].SingleOrDefault(ar => ar.Type == activityType && ar.Name == activityName);
            if (activityRegistration == null)
                throw new ArgumentException(string.Format("No matching activity found for type {0} and name '{1}'", activityType, activityName));
            #endregion
            
            GuardRegistration guardRegistration = new GuardRegistration {
                                                                            Configuration = configuration,
                                                                            Type = guardType
                                                                        };

            activityRegistration.Guards.Add(guardRegistration);
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

            // check all guards
            IEnumerable<ActivityRegistration> activityRegistrations = FilterByGuards(this.activities[currentState], item, currentState);

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

            Type[] additionalInterfaces = baseActivity.GetType().GetInterfaces();
            
            return (IActivity)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IActivity), additionalInterfaces, baseActivity, proxyGenerationOptions, this.interceptors);
        }

        private IEnumerable<ActivityRegistration> FilterByGuards(IEnumerable<ActivityRegistration> activityRegistrations, TItem item, string state) {
            return activityRegistrations.Where(ar => ar.Guards.All(gr => {
                                                                       WorkflowContext<TItem> context = new WorkflowContext<TItem>(this, item, ar.Name, ar.Type, state);
                                                                       return this.XmlGuardFactory.CreateComponent(gr.Type, gr.Configuration).Check(context);
                                                                   }));
        }

        private IAutoTriggerActivity FindAutoTriggerActivity(TItem item, string currentState) {
            IEnumerable<ActivityRegistration> activityRegistrations = this.activities[currentState].Where(ar => typeof (IAutoTriggerActivity).IsAssignableFrom(ar.Type));
            activityRegistrations = FilterByGuards(activityRegistrations, item, currentState);

            return activityRegistrations.Select(ar => CreateActivity(ar, item, currentState)).Cast<IAutoTriggerActivity>().FirstOrDefault();
        }
        #endregion

        #region Events
        // ReSharper disable StaticFieldInGenericType
        private readonly EventHandlerList eventHandlerList = new EventHandlerList();

        private static readonly object eventPreInvokeKey = new object();
        /// <summary>
        /// Occurs before an activity is invoked.
        /// </summary>
        public event WorkflowInvocationDelegate PreInvoke {
            add { eventHandlerList.AddHandler(eventPreInvokeKey, value);}
            remove { eventHandlerList.RemoveHandler(eventPreInvokeKey, value);}
        }
        
        private void OnPreInvoke(IActivity activity) {
            WorkflowInvocationDelegate handler = eventHandlerList[eventPreInvokeKey] as WorkflowInvocationDelegate;
            if (handler != null)
                handler(this, activity, activity.Context.State);
        }


        private static readonly object eventPostInvokeKey = new object();
        /// <summary>
        /// Occurs after an activity is invoked.
        /// </summary>
        public event WorkflowInvocationDelegate PostInvoke {
            add { eventHandlerList.AddHandler(eventPostInvokeKey, value);}
            remove { eventHandlerList.RemoveHandler(eventPostInvokeKey, value);}
        }
        private void OnPostInvoke(IActivity activity, string newState)
        {
            WorkflowInvocationDelegate handler = eventHandlerList[eventPostInvokeKey] as WorkflowInvocationDelegate;
            if (handler != null)
                handler(this, activity, newState);
        }
        // ReSharper restore StaticFieldInGenericType
        #endregion

        #region Interception
        internal void Intercept(IInvocation invocation) {
            IActivity activity = (IActivity)invocation.InvocationTarget;
            OnPreInvoke(activity);

            invocation.Proceed();

            string newState = (string)invocation.ReturnValue;
            OnPostInvoke(activity, newState);

            this.StateManager.ChangeState((TItem)activity.Context.Item, newState);
            IAutoTriggerActivity autoTriggerActivity = FindAutoTriggerActivity((TItem)activity.Context.Item, newState);
            if (autoTriggerActivity != null)
                autoTriggerActivity.Invoke();
        }
        #endregion
    }
}
