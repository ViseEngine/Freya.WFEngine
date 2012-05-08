using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class TransitionActivity : Activity, ITransitionActivity
    {
        public TransitionActivity(string exitState) {
            if (exitState == null)
                throw new ArgumentNullException("exitState");

            this.ExitState = exitState;
        }

        public string ExitState {
            get;
            private set;
        }

        public virtual string Invoke() {
            return this.ExitState;
        }
    }
}
