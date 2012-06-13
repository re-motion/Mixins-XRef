using System;
using System.Xml.Linq;
using MixinXRef.Report;
using MixinXRef.UnitTests.Stub;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class CompositeReportGeneratorTest
  {
    private CompositeReportGenerator _reportGenerator;

    [SetUp]
    public void SetUp ()
    {
      _reportGenerator = new CompositeReportGenerator (new ReportGeneratorStub ("Assemblies"), new ReportGeneratorStub ("InvolvedTypes"));
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