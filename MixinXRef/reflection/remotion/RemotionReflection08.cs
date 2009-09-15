using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflection08 : IRemotionReflection
  {
    private readonly Assembly _remotionAssembly;

    public RemotionReflection08 (Assembly remotionAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);

      _remotionAssembly = remotionAssembly;
    }

    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }

    public bool IsConfigurationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public bool IsValidationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType ("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", assemblies);
    }

    public Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName().Name == "Remotion");
    }
  }
}