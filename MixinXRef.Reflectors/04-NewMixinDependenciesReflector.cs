using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", MinVersion = "1.13.94")]
  public class NewMixinDependenciesReflector : RemotionReflectorBase
  {
    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return new ReflectedObject ("NewMixinDependenciesReflector");
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return new ReflectedObject ("NewMixinDependenciesReflector");
    }
  }
}
