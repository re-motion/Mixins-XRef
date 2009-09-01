using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflection _remotionReflection;

    public MixinReferenceReportGenerator (
        InvolvedType involvedType,
        ReflectedObject mixinConfiguration,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflection remotionReflection
        )
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _involvedType = involvedType;
      _mixinConfiguration = mixinConfiguration;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      if (!_involvedType.IsTarget)
        return null;

      return new XElement (
          "Mixins",
          from mixin in _involvedType.ClassContext.Mixins
          select GenerateMixinElement (new ReflectedObject (mixin)));
    }

    private XElement GenerateMixinElement (ReflectedObject mixinContext)
    {
      var mixinElement = new XElement (
          "Mixin",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (mixinContext.GetProperty ("MixinType").To<Type>())),
          new XAttribute ("relation", mixinContext.GetProperty ("MixinKind"))
          );

      if (!_involvedType.Type.IsGenericTypeDefinition)
      {
        try
        {
          // may throw ConfigurationException or ValidationException
          var targetClassDefinition = _remotionReflection.GetTargetClassDefinition (_involvedType.Type, _mixinConfiguration);
          var mixinDefinition = targetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinContext.GetProperty ("MixinType").To<Type>());

          // TODO: use mixinDefinition.Type.GetGenericArguments() to find generic parameter instantiations

          mixinElement.Add (
              new InterfaceIntroductionReportGenerator (mixinDefinition.GetProperty("InterfaceIntroductions"), _interfaceIdentifierGenerator).
                  GenerateXml());
          mixinElement.Add (
              new AttributeIntroductionReportGenerator (mixinDefinition.GetProperty("AttributeIntroductions"), _attributeIdentifierGenerator, _remotionReflection).GenerateXml());
          mixinElement.Add (
              new MemberOverrideReportGenerator (mixinDefinition.CallMethod("GetAllOverrides")).GenerateXml());
        }
        catch (Exception configurationOrValidationException)
        {
          if (_remotionReflection.IsConfigurationException (configurationOrValidationException))
            _configurationErrors.AddException (configurationOrValidationException);
          else if (_remotionReflection.IsValidationException (configurationOrValidationException))
            _validationErrors.AddException (configurationOrValidationException);
          else
            throw;
        }
      }

      return mixinElement;
    }
  }
}