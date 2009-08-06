using System;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AttributeReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroAttributes ()
    {
      var reportGenerator = new AttributeReferenceReportGenerator (typeof (UselessObject), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Attributes");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributes ()
    {
      // Mixin2 has SerializableAttribute
      var reportGenerator = new AttributeReferenceReportGenerator (typeof (Mixin2), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement ("Attribute", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}