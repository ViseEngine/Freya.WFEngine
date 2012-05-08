﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public interface IActivity
    {
        IActivity BaseActivity { get; }

        ActivityContext Context { get; set; }
    }

    public interface IActivity<TItem> : IActivity
    {
        TItem Item { get; set; }
    }
}
