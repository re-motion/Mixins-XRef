using System;
using System.Reflection;
using System.Xml.Linq;
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
          new IdentifierGenerator<Assembly> (),
          new IdentifierGenerator<Type> (),
          new IdentifierGenerator<Type> (),
          new InvolvedTypeFinderStub ()
          );
    }

    [Test]
    public void GenerateXml_NoAttributes ()
    {
      var reportGenerator = new AttributeReportGenerator (_context);

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("Attributes");
      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}