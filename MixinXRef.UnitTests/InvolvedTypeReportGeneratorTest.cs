using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = CreateReportGenerator (new Assembly[0]);
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (TargetClass2), true, false);
      var involvedType3 = new InvolvedType (typeof (Mixin1), false, true);
      InvolvedTypeReportGenerator reportGenerator = CreateReportGenerator (new Assembly[0], involvedType1, involvedType2, involvedType3);

      XElement output = reportGenerator.GenerateXml();

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
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "2"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin1"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (object), false, false);

      InvolvedTypeReportGenerator reportGenerator =
          CreateReportGenerator (
              new[] { typeof (IdentifierGenerator<>).Assembly, typeof (InvolvedTypeReportGeneratorTest).Assembly, typeof (object).Assembly },
              involvedType1,
              involvedType2);

      XElement output = reportGenerator.GenerateXml();

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

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private InvolvedTypeReportGenerator CreateReportGenerator (Assembly[] referencedAssemblies, params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      foreach (var referencedAssembly in referencedAssemblies)
        assemblyIdentifierGenerator.GetIdentifier (referencedAssembly);

      return new InvolvedTypeReportGenerator (
          involvedTypes,
          assemblyIdentifierGenerator,
          new IdentifierGenerator<Type>());
    }
  }
}