using System;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AssemblyReportGeneratorTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;
    private IdentifierGenerator<Assembly> _identifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _identifierGenerator = new IdentifierGenerator<Assembly>();
      _assembly1 = typeof (ReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var assemblies = new Assembly[0];
      var reportGenerator = new AssemblyReportGenerator (assemblies, _identifierGenerator);
      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("Assemblies");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var assemblies = new[] { _assembly1};
      var reportGenerator = new AssemblyReportGenerator (assemblies, _identifierGenerator);
      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("full-name", _assembly1.FullName),
              new XAttribute ("code-base", _assembly1.CodeBase)));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_MoreAssemblies ()
    {
      var assemblies =  new[] { _assembly1, _assembly2 };
      var reportGenerator = new AssemblyReportGenerator (assemblies, _identifierGenerator);
      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("full-name", _assembly1.FullName),
              new XAttribute ("code-base", _assembly1.CodeBase)),
          new XElement (
              "Assembly",
              new XAttribute ("id", "1"),
              new XAttribute ("full-name", _assembly2.FullName),
              new XAttribute ("code-base", _assembly2.CodeBase)));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}