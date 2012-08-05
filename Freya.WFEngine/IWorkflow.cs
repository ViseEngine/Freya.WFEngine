using System;
using System.Collections.Generic;

namespace Freya.WFEngine
{
    public interface IWorkflow
    {
        /// <summary>
        /// Gets the item state manager.
        /// </summary>
        IStateManager<object> StateManager { get; }

        /// <summary>
        /// Gets registered workflow states.
        /// </summary>
        ICollection<string> States { get; }

        /// <summary>
        /// Gets the activity factory.
        /// </summary>
        CompositeComponentFactory<IActivity> ActivityFactory { get; }

        /// <summary>
        /// Gets the activity guard factory.
        /// </summary>
        CompositeComponentFactory<IGuard> GuardFactory { get; }

        /// <summary>
        /// Adds a state to the workflow.
        /// </summary>
        /// <returns><c>true</c> when the state is added, <c>false</c> when the state has been already registered.</returns>
        void AddState(string stateName);

        /// <summary>
        /// Adds an activity for the specified <paramref name="state"/>.
        /// </summary>
        /// <param name="state">state name</param>
        /// <param name="activityType">type of the activity</param>
        /// <param name="parameters">activity parameters</param>
        /// <param name="activityName">activity name (optional)</param>
        ActivityDescription AddActivity(string state, Type activityType, IDictionary<string, object> parameters, string activityName = null);

        /// <summary>
        /// Returns all invokable activities for the specified <paramref name="item"/>.
        /// </summary>
        IEnumerable<IActivity> GetActivitiesForItem(object item);

        /// <summary>
        /// Returns all invokable activities of type <typeparamref name="TActivity"/> for the specified <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity. Must be an interface.</typeparam>
        IEnumerable<TActivity> GetActivitiesForItem<TActivity>(object item) where TActivity : IActivity;

        /// <summary>
        /// Occurs before an activity is invoked.
        /// </summary>
        event WorkflowInvocationDelegate PreInvoke;

        /// <summary>
        /// Occurs after an activity is invoked.
        /// </summary>
        event WorkflowInvocationDelegate PostInvoke;
    }
}