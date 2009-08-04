using System;
using System.Collections.Generic;
using System.Reflection;
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
      _reportGenerator = new ReportGenerator();
    }

    [Test]
    public void GenerateXml ()
    {
      var assemblies = new HashSet<Assembly> ();
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement ("MixinXRefReport", new XElement ("Assemblies"));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}