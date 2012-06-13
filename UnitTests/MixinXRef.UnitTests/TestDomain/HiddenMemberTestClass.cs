using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class HiddenMemberTestClass
  {
    public class TargetBase
    {
      public void HiddenMethod ()
      {
      }
    }

    public class Target : TargetBase
    {
      public new void HiddenMethod ()
      {
      }
    }

    public class Mixin1 : Mixin<Target>
    {
    }
  }
}