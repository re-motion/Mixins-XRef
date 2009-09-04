using System;
using System.Reflection;
using System.Text;

namespace MixinXRef
{
  public class MemberSignatureUtility
  {
    public string GetMemberSignatur (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
          var methodInfo = (MethodInfo) memberInfo;
          return GetShortName(methodInfo.ReturnType.Name) + " " + methodInfo.Name + " (" + GetParamterList (methodInfo.GetParameters()) + ")";

        case MemberTypes.Constructor:
          var constructorInfo = (ConstructorInfo) memberInfo;
          return GetShortName (memberInfo.DeclaringType.Name) + " (" + GetParamterList (constructorInfo.GetParameters ()) + ")";
          // return GetShortName (constructorInfo.Name) + " (" + GetParamterList (constructorInfo.GetParameters()) + ")";

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          var eventHandlerType = eventInfo.EventHandlerType != null ? eventInfo.EventHandlerType.Name : "UnknownEventHandler";
          return "event " + GetShortName (eventHandlerType) + " " + eventInfo.Name;
          // +" (" + GetParamterList (eventInfo.GetAddMethod (true).GetParameters ()) + ")";

        case MemberTypes.Field:
        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return GetShortName(propertyInfo.PropertyType.Name) + " " + propertyInfo.Name;

        case MemberTypes.NestedType:
          return "TODO nested type";

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return "TODO special MemberTypes";

        default:
          throw new Exception ("unknown member type");
      }
    }

    private string GetShortName (string name)
    {
      var index = name.LastIndexOf ('.');

      if (index == -1)
      {
        switch (name)
        {
          case "Boolean":
            return "bool";
          case "Int16":
            return "short";
          case "Int32":
            return "int";
          case "Int64":
            return "long";
          case "Single":
            return "flaot";
          case "UInt16":
            return "ushort";
          case "UInt32":
            return "uint";
          case "UInt64":
            return "ulong";
          case "Byte":
          case "Char":
          case "Decimal":
          case "Double":
          case "SByte":
          case "String":
            return name.ToLower();
          default:
            return name;
        }
      }

      var shortParameterName = name.Substring (
          index, name.Length - index);

      return shortParameterName;
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

        parameterList.Append (String.Format ("{0} {1}", GetShortName(parameterInfo.ParameterType.Name), parameterInfo.Name));
      }
      return parameterList.ToString();
    }
  }
}