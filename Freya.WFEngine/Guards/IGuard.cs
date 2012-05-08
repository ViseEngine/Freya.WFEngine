using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public interface IGuard<TItem>
    {
        bool Check(TItem item);
    }
}
