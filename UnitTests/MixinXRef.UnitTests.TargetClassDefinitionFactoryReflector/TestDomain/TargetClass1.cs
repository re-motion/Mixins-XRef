using System;

namespace MixinXRef.UnitTests.TargetClassDefinitionFactoryReflector.TestDomain
{
  public class TargetClass1 : IDisposable
  {
    public void Dispose ()
    {
      throw new NotImplementedException();
    }
  }
}