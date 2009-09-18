using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.UnitTests.CustomRemotionReflector
{
  public class CustomRemotionReflector : IRemotionReflector
  {
    private readonly Assembly _remotionAssembly;
    private readonly Assembly _remotionInterfaceAssembly;

    public CustomRemotionReflector(Assembly remotionAssembly, Assembly remotionInterfaceAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);
      ArgumentUtility.CheckNotNull ("remotionInterfaceAssembly", remotionInterfaceAssembly);

      _remotionAssembly = remotionAssembly;
      _remotionInterfaceAssembly = remotionInterfaceAssembly;
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

    public bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
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