using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public class ReflectedObject : IEnumerable<ReflectedObject>
  {
    private readonly object _wrappedObject;

    public ReflectedObject (object wrappedObject)
    {
      ArgumentUtility.CheckNotNull ("wrappedObject", wrappedObject);

      if (wrappedObject is ReflectedObject)
        throw new ArgumentException ("There is no point in wrapping an instance of 'MixinXRef.Reflection.ReflectedObject'.");

      _wrappedObject = wrappedObject;
    }


    public static ReflectedObject Create (Assembly assembly, string fullName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("fullName", fullName);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      return new ReflectedObject (Activator.CreateInstance (assembly.GetType (fullName, true), UnWrapParameters (parameters)));
    }

    public static ReflectedObject CallMethod (Type type, string methodName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      return InvokeMember (type, methodName, BindingFlags.InvokeMethod, null, parameters);
    }


    private static ReflectedObject InvokeMember (
        Type wrappedObjectType, string memberName, BindingFlags memberType, object wrappedObject, object[] parameters)
    {
      try
      {
        var returnValue = wrappedObjectType.InvokeMember(memberName, memberType, null, wrappedObject, UnWrapParameters(parameters));
        return returnValue == null ? null : new ReflectedObject(returnValue);
      }
      catch (TargetInvocationException targetInvocationException)
      {
        throw targetInvocationException.InnerException;
      }
    }

    private static object[] UnWrapParameters (object[] parameters)
    {
      if (parameters == null)
        return null;

      for (int i = 0; i < parameters.Length; i++)
      {
        var parameter = parameters[i] as ReflectedObject;
        if (parameter != null)
          parameters[i] = parameter.To<object>();
      }
      return parameters;
    }


    public T To<T> ()
    {
      return (T) _wrappedObject;
    }

    public ReflectedObject CallMethod (string methodName, params object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      return InvokeMember (_wrappedObject.GetType(), methodName, BindingFlags.InvokeMethod, _wrappedObject, parameters);
    }

    public ReflectedObject GetProperty (string propertyName)
    {
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      return InvokeMember (_wrappedObject.GetType(), propertyName, BindingFlags.GetProperty, _wrappedObject, null);
    }

    public IEnumerator<ReflectedObject> GetEnumerator ()
    {
      var wrappedObjectAsEnumerable = _wrappedObject as IEnumerable;

      if (wrappedObjectAsEnumerable != null)
      {
        foreach (var item in wrappedObjectAsEnumerable)
          yield return new ReflectedObject (item);
      }
      else
        throw new NotSupportedException (String.Format ("The reflected object '{0}' is not enumerable.", _wrappedObject.GetType()));
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public IEnumerable AsEnumerable<T> ()
    {
      return this.Select (reflectedObject => reflectedObject.To<T>());
    }

    public override string ToString ()
    {
      return _wrappedObject.ToString();
    }
  }
}