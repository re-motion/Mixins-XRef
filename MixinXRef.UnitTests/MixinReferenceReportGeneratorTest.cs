using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType (typeof (object));
      var reportGenerator = new MixinReferenceReportGenerator(involvedTypeDummy, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_WithMixins ()
    {
      var targetType = new InvolvedType (typeof (TargetClass1));
      targetType.IsTarget = true;
      targetType.ClassContext = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration().ClassContexts.First();
      var reportGenerator = new MixinReferenceReportGenerator(targetType, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}