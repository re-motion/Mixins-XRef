using System;
using MixinXRef.UnitTests.TestDomain;
using Remotion.Mixins;

namespace WebAppWithWebConfigTest
{
  public class TestMixin : Mixin<IDoSomething>
  {
    [OverrideTarget]
    public void DoSomething ()
    {
      throw new NotImplementedException();
    }
  }
}