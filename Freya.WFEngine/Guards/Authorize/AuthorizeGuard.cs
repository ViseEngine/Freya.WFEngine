using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Freya.WFEngine
{
    public class AuthorizeGuard<TItem> : IGuard<TItem>
    {
        public bool Check(TItem item) {
            return Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }
    }
}
