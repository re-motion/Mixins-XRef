using System;

namespace MixinXRef
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
  }
}