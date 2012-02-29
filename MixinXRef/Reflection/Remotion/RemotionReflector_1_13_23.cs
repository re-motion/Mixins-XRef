using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_13_23 : RemotionReflector_1_11_20
  {
    public new static readonly string[] RemotionAssemblyFileNames = new[] { "Remotion.dll", "Remotion.Interfaces.dll" };

    private readonly Assembly _remotionAssembly;

    public RemotionReflector_1_13_23 (string assemblyDirectory)
      : base (ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory))
    {
      _remotionAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.dll");
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = _remotionAssembly.GetType("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
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