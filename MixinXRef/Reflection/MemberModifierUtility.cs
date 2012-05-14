using System;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public class MemberModifierUtility
  {
    private readonly TypeModifierUtility _typeModifierUtility = new TypeModifierUtility();

    public bool IsOverriddenMember (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      var methodInfo = memberInfo as MethodInfo;
      if (methodInfo != null)
        return IsOverriddenMethod (methodInfo);

      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null)
      {
        return IsOverriddenMethod (propertyInfo.GetGetMethod (true))
               || IsOverriddenMethod (propertyInfo.GetSetMethod (true));
      }

      var eventInfo = memberInfo as EventInfo;
      if (eventInfo != null)
      {
        return IsOverriddenMethod (eventInfo.GetAddMethod (true))
               || IsOverriddenMethod (eventInfo.GetRaiseMethod (true))
               || IsOverriddenMethod (eventInfo.GetRemoveMethod (true));
      }

      return false;
    }

    public string GetMemberModifiers (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
        case MemberTypes.Constructor:
          return GetMethodModifiers (memberInfo, memberInfo);
        case MemberTypes.Field:
          return GetFieldModifiers ((FieldInfo)memberInfo);

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return GetMethodModifiers (propertyInfo.GetGetMethod (true) ?? propertyInfo.GetSetMethod (true), memberInfo);

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          return GetMethodModifiers (eventInfo.GetAddMethod (true), memberInfo);

        case MemberTypes.NestedType:
          return _typeModifierUtility.GetTypeModifiers ((Type) memberInfo);

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return "TODO special MemberTypes";

        default:
          throw new Exception ("unknown member type");
      }
    }


    private string GetMethodModifiers (MemberInfo methodFieldOrConstructor, MemberInfo memberInfoForOverride)
    {
      var methodInfo = (MethodBase) methodFieldOrConstructor;
      var modifiers = "";

      if (methodInfo.IsPublic)
        modifiers = "public";
      else if (methodInfo.IsFamily)
        modifiers = "protected";
      else if (methodInfo.IsFamilyOrAssembly)
        modifiers = "protected internal";
      else if (methodInfo.IsAssembly)
        modifiers = "internal";
      else if (methodInfo.IsPrivate)
        modifiers = "private";

      if (methodFieldOrConstructor is MethodInfo)
      {
        var isOverriddenMember = IsOverriddenMember (memberInfoForOverride);

        if (methodInfo.IsAbstract)
          modifiers += " abstract";
        else if (methodInfo.IsFinal && (!methodInfo.IsVirtual || isOverriddenMember))
          modifiers += " sealed";
        if (isOverriddenMember)
          modifiers += " override";
        if (!isOverriddenMember
            && !methodInfo.IsAbstract
            && !methodInfo.IsFinal
            && methodInfo.IsVirtual)
          modifiers += " virtual";

        // explicit interface implementation
        if (methodInfo.IsHideBySig
            && methodInfo.IsPrivate
            && methodInfo.IsFinal
            && methodInfo.IsVirtual)
          return "";
      }

      if (methodInfo.IsStatic)
        modifiers += " static";

      return modifiers;
    }

    private string GetFieldModifiers (FieldInfo methodInfo)
    {
      var modifiers = "";

      if (methodInfo.IsPublic)
        modifiers = "public";
      else if (methodInfo.IsFamily)
        modifiers = "protected";
      else if (methodInfo.IsFamilyOrAssembly)
        modifiers = "protected internal";
      else if (methodInfo.IsAssembly)
        modifiers = "internal";
      else if (methodInfo.IsPrivate)
        modifiers = "private";

      if (methodInfo.IsStatic)
        modifiers += " static";

      if (methodInfo.IsInitOnly)
        modifiers += " readonly";

      return modifiers;
    }

    private bool IsOverriddenMethod (MethodInfo methodInfo)
    {
      return methodInfo != null && methodInfo.DeclaringType != methodInfo.GetBaseDefinition ().DeclaringType;
    }
  }
}