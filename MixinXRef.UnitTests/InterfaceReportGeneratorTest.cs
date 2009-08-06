using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InterfaceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var reportGenerator = CreateReportGenerator();
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Interfaces");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposable
      var involvedType = new InvolvedType (typeof (TargetClass1));
      var reportGenerator = CreateReportGenerator (involvedType);
      var memberReportGenerator = new MemberReportGenerator (typeof(IDisposable));

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Interfaces",
          new XElement (
              "Interface",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "IDisposable"),
              memberReportGenerator.GenerateXml()
              ));
      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    private InterfaceReportGenerator CreateReportGenerator (params InvolvedType[] involvedTypes)
    {
      var context = new ReportContext (
          new Assembly[0],
          involvedTypes,
          new IdentifierGenerator<Assembly> (),
          new IdentifierGenerator<Type> (),
          new IdentifierGenerator<Type> (),
          new IdentifierGenerator<Type> ());
      return new InterfaceReportGenerator (context);
    }
  }
}