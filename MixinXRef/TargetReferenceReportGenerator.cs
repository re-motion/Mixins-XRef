using System;
using System.Linq;
using System.Xml.Linq;

namespace MixinXRef
{
  public class TargetReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _mixinType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public TargetReferenceReportGenerator (InvolvedType mixinType, IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _mixinType = mixinType;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      if (!_mixinType.IsMixin)
        return null;

      return new XElement (
          "Targets",
          from targetType in _mixinType.TargetTypes
          select GenerateTargetElement (targetType)
          );
    }

    private XElement GenerateTargetElement (Type targetType)
    {
      return new XElement (
          "Target",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (targetType))
          );
    }
  }
}