using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    // MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    private readonly SummaryPicker _summaryPicker = new SummaryPicker();
    private readonly TypeModifierUtility _typeModifierUtility = new TypeModifierUtility();

    public InvolvedTypeReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IRemotionReflector remotionReflector,
        IOutputFormatter outputFormatter
        )
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflector = remotionReflector;
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

      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _involvedTypeIdentifierGenerator.GetIdentifier (realType)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (realType.Assembly)),
          new XAttribute ("namespace", realType.Namespace),
          new XAttribute ("name", _outputFormatter.GetShortFormattedTypeName (realType)),
          new XAttribute ("base", GetCSharpLikeNameForBaseType (realType)),
          new XAttribute ("base-ref", (realType.BaseType == null ? "none" : _involvedTypeIdentifierGenerator.GetIdentifier (realType.BaseType))),
          new XAttribute ("is-target", involvedType.IsTarget),
          new XAttribute ("is-mixin", involvedType.IsMixin),
          new XAttribute ("is-generic-definition", realType.IsGenericTypeDefinition),
          _outputFormatter.CreateModifierMarkup (GetAlphabeticOrderingAttribute (involvedType), _typeModifierUtility.GetTypeModifiers (realType)),
          _summaryPicker.GetSummary (realType),
          new MemberReportGenerator (realType, involvedType, _involvedTypeIdentifierGenerator, _outputFormatter).GenerateXml(),
          new InterfaceReferenceReportGenerator (
              involvedType, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml(),
          new AttributeReferenceReportGenerator (
              realType, _attributeIdentifierGenerator, _remotionReflector).GenerateXml(),
          new MixinReferenceReportGenerator (
              involvedType,
              _involvedTypeIdentifierGenerator,
              _interfaceIdentifierGenerator,
              _attributeIdentifierGenerator,
              _remotionReflector,
              _outputFormatter).GenerateXml(),
          new TargetReferenceReportGenerator (
              involvedType, _involvedTypeIdentifierGenerator).GenerateXml()
          );
    }

    public string GetAlphabeticOrderingAttribute (InvolvedType involvedType)
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);

      foreach (var mixinDefinition in involvedType.TargetTypes.Values)
      {
        if (mixinDefinition == null)
          continue;

        if (mixinDefinition.GetProperty ("AcceptsAlphabeticOrdering").To<bool>())
          return "AcceptsAlphabeticOrdering ";
      }
      return "";
    }

    private string GetCSharpLikeNameForBaseType (Type type)
    {
      return type.BaseType == null ? "none" : _outputFormatter.GetShortFormattedTypeName (type.BaseType);
    }
  }
}