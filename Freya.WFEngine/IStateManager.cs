using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public interface IStateManager<TItem>
    {
        string GetCurrentState(TItem item);

        void ChangeState(TItem item, string newState);
    }
}
