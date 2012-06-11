using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", "1.13.141")]
  public class MixinAssemblyReflector : RemotionReflectorBase
  {
    private readonly Assembly _mixinsAssembly;

    public MixinAssemblyReflector (string assemblyDirectory)
    {
      _mixinsAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.Mixins.dll");
    }

    public override bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return assembly.GetReferencedAssemblies ().Any (r => r.FullName == _mixinsAssembly.GetName ().FullName);
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName ().Name == "Remotion.Mixins";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _mixinsAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _mixinsAssembly.GetType ("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = _mixinsAssembly.GetType ("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
      return ReflectedObject.CallMethod (targetClassDefinitionFactoryType, "CreateTargetClassDefinition", classContext);
    }

    public override ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      ArgumentUtility.CheckNotNull ("validationException", validationException);

      return new ReflectedObject (validationException).GetProperty ("ValidationLogData");
    }
  }
}
