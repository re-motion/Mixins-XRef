using System;
using System.Collections.Generic;
using System.Linq;

namespace MixinXRef
{
  public class InvolvedTypeStore
  {
    private readonly Dictionary<Type, InvolvedType> _involvedTypes = new Dictionary<Type, InvolvedType>();

    public InvolvedType GetOrCreateValue (Type key)
    {
      if (!_involvedTypes.ContainsKey (key))
      {
        _involvedTypes.Add (key, new InvolvedType (key, false, false));
      }

      return _involvedTypes[key];
    }

    public InvolvedType[] ToArray ()
    {
      return _involvedTypes.Values.ToArray();
    }
  }
}