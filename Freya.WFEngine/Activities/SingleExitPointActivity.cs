using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public abstract class SingleExitPointActivity : Activity
    {
        protected SingleExitPointActivity(string exitState)
        {
            if (exitState == null)
                throw new ArgumentNullException("exitState");

            this.ExitState = exitState;
        }

        public virtual string ExitState {
            get;
            private set;
        }
    }
}
