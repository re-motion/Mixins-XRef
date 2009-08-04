using System;
using System.Collections.Generic;
using System.Reflection;

namespace MixinXRef
{
  public class IdentifierGenerator
  {
    private readonly Dictionary<Assembly, string> _identifiers = new Dictionary<Assembly, string>();
    
    public string GetIdentifier (Assembly assembly)
    {
      if (!_identifiers.ContainsKey (assembly))
      {
        var newIdentifier = _identifiers.Count.ToString();
        _identifiers.Add (assembly, newIdentifier);
      }

      return _identifiers[assembly];
    }
  }
}