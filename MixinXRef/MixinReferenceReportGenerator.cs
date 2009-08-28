using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflection _remotionReflection;

    public MixinReferenceReportGenerator (
        InvolvedType involvedType,
        MixinConfiguration mixinConfiguration,
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
          select GenerateMixinElement (mixin));
    }

    private XElement GenerateMixinElement (MixinContext mixinContext)
    {
      var mixinElement = new XElement (
          "Mixin",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (mixinContext.MixinType)),
          new XAttribute ("relation", mixinContext.MixinKind)
          );

      if (!_involvedType.Type.IsGenericTypeDefinition)
      {
        try
        {
          // may throw Exception and Exception
          var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (_involvedType.Type, _mixinConfiguration);
          var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (mixinContext.MixinType);

          mixinElement.Add (
              new InterfaceIntroductionReportGenerator (new ReflectedObject(mixinDefinition.InterfaceIntroductions), _interfaceIdentifierGenerator).GenerateXml());
          mixinElement.Add (
            new AttributeIntroductionReportGenerator (new ReflectedObject(mixinDefinition.AttributeIntroductions), _attributeIdentifierGenerator, _remotionReflection).GenerateXml());
          mixinElement.Add (
              new MemberOverrideReportGenerator (mixinDefinition.GetAllOverrides()).GenerateXml());
        }
        catch (Exception configurationOrException)
        {
          if (_remotionReflection.IsConfigurationException (configurationOrException))
            _configurationErrors.AddException (configurationOrException);
          else if (_remotionReflection.IsValidationException (configurationOrException))
            _validationErrors.AddException (configurationOrException);
          else
            throw;
        }
      }

      return mixinElement;
    }
  }
}