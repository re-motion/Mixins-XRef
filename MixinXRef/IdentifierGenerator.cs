using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace MixinXRef
{
  /// <summary>
  /// Generates a identifier for a given <see cref="Assembly"/> object. When the same <see cref="Assembly"/> is given twice,
  /// the same identifier will be returned.
  /// </summary>
  public class IdentifierGenerator
  {
    private readonly Dictionary<Assembly, string> _identifiers = new Dictionary<Assembly, string>();
    
    public string GetIdentifier (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      if (!_identifiers.ContainsKey (assembly))
      {
        var newIdentifier = _identifiers.Count.ToString();
        _identifiers.Add (assembly, newIdentifier);
      }

      return _identifiers[assembly];
    }
  }
}