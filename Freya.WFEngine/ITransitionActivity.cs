using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public interface ITransitionActivity : IActivity
    {
        [InvocationMethod]
        string Invoke();
    }
}
