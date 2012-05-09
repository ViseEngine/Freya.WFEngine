using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public class BeepActivity : Activity, IBeepActivity
    {
        private readonly string exitState;

        public BeepActivity(string exitState) {
            if (exitState == null)
                throw new ArgumentNullException("exitState");
            
            this.exitState = exitState;
        }

        public string Invoke() {
            Console.Beep();
            return exitState;
        }
    }
}
