using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", MinVersion = "1.13.94")]
  public class NewMixinDependenciesReflector : RemotionReflectorBase
  {
    public override ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);

      return mixinDefinition.GetProperty ("NextCallDependencies");
    }

    public override ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      ArgumentUtility.CheckNotNull ("mixinDefinition", mixinDefinition);

      return mixinDefinition.GetProperty ("TargetCallDependencies");
    }
  }
}
