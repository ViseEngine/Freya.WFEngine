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