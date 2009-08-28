using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberReportGeneratorTest
  {
    [Test]
    public void GenerateXml_InterfaceWithZeroMembers ()
    {
      var reportGenerator = new MemberReportGenerator (typeof (IUseless));

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("PublicMembers");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InterfaceWithMembers ()
    {
      var reportGenerator = new MemberReportGenerator (typeof (IDisposable));

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "Dispose"),
              new XAttribute ("overridden", false)
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ObjectWithoutOwnMembers ()
    {
      var reportGenerator = new MemberReportGenerator (typeof (UselessObject));

      var output = reportGenerator.GenerateXml();

      // enhancement: surpress output of default constructor if generated by compiler
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XAttribute ("overridden", false)
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_PropertyWithoutGetAndSet_Overriden ()
    {
      var reportGenerator = new MemberReportGenerator (typeof (ClassWithProperty));

      var output = reportGenerator.GenerateXml();

      // MemberReportGenerator removes get_* and set_* functions of properties
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XAttribute ("overridden", false)
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Property),
              new XAttribute ("name", "PropertyName"),
              new XAttribute ("overridden", true)
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}