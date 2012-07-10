using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<MemberInfo> _memberIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility ();
    private readonly MemberSignatureUtility _memberSignatureUtility;

    public MemberReportGenerator (
      Type type,
      InvolvedType involvedTypeOrNull,
      IIdentifierGenerator<Type> involvedTypeIdentifierGeneratorOrNull,
      IIdentifierGenerator<MemberInfo> memberIdentifierGeneratorOrNull,
      IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("involvedTypeOrNull", involvedTypeOrNull);
      // ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGeneratorOrNull", involvedTypeIdentifierGeneratorOrNull);
      // ArgumentUtility.CheckNotNull ("memberIdentifierGeneratorOrNull", memberIdentifierGeneratorOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _involvedType = involvedTypeOrNull;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGeneratorOrNull;
      _memberIdentifierGenerator = memberIdentifierGeneratorOrNull;
      _outputFormatter = outputFormatter;
      _memberSignatureUtility = new MemberSignatureUtility (outputFormatter);
    }

    public XElement GenerateXml ()
    {
      var type = _involvedType ?? new InvolvedType (_type);
      return new XElement ("Members", type.Members.Select (CreateMemberElement));
    }

    private string GetMemberName(MemberInfo memberInfo)
    {
      // remove interface name if member is explicit interface implementation
      // greater 0 because ".ctor" would be changed to "ctor"
      var lastPoint = memberInfo.Name.LastIndexOf ('.');
      return (lastPoint > 0) ? memberInfo.Name.Substring (lastPoint + 1) : memberInfo.Name;
    }

    private XElement CreateMemberElement (InvolvedTypeMember member)
    {
      var memberInfo = member.MemberInfo;

      var memberModifier = _memberModifierUtility.GetMemberModifiers (memberInfo);
      if (memberModifier.Contains ("private")) // memberModifier.Contains ("internal")
        return null;

      var memberName = GetMemberName(memberInfo);

      var attributes = new StringBuilder ();

      XElement overridesElement = null;
      XElement overriddenElement = null;
      if (_involvedType != null)
      {
        if (HasOverrideMixinAttribute (memberInfo))
          attributes.Append ("OverrideMixin ");
        if (HasOverrideTargetAttribute (memberInfo))
          attributes.Append ("OverrideTarget ");

        overridesElement = CreateOverridesElement (member);
        overriddenElement = CreateOverriddenElement (member);
      }

      if (memberInfo.DeclaringType != _type &&
          overridesElement == null && overriddenElement == null)
        return null;

      var element = new XElement("Member", new XAttribute("id", _memberIdentifierGenerator.GetIdentifier(memberInfo)),
                                 new XAttribute("type", memberInfo.MemberType),
                                 new XAttribute("name", memberName),
                                 new XAttribute("is-declared-by-this-class", memberInfo.DeclaringType == _type),
                                 _outputFormatter.CreateModifierMarkup(attributes.ToString(), memberModifier),
                                 _memberSignatureUtility.GetMemberSignature(memberInfo),
                                 member.SubMemberInfos.Select(CreateSubMemberElement),
                                 overridesElement,
                                 overriddenElement);
      return element;
    }

    private XElement CreateSubMemberElement(MemberInfo memberInfo)
    {
      var memberModifier = _memberModifierUtility.GetMemberModifiers (memberInfo);
      if (memberModifier.Contains ("private")) // memberModifier.Contains ("internal")
        return null;

      var memberName = GetMemberName (memberInfo);

      var attributes = new StringBuilder ();

      var element = new XElement("SubMember", new XAttribute("id", _memberIdentifierGenerator.GetIdentifier(memberInfo)),
                                 new XAttribute("type", memberInfo.MemberType),
                                 new XAttribute("name", memberName),
                                 _outputFormatter.CreateModifierMarkup(attributes.ToString(), memberModifier),
                                 _memberSignatureUtility.GetMemberSignature(memberInfo));
      return element;
    }

    private XElement CreateOverridesElement (InvolvedTypeMember member)
    {
      var overridesElement = new XElement ("Overrides");

      var overridingMixinTypes = member.OverridingMixinTypes;
      var overridingTargetTypes = member.OverridingTargetTypes;

      if (!overridingMixinTypes.Any () && !overridingTargetTypes.Any ())
        return null;

      foreach (var overridingType in overridingMixinTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Mixin-Reference", overridingType));

      foreach (var overridingType in overridingTargetTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Target-Reference", overridingType));

      return overridesElement;
    }

    private XElement CreateOverriddenElement (InvolvedTypeMember member)
    {
      var overriddenMembersElement = new XElement ("OverriddenMembers");

      var overriddenMixinMembers = member.MixinOverrideInfos;
      var overriddenTargetMembers = member.TargetOverrideInfos;

      if (!overriddenMixinMembers.Any () && !overriddenTargetMembers.Any ())
        return null;

      foreach (var overriddenMember in overriddenMixinMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement ("OverrideMixin", overriddenMember));

      foreach (var overriddenMember in overriddenTargetMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement ("OverrideTarget", overriddenMember));

      return overriddenMembersElement;
    }

    private XElement CreateInvolvedTypeReferenceElement (string tagName, Type overridingType)
    {
      return new XElement (tagName, new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (overridingType)),
                                    new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (overridingType)));
    }

    private XElement CreateMemberReferenceElement (string typeName, OverrideInfo overrideInfo)
    {
      return new XElement("Member-Reference",
                          new XAttribute("ref", _memberIdentifierGenerator.GetIdentifier(overrideInfo.MainMemberInfo)),
                          new XAttribute("type", typeName),
                          new XAttribute ("member-name", overrideInfo.MainMemberInfo.Name),
                          new XAttribute("member-signature", overrideInfo.ToString()),
                          overrideInfo.SubMemberInfos.Select(CreateSubMemberReferenceElement));
    }

    private XElement CreateSubMemberReferenceElement (OverriddenMemberInfo memberInfo)
    {
      return new XElement("SubMember-Reference",
                          new XAttribute("override-ref", _memberIdentifierGenerator.GetIdentifier(memberInfo.OverrideMemberInfo)),
                          new XAttribute("base-ref", _memberIdentifierGenerator.GetIdentifier(memberInfo.BaseMemberInfo)),
                          new XAttribute("member-name", memberInfo.BaseMemberInfo),
                          new XAttribute("member-signature", memberInfo.ToString()));
    }

    private static bool HasOverrideMixinAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideMixinAttribute");
    }

    private static bool HasOverrideTargetAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideTargetAttribute");
    }
  }
}