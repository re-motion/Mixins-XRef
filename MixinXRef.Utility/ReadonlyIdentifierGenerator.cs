using System;
using System.Collections.Generic;

namespace MixinXRef.Utility
{
  public class ReadonlyIdentifierGenerator<T> : IIdentifierGenerator<T>
  {
    private readonly IIdentifierGenerator<T> _identifierGenerator;

    private readonly string _defaultValue;

    public ReadonlyIdentifierGenerator (IIdentifierGenerator<T> identifierGenerator, string defaultValue)
    {
      ArgumentUtility.CheckNotNull ("identifierGenerator", identifierGenerator);
      ArgumentUtility.CheckNotNull ("defaultValue", defaultValue);

      _identifierGenerator = identifierGenerator;
      _defaultValue = defaultValue;
    }

    public string GetIdentifier (T item)
    {
      return _identifierGenerator.GetIdentifier (item, _defaultValue);
    }

    public string GetIdentifier (T item, string defaultIfNotPresent)
    {
      return _identifierGenerator.GetIdentifier (item, defaultIfNotPresent);
    }

    public IEnumerable<T> Elements
    {
      get { return _identifierGenerator.Elements; }
    }
  }
}