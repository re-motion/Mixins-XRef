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
    private readonly MemberModifierUtility _memberModifierUtility = new MemberModifierUtility();

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
      // TODO: find a better way to remove private modifiers
      return new XElement (
          "Members",
          from memberInfo in _type.GetMembers (BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
          where memberInfo.DeclaringType == _type && 
                !IsSpecialName (memberInfo) && 
                !_memberModifierUtility.GetMemberModifiers (memberInfo).Contains("private")
          select new XElement (
              "Member",
              new XAttribute ("type", memberInfo.MemberType),
              new XAttribute ("name", memberInfo.Name),
              _outputFormatter.CreateModifierMarkup (_memberModifierUtility.GetMemberModifiers (memberInfo)),
              new XElement ("signature", memberInfo)
              )
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