using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType (typeof (object));

      var reportGenerator = new MixinReferenceReportGenerator (
          involvedTypeDummy,
          new MixinConfiguration(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>()
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
          targetType, mixinConfiguration, new IdentifierGenerator<Type>(), interfaceIdentifierGenerator, attributeIdentifierGenerator);

      var output = reportGenerator.GenerateXml();

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));

      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("relation", "Extending"),
              new InterfaceIntroductionReportGenerator (mixinDefinition.InterfaceIntroductions, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeIntroductionReportGenerator (mixinDefinition.AttributeIntroductions, attributeIdentifierGenerator).GenerateXml(),
              new MemberOverrideReportGenerator (mixinDefinition.GetAllOverrides()).GenerateXml()
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var targetType = new InvolvedType (typeof (GenericTarget<>));


      var mixinConfiguration = MixinConfiguration.ActiveConfiguration;
      targetType.ClassContext = mixinConfiguration.ClassContexts.Where (classContext => classContext.Type.IsGenericTypeDefinition).Single();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType, mixinConfiguration, new IdentifierGenerator<Type>(), interfaceIdentifierGenerator, attributeIdentifierGenerator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("relation", "Used")),
          new XElement (
              "Mixin",
              new XAttribute ("ref", "1"),
              new XAttribute ("relation", "Used"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}