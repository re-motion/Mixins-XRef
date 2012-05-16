using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeStore : IEnumerable<InvolvedType>
  {
    public static InvolvedTypeStore LastInstance { get; private set; }

    private readonly Dictionary<Type, InvolvedType> _involvedTypes = new Dictionary<Type, InvolvedType>();

    public InvolvedTypeStore()
    {
      LastInstance = this;
    }

    public InvolvedType GetOrCreateValue (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (!_involvedTypes.ContainsKey (key))
        _involvedTypes.Add (key, InvolvedType.FromType (key));

      return _involvedTypes[key];
    }

    public IEnumerator<InvolvedType> GetEnumerator()
    {
      return _involvedTypes.Values.OrderBy (t => t.Type.FullName).GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}