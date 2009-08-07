using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public MixinReferenceReportGenerator (InvolvedType involvedType, 
      MixinConfiguration mixinConfiguration, 
      IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      IIdentifierGenerator<Type> interfaceIdentifierGenerator
      )
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);


      _involvedType = involvedType;
      _mixinConfiguration = mixinConfiguration;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
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
          new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(mixinContext.MixinType)),
          new InterfaceIntroductionGenerator (_involvedType.Type, mixinContext.MixinType, _mixinConfiguration, _interfaceIdentifierGenerator).GenerateXml ()
          );

    }
  }
}