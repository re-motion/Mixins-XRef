using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Formatting
{
  public class MemberSignatureUtility
  {
    private readonly IOutputFormatter _outputFormatter;

    public MemberSignatureUtility (IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _outputFormatter = outputFormatter;
    }

    public XElement GetMemberSignature (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
          var methodInfo = (MethodInfo) memberInfo;
          return _outputFormatter.CreateMethodMarkup (methodInfo.Name, methodInfo.ReturnType, methodInfo.GetParameters (), methodInfo.GetGenericArguments ());

        case MemberTypes.Constructor:
          var constructorInfo = (ConstructorInfo) memberInfo;
          return _outputFormatter.CreateConstructorMarkup (_outputFormatter.GetConstructorName (memberInfo.DeclaringType), constructorInfo.GetParameters ());

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          return _outputFormatter.CreateEventMarkup (eventInfo.Name, eventInfo.EventHandlerType);

        case MemberTypes.Field:
          var fieldInfo = (FieldInfo) memberInfo;
          return _outputFormatter.CreateFieldMarkup (fieldInfo.Name, fieldInfo.FieldType);

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return _outputFormatter.CreatePropertyMarkup (propertyInfo.Name, propertyInfo.PropertyType);

        case MemberTypes.NestedType:
          var nestedType = (Type) memberInfo;
          return _outputFormatter.CreateNestedTypeMarkup (nestedType);

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return null;

        default:
          throw new Exception ("unknown member type");
      }
    }
  }
}