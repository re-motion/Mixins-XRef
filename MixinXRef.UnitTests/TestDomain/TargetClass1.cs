using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class TargetClass1 : UselessObject, IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}