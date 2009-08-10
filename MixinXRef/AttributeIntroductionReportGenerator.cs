using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeIntroductionReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _targetType;
    private readonly Type _mixinType;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeIntroductionReportGenerator (
        InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration, IIdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _targetType = targetType;
      _mixinType = mixinType;
      _mixinConfiguration = mixinConfiguration;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      if (_targetType.IsGenericTypeDefinition || _mixinType.IsGenericTypeDefinition)
        return null;

      var targetClassDefinition = _targetType.GetTargetClassDefinition (_mixinConfiguration);

      return new XElement (
          "AttributeIntroductions",
          from introducedAttribute in targetClassDefinition.Mixins[_mixinType].AttributeIntroductions
          select GenerateAttributeReferanceElement (introducedAttribute.AttributeType));
    }

    private XElement GenerateAttributeReferanceElement (Type introducedAttribute)
    {
      return new XElement (
          "Attribute",
          new XAttribute ("ref", _attributeIdentifierGenerator.GetIdentifier (introducedAttribute))
          );
    }
  }
}