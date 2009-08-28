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
              new XAttribute ("overridden", IsOverridden (memberInfo))
              )
          );
    }

    private bool IsOverridden (MemberInfo memberInfo)
    {
      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null)
        return (methodInfo != methodInfo.GetBaseDefinition());

      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null)
      {
        return propertyInfo.GetGetMethod() != propertyInfo.GetGetMethod().GetBaseDefinition() ||
               propertyInfo.GetSetMethod() != propertyInfo.GetSetMethod().GetBaseDefinition();
      }

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
      {
        return eventInfo.GetAddMethod() != eventInfo.GetAddMethod().GetBaseDefinition() ||
               eventInfo.GetRaiseMethod() != eventInfo.GetRaiseMethod().GetBaseDefinition() ||
               eventInfo.GetRemoveMethod() != eventInfo.GetRemoveMethod().GetBaseDefinition();
        
      }

      return false;
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