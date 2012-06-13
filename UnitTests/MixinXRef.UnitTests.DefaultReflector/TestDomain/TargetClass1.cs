using System;

namespace MixinXRef.UnitTests.DefaultReflector.TestDomain
{
  public class TargetClass1 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}