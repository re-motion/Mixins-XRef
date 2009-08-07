using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public MixinReferenceReportGenerator (InvolvedType involvedType, IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _involvedType = involvedType;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      if (!_involvedType.IsTarget)
        return null;

      return new XElement (
        "Mixins",
        from mixin in _involvedType.ClassContext.Mixins
        select GenerateMixinElement(mixin));
    }

    private XElement GenerateMixinElement (MixinContext mixinContext)
    {
      return new XElement (
          "Mixin",
          new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(mixinContext.MixinType))
          );

    }
  }
}