using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ReportGeneratorTest
  {
    private ReportGenerator _reportGenerator;

    [SetUp]
    public void SetUp ()
    {
      _reportGenerator = new ReportGenerator (new AssemblyReportGeneratorStub("Assemblies"), new InvolvedTypeReportGeneratorStub ("InvolvedTypes"));
    }

    [Test]
    public void GenerateXml ()
    {
      XElement output = _reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("MixinXRefReport", new XElement ("Assemblies"), new XElement ("InvolvedTypes"));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}