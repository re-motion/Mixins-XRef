using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AttributeReportGeneratorTest
  {
    private ReportContext _context;

    [SetUp]
    public void SetUp ()
    {
      _context = new ReportContext (
          new Assembly[0],
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new InvolvedTypeFinderStub()
          );
    }

    [Test]
    public void GenerateXml_NoAttributes ()
    {
      // UselessObject has no attributes
      var involvedType = new InvolvedType (typeof (UselessObject));
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub (involvedType);
      var reportGenerator = new AttributeReportGenerator (_context);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Attributes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithAttributes ()
    {
      // Mixin2 has Serializable attribute
      var involvedType = new InvolvedType (typeof (Mixin2));
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub (involvedType);

      var reportGenerator = new AttributeReportGenerator (_context);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Attributes",
          new XElement (
              "Attribute",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "SerializableAttribute")
              )
          );
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}