using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public class BeepActivity : SingleExitPointActivity, IBeepActivity
    {
        public BeepActivity(string exitState) : base(exitState) {
        }

        public void Invoke() {
            Console.Beep();
            this.Context.State = ExitState;
        }
    }
}
