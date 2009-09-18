using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_11_20 : IRemotionReflector
  {
    protected readonly Assembly _remotionAssembly;
    protected readonly Assembly _remotionInterfaceAssembly;

    public RemotionReflector_1_11_20 (Assembly remotionAssembly, Assembly remotionInterfaceAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);
      ArgumentUtility.CheckNotNull ("remotionInterfaceAssembly", remotionInterfaceAssembly);

      _remotionAssembly = remotionAssembly;
      _remotionInterfaceAssembly = remotionInterfaceAssembly;
    }

    public virtual bool IsNonApplicationAssembly(Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }

    public virtual bool IsConfigurationException(Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public virtual bool IsValidationException(Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public virtual bool IsInfrastructureType(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public virtual bool IsInheritedFromMixin(Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }


    public virtual ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public virtual ReflectedObject BuildConfigurationFromAssemblies(Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", assemblies);
    }

    public virtual Assembly FindRemotionAssembly(Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName().Name == "Remotion");
    }
  }
}