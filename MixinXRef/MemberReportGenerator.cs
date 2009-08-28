using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;

    public MemberReportGenerator (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
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
              new XAttribute ("overridden", IsOverriddenMember (memberInfo)),
              new XAttribute ("signature", memberInfo)
              )
          );
    }

    private bool IsOverriddenMember (MemberInfo memberInfo)
    {
      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null)
        return IsOverriddenMethod(methodInfo);

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