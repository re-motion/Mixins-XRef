using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", MaxVersion = "1.13.93")]
  public class OldMixinDependenciesReflector : RemotionReflectorBase
  {
    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);

      return mixinDefinition.GetProperty ("BaseDependencies");
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);

      return mixinDefinition.GetProperty ("ThisDependencies");
    }
  }
}
