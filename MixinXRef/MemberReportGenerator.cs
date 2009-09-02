using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    // TargetClassDefinition
    private readonly ReflectedObject _targetClassDefinition;
    private readonly IOutputFormatter _outputFormatter;

    public MemberReportGenerator (Type type, ReflectedObject targetClassDefinitionOrNull, IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("targetClassDefinitionOrNull", targetClassDefinitionOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _targetClassDefinition = targetClassDefinitionOrNull;
      _outputFormatter = outputFormatter;
    }


    public XElement GenerateXml ()
    {
      return new XElement (
          "PublicMembers",
          from memberInfo in _type.GetMembers()
          where memberInfo.DeclaringType == _type && !IsSpecialName (memberInfo)
          select new XElement (
              "Member",
              new XAttribute ("type", memberInfo.MemberType),
              new XAttribute ("name", memberInfo.Name),
              GenerateModifiers(memberInfo),
              new XElement ("signature", memberInfo)
              )
          );
    }

    public XElement GenerateModifiers(MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      return new XElement("modifiers" , new XCData(IsOverriddenMember (memberInfo) ? "overridden" : ""));
    }

    public bool IsOverriddenMember (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null)
        return IsOverriddenMethod (methodInfo);

      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null)
      {
        return IsOverriddenMethod (propertyInfo.GetGetMethod()) ||
               IsOverriddenMethod (propertyInfo.GetSetMethod());
      }

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
      {
        return IsOverriddenMethod (eventInfo.GetAddMethod()) ||
               IsOverriddenMethod (eventInfo.GetRaiseMethod()) ||
               IsOverriddenMethod (eventInfo.GetRemoveMethod());
      }

      return false;
    }

    private bool IsOverriddenMethod (MethodInfo methodInfo)
    {
      return (methodInfo != null) && (methodInfo != methodInfo.GetBaseDefinition());
    }

    private bool IsSpecialName (MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Method)
      {
        var methodName = memberInfo.Name;
        var methodInfo = memberInfo as MethodInfo;
        if (methodInfo == null)
          return false;

        return (
                   methodInfo.IsSpecialName &&
                   (
                       methodName.StartsWith ("add_") ||
                       methodName.StartsWith ("remove_") ||
                       methodName.StartsWith ("get_") ||
                       methodName.StartsWith ("set_")
                   )
               );
      }
      return false;
    }
  }
}