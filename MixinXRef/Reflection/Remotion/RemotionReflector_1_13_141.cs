using System;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_13_141 : RemotionReflector_1_13_23
  {
    private readonly Assembly _mixinsAssembly;

    public RemotionReflector_1_13_141 (string assemblyDirectory)
        : base (ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory))
    {
      _mixinsAssembly = AssemblyHelper.LoadFileOrNull (assemblyDirectory, "Remotion.Mixins.dll");
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