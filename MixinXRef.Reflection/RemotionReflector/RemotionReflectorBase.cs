using System;
using System.Reflection;
using MixinXRef.Reflection.Utility;

namespace MixinXRef.Reflection.RemotionReflector
{
  public abstract class RemotionReflectorBase : IRemotionReflector
  {
    public virtual bool IsNonApplicationAssembly (Assembly assembly)
    {
      throw new NotSupportedException ();
    }

    public virtual bool IsConfigurationException (Exception exception)
    {
      throw new NotSupportedException ();
    }

    public virtual bool IsValidationException (Exception exception)
    {
      throw new NotSupportedException ();
    }

    public virtual bool IsInfrastructureType (Type type)
    {
      throw new NotSupportedException ();
    }

    public virtual bool IsInheritedFromMixin (Type type)
    {
      throw new NotSupportedException ();
    }

    public virtual ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      throw new NotSupportedException ();
    }

    public virtual ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      throw new NotSupportedException ();
    }

    public virtual ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      throw new NotSupportedException ();
    }

    public virtual ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      throw new NotSupportedException ();
    }

    public virtual ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      throw new NotSupportedException ();
    }
  }
}
