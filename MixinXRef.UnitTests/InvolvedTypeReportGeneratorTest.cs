using System;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using MixinXRef.UnitTests.TestDomain;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
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
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = new InvolvedTypeReportGenerator (_context);
      
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (TargetClass2), true, false);
      var involvedType3 = new InvolvedType (typeof (Mixin1), false, true);
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub ( involvedType1, involvedType2, involvedType3 );
      var reportGenerator = new InvolvedTypeReportGenerator (_context);

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
      
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub ( involvedType1, involvedType2 );
      var reportGenerator = new InvolvedTypeReportGenerator (_context);

      _context.AssemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGenerator<>).Assembly); // 0
      _context.AssemblyIdentifierGenerator.GetIdentifier (typeof (InvolvedTypeReportGeneratorTest).Assembly); // 1
      _context.AssemblyIdentifierGenerator.GetIdentifier (typeof (object).Assembly); // 2

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