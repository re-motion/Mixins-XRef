using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Utility;


namespace MixinXRef.Report
{
  public class MemberOverrideReportGenerator : IReportGenerator
  {
    // IEnumerable<MemberDefinitionBase>
    private readonly ReflectedObject _memberDefinitions;

    public MemberOverrideReportGenerator(ReflectedObject memberDefinitions)
    {
      ArgumentUtility.CheckNotNull ("memberDefinitions", memberDefinitions);

      _memberDefinitions = memberDefinitions;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "MemberOverrides",
          from overridenMember in _memberDefinitions
          select GenerateOverridenMemberElement (overridenMember));
    }

    private XElement GenerateOverridenMemberElement(ReflectedObject overriddenMember)
    {
      return new XElement (
          "Member",
          new XAttribute ("type", overriddenMember.GetProperty("MemberType")),
          new XAttribute ("name", overriddenMember.GetProperty("Name"))
          );
    }
  }
}