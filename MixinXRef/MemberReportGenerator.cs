using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly InvolvedType _involvedType;
    // TargetClassDefinition
    private readonly ReflectedObject _targetClassDefinition;
    private readonly IOutputFormatter _outputFormatter;
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility();
    private readonly MemberSignatureUtility _memberSignatureUtility;


    public MemberReportGenerator (Type type, InvolvedType involvedTypeOrNull, ReflectedObject targetClassDefinitionOrNull, IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // may be null
      // ArgumentUtility.CheckNotNull ("involvedTypeOrNull", involvedTypeOrNull);
      // ArgumentUtility.CheckNotNull ("targetClassDefinitionOrNull", targetClassDefinitionOrNull);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _type = type;
      _involvedType = involvedTypeOrNull;
      _targetClassDefinition = targetClassDefinitionOrNull;
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
      //var memberAttributes = new StringBuilder();
      //if (_targetClassDefinition != null)
      //{
      //  var memberDefintionBase = _targetClassDefinition.CallMethod ("GetAllMembers")
      //      .Where (memberDefinitionBase => memberDefinitionBase.GetProperty ("MemberInfo").To<MemberInfo>() == memberInfo).Single();

      //  if (memberDefintionBase.GetProperty ("Overrides").GetProperty ("Count").To<int>() > 0)
      //    ; // is overriden?!;

      //  MemberDefinitionBase mdb;
        
      //  TargetClassDefinition tcd;


      //  // member visibility attribute
        
      //}

      return new XElement (
          "Member",
          new XAttribute ("type", memberInfo.MemberType),
          new XAttribute ("name", memberInfo.Name),
          _outputFormatter.CreateModifierMarkup ("", _memberModifierUtility.GetMemberModifiers (memberInfo)),
          _memberSignatureUtility.GetMemberSignatur (memberInfo)
          );
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