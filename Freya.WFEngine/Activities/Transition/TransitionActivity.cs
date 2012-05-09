using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class TransitionActivity : SingleExitPointActivity, ITransitionActivity
    {
        public TransitionActivity(string exitState) : base(exitState) {
        }

        public virtual string Invoke() {
            return this.ExitState;
        }
    }
}
