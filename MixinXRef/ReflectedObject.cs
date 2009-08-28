using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MixinXRef
{
  public class ReflectedObject : IEnumerable<ReflectedObject>
  {
    private readonly object _wrappedObject;

    public ReflectedObject (object wrappedObject)
    {
      ArgumentUtility.CheckNotNull ("wrappedObject", wrappedObject);

      _wrappedObject = wrappedObject;
    }


    public static ReflectedObject Create(Assembly assembly, string fullName, params object[] parameters)
    {
      var wrappedObjectType = GetForeignType(assembly, fullName);
      return new ReflectedObject(Activator.CreateInstance(wrappedObjectType, UnWrapParameters(parameters)));
    }

    public static Type GetForeignType(Assembly assembly, string fullName)
    {
      return assembly.GetType(fullName, true);
    }


    public T To<T> ()
    {
      return (T) Convert.ChangeType (_wrappedObject, typeof (T));
    }

    public ReflectedObject CallMethod (string methodName, params object[] parameters)
    {
      return InvokeMember (methodName, BindingFlags.InvokeMethod, parameters);
    }

    public ReflectedObject GetProperty (string propertyName)
    {
      return InvokeMember (propertyName, BindingFlags.GetProperty, null);
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

    private ReflectedObject InvokeMember(string memberName, BindingFlags memberType, object[] parameters)
    {
      return new ReflectedObject(_wrappedObject.GetType().InvokeMember(memberName, memberType, null, _wrappedObject, UnWrapParameters(parameters)));
    }

    private static object[] UnWrapParameters(object[] parameters)
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

  }
}