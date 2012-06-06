using System;
using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Utility;
using System.Linq;

namespace MixinXRef.Reflection.Utility
{
  public class FastMemberInvokerCache
  {
    public class CacheKey
    {
      private readonly Type _declaringType;
      private readonly string _memberName;
      private readonly Type[] _typeParameters;
      private readonly Type[] _argumentTypes;

      public CacheKey(Type declaringType, string memberName, Type[] typeParameters, Type[] argumentTypes)
      {
        ArgumentUtility.CheckNotNull ("declaringType", declaringType);
        ArgumentUtility.CheckNotNull ("memberName", memberName);
        ArgumentUtility.CheckNotNull ("argumentTypes", argumentTypes);

        _declaringType = declaringType;
        _memberName = memberName;
        _typeParameters = typeParameters;
        _argumentTypes = argumentTypes;
      }

      public override int GetHashCode ()
      {
        return _declaringType.GetHashCode () 
            ^ _memberName.GetHashCode ()
            ^ _typeParameters.Length
            ^ _argumentTypes.Length;
      }

      public override bool Equals (object obj)
      {
        var other = (CacheKey) obj;
        return _declaringType == other._declaringType
            && _memberName == other._memberName
            && _typeParameters.Length == other._typeParameters.Length
            && _typeParameters.SequenceEqual(other._typeParameters)
            && _argumentTypes.Length == other._argumentTypes.Length
            && _argumentTypes.SequenceEqual (other._argumentTypes);
      }
    }

    private readonly FastMemberInvokerGenerator _generator = new FastMemberInvokerGenerator ();

    private readonly Dictionary<CacheKey, Func<object, object[], object>> _cache = new Dictionary<CacheKey, Func<object, object[], object>> ();

    public Func<object, object[], object> GetOrCreateFastMethodInvoker (Type declaringType, string methodName, Type[] typeParameters, Type[] argumentTypes, BindingFlags bindingFlags)
    {
      Func<object, object[], object> invoker;

      var key = new CacheKey(declaringType, methodName, typeParameters, argumentTypes);
      if (!_cache.TryGetValue (key, out invoker))
      {
        invoker = _generator.GetFastMethodInvoker (declaringType, methodName, typeParameters, argumentTypes, bindingFlags);
        _cache.Add (key, invoker);
      }

      return invoker;
    }
  }
}