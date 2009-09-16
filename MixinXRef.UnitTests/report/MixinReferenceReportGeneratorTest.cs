using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    private IRemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = ProgramTest.GetRemotionReflection();
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType (typeof (object));

      var reportGenerator = new MixinReferenceReportGenerator (
          involvedTypeDummy,
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          _remotionReflector,
          _outputFormatter
          );

      var output = reportGenerator.GenerateXml();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_WithMixins ()
    {
      var targetType = new InvolvedType (typeof (TargetClass1));

      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      targetType.TargetClassDefintion = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration));

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _remotionReflector,
          _outputFormatter
          );

      var output = reportGenerator.GenerateXml();

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));

      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("index", "0"),
              new XAttribute ("relation", "Extending"),
              new XAttribute ("instance-name", "Mixin1"),
              new XAttribute ("introduced-member-visibility", "Private"),
              // has no dependencies
              new XElement ("AdditionalDependencies"),
              new InterfaceIntroductionReportGenerator (new ReflectedObject (mixinDefinition.InterfaceIntroductions), interfaceIdentifierGenerator).
                  GenerateXml(),
              new AttributeIntroductionReportGenerator (
                  new ReflectedObject (mixinDefinition.AttributeIntroductions), attributeIdentifierGenerator, ProgramTest.GetRemotionReflection()).
                  GenerateXml(),
              new MemberOverrideReportGenerator (new ReflectedObject (mixinDefinition.GetAllOverrides())).GenerateXml()
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
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType,
          // generic target class
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _remotionReflector,
          _outputFormatter);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("index", "n/a"),
              new XAttribute ("relation", "Extending"),
              new XAttribute ("instance-name", "ClassWithBookAttribute"),
              new XAttribute ("introduced-member-visibility", "Private"),
              // has no dependencies
              new XElement ("AdditionalDependencies")
              ),
          new XElement (
              "Mixin",
              new XAttribute ("ref", "1"),
              new XAttribute ("index", "n/a"),
              new XAttribute ("relation", "Extending"),
              new XAttribute ("instance-name", "Mixin3"),
              new XAttribute ("introduced-member-visibility", "Private"),
              // has no dependencies
              new XElement ("AdditionalDependencies")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}