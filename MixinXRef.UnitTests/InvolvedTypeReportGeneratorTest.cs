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
      var iType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var iType2 = new InvolvedType (typeof (TargetClass2), true, false);
      var iType3 = new InvolvedType (typeof (Mixin1), false, true);
      var finder = new InvolvedTypeFinderStub ( iType1, iType2, iType3 );
      var reportGenerator = new InvolvedTypeReportGenerator (finder, _typeIdentifierGenerator, _assemblyIdentifierGenerator);

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false)),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false)),
         new XElement(
              "InvolvedType",
              new XAttribute("id", "2"),
              new XAttribute("assembly-ref", "0"),
              new XAttribute("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute("name", "Mixin1"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true))
        );

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (object), false, false);
      var finder = new InvolvedTypeFinderStub ( involvedType1, involvedType2 );
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
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false)),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "2"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false)));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}