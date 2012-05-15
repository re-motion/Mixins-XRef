using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
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
          from overriddenMember in _memberDefinitions
          select GenerateOverriddenMemberElement (overriddenMember));
    }

    private XElement GenerateOverriddenMemberElement(ReflectedObject overriddenMember)
    {
      return new XElement (
          "OverriddenMember",
          new XAttribute ("type", overriddenMember.GetProperty("MemberType")),
          new XAttribute ("name", overriddenMember.GetProperty("Name"))
          );
    }
  }
}