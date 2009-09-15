using System;

namespace MixinXRef.Utility
{
  public interface IIdentifierGenerator<T>
  {
    string GetIdentifier (T item);
    string GetIdentifier (T item, string defaultIfNotPresent);
  }
}