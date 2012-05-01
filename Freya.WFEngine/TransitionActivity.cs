using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    [ExitPoint(DefaultExitPoint)]
    public class TransitionActivity : Activity, ITransitionActivity
    {
        public virtual string Invoke() {
            return DefaultExitPoint;
        }
    }
}
