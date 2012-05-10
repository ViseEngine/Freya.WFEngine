using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public interface IAutoTriggerActivity
    {
        [InvocationMethod]
        void Invoke();
    }
}
