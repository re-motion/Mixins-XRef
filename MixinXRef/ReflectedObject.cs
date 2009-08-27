using System;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReflectedObject
  {
    private readonly object _wrappedObject;

    public ReflectedObject (object wrappedObject)
    {
      ArgumentUtility.CheckNotNull ("wrappedObject", wrappedObject);

      _wrappedObject = wrappedObject;
    }

    public T To<T> ()
    {
      try
      {
        return (T)Convert.ChangeType(_wrappedObject, typeof(T));
      }
      catch (InvalidCastException invalidCastException)
      {
        var message = String.Format ("Cannot convert from {0} to {1}", _wrappedObject.GetType(), typeof(T));
        throw new InvalidCastException(message, invalidCastException);
      }
    }
  }
}