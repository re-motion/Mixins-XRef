using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflection _remotionReflection;
    private readonly IOutputFormatter _outputFormatter;

    private readonly SummaryPicker _summaryPicker = new SummaryPicker();
    private readonly TypeModifierUtility _typeModifierUtility = new TypeModifierUtility();

    public InvolvedTypeReportGenerator (
        InvolvedType[] involvedTypes,
        ReflectedObject mixinConfiguration,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflection remotionReflection,
        IOutputFormatter outputFormatter
      )
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _mixinConfiguration = mixinConfiguration;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _remotionReflection = remotionReflection;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var involvedType in _involvedTypes)
        involvedTypesElement.Add (CreateInvolvedTypeElement (involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (InvolvedType involvedType)
    {
      var realType = involvedType.Type;
      var targetClassDefinition = GetTargetClassDefinition (involvedType);

      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _involvedTypeIdentifierGenerator.GetIdentifier (realType)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (realType.Assembly)),
          new XAttribute ("namespace", realType.Namespace),
          new XAttribute ("name", _outputFormatter.GetFormattedTypeName (realType)),
          new XAttribute ("base", GetCSharpLikeNameForBaseType (realType)),
          new XAttribute ("base-ref", (realType.BaseType == null ? "none" : _involvedTypeIdentifierGenerator.GetIdentifier (realType.BaseType))),
          new XAttribute ("is-target", involvedType.IsTarget),
          new XAttribute ("is-mixin", involvedType.IsMixin),
          new XAttribute ("is-generic-definition", realType.IsGenericTypeDefinition),
          _outputFormatter.CreateModifierMarkup(_typeModifierUtility.GetTypeModifiers (realType)),
          _summaryPicker.GetSummary(realType),
          new MemberReportGenerator(realType, targetClassDefinition, _outputFormatter).GenerateXml(),
          new InterfaceReferenceReportGenerator (
              realType, _interfaceIdentifierGenerator, _remotionReflection).GenerateXml(),
          new AttributeReferenceReportGenerator (
              realType, _attributeIdentifierGenerator, _remotionReflection).GenerateXml(),
          new MixinReferenceReportGenerator (
              involvedType,
              targetClassDefinition,
              _involvedTypeIdentifierGenerator,
              _interfaceIdentifierGenerator,
              _attributeIdentifierGenerator,
              _remotionReflection,
              _outputFormatter).GenerateXml(),
          new TargetReferenceReportGenerator (
              involvedType, _involvedTypeIdentifierGenerator).GenerateXml()
          );
    }

    public ReflectedObject GetTargetClassDefinition (InvolvedType involvedType)
    {
      if (!involvedType.IsTarget || involvedType.Type.IsGenericTypeDefinition)
        return null;

      try
        {
          // may throw ConfigurationException or ValidationException
          return _remotionReflection.GetTargetClassDefinition(involvedType.Type, _mixinConfiguration);
        }
       catch (Exception configurationOrValidationException)
       {
         if (_remotionReflection.IsConfigurationException(configurationOrValidationException))
           _configurationErrors.AddException(configurationOrValidationException);
         else if (_remotionReflection.IsValidationException(configurationOrValidationException))
           _validationErrors.AddException(configurationOrValidationException);
         else
           throw;
       }
      // MixinConfiguration is not valid
      return null;
    }

    private string GetCSharpLikeNameForBaseType(Type type)
    {
      return type.BaseType == null ? "none" : _outputFormatter.GetFormattedTypeName(type.BaseType);
    }
  }
}