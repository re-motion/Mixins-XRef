using System;
using System.Collections.Generic;
using System.Linq;

namespace MixinXRef.Utility
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<T> DistinctBy<T, TIdentity> (this IEnumerable<T> enumerable, Func<T, TIdentity> identity)
    {
      return enumerable.Distinct (new DelegateComparer<T, TIdentity> (identity));
    }

    private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
    {
      private readonly Func<T, TIdentity> _identity;

      public DelegateComparer (Func<T, TIdentity> identity)
      {
        _identity = identity;
      }

      public bool Equals (T x, T y)
      {
        return Equals (_identity (x), _identity (y));
      }

      public int GetHashCode (T obj)
      {
        return _identity (obj).GetHashCode ();
      }
    }
  }
}
