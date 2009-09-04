using System;
using System.Reflection;
using System.Text;
using MixinXRef.Formatting;

namespace MixinXRef
{
  public class MemberSignatureUtility
  {
    private IOutputFormatter _outputFormatter = new OutputFormatter();

    public string GetMemberSignatur (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
          var methodInfo = (MethodInfo) memberInfo;
          return _outputFormatter.GetShortName (methodInfo.ReturnType) + " " + methodInfo.Name + " (" + GetParamterList (methodInfo.GetParameters ()) + ")";

        case MemberTypes.Constructor:
          var constructorInfo = (ConstructorInfo) memberInfo;
          return _outputFormatter.GetShortName (memberInfo.DeclaringType) + " (" + GetParamterList (constructorInfo.GetParameters ()) + ")";
          // return GetShortName (constructorInfo.Name) + " (" + GetParamterList (constructorInfo.GetParameters()) + ")";

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          var eventHandlerType = eventInfo.EventHandlerType ?? null;
          return "event " + _outputFormatter.GetShortName (eventHandlerType) + " " + eventInfo.Name;
          // +" (" + GetParamterList (eventInfo.GetAddMethod (true).GetParameters ()) + ")";

        case MemberTypes.Field:
          var fieldInfo = (FieldInfo) memberInfo;
          return _outputFormatter.GetShortName (fieldInfo.FieldType) + " " + fieldInfo.Name;

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return _outputFormatter.GetShortName (propertyInfo.PropertyType) + " " + propertyInfo.Name;

        case MemberTypes.NestedType:
          return "TODO nested type";

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return "TODO special MemberTypes";

        default:
          throw new Exception ("unknown member type");
      }
    }

    private string GetParamterList (ParameterInfo[] parameterInfos)
    {
      StringBuilder parameterList = new StringBuilder ();

      bool firstRun = true;
      foreach (var parameterInfo in parameterInfos)
      {
        if (firstRun)
          firstRun = false;
        else
          parameterList.Append (", ");

        parameterList.Append (String.Format ("{0} {1}", _outputFormatter.GetShortName (parameterInfo.ParameterType), parameterInfo.Name));
      }
      return parameterList.ToString();
    }
  }
}