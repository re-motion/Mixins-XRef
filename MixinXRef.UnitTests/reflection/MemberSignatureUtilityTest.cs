using System;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class MemberSignatureUtilityTest
  {

    private MemberSignatureUtility _memberSignatureUtility;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
      _memberSignatureUtility = new MemberSignatureUtility(_outputFormatter);
    }

    [Test]
    public void GetMemberSignature_MethodWithParams ()
    {
      var methodInfo = typeof (MemberSignatureTestClass).GetMethod ("MethodWithParams");
      var output = _memberSignatureUtility.GetMemberSignatur(methodInfo);
      var expectedOutput = _outputFormatter.CreateMethodMarkup(methodInfo.Name, methodInfo.ReturnType, methodInfo.GetParameters());

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Property ()
    {
      var propertyInfo = typeof (MemberSignatureTestClass).GetProperty ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignatur(propertyInfo);
      var expectedOutput = _outputFormatter.CreatePropertyMarkup(propertyInfo.Name, propertyInfo.PropertyType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Constructor ()
    {
      var constructorInfo = typeof (MemberSignatureTestClass).GetConstructors (BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur(constructorInfo);
      var expectedOutput = _outputFormatter.CreateConstructorMarkup("MemberSignatureTestClass", constructorInfo.GetParameters());

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Event ()
    {
      var eventInfo = typeof (MemberSignatureTestClass).GetEvent ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignatur(eventInfo);
      var expectedOutput = _outputFormatter.CreateEventMarkup (eventInfo.Name, eventInfo.EventHandlerType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_Field ()
    {
      var fieldInfo = typeof (MemberSignatureTestClass).GetField ("_dictionary", BindingFlags.Instance | BindingFlags.NonPublic);
      var output = _memberSignatureUtility.GetMemberSignatur(fieldInfo);
      var expectedOutput = _outputFormatter.CreateFieldMarkup (fieldInfo.Name, fieldInfo.FieldType);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetMemberSignature_NestedClass ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember ("NestedClass", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);
      var expectedOutput = _outputFormatter.CreateNestedTypeMarkup ((Type) memberInfo);

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}