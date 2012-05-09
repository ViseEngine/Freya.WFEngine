#if !NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
    public interface IStructuralEquatable
    {
        // Methods
        bool Equals(object other, IEqualityComparer comparer);
        int GetHashCode(IEqualityComparer comparer);
    }
}
#endif
