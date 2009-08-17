using System;

namespace MixinXRef
{
  public interface IIdentifierGenerator<T>
  {
    string GetIdentifier (T item);
    string GetIdentifier (T item, string defaultIfNotPresent);
  }
}