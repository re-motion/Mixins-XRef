using System;

namespace MixinXRef.UnitTests.NewMixinDependenciesReflector.TestDomain
{
  public class TargetClass1 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}