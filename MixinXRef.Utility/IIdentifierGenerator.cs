using System;
using System.Collections.Generic;

namespace MixinXRef.Utility
{
  public interface IIdentifierGenerator<T>
  {
    string GetIdentifier (T item);
    string GetIdentifier (T item, string defaultIfNotPresent);

    IEnumerable<T> Elements { get; }
  }
}