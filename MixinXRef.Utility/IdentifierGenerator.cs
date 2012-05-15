using System;
using System.Collections.Generic;

namespace MixinXRef.Utility
{
  public class IdentifierGenerator<T> : IIdentifierGenerator<T>
  {
    private readonly Dictionary<T, string> _identifiers = new Dictionary<T, string>();

    public string GetIdentifier (T item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      if (!_identifiers.ContainsKey (item))
      {
        var newIdentifier = _identifiers.Count.ToString();
        _identifiers.Add (item, newIdentifier);
      }

      return _identifiers[item];
    }

    public string GetIdentifier (T item, string defaultIfNotPresent)
    {
      return _identifiers.ContainsKey (item) ? _identifiers[item] : defaultIfNotPresent;
    }

    public ReadonlyIdentifierGenerator<T> GetReadonlyIdentiferGenerator (string defaultValue)
    {
      return new ReadonlyIdentifierGenerator<T> (this, defaultValue);
    }
  }
}