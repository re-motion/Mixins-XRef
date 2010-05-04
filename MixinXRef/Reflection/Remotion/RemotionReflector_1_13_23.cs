using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_13_23 : RemotionReflector_1_11_20
  {

    public RemotionReflector_1_13_23 (Assembly remotionAssembly, Assembly remotionInterfaceAssembly)
        : base(remotionAssembly, remotionInterfaceAssembly) { }


    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var classContext = mixinConfiguration.GetProperty ("ClassContexts").CallMethod ("GetWithInheritance", targetType);
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