using System;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using MixinXRef.UnitTests.TestDomain;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    private IdentifierGenerator<Type> _typeIdentifierGenerator;
    private IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _typeIdentifierGenerator = new IdentifierGenerator<Type>();
      _assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
    }

    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var finder = new InvolvedTypeFinderStub ();
      var reportGenerator = new InvolvedTypeReportGenerator (finder, _typeIdentifierGenerator, _assemblyIdentifierGenerator);
      
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var finder = new InvolvedTypeFinderStub (typeof (TargetClass1), typeof (TargetClass2));
      var reportGenerator = new InvolvedTypeReportGenerator (finder, _typeIdentifierGenerator, _assemblyIdentifierGenerator);

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1")),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2")));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var finder = new InvolvedTypeFinderStub (typeof (TargetClass1), typeof (object));
      var reportGenerator = new InvolvedTypeReportGenerator (finder, _typeIdentifierGenerator, _assemblyIdentifierGenerator);

      _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGenerator<>).Assembly); // 0
      _assemblyIdentifierGenerator.GetIdentifier (typeof (InvolvedTypeReportGeneratorTest).Assembly); // 1
      _assemblyIdentifierGenerator.GetIdentifier (typeof (object).Assembly); // 2

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
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
              new XAttribute ("assembly-ref", "2"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object")));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}