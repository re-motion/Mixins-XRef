using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.UnitTests.CustomRemotionReflector
{
  public class CustomRemotionReflector : RemotionReflectorBase
  {
    public static readonly string[] RemotionAssemblyFileNames = new[] { "Remotion.dll", "Remotion.Interfaces.dll" };

    private readonly Assembly _remotionAssembly;
    private readonly Assembly _remotionInterfaceAssembly;

    public CustomRemotionReflector (string assemblyDirectory)
      : base (ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory), RemotionAssemblyFileNames)
    {
      _remotionAssembly = RemotionAssemblies[0];
      _remotionInterfaceAssembly = RemotionAssemblies[1];
    }

    public IEnumerable<string> GetRemotionAssemblyFileNames ()
    {
      return new[] { "Remotion.dll", "Remotion.Interfaces.dll" };
    }

    public void LoadRemotionAssemblies ()
    {
      throw new NotImplementedException();
    }

    public override bool IsNonApplicationAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType ().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
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

    public override Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName ().Name == "Remotion");
    }
  }
}