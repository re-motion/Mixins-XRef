using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MemberOverrideReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _targetType;
    private readonly Type _mixinType;
    private readonly MixinConfiguration _mixinConfiguration;

    public MemberOverrideReportGenerator (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      _targetType = targetType;
      _mixinType = mixinType;
      _mixinConfiguration = mixinConfiguration;
    }

    public XElement GenerateXml ()
    {
      if (_targetType.IsGenericTypeDefinition)
        return null;

      var targetClassDefinition = _targetType.GetTargetClassDefinition (_mixinConfiguration);

        return new XElement (
          "MemberOverrides",
          from overridenMember in targetClassDefinition.GetMixinByConfiguredType (_mixinType).GetAllOverrides()
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