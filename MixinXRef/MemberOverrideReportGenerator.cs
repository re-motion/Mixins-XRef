using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MemberOverrideReportGenerator : IReportGenerator
  {
    private readonly IEnumerable<MemberDefinitionBase> _memberDefinitions;

    public MemberOverrideReportGenerator (IEnumerable<MemberDefinitionBase> memberDefinitions)
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

    private XElement GenerateOverridenMemberElement (MemberDefinitionBase overriddenMember)
    {
      
      return new XElement(
          "Member",
          new XAttribute("type", overriddenMember.MemberType),
          new XAttribute("name", overriddenMember.Name)
          );
    }
  }
}