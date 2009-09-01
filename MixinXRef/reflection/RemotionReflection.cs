using System;
using System.Linq;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public class RemotionReflection : IRemotionReflection
  {
    private readonly Assembly _remotionAssembly;

    public RemotionReflection (Assembly remotionAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);

      _remotionAssembly = remotionAssembly;
    }

    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }

    public bool IsConfigurationException (Exception exception)
    {
      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public bool IsValidationException (Exception exception)
    {
      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public bool IsInfrastructureType (Type type)
    {
      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration)
    {
      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }
  }
}