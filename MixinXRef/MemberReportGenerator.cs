using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly InvolvedType _involvedType;
    private readonly IOutputFormatter _outputFormatter;
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility();
    private readonly MemberSignatureUtility _memberSignatureUtility;


    public MemberReportGenerator (Type type, InvolvedType involvedTypeOrNull, IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("involvedTypeOrNull", involvedTypeOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _involvedType = involvedTypeOrNull;
      _outputFormatter = outputFormatter;
      _memberSignatureUtility = new MemberSignatureUtility (outputFormatter);
    }


    public XElement GenerateXml ()
    {
      // TODO: find a better way to remove private modifiers
      return new XElement (
          "Members",
          from memberInfo in _type.GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          where memberInfo.DeclaringType == _type &&
                !IsSpecialName (memberInfo) &&
                !_memberModifierUtility.GetMemberModifiers (memberInfo).Contains ("private")
          select CreateMemberElement (memberInfo)
          );
    }


    private XElement CreateMemberElement (MemberInfo memberInfo)
    {
      var lastPoint = memberInfo.Name.LastIndexOf ('.');
      var memberName = memberInfo.Name;

      // member is explicit interface implementation
      if (lastPoint != -1)
        memberName = memberInfo.Name.Substring (lastPoint+1, memberInfo.Name.Length - lastPoint-1);
      
      var attributes = new StringBuilder ();
      if (_involvedType != null)
      {
        if (HasOverrideMixinAttribute (memberInfo))
          attributes.Append ("OverrideMixin ");

        if (HasOverrideTargetAttribute (memberInfo))
          attributes.Append ("OverrideTarget ");
      }

      return new XElement (
          "Member",
          new XAttribute ("type", memberInfo.MemberType),
          new XAttribute ("name", memberName),
          _outputFormatter.CreateModifierMarkup (attributes.ToString(), _memberModifierUtility.GetMemberModifiers (memberInfo)),
          _memberSignatureUtility.GetMemberSignatur (memberInfo)
          );
    }

    public bool HasOverrideMixinAttribute (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      // means involved type is a target
      if (!_involvedType.HasTargetClassDefintion)
        return false;

      foreach (var mixinDefinition in _involvedType.TargetClassDefintion.GetProperty ("Mixins"))
      {
        // compared with ToString because MemberInfo has no own implementation of Equals
        var mixinMemberDefinition =
            mixinDefinition.CallMethod ("GetAllMembers").Where (mdb => mdb.GetProperty ("MemberInfo").ToString() == memberInfo.ToString()).
                SingleOrDefault();
        if (mixinMemberDefinition != null && mixinMemberDefinition.GetProperty ("Overrides").CallMethod ("ContainsKey", _type).To<bool>())
          return true;
      }
      return false;
    }

    public bool HasOverrideTargetAttribute (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);


      if (!_involvedType.IsMixin)
        return false;

      foreach (var typeAndMixinDefinitionPair in _involvedType.TargetTypes)
      {
        if (typeAndMixinDefinitionPair.Value == null)
          continue;

        var targetMemberDefinition = typeAndMixinDefinitionPair.Value.GetProperty ("TargetClass")
            .CallMethod ("GetAllMembers").Where (mdb => mdb.GetProperty ("MemberInfo").ToString() == memberInfo.ToString())
            .SingleOrDefault();
        if (targetMemberDefinition != null && targetMemberDefinition.GetProperty ("Overrides").CallMethod ("ContainsKey", _type).To<bool>())
          return true;
      }

      return false;
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