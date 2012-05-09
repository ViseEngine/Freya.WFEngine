#if !NET40
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
    public interface IStructuralComparable
    {
        // Methods
        int CompareTo(object other, IComparer comparer);
    }
}
#endif
