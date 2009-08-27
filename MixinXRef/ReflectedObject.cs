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
      return InvokeMember (methodName, BindingFlags.InvokeMethod, parameters);
    }

    public ReflectedObject GetProperty(string propertyName)
    {
      return InvokeMember(propertyName, BindingFlags.GetProperty, null);
    }

    private ReflectedObject InvokeMember(string memberName, BindingFlags memberType, object[] parameters)
    {
      return new ReflectedObject(_wrappedObject.GetType().InvokeMember(memberName, memberType, null, _wrappedObject, parameters));
    }


  }
}