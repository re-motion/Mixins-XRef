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
      var assemblies = new Assembly[0];
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement ("MixinXRefReport", new XElement ("Assemblies"), new XElement ("InvolvedTypes"));
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_IntegrationOfAssembliesAndInvolvedTypes ()
    {
      var assembly1 = typeof (object).Assembly;
      var assembly2 = typeof (ReportGeneratorTest).Assembly;
      var assemblies = new[] { assembly1, assembly2};
      XElement output = _reportGenerator.GenerateXml (assemblies);

      var expectedOutput = new XElement (
          "MixinXRefReport",
          new XElement (
              "Assemblies",
              new XElement (
                  "Assembly",
                  new XAttribute ("id", "0"),
                  new XAttribute ("full-name", assembly1.FullName),
                  new XAttribute ("code-base", assembly1.CodeBase)),
              new XElement (
                  "Assembly",
                  new XAttribute ("id", "1"),
                  new XAttribute ("full-name", assembly2.FullName),
                  new XAttribute ("code-base", assembly2.CodeBase))), 
          new XElement (
              "InvolvedTypes",
              new XElement (
                "InvolvedType",
                new XAttribute ("id", "0"),
                new XAttribute ("assembly-ref", "1"),
                new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
                new XAttribute ("name", "TargetClass1")),
              new XElement (
                  "InvolvedType",
                  new XAttribute ("id", "1"),
                  new XAttribute ("assembly-ref", "1"),
                  new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
                  new XAttribute ("name", "Mixin1")),
              new XElement(
                  "InvolvedType",
                  new XAttribute("id", "2"),
                  new XAttribute("assembly-ref", "1"),
                  new XAttribute("namespace", "MixinXRef.UnitTests.TestDomain"),
                  new XAttribute("name", "TargetClass2")),
              new XElement (
                  "InvolvedType",
                  new XAttribute ("id", "3"),
                  new XAttribute ("assembly-ref", "1"),
                  new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
                  new XAttribute ("name", "Mixin2"))
      ));
      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}