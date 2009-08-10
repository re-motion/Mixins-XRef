using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class MixinDoSomething : IDoSomething
  {
    [OverrideTarget]
    public void DoSomething ()
    {
      throw new NotImplementedException();
    }
  }
}