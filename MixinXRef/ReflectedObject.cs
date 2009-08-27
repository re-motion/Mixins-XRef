using System;
using System.Reflection;
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
      return (T)Convert.ChangeType(_wrappedObject, typeof(T));
    }


    public ReflectedObject CallMethod (string methodName, params object[] parameters)
    {
      return new ReflectedObject(_wrappedObject.GetType().InvokeMember (methodName, BindingFlags.InvokeMethod, null, _wrappedObject, parameters));
    }
  }
}