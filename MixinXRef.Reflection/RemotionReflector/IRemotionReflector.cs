using System;
using System.Reflection;
using MixinXRef.Reflection.Utility;

namespace MixinXRef.Reflection.RemotionReflector
{
  public interface IRemotionReflector
  {
    bool IsNonApplicationAssembly (Assembly assembly);
    bool IsConfigurationException (Exception exception);
    bool IsValidationException (Exception exception);
    bool IsInfrastructureType (Type type);
    bool IsInheritedFromMixin (Type type);

    ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext);
    ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies);
    ReflectedObject GetValidationLogFromValidationException (Exception validationException);
  }
}