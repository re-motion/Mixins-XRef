using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", "1.13.23")]
  public class TargetClassDefinitionFactoryReflector : RemotionReflectorBase
  {
    private readonly Assembly _remotionAssembly;

    public TargetClassDefinitionFactoryReflector (string assemblyDirectory)
    {
      _remotionAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.dll");
      AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.Interfaces.dll");
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = _remotionAssembly.GetType ("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
      return ReflectedObject.CallMethod (targetClassDefinitionFactoryType, "CreateTargetClassDefinition", classContext);
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return assembly.GetCustomAttributes (false).Any (
          attribute => attribute.GetType ().FullName == "Remotion.Reflection.TypeDiscovery.NonApplicationAssemblyAttribute");
    }
  }
}
