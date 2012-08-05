using System;
using System.Collections.Generic;

namespace Freya.WFEngine
{
    public interface IWorkflow
    {
        /// <summary>
        /// Gets the item state manager.
        /// </summary>
        IStateManager StateManager { get; }

        /// <summary>
        /// Gets registered workflow states.
        /// </summary>
        StateSet States { get; }

        /// <summary>
        /// Gets the activity factory.
        /// </summary>
        CompositeComponentFactory<IActivity> ActivityFactory { get; }

        /// <summary>
        /// Gets the activity guard factory.
        /// </summary>
        CompositeComponentFactory<IGuard> GuardFactory { get; }

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