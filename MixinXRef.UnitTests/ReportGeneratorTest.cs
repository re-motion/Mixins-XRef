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
    private Assembly _assembly1;
    private Assembly _assembly2;

    [SetUp]
    public void SetUp ()
    {
      _reportGenerator = new ReportGenerator();
      _assembly1 = typeof (ReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var assemblies = CreateAssemblySet ();
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement ("MixinXRefReport", new XElement ("Assemblies"));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var assemblies = CreateAssemblySet (_assembly1);
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement (
          "MixinXRefReport", 
          new XElement ("Assemblies",
              new XElement ("Assembly",
                  new XAttribute ("id", "0"),
                  new XAttribute ("fullName", _assembly1.FullName),
                  new XAttribute ("codeBase", _assembly1.CodeBase))));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_MoreAssemblies ()
    {
      var assemblies = CreateAssemblySet (_assembly1, _assembly2);
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement (
          "MixinXRefReport",
          new XElement ("Assemblies",
              new XElement ("Assembly",
                  new XAttribute ("id", "0"),
                  new XAttribute ("fullName", _assembly1.FullName),
                  new XAttribute ("codeBase", _assembly1.CodeBase)),
              new XElement ("Assembly",
                  new XAttribute ("id", "1"),
                  new XAttribute ("fullName", _assembly2.FullName),
                  new XAttribute ("codeBase", _assembly2.CodeBase))));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    private HashSet<Assembly> CreateAssemblySet (params Assembly[] assemblies)
    {
      return new HashSet<Assembly> (assemblies);
    }
  }
}