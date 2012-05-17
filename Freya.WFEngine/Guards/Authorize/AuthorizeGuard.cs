#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Freya.WFEngine
{
    public class AuthorizeGuard<TItem> : IGuard<TItem>
    {
        public bool Check(WorkflowContext<TItem> item) {
            return Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity.IsAuthenticated;
        }
    }
}
