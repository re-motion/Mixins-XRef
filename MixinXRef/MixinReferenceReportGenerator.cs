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
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public MixinReferenceReportGenerator (InvolvedType involvedType, 
      MixinConfiguration mixinConfiguration, 
      IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      IIdentifierGenerator<Type> interfaceIdentifierGenerator,
      IIdentifierGenerator<Type> attributeIdentifierGenerator
      )
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _involvedType = involvedType;
      _mixinConfiguration = mixinConfiguration;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
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
      var mixinElement = new XElement (
          "Mixin",
          new XAttribute("ref", _involvedTypeIdentifierGenerator.GetIdentifier(mixinContext.MixinType)),
          new XAttribute ("relation", GetMixinKind(mixinContext))
          );

      if (!_involvedType.Type.IsGenericTypeDefinition)
      {
        var targetClassDefinition = _involvedType.GetTargetClassDefinition (_mixinConfiguration);
        var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (mixinContext.MixinType);
        mixinElement.Add (new InterfaceIntroductionReportGenerator (mixinDefinition.InterfaceIntroductions, _interfaceIdentifierGenerator).GenerateXml ());
        mixinElement.Add (new AttributeIntroductionReportGenerator (mixinDefinition.AttributeIntroductions, _attributeIdentifierGenerator).GenerateXml ());
        mixinElement.Add (new MemberOverrideReportGenerator (mixinDefinition.GetAllOverrides()).GenerateXml ());
      }

      return mixinElement;
    }

    private String GetMixinKind (MixinContext mixinContext)
    {
      if (mixinContext.MixinKind == MixinKind.Extending)
        return "extends";
      // else: mixinContext.MixinKind == MixinKind.Used
      return "used by";
    }
  }
}