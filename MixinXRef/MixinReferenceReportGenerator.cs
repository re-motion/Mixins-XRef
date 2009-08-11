using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Validation;
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
    private readonly ErrorAggregator<ConfigurationException> _configurationError;
    private readonly ErrorAggregator<ValidationException> _validationErrors;

    public MixinReferenceReportGenerator (InvolvedType involvedType, 
      MixinConfiguration mixinConfiguration, 
      IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
      IIdentifierGenerator<Type> interfaceIdentifierGenerator,
      IIdentifierGenerator<Type> attributeIdentifierGenerator,
      ErrorAggregator<ConfigurationException> configurationError,
      ErrorAggregator<ValidationException> validationErrors
      )
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("configurationError", configurationError);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);

      _involvedType = involvedType;
      _mixinConfiguration = mixinConfiguration;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _configurationError = configurationError;
      _validationErrors = validationErrors;
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
          new XAttribute ("relation", mixinContext.MixinKind)
          );

      if (!_involvedType.Type.IsGenericTypeDefinition)
      {
        try
        {
          // may throw ConfigurationException and ValidationException
          var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (_involvedType.Type, _mixinConfiguration);
          var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (mixinContext.MixinType);

          mixinElement.Add (
              new InterfaceIntroductionReportGenerator (mixinDefinition.InterfaceIntroductions, _interfaceIdentifierGenerator).GenerateXml());
          mixinElement.Add (
              new AttributeIntroductionReportGenerator (mixinDefinition.AttributeIntroductions, _attributeIdentifierGenerator).GenerateXml());
          mixinElement.Add (
              new MemberOverrideReportGenerator (mixinDefinition.GetAllOverrides()).GenerateXml());
        }
        catch (ConfigurationException configurationException)
        {
          _configurationError.AddException(configurationException);
        }
        catch (ValidationException validationException)
        {
          _validationErrors.AddException (validationException);
        }
      }

      return mixinElement;
    }
  }
}