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

        public virtual void Invoke() {
            this.Context.State = this.ExitState;
        }
    }
}
