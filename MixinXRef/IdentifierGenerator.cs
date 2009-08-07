using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace MixinXRef
{
  /// <summary>
  /// Generates a identifier for a given item. When the same item is given twice, the same identifier will be returned.
  /// </summary>
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
  }
}