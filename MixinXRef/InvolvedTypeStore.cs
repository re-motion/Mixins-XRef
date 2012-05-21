using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeStore : IEnumerable<InvolvedType>
  {
    private readonly Dictionary<Type, InvolvedType> _involvedTypes = new Dictionary<Type, InvolvedType> ();

    public InvolvedType GetOrCreateValue (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (!_involvedTypes.ContainsKey (type))
        _involvedTypes.Add (type, new InvolvedType (type));

      return _involvedTypes[type];
    }

    public IEnumerator<InvolvedType> GetEnumerator ()
    {
      return _involvedTypes.Values.OrderBy (t => t.Type.FullName).GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }
  }
}