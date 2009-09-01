using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    private ErrorAggregator<Exception> _configurationErrors;
    private ErrorAggregator<Exception> _validationErrors;
    private IRemotionReflection _remotionReflection;

    [SetUp]
    public void SetUp ()
    {
      _configurationErrors = new ErrorAggregator<Exception>();
      _validationErrors = new ErrorAggregator<Exception>();
      _remotionReflection = ProgramTest.GetRemotionReflection();
    }

    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType (typeof (object));

      var reportGenerator = new MixinReferenceReportGenerator (
          involvedTypeDummy,
          new MixinConfiguration(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          _configurationErrors,
          _validationErrors,
          _remotionReflection
          );

      var output = reportGenerator.GenerateXml();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_WithMixins ()
    {
      var targetType = new InvolvedType (typeof (TargetClass1));


      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          mixinConfiguration,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _configurationErrors,
          _validationErrors,
          _remotionReflection
          );

      var output = reportGenerator.GenerateXml();

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));

      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("relation", "Extending"),
              new InterfaceIntroductionReportGenerator (new ReflectedObject(mixinDefinition.InterfaceIntroductions), interfaceIdentifierGenerator).GenerateXml(),
              new AttributeIntroductionReportGenerator(new ReflectedObject(mixinDefinition.AttributeIntroductions), attributeIdentifierGenerator, ProgramTest.GetRemotionReflection()).GenerateXml(),
              new MemberOverrideReportGenerator (new ReflectedObject(mixinDefinition.GetAllOverrides())).GenerateXml()
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var targetType = new InvolvedType (typeof (GenericTarget<,>));

      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<ClassWithBookAttribute>().AddMixin<Mixin3>()
          .BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          mixinConfiguration,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _configurationErrors,
          _validationErrors,
          _remotionReflection);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("relation", "Extending")),
          new XElement (
              "Mixin",
              new XAttribute ("ref", "1"),
              new XAttribute ("relation", "Extending"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_MixinConfigurationError ()
    {
      var targetType = new InvolvedType (typeof (UselessObject));

      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<MixinWithConfigurationError>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.Last();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          mixinConfiguration,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _configurationErrors,
          _validationErrors,
          _remotionReflection);

      reportGenerator.GenerateXml();

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (1));
    }

    [Test]
    public void GenerateXml_MixinValidationError ()
    {
      var targetType = new InvolvedType (typeof (UselessObject));

      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<UselessObject>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          mixinConfiguration,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _configurationErrors,
          _validationErrors,
          _remotionReflection);

      reportGenerator.GenerateXml();

      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (1));
    }
  }
}