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

namespace Freya.WFEngine
{
    public interface IStateManager
    {
        /// <summary>
        /// Returns current state of the specified item
        /// </summary>
        string GetCurrentState(object item);

        /// <summary>
        /// Sets new state for the specified item
        /// </summary>
        void ChangeState(object item, string newState);
    }

    public interface IStateManager<in TItem> : IStateManager
    {
        /// <summary>
        /// Returns current state of the specified item
        /// </summary>
        string GetCurrentState(TItem item);

        /// <summary>
        /// Sets new state for the specified item
        /// </summary>
        void ChangeState(TItem item, string newState);
    }
}
