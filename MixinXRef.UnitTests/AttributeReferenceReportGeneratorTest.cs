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
      // Mixin2 has SerializableAttribute, SerializableAttribute has no parameters
      var reportGenerator = new AttributeReferenceReportGenerator (typeof (Mixin2), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement ("Attribute", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributesWithParameters ()
    {
      // ClassWithBookAttribute has the following attribute: [Book (1, Title = "C# in depth")]
      var reportGenerator = new AttributeReferenceReportGenerator (typeof (ClassWithBookAttribute), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("ref", "0"),
              new XElement (
                  "Argument",
                  new XAttribute ("kind", "constructor"),
                  new XAttribute ("type", "Int32"),
                  new XAttribute ("name", "id"),
                  new XAttribute ("value", 1337)),
              new XElement (
                  "Argument",
                  new XAttribute ("kind", "named"),
                  new XAttribute ("type", "String"),
                  new XAttribute ("name", "Title"),
                  new XAttribute ("value", "C# in depth"))
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributesWithFieldParameter ()
    {
      // ClassWithAttributeFieldParam has the following attribute: [FieldParam(new[] { "AttributeParam1", "AttributeParam2"})]
      var reportGenerator = new AttributeReferenceReportGenerator (typeof (ClassWithAttributeFieldParam), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("ref", "0"),
              new XElement (
                  "Argument",
                  new XAttribute ("kind", "constructor"),
                  new XAttribute ("type", "String[]"),
                  new XAttribute ("name", "stringArray"),
                  new XAttribute ("value", "{AttributeParam1, AttributeParam2}")
                  )
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}