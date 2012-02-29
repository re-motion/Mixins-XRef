using System;
using System.Collections.Generic;
using System.Reflection;

namespace MixinXRef.Reflection.Remotion
{
  public interface IRemotionReflector
  {
    IEnumerable<string> GetRemotionAssemblyNames ();

    bool IsNonApplicationAssembly (Assembly assembly);
    bool IsConfigurationException (Exception exception);
    bool IsValidationException (Exception exception);
    bool IsInfrastructureType (Type type);
    bool IsInheritedFromMixin (Type type);

    ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext);
    ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies);
    
    Assembly FindRemotionAssembly (Assembly[] assemblies);
  }
}