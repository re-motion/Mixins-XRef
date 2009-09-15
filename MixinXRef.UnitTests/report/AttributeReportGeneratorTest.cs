using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AttributeReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoAttributes ()
    {
      // UselessObject has no attributes
      var involvedType = new InvolvedType (typeof (UselessObject));
      var reportGenerator = CreateReportGenerator (involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Attributes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributes ()
    {
      // Mixin2 has Serializable attribute
      var involvedType = new InvolvedType (typeof (Mixin2));
      var reportGenerator = CreateReportGenerator (involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "SerializableAttribute"),
              new XElement (
                  "AppliedTo",
                  new XElement (
                      "InvolvedType",
                      new XAttribute ("ref", "0")
                      )
                  )
              )
          );
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithNestedAttribute ()
    {
      // ClassWithNestedAttribute has 'ClassWithNestedAttribute.NestedAttribute' applied
      var involvedType = new InvolvedType (typeof (ClassWithNestedAttribute));
      var reportGenerator = CreateReportGenerator (involvedType);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "ClassWithNestedAttribute.NestedAttribute"),
              new XElement (
                  "AppliedTo",
                  new XElement (
                      "InvolvedType",
                      new XAttribute ("ref", "0")
                      )
                  )
              )
          );
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }


    private AttributeReportGenerator CreateReportGenerator (params InvolvedType[] involvedTypes)
    {
      return new AttributeReportGenerator (
          involvedTypes,
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          ProgramTest.GetRemotionReflection(),
          new OutputFormatter());
    }
  }
}