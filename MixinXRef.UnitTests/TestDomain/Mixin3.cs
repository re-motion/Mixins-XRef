using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class Mixin3 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}