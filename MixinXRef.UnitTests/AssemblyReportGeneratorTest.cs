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
    private IdentifierGenerator<Assembly> _identifierGenerator;
    private IdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _identifierGenerator = new IdentifierGenerator<Assembly>();
      _assembly1 = typeof (CompositeReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;

      _involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var assemblies = new Assembly[0];
      var finder = new InvolvedTypeFinderStub();

      var reportGenerator = new AssemblyReportGenerator (assemblies, finder, _identifierGenerator, _involvedTypeIdentifierGenerator);
      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("Assemblies");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var assemblies = new[] { _assembly1};

      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (TargetClass2), true, false);
      var involvedType3 = new InvolvedType (typeof (Mixin1), false, true);
      var involvedType4 = new InvolvedType (typeof (Mixin2), false, true);
      var finder = new InvolvedTypeFinderStub (involvedType1, involvedType2, involvedType3, involvedType4);

      var reportGenerator = new AssemblyReportGenerator (assemblies, finder, _identifierGenerator, _involvedTypeIdentifierGenerator);
      XElement output = reportGenerator.GenerateXml ();

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

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_MoreAssemblies ()
    {
      var finder = new InvolvedTypeFinderStub ();

      var assemblies =  new[] { _assembly1, _assembly2 };
      var reportGenerator = new AssemblyReportGenerator (assemblies, finder, _identifierGenerator, _involvedTypeIdentifierGenerator);
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