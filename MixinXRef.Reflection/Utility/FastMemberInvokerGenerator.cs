using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Utility
{
  public class FastMemberInvokerGenerator
  {
    public Func<object, object[], object> GetFastMethodInvoker (Type declaringType, string memberName, Type[] typeParameters, Type[] argumentTypes, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("memberName", memberName);
      ArgumentUtility.CheckNotNull ("typeParameters", typeParameters);
      ArgumentUtility.CheckNotNull ("argumentTypes", argumentTypes);

      var overloads = (MethodBase[]) declaringType.GetMember (memberName, MemberTypes.Method, bindingFlags);

      if (overloads.Length == 0)
        throw new MissingMethodException (string.Format ("Method '{0}' not found on type '{1}'.", memberName, declaringType));

      MethodInfo method;
      if (typeParameters.Length > 0)
      {
        var methods = overloads.Where (m => m.GetGenericArguments ().Length == typeParameters.Length)
                               .Select (m => ((MethodInfo) m).MakeGenericMethod (typeParameters));

        method = methods.SingleOrDefault (m => m.GetParameters ().Select (p => p.ParameterType).SequenceEqual (argumentTypes));
        if (method == null)
          throw new MissingMethodException (string.Format ("Generic overload of method '{0}`{1}' not found on type '{2}'.", memberName, typeParameters.Length, declaringType));
      }
      else
      {
        method = (MethodInfo) Type.DefaultBinder.SelectMethod (bindingFlags, overloads, argumentTypes, null);
        if (method == null)
          throw new MissingMethodException (string.Format ("Overload of method '{0}' not found on type '{1}'.", memberName, declaringType));
      }

      return CreateDelegateForMethod (method);
    }

    private Func<object, object[], object> CreateDelegateForMethod (MethodInfo methodInfo)
    {
      if (methodInfo.ReturnType == typeof (void))
        throw new NotSupportedException ("Void methods are not supported.");

      var instanceParameter = Expression.Parameter (typeof (object), "instance");
      var argsParameter = Expression.Parameter (typeof (object[]), "args");

      var extractedParameters = from parameterInfo in methodInfo.GetParameters ()
                                let arrayElementExpression = Expression.ArrayIndex (argsParameter, Expression.Constant (parameterInfo.Position))
                                select (Expression) Expression.Convert (arrayElementExpression, parameterInfo.ParameterType);
      var callExpression = Expression.Call (Expression.Convert (instanceParameter, methodInfo.DeclaringType), methodInfo, extractedParameters);
      var convertedCallResult = Expression.Convert (callExpression, typeof (object));

      var lambda = Expression.Lambda<Func<object, object[], object>> (convertedCallResult, instanceParameter, argsParameter);
      return lambda.Compile ();
    }
  }
}