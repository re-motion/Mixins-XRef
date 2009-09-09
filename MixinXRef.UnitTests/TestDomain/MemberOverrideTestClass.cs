using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class MemberOverrideTestClass
  {
    public class Target
    {
      [OverrideMixin]
      public void TemplateMethod ()
      {
      }
    }

    public class Mixin1 : Mixin<Target>
    {
      public virtual void TemplateMethod ()
      {
      }
    }
  }
}