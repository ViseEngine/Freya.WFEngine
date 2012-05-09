using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class AutoTransitionActivity : TransitionActivity, IAutoTriggerActivity
    {
        public AutoTransitionActivity(string exitState) : base(exitState) {
        }
    }
}
