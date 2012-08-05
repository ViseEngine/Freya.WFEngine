using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    /// <summary>
    /// Default IStatefulItem implementation for items of type <see>IStatefulItem</see>.
    /// </summary>
    public class DefaultStateManager<TItem> : IStateManager<TItem> where TItem : IStatefulItem
    {
        string IStateManager.GetCurrentState(object item) {
            return GetCurrentState((TItem) item);
        }

        public virtual string GetCurrentState(TItem item) {
            return item.CurrentState;
        }

        void IStateManager.ChangeState(object item, string newState) {
            ChangeState((TItem) item, newState);
        }

        public virtual void ChangeState(TItem item, string newState) {
            if (item.CurrentState != newState) {
                item.CurrentState = newState;
                OnStateChanged(item, newState);    
            }
        }

        /// <summary>
        /// Invoked when the item's state changes
        /// </summary>
        protected virtual void OnStateChanged(TItem item, string newState) {
        }
    }
}
