using System;
using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Utility;
using System.Linq;

namespace MixinXRef.Reflection
{
  public class FastMemberInvokerCache
  {
    public class CacheKey
    {
      private readonly Type _declaringType;
      private readonly string _memberName;
      private readonly Type[] _argumentTypes;

      public CacheKey(Type declaringType, string memberName, Type[] argumentTypes)
      {
        ArgumentUtility.CheckNotNull ("declaringType", declaringType);
        ArgumentUtility.CheckNotNull ("memberName", memberName);
        ArgumentUtility.CheckNotNull ("argumentTypes", argumentTypes);

        _declaringType = declaringType;
        _memberName = memberName;
        _argumentTypes = argumentTypes;
      }

      public override int GetHashCode ()
      {
        return _declaringType.GetHashCode () 
            ^ _memberName.GetHashCode ()
            ^ _argumentTypes.Length;
      }

      public override bool Equals (object obj)
      {
        var other = (CacheKey) obj;
        return _declaringType == other._declaringType
            && _memberName == other._memberName
            && _argumentTypes.Length == other._argumentTypes.Length
            && _argumentTypes.SequenceEqual (other._argumentTypes);
      }
    }

    private readonly FastMemberInvokerGenerator _generator = new FastMemberInvokerGenerator ();

    private readonly Dictionary<CacheKey, Func<object, object[], object>> _cache = new Dictionary<CacheKey, Func<object, object[], object>> ();

    public Func<object, object[], object> GetOrCreateFastMethodInvoker (
        Type declaringType, 
        string methodName, 
        Type[] argumentTypes, 
        BindingFlags bindingFlags)
    {
      Func<object, object[], object> invoker;

      var key = new CacheKey(declaringType, methodName, argumentTypes);
      if (!_cache.TryGetValue (key, out invoker))
      {
        invoker = _generator.GetFastMethodInvoker (declaringType, methodName, argumentTypes, bindingFlags);
        _cache.Add (key, invoker);
      }

      return invoker;
    }
  }
}