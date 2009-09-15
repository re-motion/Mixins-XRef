using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AssemblyReportGeneratorTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = typeof (CompositeReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var reportGenerator = CreateReportGenerator (new Assembly[0]);
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Assemblies");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1));
      var involvedType2 = new InvolvedType (typeof (TargetClass2));
      var involvedType3 = new InvolvedType (typeof (Mixin1));
      var involvedType4 = new InvolvedType (typeof (Mixin2));

      var reportGenerator = CreateReportGenerator (new[] { _assembly1 }, involvedType1, involvedType2, involvedType3, involvedType4);
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute("name", _assembly1.GetName().Name),
              new XAttribute("version", _assembly1.GetName().Version),
              new XAttribute ("location", "./" + Path.GetFileName (_assembly1.Location)),
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

      var reportGenerator = CreateReportGenerator (new[] { _assembly1, _assembly2 });
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("name", _assembly1.GetName().Name),
              new XAttribute("version", _assembly1.GetName().Version),
              new XAttribute("location", "./" + Path.GetFileName(_assembly1.Location))),
          new XElement (
              "Assembly",
              new XAttribute ("id", "1"),
              new XAttribute("name", _assembly2.GetName().Name),
              new XAttribute("version", _assembly2.GetName().Version),
              // _assembly2 is of type object - which is a GAC (mscorlib.dll)
              new XAttribute ("location", _assembly2.Location)));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GetShortAssemblyLocation ()
    {
      var reportGenerator = CreateReportGenerator (new Assembly[0]);
      // non-GAC assembly
      Assert.That (reportGenerator.GetShortAssemblyLocation (_assembly1), Is.EqualTo ("./" + Path.GetFileName(_assembly1.Location)));
      // GAC assembly
      Assert.That (reportGenerator.GetShortAssemblyLocation (_assembly2), Is.EqualTo (_assembly2.Location));
    }

    private AssemblyReportGenerator CreateReportGenerator (Assembly[] assemblies, params InvolvedType[] involvedTypes)
    {
      return new AssemblyReportGenerator (
          assemblies,
          involvedTypes,
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>());
    }
  }
}