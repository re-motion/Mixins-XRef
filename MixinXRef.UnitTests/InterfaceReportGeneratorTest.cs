using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InterfaceReportGeneratorTest
  {
    private ReportContext _context;

    [SetUp]
    public void SetUp ()
    {
      _context = new ReportContext (
          new Assembly[0],
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new InvolvedTypeFinderStub()
          );
    }

    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var reportGenerator = new InterfaceReportGenerator (_context);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Interfaces");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    //[Test]
    //public void GenerateXml_WithInterfaces ()
    //{
    //  // TargetClass1 implements IDisposable
    //  var involvedType = new InvolvedType (typeof (TargetClass1));
    //  _context.InvolvedTypeFinder = new InvolvedTypeFinderStub (involvedType);
    //  var reportGenerator = new InterfaceReportGenerator (_context);

    //  XElement output = reportGenerator.GenerateXml();

    //  var expectedOutput = new XElement (
    //      "Interfaces",
    //      new XElement (
    //          "Interface",
    //          new XAttribute ("id", "0"),
    //          new XAttribute ("assembly-ref", "0"),
    //          new XAttribute ("namespace", "System"),
    //          new XAttribute ("name", "IDisposable")
    //          ));
    //  Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    //}
  }
}