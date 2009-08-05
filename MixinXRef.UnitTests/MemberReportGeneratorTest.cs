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

      var output = reportGenerator.GenerateXml ();
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement(
              "Member",
              new XAttribute("type", MemberTypes.Method),
              new XAttribute("name", "Dispose")
              )
          );

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}