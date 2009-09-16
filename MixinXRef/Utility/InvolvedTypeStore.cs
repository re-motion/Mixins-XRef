using System;
using System.Collections.Generic;
using System.Linq;

namespace MixinXRef.Utility
{
  public class InvolvedTypeStore
  {
    private readonly Dictionary<Type, InvolvedType> _involvedTypes = new Dictionary<Type, InvolvedType>();

    public InvolvedType GetOrCreateValue (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (!_involvedTypes.ContainsKey (key))
        _involvedTypes.Add (key, new InvolvedType (key));

      return _involvedTypes[key];
    }

    public InvolvedType[] ToArray ()
    {
      return _involvedTypes.Values.ToArray();
    }
  }
}