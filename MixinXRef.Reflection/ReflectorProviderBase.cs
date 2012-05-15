using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public abstract class ReflectorProviderBase
  {
    private class ReflectorVersionComparer : IComparer<ReflectorSupportAttribute>
    {
      public int Compare (ReflectorSupportAttribute x, ReflectorSupportAttribute y)
      {
        var xMin = x.GetMinVersion ();
        var xMax = x.GetMaxVersion ();
        var yMin = y.GetMinVersion ();
        var yMax = y.GetMaxVersion ();

        if (xMax != yMax)
        {
          if (xMax == null)
            return -1;
          else if (yMax == null)
            return 1;
          else
            return xMax.CompareTo (yMax);
        }
        else if (xMin != yMin)
        {
          if (xMin == null)
            return -1;
          else if (yMin == null)
            return 1;
          else
            return xMin.CompareTo (yMin);
        }
        else
          return 0;
      }
    }

    private static readonly IComparer<ReflectorSupportAttribute> s_reflectorVersionComparer = new ReflectorVersionComparer ();
    private static readonly FastMemberInvokerCache s_cache = new FastMemberInvokerCache ();

    protected string Component;
    protected Version Version;
    private IEnumerable<Type> _validTypes;
    private readonly IDictionary<MethodBase, MethodBase> _methods = new Dictionary<MethodBase, MethodBase> ();
    private readonly IDictionary<MethodBase, object> _instances = new Dictionary<MethodBase, object> ();

    public virtual object[] GetParameters ()
    {
      return new object[0];
    }

    protected abstract IEnumerable<Assembly> GetAssemblies ();

    private IEnumerable<Type> GetValidTypes ()
    {
      if (_validTypes == null)
        _validTypes = GetAssemblies ().SelectMany (a => a.GetExportedTypes ()).Where (IsValidReflector);
      return _validTypes;
    }

    protected T CallCompatibleMethod<T> (MethodBase methodBase, params object[] parameters)
    {
      var method = GetCompatibleMethod (methodBase);

      if (!_instances.ContainsKey (method))
        _instances.Add (method, CreateInstanceOf (method.DeclaringType, GetParameters ()));

      var invoker = s_cache.GetOrCreateFastMethodInvoker (_instances[method].GetType (), method.Name,
                                                         method.GetParameters ().Select (p => p.ParameterType).ToArray (),
                                                         BindingFlags.Public | BindingFlags.Instance);
      try
      {
        return (T) invoker (_instances[method], parameters);
      }
      catch (Exception ex)
      {
        throw new NotSupportedException (string.Format ("The reflector method {2} in {3} seems to be incompatible with version {0} of {1}", Version, Component, method, method.DeclaringType), ex);
      }
    }

    private MethodBase GetCompatibleMethod (MethodBase methodBase)
    {
      MethodBase method;
      if (!_methods.TryGetValue (methodBase, out method))
        _methods.Add (methodBase, method = FindCompatibleMethod (methodBase));
      return method;
    }

    private MethodBase FindCompatibleMethod (MethodBase methodBase)
    {
      var parameters = methodBase.GetParameters ().Select (p => p.ParameterType).ToArray ();
      var methods = GetValidTypes ()
        .Select (t => t.GetMethod (methodBase.Name, parameters))
        .Where (m => m != null && m.DeclaringType.GetAttribute<ReflectorSupportAttribute>() != null);

      if (!methods.Any ())
        throw new NotSupportedException ("There are no matching reflector methods for " + methodBase);

      return methods.OrderByDescending(m => m.DeclaringType.GetAttribute<ReflectorSupportAttribute>(),
                                       s_reflectorVersionComparer).First();
    }

    private static object CreateInstanceOf (Type type, object[] parameters)
    {
      try
      {
        return Activator.CreateInstance (type, parameters);
      }
      catch (MissingMethodException)
      {
        return Activator.CreateInstance (type);
      }
    }

    private bool IsValidReflector (Type type)
    {
      var attribute = type.GetAttribute<ReflectorSupportAttribute> ();
      if (attribute == null)
        return false;
      var minVersion = attribute.GetMinVersion ();
      var maxVersion = attribute.GetMaxVersion ();
      var isReflector = typeof (IRemotionReflector).IsAssignableFrom (type);
      return attribute != null &&
             attribute.Component == Component &&
             (minVersion == null || Version >= minVersion) &&
             (maxVersion == null || Version <= maxVersion) &&
             isReflector;
    }
  }
}