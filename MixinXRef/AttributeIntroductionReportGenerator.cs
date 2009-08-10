using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeIntroductionReportGenerator : IReportGenerator
  {
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeIntroductionReportGenerator (
        MultiDefinitionCollection<Type, AttributeIntroductionDefinition> attributeIntroductionDefinitions,
        IIdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("attributeIntroductionDefinitions", attributeIntroductionDefinitions);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _attributeIntroductionDefinitions = attributeIntroductionDefinitions;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "AttributeIntroductions",
          from introducedAttribute in _attributeIntroductionDefinitions
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