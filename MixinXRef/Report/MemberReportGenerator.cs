using System;
using System.Collections.Generic;
using System.Diagnostics;
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
      var members =
        _type.GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).
          Where (m => !HasSpecialName (m)).OrderBy (m => m.Name).Select (CreateMemberElement);

      return new XElement ("Members", members);
    }

    private XElement CreateMemberElement (MemberInfo memberInfo)
    {
      var memberModifier = _memberModifierUtility.GetMemberModifiers (memberInfo);
      if (memberModifier.Contains ("private")) // memberModifier.Contains ("internal")
        return null;

      // remove interface name if member is explicit interface implementation
      // greater 0 because ".ctor" would be changed to "ctor"
      var lastPoint = memberInfo.Name.LastIndexOf ('.');
      var memberName = (lastPoint > 0) ? memberInfo.Name.Substring (lastPoint + 1) : memberInfo.Name;

      var element = new XElement ("Member", new XAttribute ("id", _memberIdentifierGenerator.GetIdentifier (memberInfo)),
                                            new XAttribute ("type", memberInfo.MemberType),
                                            new XAttribute ("name", memberName),
                                            new XAttribute ("is-declared-by-this-class", memberInfo.DeclaringType == _type));

      var attributes = new StringBuilder ();

      XElement overridesElement = null;
      XElement overriddenElement = null;
      if (_involvedType != null)
      {
        if (HasOverrideMixinAttribute (memberInfo))
          attributes.Append ("OverrideMixin ");
        if (HasOverrideTargetAttribute (memberInfo))
          attributes.Append ("OverrideTarget ");

        overridesElement = CreateOverridesElement (memberInfo);
        overriddenElement = CreateOverriddenElement (memberInfo);
      }

      if (memberInfo.DeclaringType != _type &&
          overridesElement == null && overriddenElement == null)
        return null;

      element.Add (_outputFormatter.CreateModifierMarkup (attributes.ToString (), memberModifier),
                   _memberSignatureUtility.GetMemberSignatur (memberInfo),
                   overridesElement,
                   overriddenElement);

      return element;
    }

    private XElement CreateOverridesElement (MemberInfo memberInfo)
    {
      var overridesElement = new XElement ("Overrides");

      var overridingMixinTypes = GetOverridingMixinTypes (memberInfo);
      var overridingTargetTypes = GetOverridingTargetTypes (memberInfo);

      if (!overridingMixinTypes.Any () && !overridingTargetTypes.Any ())
        return null;

      foreach (var overridingType in overridingMixinTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Mixin-Reference", overridingType));

      foreach (var overridingType in overridingTargetTypes)
        overridesElement.Add (CreateInvolvedTypeReferenceElement ("Target-Reference", overridingType));

      return overridesElement;
    }

    private XElement CreateOverriddenElement (MemberInfo memberInfo)
    {
      var overriddenMembersElement = new XElement ("OverriddenMembers");

      var overriddenMixinMembers = GetOverriddenMixinMembers (memberInfo);
      var overriddenTargetMembers = GetOverriddenTargetMembers (memberInfo);

      if (!overriddenMixinMembers.Any () && !overriddenTargetMembers.Any ())
        return null;

      foreach (var overriddenMember in overriddenMixinMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement (overriddenMember));

      foreach (var overriddenMember in overriddenTargetMembers)
        overriddenMembersElement.Add (CreateMemberReferenceElement (overriddenMember));

      return overriddenMembersElement;
    }

    private XElement CreateInvolvedTypeReferenceElement (string tagName, Type overridingType)
    {
      return new XElement (tagName, new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (overridingType)),
                                    new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (overridingType)));
    }

    private XElement CreateMemberReferenceElement (MemberInfo memberInfo)
    {
      return new XElement ("Member-Reference", new XAttribute ("ref", _memberIdentifierGenerator.GetIdentifier (memberInfo)),
                                               new XAttribute ("member-name", memberInfo.Name),
                                               new XAttribute ("member-signature", memberInfo.ToString()));
    }

    private IEnumerable<MemberInfo> GetOverriddenTargetMembers (MemberInfo memberInfo)
    {
      Debug.Assert (_involvedType != null);

      List<ReflectedObject> memberDefinitions;
      _involvedType.MixinMemberDefinitions.TryGetValue (memberInfo, out memberDefinitions);

      if (memberDefinitions == null)
        return Enumerable.Empty<MemberInfo> ();

      return memberDefinitions.Select (m => m.GetProperty ("BaseAsMember")).Where (m => m != null).Select (m => m.GetProperty ("MemberInfo").To<MemberInfo> ()).Distinct ();
    }

    private IEnumerable<MemberInfo> GetOverriddenMixinMembers (MemberInfo memberInfo)
    {
      Debug.Assert (_involvedType != null);

      ReflectedObject memberDefinition;
      _involvedType.TargetMemberDefinitions.TryGetValue (memberInfo, out memberDefinition);

      if (memberDefinition == null)
        return Enumerable.Empty<MemberInfo> ();

      var baseMember = memberDefinition.GetProperty ("BaseAsMember");

      if (baseMember == null)
        return Enumerable.Empty<MemberInfo> ();

      return new[] { baseMember.GetProperty ("MemberInfo").To<MemberInfo> () };
    }

    private IEnumerable<Type> GetOverridingMixinTypes (MemberInfo memberInfo)
    {
      Debug.Assert (_involvedType != null);
      
      ReflectedObject memberDefinition;
      _involvedType.TargetMemberDefinitions.TryGetValue (memberInfo, out memberDefinition);

      if (memberDefinition == null)
        return Enumerable.Empty<Type> ();

      return memberDefinition.GetProperty ("Overrides").Select (o => o.GetProperty ("DeclaringClass").GetProperty ("Type").To<Type> ());
    }

    private IEnumerable<Type> GetOverridingTargetTypes (MemberInfo memberInfo)
    {
      Debug.Assert (_involvedType != null);

      List<ReflectedObject> memberDefinition;
      _involvedType.MixinMemberDefinitions.TryGetValue (memberInfo, out memberDefinition);

      if (memberDefinition == null)
        return Enumerable.Empty<Type> ();

      return memberDefinition.SelectMany (m => m.GetProperty ("Overrides")).Select (o => o.GetProperty ("DeclaringClass").GetProperty ("Type").To<Type> ());
    }

    private static bool HasOverrideMixinAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideMixinAttribute");
    }

    private static bool HasOverrideTargetAttribute (MemberInfo memberInfo)
    {
      return memberInfo.GetCustomAttributes (true).Any (a => a.GetType ().Name == "OverrideTargetAttribute");
    }

    private static bool HasSpecialName (MemberInfo memberInfo)
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
  }
}