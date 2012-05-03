using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public abstract class Activity<TItem> : Activity, IActivity<TItem>
    {
        public TItem Item { get; set; }
    }

    public abstract class Activity : IActivity
    {

        public const string DefaultExitPoint = "Default";

        private string name;
        public string Name {
            get { return this.name; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length == 0)
                    throw new ArgumentException("Name cannot be an empty string.", "value");

                this.name = value;
            }
        }

        public virtual IActivity BaseActivity {
            get { return this; }
        }
    }
}
