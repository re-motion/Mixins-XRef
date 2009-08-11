using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class MixinWithConfigurationError
  {
    [OverrideTarget]
    public void MethodThatDoesNotExistOnTarget()
    {
      throw new NotImplementedException();
    }
  }
}