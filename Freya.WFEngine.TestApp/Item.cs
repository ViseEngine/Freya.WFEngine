using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine.TestApp
{
    public class Item : IStatefulItem
    {
        public int ID { get; set; }
        public string CurrentState { get; set; }
    }
}
