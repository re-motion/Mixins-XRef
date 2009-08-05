using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AssemblyReportGeneratorTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;
    private ReportContext _context;

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = typeof (CompositeReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;

      _context = new ReportContext (
          new Assembly[0],
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type> (),
          new InvolvedTypeFinderStub()
          );
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var reportGenerator = new AssemblyReportGenerator (_context);
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Assemblies");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (TargetClass2), true, false);
      var involvedType3 = new InvolvedType (typeof (Mixin1), false, true);
      var involvedType4 = new InvolvedType (typeof (Mixin2), false, true);

      _context.Assemblies = new[] { _assembly1 };
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub (involvedType1, involvedType2, involvedType3, involvedType4);

      var reportGenerator = new AssemblyReportGenerator (_context);
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("full-name", _assembly1.FullName),
              new XAttribute ("code-base", _assembly1.CodeBase),
              new XElement ("InvolvedType", new XAttribute ("ref", "0")),
              new XElement ("InvolvedType", new XAttribute ("ref", "1")),
              new XElement ("InvolvedType", new XAttribute ("ref", "2")),
              new XElement ("InvolvedType", new XAttribute ("ref", "3"))
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_MoreAssemblies ()
    {
      _context.InvolvedTypeFinder = new InvolvedTypeFinderStub();
      _context.Assemblies= new[] { _assembly1, _assembly2 };
      var reportGenerator = new AssemblyReportGenerator (_context);
      
      XElement output = reportGenerator.GenerateXml();

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

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}