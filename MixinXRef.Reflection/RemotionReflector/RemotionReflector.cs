using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MixinXRef.Reflection.RemotionReflector
{
  public class RemotionReflector : ReflectorProvider<IRemotionReflector>, IRemotionReflector
  {
    public RemotionReflector (string component, Version version, IEnumerable<_Assembly> assemblies, IEnumerable<object> constructParameters)
      : base (component, version, assemblies, constructParameters)
    { }

    public bool IsRelevantAssemblyForConfiguration (Assembly assembly)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsRelevantAssemblyForConfiguration (assembly);
    }

    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsNonApplicationAssembly (assembly);
    }

    public bool IsConfigurationException (Exception exception)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsConfigurationException (exception);
    }

    public bool IsValidationException (Exception exception)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsValidationException (exception);
    }

    public bool IsInfrastructureType (Type type)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsInfrastructureType (type);
    }

    public bool IsInheritedFromMixin (Type type)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).IsInheritedFromMixin (type);
    }

    public ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).GetTargetClassDefinition (targetType, mixinConfiguration, classContext);
    }

    public ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).BuildConfigurationFromAssemblies (assemblies);
    }

    public ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).GetValidationLogFromValidationException (validationException);
    }

    public ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).GetTargetCallDependencies (mixinDefinition);
    }

    public ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return GetCompatibleReflector (MethodBase.GetCurrentMethod ()).GetNextCallDependencies (mixinDefinition);
    }
  }
}