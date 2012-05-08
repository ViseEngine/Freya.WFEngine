using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public class ActivityContext
    {
        public string Name { get; set; }
        public string State { get; set; }
        public object Item { get; set; }
    }
}
