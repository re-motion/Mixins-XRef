using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class MixinReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    // TargetClassDefinition _targetClassDefinition
    private readonly ReflectedObject _targetClassDefinition;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;
    private readonly IOutputFormatter _outputFormatter;

    public MixinReferenceReportGenerator (
        InvolvedType involvedType,
        ReflectedObject targetClassDefinitionOrNull,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        IRemotionReflection remotionReflection,
        IOutputFormatter outputFormatter
        )
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      // may be null
      // ArgumentUtility.CheckNotNull ("targetClassDefinitionOrNull", targetClassDefinitionOrNull);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _involvedType = involvedType;
      _targetClassDefinition = targetClassDefinitionOrNull;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflection = remotionReflection;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      if (!_involvedType.IsTarget)
        return null;

      return new XElement (
          "Mixins",
          from mixin in _involvedType.ClassContext.GetProperty("Mixins")
          select GenerateMixinElement (mixin));
    }

    private XElement GenerateMixinElement (ReflectedObject mixinContext)
    {
      // property MixinType on mixinContext always return the generic type definition, not the type of the actual instance
      var specificName = new XAttribute ("instance-name", _outputFormatter.GetCSharpLikeName (mixinContext.GetProperty ("MixinType").To<Type>()));
      var mixinElement = new XElement (
          "Mixin",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (mixinContext.GetProperty ("MixinType").To<Type>())),
          new XAttribute ("relation", mixinContext.GetProperty ("MixinKind")),
          specificName
          );

      if (_targetClassDefinition != null)
      {
        var mixinDefinition = _targetClassDefinition.CallMethod("GetMixinByConfiguredType", mixinContext.GetProperty("MixinType").To<Type>());

        // set more specific name for mixin references
        specificName.Value = _outputFormatter.GetCSharpLikeName(mixinDefinition.GetProperty("Type").To<Type>());

        mixinElement.Add(
            new InterfaceIntroductionReportGenerator(mixinDefinition.GetProperty("InterfaceIntroductions"), _interfaceIdentifierGenerator).
                GenerateXml());
        mixinElement.Add(
            new AttributeIntroductionReportGenerator(mixinDefinition.GetProperty("AttributeIntroductions"), _attributeIdentifierGenerator, _remotionReflection).GenerateXml());
        mixinElement.Add(
            new MemberOverrideReportGenerator(mixinDefinition.CallMethod("GetAllOverrides")).GenerateXml());
      }

      return mixinElement;
    }
  }
}