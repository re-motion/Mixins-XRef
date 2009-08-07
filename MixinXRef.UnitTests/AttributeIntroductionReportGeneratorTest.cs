using System;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AttributeIntroductionReportGeneratorTest
  {

    [Test]
    public void GenerateXml_NoIntroducedAttribute ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass2> ().AddMixin<Mixin1> ()
          .BuildConfiguration ();
      var reportGenerator = new AttributeIntroductionReportGenerator (typeof (TargetClass2), typeof (Mixin1), mixinConfiguration, new IdentifierGenerator<Type> ());
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("AttributeIntroductions");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_WithIntroducedAttributes ()
    {
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<UselessObject> ().AddMixin<ObjectWithInheritableAttribute> ()
          .BuildConfiguration ();

      var reportGenerator = new AttributeIntroductionReportGenerator (typeof (UselessObject), typeof (ObjectWithInheritableAttribute), mixinConfiguration, attributeIdentifierGenerator);

      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "AttributeIntroductions",
          new XElement (
              "Attribute",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}