using System;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InterfaceReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var reportGenerator = new InterfaceReferenceReportGenerator (typeof (object), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Interfaces");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposealbe
      var reportGenerator = new InterfaceReferenceReportGenerator (typeof (TargetClass1), new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Interfaces",
          new XElement ("Interface", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}