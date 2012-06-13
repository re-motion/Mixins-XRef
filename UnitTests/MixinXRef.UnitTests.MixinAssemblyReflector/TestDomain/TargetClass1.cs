using System;

namespace MixinXRef.UnitTests.MixinAssemblyReflector.TestDomain
{
  public class TargetClass1 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}