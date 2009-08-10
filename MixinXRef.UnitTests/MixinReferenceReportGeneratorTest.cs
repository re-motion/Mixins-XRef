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
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("relation", "extends")
              ));
      if (!targetType.Type.IsGenericTypeDefinition)
      {
        var targetClassDefinition = targetType.GetTargetClassDefinition (mixinConfiguration);
        var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));
        expectedOutput.Descendants().First().Add (
            new InterfaceIntroductionReportGenerator (mixinDefinition.InterfaceIntroductions, interfaceIdentifierGenerator).GenerateXml());
        expectedOutput.Descendants().First().Add (
            new AttributeIntroductionReportGenerator (mixinDefinition.AttributeIntroductions, attributeIdentifierGenerator).GenerateXml());
        expectedOutput.Descendants().First().Add (new MemberOverrideReportGenerator (mixinDefinition.GetAllOverrides()).GenerateXml());
      }
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}