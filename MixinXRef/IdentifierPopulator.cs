using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace MixinXRef
{
  public class IdentifierPopulator<T>
  {
    private readonly IdentifierGenerator<T> _identifierGenerator = new IdentifierGenerator<T>();


    public IdentifierPopulator (IEnumerable<T> items)
    {
      ArgumentUtility.CheckNotNull ("items", items);

      foreach (var item in items)
        _identifierGenerator.GetIdentifier (item);
    }

    public ReadonlyIdentifierGenerator<T> GetReadonlyIdentifierGenerator (string defaultValue)
    {
      return _identifierGenerator.GetReadonlyIdentiferGenerator (defaultValue);
    }
  }
}