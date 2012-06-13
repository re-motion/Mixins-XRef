using System.Collections.Generic;
using System.Linq;

namespace MixinXRef.UnitTests.TestDomain.FastMethodInvoker
{
  public class ClassWithMethods
  {
    public int Count<T> (IEnumerable<T> a)
    {
      return a.Count ();
    }

    public int Count<T> (IEnumerable<T> a, T b)
    {
      return a.Count () + b.ToString ().Length;
    }

    public int Count<T1, T2> (IEnumerable<T1> a, T2 b)
    {
      return a.Count () + b.ToString ().Length;
    }

    public static int Count<T> (IEnumerable<T> a, int b)
    {
      return a.Count () + b;
    }
  }
}
