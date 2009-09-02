using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberReportGeneratorTest
  {
    private const string c_overriddenSpan = "<span class=\"keyword\">overridden</span>";
    private IOutputFormatter _outputFormatter;
    
    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_InterfaceWithZeroMembers ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(IUseless), null, _outputFormatter);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("PublicMembers");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InterfaceWithMembers ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(IDisposable), null, _outputFormatter);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "Dispose"),
              new XElement("modifiers", new XCData("")),
              new XElement("signature", "Void Dispose()")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ObjectWithoutOwnMembers ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(UselessObject), null, _outputFormatter);

      var output = reportGenerator.GenerateXml();

      // enhancement: surpress output of default constructor if generated by compiler
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XElement ("modifiers", new XCData ("")),
              new XElement("signature", "Void .ctor()")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_PropertyWithoutGetAndSet_Overriden ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(ClassWithProperty), null, _outputFormatter);

      var output = reportGenerator.GenerateXml();

      // MemberReportGenerator removes get_* and set_* functions of properties
      var expectedOutput = new XElement (
          "PublicMembers",
          new XElement(
              "Member",
              new XAttribute("type", MemberTypes.Method),
              new XAttribute("name", "DoSomething"),
              new XElement("modifiers", new XCData(c_overriddenSpan)),
              new XElement("signature", "Void DoSomething()")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XElement("modifiers", new XCData("")),
              new XElement("signature", "Void .ctor()")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Property),
              new XAttribute ("name", "PropertyName"),
              new XElement("modifiers", new XCData(c_overriddenSpan)),
              new XElement("signature", "System.String PropertyName")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void IsOverriddenMember_Method ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(object), null, _outputFormatter);

      var baseMethodInfo = typeof (BaseClassWithProperty).GetMethod ("DoSomething");
      var subMethodInfo = typeof (ClassWithProperty).GetMethod ("DoSomething");

      Assert.That (reportGenerator.IsOverriddenMember (baseMethodInfo), Is.EqualTo (false));
      Assert.That (reportGenerator.IsOverriddenMember (subMethodInfo), Is.EqualTo (true));
    }

    [Test]
    public void IsOverridenMember_Property ()
    {
      var reportGenerator = new MemberReportGenerator(typeof(object), null, _outputFormatter);

      var basePropertyInfo = typeof (BaseClassWithProperty).GetProperty ("PropertyName");
      var subPropertyInfo = typeof (ClassWithProperty).GetProperty ("PropertyName");

      Assert.That (reportGenerator.IsOverriddenMember (basePropertyInfo), Is.EqualTo (false));
      Assert.That (reportGenerator.IsOverriddenMember (subPropertyInfo), Is.EqualTo (true));
    }
  }
}