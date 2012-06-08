using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", "1.11.20")]
  public class DefaultReflector : RemotionReflectorBase
  {
    private readonly Assembly _remotionAssembly;
    private readonly Assembly _remotionInterfaceAssembly;

    public DefaultReflector (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      _remotionAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.dll");
      _remotionInterfaceAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.Interfaces.dll");
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return assembly.GetReferencedAssemblies ().Any (r => r.FullName == _remotionInterfaceAssembly.GetName ().FullName);
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return assembly.GetCustomAttributes (false).Any (attribute => attribute.GetType ().Name == "NonApplicationAssemblyAttribute");
    }

    public override bool IsConfigurationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType ().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public override bool IsValidationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType ().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName ().Name == "Remotion.Interfaces";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType ("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      ArgumentUtility.CheckNotNull ("validationException", validationException);

      return new ReflectedObject (validationException).GetProperty ("ValidationLog");
    }
  }
}