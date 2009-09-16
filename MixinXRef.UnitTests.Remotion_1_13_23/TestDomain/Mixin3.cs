using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Remotion_1_13_23.TestDomain
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