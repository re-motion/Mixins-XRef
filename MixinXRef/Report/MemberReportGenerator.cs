using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility();
    private readonly MemberSignatureUtility _memberSignatureUtility;
    private readonly Dictionary<MemberInfo, ReflectedObject> _memberDefinitionDictionary = new Dictionary<MemberInfo, ReflectedObject>();
    private Dictionary<MemberInfo, ReflectedObject> _memberInfoToMemberDefinitionDictionary = new Dictionary<MemberInfo, ReflectedObject>();


    public MemberReportGenerator (
        Type type,
        InvolvedType involvedTypeOrNull,
        IIdentifierGenerator<Type> involvedTypeIdentifierGeneratorOrNull,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("involvedTypeOrNull", involvedTypeOrNull);
      // ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGeneratorOrNull", involvedTypeIdentifierGeneratorOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _involvedType = involvedTypeOrNull;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGeneratorOrNull;
      _outputFormatter = outputFormatter;
      _memberSignatureUtility = new MemberSignatureUtility (outputFormatter);
    }


    public XElement GenerateXml ()
    {
      GetAllMembers();

      return new XElement (
          "Members",
          from memberInfo in _type.GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          where memberInfo.DeclaringType.IsAssignableFrom (_type)
                && !IsSpecialName (memberInfo)
          select CreateMemberElement (memberInfo)
          );
    }

    private void GetAllMembers ()
    {
      if (_involvedType == null || !_involvedType.HasTargetClassDefintion)
        return;

      _memberInfoToMemberDefinitionDictionary = _involvedType.TargetClassDefintion.CallMethod ("GetAllMembers")
          .ToDictionary (a => a.GetProperty ("MemberInfo").To<MemberInfo>(), a => a);
    }

    private XElement CreateMemberElement (MemberInfo memberInfo)
    {
      var memberModifier = _memberModifierUtility.GetMemberModifiers (memberInfo);
      if (IsPrivateOrInternal (memberModifier))
        return null;

      var lastPoint = memberInfo.Name.LastIndexOf ('.');
      var memberName = memberInfo.Name;

      // member is explicit interface implementation
      // greater 0 because ".ctor" would be changed to "ctor"
      if (lastPoint > 0)
        memberName = memberInfo.Name.Substring (lastPoint + 1, memberInfo.Name.Length - lastPoint - 1);

      var attributes = new StringBuilder();
      XElement overrides = null;

      if (_involvedType != null)
      {
        // check involvedType is a target
        if (_involvedType.HasTargetClassDefintion)
        {
          var memberDefinitionBase = GetMemberDefinitionBase(memberInfo);
          _memberDefinitionDictionary.Add (memberInfo, memberDefinitionBase);

          if (HasOverrideMixinAttribute (memberInfo))
            attributes.Append ("OverrideMixin ");

          overrides = GetOverrides (memberInfo);
        }

        if (_involvedType.IsMixin && HasOverrideTargetAttribute (memberInfo))
          attributes.Append ("OverrideTarget ");
      }


      if (memberInfo.DeclaringType == _type || IsOverriddenBaseClassMember (memberInfo, overrides))
      {
        return new XElement (
            "Member",
            new XAttribute ("type", memberInfo.MemberType),
            new XAttribute ("name", memberName),
            new XAttribute ("is-declared-by-this-class", memberInfo.DeclaringType == _type),
            _outputFormatter.CreateModifierMarkup (attributes.ToString (), memberModifier),
            _memberSignatureUtility.GetMemberSignatur (memberInfo),
            overrides
            );
      }
      return null;
    }

    private ReflectedObject GetMemberDefinitionBase (MemberInfo memberInfo)
    {
      var keyValuePair =
          _memberInfoToMemberDefinitionDictionary
              .Where (mdb => MemberInfoEqualityUtility.MemberEquals (mdb.Key, memberInfo))
              .SingleOrDefault();

      // performance optimization
      if (keyValuePair.Key != null)
      {
        _memberInfoToMemberDefinitionDictionary.Remove (keyValuePair.Key);
      }

      return keyValuePair.Value;
    }

    private bool HasOverrideMixinAttribute (MemberInfo memberInfo)
    {
      var memberDefinition = _memberDefinitionDictionary[memberInfo];
      if (memberDefinition == null)
        return false;

      var baseAsMember = memberDefinition.GetProperty ("BaseAsMember");
      return (baseAsMember != null);
    }

    private bool HasOverrideTargetAttribute (MemberInfo memberInfo)
    {

      foreach (var typeAndMixinDefinitionPair in _involvedType.TargetTypes)
      {
        if (typeAndMixinDefinitionPair.Value == null)
          continue;

        var overrideCollection = typeAndMixinDefinitionPair.Value.CallMethod ("GetAllOverrides");

        foreach (var memberDefinitionBase in overrideCollection)
        {
          if (MemberInfoEqualityUtility.MemberEquals (memberDefinitionBase.GetProperty ("MemberInfo").To<MemberInfo>(), memberInfo))
            return true;
        }
      }

      return false;
    }

    private XElement GetOverrides (MemberInfo memberInfo)
    {
      var overrides = new XElement ("Overrides");

      var memberDefinition = _memberDefinitionDictionary[memberInfo];

      // when MemberDefinition is null, the member has no relevance for the mixin engine; so return an empty overrides element. 
      if (memberDefinition == null)
        return overrides;

      foreach (var overrideDefinition in memberDefinition.GetProperty ("Overrides"))
      {
        var type = overrideDefinition.GetProperty ("DeclaringClass").GetProperty ("Type").To<Type>();
        overrides.Add (
            new XElement (
                "Mixin-Reference",
                new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (type)),
                new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (type))
                )
            );
      }

      return overrides;
    }

    private bool IsSpecialName (MemberInfo memberInfo)
    {
      if (memberInfo.MemberType == MemberTypes.Method)
      {
        var methodInfo = memberInfo as MethodInfo;
        if (methodInfo == null)
          return false;

        var methodName = methodInfo.Name;
        // only explicit interface implementations contain a '.'
        if (methodName.Contains ('.'))
        {
          var parts = methodName.Split ('.');
          var partCount = parts.Length;
          methodName = parts[partCount - 1];
        }

        return
            (methodInfo.IsSpecialName
             && (methodName.StartsWith ("add_")
                 || methodName.StartsWith ("remove_")
                 || methodName.StartsWith ("get_")
                 || methodName.StartsWith ("set_")
                )
            );
      }
      return false;
    }

    private bool IsPrivateOrInternal (string memberModifiers)
    {
      return memberModifiers.Contains ("internal") || memberModifiers.Contains ("private");
    }

    private bool IsOverriddenBaseClassMember (MemberInfo memberInfo, XElement overrides)
    {
      if (overrides == null)
        return false;

      return !(memberInfo.DeclaringType != _type && overrides.HasElements == false);
    }
  }
}