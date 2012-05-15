using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", MaxVersion = "1.13.93")]
  public class OldMixinDependenciesReflector : RemotionReflectorBase
  {
    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return new ReflectedObject("OldMixinDependenciesReflector");
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return new ReflectedObject ("OldMixinDependenciesReflector");
    }
  }
}
