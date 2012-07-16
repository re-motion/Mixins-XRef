using System;
using System.Reflection;

namespace MixinXRef.Reflection.RemotionReflector
{
  public interface IRemotionReflector
  {
    IRemotionReflector Initialize(string assemblyDirectory);

    bool IsRelevantAssemblyForConfiguration (Assembly assembly);
    bool IsNonApplicationAssembly (Assembly assembly);
    bool IsConfigurationException (Exception exception);
    bool IsValidationException (Exception exception);
    bool IsInfrastructureType (Type type);
    bool IsInheritedFromMixin (Type type);

    ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext);
    ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies);
    ReflectedObject GetValidationLogFromValidationException (Exception validationException);
    ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition);
    ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition);
  }
}