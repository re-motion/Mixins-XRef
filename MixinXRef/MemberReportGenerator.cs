using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;

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
              GenerateModifiers (memberInfo),
              new XElement ("signature", memberInfo)
              )
          );
    }

    public XElement GenerateModifiers (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      return _outputFormatter.CreateModifierMarkup(GetMemberModifiers(memberInfo));
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
        return IsOverriddenMethod (propertyInfo.GetGetMethod (true)) ||
               IsOverriddenMethod (propertyInfo.GetSetMethod (true));
      }

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
      {
        return IsOverriddenMethod (eventInfo.GetAddMethod (true)) ||
               IsOverriddenMethod (eventInfo.GetRaiseMethod (true)) ||
               IsOverriddenMethod (eventInfo.GetRemoveMethod (true));
      }

      return false;
    }

    public string GetMemberModifiers (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      if (memberInfo is MethodInfo)
        return GetMethodModifiers(memberInfo, memberInfo);

      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null)
        return GetMethodModifiers (propertyInfo.GetGetMethod (true) ?? propertyInfo.GetSetMethod(true), memberInfo);

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
        return GetMethodModifiers (eventInfo.GetAddMethod (true), memberInfo);

      // has to be a field or constructor (which both have visibility properties like MethodInfo)
      return GetMethodModifiers(memberInfo, memberInfo);
    }


    private string GetMethodModifiers(MemberInfo methodFieldOrConstructor, MemberInfo memberInfoForOverride)
    {
      var methodFieldOrConstructorInfo = new ReflectedObject(methodFieldOrConstructor);

      var modifiers = "";

      if (methodFieldOrConstructorInfo.GetProperty ("IsPublic").To<bool>())
        modifiers = "public";
      if (methodFieldOrConstructorInfo.GetProperty ("IsFamily").To<bool>())
        modifiers = "protected";
      if (methodFieldOrConstructorInfo.GetProperty ("IsFamilyOrAssembly").To<bool>())
        modifiers = "protected internal";
      if (methodFieldOrConstructorInfo.GetProperty ("IsAssembly").To<bool>())
        modifiers = "internal";
      if (methodFieldOrConstructorInfo.GetProperty ("IsPrivate").To<bool>())
        modifiers = "private";

      if (methodFieldOrConstructor is MethodInfo || methodFieldOrConstructor is PropertyInfo || methodFieldOrConstructor is EventInfo)
      {
        if (methodFieldOrConstructorInfo.GetProperty("IsAbstract").To<bool>())
          modifiers += " abstract";
        if (IsOverriddenMember(memberInfoForOverride))
          modifiers += " override";
        if (methodFieldOrConstructorInfo.GetProperty("IsFinal").To<bool>())
          modifiers += " sealed";
        if (!modifiers.Contains("override") && !modifiers.Contains("abstract") && methodFieldOrConstructorInfo.GetProperty ("IsVirtual").To<bool>())
          modifiers += " virtual";
      }
      return modifiers;
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