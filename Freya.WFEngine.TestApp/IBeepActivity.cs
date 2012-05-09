using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public interface IBeepActivity : IActivity
    {
        [InvocationMethod]
        string Invoke();
    }
}
