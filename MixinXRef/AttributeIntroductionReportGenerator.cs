using System;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeIntroductionReportGenerator : IReportGenerator
  {
    private readonly Type _targetType;
    private readonly Type _mixinType;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeIntroductionReportGenerator (Type targetType, Type mixinType, MixinConfiguration mixinConfiguration, IdentifierGenerator<Type> attributeIdentifierGenerator)
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
      return new XElement ("AttributeIntroductions");
    }
  }
}