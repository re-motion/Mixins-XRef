using System;

namespace MixinXRef.UnitTests.OldMixinDependenciesReflector.TestDomain
{
  public class TargetClass1 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}