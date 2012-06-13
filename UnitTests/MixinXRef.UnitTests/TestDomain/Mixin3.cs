using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  [Extends (typeof(TargetClass2))]
  public class Mixin3 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}