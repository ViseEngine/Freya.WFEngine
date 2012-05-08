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
        private ActivityContext context;

        public ActivityContext Context {
            get { return this.context; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.context = value;
            }
        }

        public virtual IActivity BaseActivity {
            get { return this; }
        }
    }
}
