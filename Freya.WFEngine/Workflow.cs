#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.DynamicProxy;

namespace Freya.WFEngine
{
    /// <summary>
    /// Provides information about a specific workflow and allows you to build one.
    /// </summary>
    /// <typeparam name="TItem">Type of item the workflow is tailored to.</typeparam>
    public class Workflow<TItem> : IWorkflow
    {
        #region ctor
        public Workflow(IStateManager<TItem> stateManager) {
            if (stateManager == null)
                throw new ArgumentNullException("stateManager");

            this.StateManager = stateManager;

            this.ActivityFactory = new CompositeComponentFactory<IActivity>();
            this.ActivityFactory.Factories.Add(new DefaultActivityFactory());
            this.ActivityFactory.Factories.Add(new TransitionActivityFactory());
            
            this.GuardFactory = new CompositeComponentFactory<IGuard>();

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

        IStateManager<object> IWorkflow.StateManager {
            get { return (IStateManager<object>) this.StateManager; }
        } 

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
        public CompositeComponentFactory<IActivity> ActivityFactory {
            get; private set;
        }

        /// <summary>
        /// Gets the activity guard factory.
        /// </summary>
        public CompositeComponentFactory<IGuard> GuardFactory {
            get; private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a state to the workflow.
        /// </summary>
        /// <returns><c>true</c> when the state is added, <c>false</c> when the state has been already registered.</returns>
        public void AddState(string stateName) {
            if (this.activities.ContainsKey(stateName) == false) {
                this.activities.Add(stateName, new List<ActivityRegistration>());
            } else {
                throw new ArgumentException(string.Format("State {0} is already registered.", stateName));
            }
        }

        /// <summary>
        /// Adds an activity guard to the workflow.
        /// </summary>
        /// <param name="state">state name</param>
        /// <param name="activityType">activity type</param>
        /// <param name="activityName">activity name</param>
        /// <param name="guardType">guard type</param>
        /// <param name="parameters">guard parameters used in instantiating</param>
        public void AddGuard(string state, Type activityType, string activityName, Type guardType, IDictionary<string, object> parameters)
        {
            #region parameter check

            if (state == null)
                throw new ArgumentNullException("state");

            if (activityType == null)
                throw new ArgumentNullException("activityType");

            if (guardType == null)
                throw new ArgumentNullException("guardType");

            // autofill generic type arguments
            if (guardType.IsGenericTypeDefinition && guardType.GetGenericArguments().Length == 1) {
                guardType = guardType.MakeGenericType(typeof (TItem));
            }
            
            if (typeof(IGuard).IsAssignableFrom(guardType) == false) {
                throw new ArgumentException(string.Format("Guard type must be subtype of IGuard"));
            }
                

            if (this.activities.ContainsKey(state) == false)
                throw new ArgumentException(string.Format("State '{0}' has not been registered.", state), "state");

            ActivityRegistration activityRegistration = activities[state].SingleOrDefault(ar => ar.Type == activityType && ar.Name == activityName);
            if (activityRegistration == null)
                throw new ArgumentException(string.Format("No matching activity found for type {0} and name '{1}'", activityType, activityName));
            #endregion

            GuardDescription guardRegistration = new GuardDescription(guardType, parameters);

            activityRegistration.Guards.Add(guardRegistration);
        }

        /// <summary>
        /// Adds an activity for the specified <paramref name="state"/>.
        /// </summary>
        /// <param name="state">state name</param>
        /// <param name="activityType">type of the activity</param>
        /// <param name="parameters">activity parameters</param>
        /// <param name="activityName">activity name (optional)</param>
        public void AddActivity(string state, Type activityType, IDictionary<string, object> parameters, string activityName = null)
        {
            #region Param check
            if (this.States.Contains(state) == false)
                throw new ArgumentException(string.Format("State '{0}' has not been registered yet.", state), "state");

            if (this.activities[state].Any(r => r.Name == activityName && r.Type == activityType))
                throw new ArgumentException(string.Format("Activity with same name and type has been already registered for state {0}", state), "activityName");

            if (activityType == null)
                throw new ArgumentNullException("activityType");

            if (typeof(IActivity).IsAssignableFrom(activityType) == false)
                throw new ArgumentException(string.Format("Parameter activityType ({0}) must be a subtype of IActivity interface", activityType.FullName), "activityType");
            #endregion


            ActivityRegistration ar = new ActivityRegistration {
                                                                   Name = activityName,
                                                                   Parameters = parameters,
                                                                   Type = activityType
                                                               };

            this.activities[state].Add(ar);
        }

        IEnumerable<IActivity> IWorkflow.GetActivitiesForItem(object item) {
            return this.GetActivitiesForItem((TItem) item);
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

        IEnumerable<TActivity> IWorkflow.GetActivitiesForItem<TActivity>(object item) {
            return this.GetActivitiesForItem<TActivity>((TItem) item);
        }

        /// <summary>
        /// Returns all invokable activities of type <typeparamref name="TActivity"/> for the specified <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity. Must be an interface.</typeparam>
        public IEnumerable<TActivity> GetActivitiesForItem<TActivity>(TItem item) where TActivity : IActivity
        {
            if (typeof(TActivity).IsInterface == false)
                throw new ArgumentException("TActivity must be an interface.");

            return this.GetActivitiesForItem(item).OfType<TActivity>();
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Creates the activity and wraps it in a proxy
        /// </summary>
        private IActivity CreateActivity(ActivityRegistration activityRegistration, TItem item, string currentState) {
            IActivity baseActivity = this.ActivityFactory.CreateComponent(activityRegistration.Type, activityRegistration.Parameters);
            baseActivity.Context = new ActivityContext(activityRegistration.Name, item, currentState);

            Type[] additionalInterfaces = baseActivity.GetType().GetInterfaces();
            
            return (IActivity)proxyGenerator.CreateInterfaceProxyWithTarget(typeof(IActivity), additionalInterfaces, baseActivity, proxyGenerationOptions, this.interceptors);
        }

        private IEnumerable<ActivityRegistration> FilterByGuards(IEnumerable<ActivityRegistration> activityRegistrations, TItem item, string state) {
            return activityRegistrations.Where(ar => ar.Guards.All(gr => {
                                                                       WorkflowContext context = new WorkflowContext(this, item, ar.Name, ar.Type, state);
                                                                       return this.GuardFactory.CreateComponent(gr.Type, gr.Parameters).Check(context);
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
                handler(this, activity);
        }


        private static readonly object eventPostInvokeKey = new object();
        /// <summary>
        /// Occurs after an activity is invoked.
        /// </summary>
        public event WorkflowInvocationDelegate PostInvoke {
            add { eventHandlerList.AddHandler(eventPostInvokeKey, value);}
            remove { eventHandlerList.RemoveHandler(eventPostInvokeKey, value);}
        }
        private void OnPostInvoke(IActivity activity)
        {
            WorkflowInvocationDelegate handler = eventHandlerList[eventPostInvokeKey] as WorkflowInvocationDelegate;
            if (handler != null)
                handler(this, activity);
        }
        // ReSharper restore StaticFieldInGenericType
        #endregion

        #region Interception
        internal void Intercept(IInvocation invocation) {
            IActivity activity = (IActivity)invocation.InvocationTarget;
            OnPreInvoke(activity);

            invocation.Proceed();

            string newState = activity.Context.State;
            OnPostInvoke(activity);

            this.StateManager.ChangeState((TItem)activity.Context.Item, newState);
            IAutoTriggerActivity autoTriggerActivity = FindAutoTriggerActivity((TItem)activity.Context.Item, newState);
            if (autoTriggerActivity != null)
                autoTriggerActivity.Invoke();
        }
        #endregion
    }
}
