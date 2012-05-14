using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class ClassOverridingInheritedMixinMethod
  {
    [OverrideMixin]
    public string ProtectedInheritedMethod()
    {
      return "ClassOverridingInheritedMixinMethod.ProtectedInheritedMethod";
    }
  }
}
