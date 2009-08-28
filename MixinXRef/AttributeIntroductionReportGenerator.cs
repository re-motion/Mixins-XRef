using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class AttributeIntroductionReportGenerator : IReportGenerator
  {
    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public AttributeIntroductionReportGenerator (
        MultiDefinitionCollection<Type, AttributeIntroductionDefinition> attributeIntroductionDefinitions,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("attributeIntroductionDefinitions", attributeIntroductionDefinitions);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _attributeIntroductionDefinitions = attributeIntroductionDefinitions;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "AttributeIntroductions",
          from introducedAttribute in _attributeIntroductionDefinitions
          where !_remotionReflection.IsInfrastructureType (introducedAttribute.AttributeType)
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