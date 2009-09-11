using System;
using System.Reflection;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class MemberModifierUtility
  {
    private TypeModifierUtility _typeModifierUtility = new TypeModifierUtility();

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

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
        case MemberTypes.Constructor:
        case MemberTypes.Field:
          return GetMethodModifiers (memberInfo, memberInfo);

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo) memberInfo;
          return GetMethodModifiers (propertyInfo.GetGetMethod (true) ?? propertyInfo.GetSetMethod (true), memberInfo);

        case MemberTypes.Event:
          var eventInfo = (EventInfo) memberInfo;
          return GetMethodModifiers (eventInfo.GetAddMethod (true), memberInfo);

        case MemberTypes.NestedType:
          return _typeModifierUtility.GetTypeModifiers ((Type)memberInfo);

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return "TODO special MemberTypes";

        default:
          throw new Exception ("unknown member type");
      }
    }


    private string GetMethodModifiers (MemberInfo methodFieldOrConstructor, MemberInfo memberInfoForOverride)
    {
      var methodFieldOrConstructorInfo = new ReflectedObject (methodFieldOrConstructor);

      var modifiers = "";

      if (methodFieldOrConstructorInfo.GetProperty ("IsPublic").To<bool>())
        modifiers = "public";
      else if (methodFieldOrConstructorInfo.GetProperty ("IsFamily").To<bool>())
        modifiers = "protected";
      else if (methodFieldOrConstructorInfo.GetProperty ("IsFamilyOrAssembly").To<bool>())
        modifiers = "protected internal";
      else if (methodFieldOrConstructorInfo.GetProperty ("IsAssembly").To<bool>())
        modifiers = "internal";
      else if (methodFieldOrConstructorInfo.GetProperty ("IsPrivate").To<bool> ())
      {
        modifiers = "private";
        
        // method is explicit interface
        if (methodFieldOrConstructor is MethodInfo && ((MethodInfo) methodFieldOrConstructor).IsHideBySig)
          return "";
      }

      if (methodFieldOrConstructor is MethodInfo || methodFieldOrConstructor is PropertyInfo || methodFieldOrConstructor is EventInfo)
      {
        
        if (methodFieldOrConstructorInfo.GetProperty ("IsAbstract").To<bool>())
          modifiers += " abstract";
        else if (methodFieldOrConstructorInfo.GetProperty ("IsFinal").To<bool>() &&
            (!methodFieldOrConstructorInfo.GetProperty ("IsVirtual").To<bool>() || IsOverriddenMember (memberInfoForOverride)))
          modifiers += " sealed";
        if (IsOverriddenMember (memberInfoForOverride))
          modifiers += " override";
        if (!IsOverriddenMember (memberInfoForOverride) &&
            !methodFieldOrConstructorInfo.GetProperty ("IsAbstract").To<bool> () && 
            !methodFieldOrConstructorInfo.GetProperty ("IsFinal").To<bool>() && 
            methodFieldOrConstructorInfo.GetProperty ("IsVirtual").To<bool>())
          modifiers += " virtual";
      }

      if (!(methodFieldOrConstructor is EventInfo) && methodFieldOrConstructorInfo.GetProperty ("IsStatic").To<bool> ())
          modifiers += " static";

      if (methodFieldOrConstructor is FieldInfo && ((FieldInfo)methodFieldOrConstructor).IsInitOnly)
         modifiers += " readonly" ;

      return modifiers;
    }

    private bool IsOverriddenMethod (MethodInfo methodInfo)
    {
      return (methodInfo != null) && (methodInfo != methodInfo.GetBaseDefinition());
    }
  }
}