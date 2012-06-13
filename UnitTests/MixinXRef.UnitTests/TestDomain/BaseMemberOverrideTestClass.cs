using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class BaseMemberOverrideTestClass
  {
    public class TargetBase
    {
      public virtual void OverriddenMethod ()
      {
      }
    }

    public class Target : TargetBase
    {
    }

    public class Mixin1 : Mixin<Target>
    {
      [OverrideTarget]
      public void OverriddenMethod()
      {
      }
    }
  }
}