#region License
// 
// Author: Lukas Paluzga <sajagi@freya.cz>
// Copyright (c) 2012, Lukas Paluzga
//  
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
//  
#endregion
#if !NET40
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
    internal interface ITuple
    {
        // Methods
        int GetHashCode(IEqualityComparer comparer);
        string ToString(StringBuilder sb);

        // Properties
        int Size { get; }
    }
}
#endif