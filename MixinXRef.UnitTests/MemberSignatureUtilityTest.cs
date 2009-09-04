using System;
using System.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberSignatureUtilityTest
  {

    private MemberSignatureUtility _memberSignatureUtility;

    [SetUp]
    public void SetUp ()
    {
      _memberSignatureUtility = new MemberSignatureUtility();
    }

    [Test]
    public void GetMemberSignature_MethodNoParams ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMethod ("Dispose");
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("Void Dispose ()"));
    }

    [Test]
    public void GetMemberSignature_MethodWithParams ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMethod ("MethodWithParams");
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("long MethodWithParams (int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam)"));
    }

    [Test]
    public void GetMemberSignature_Property ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("string ProtectedProperty"));
    }

    [Test]
    public void GetMemberSignature_Constructor ()
    {
      var memberInfo = typeof (TypeModifierTestClass).GetMember (".ctor", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("TypeModifierTestClass (int Param1, string Param2, ApplicationAssemblyFinderFilter Param3)"));
      // Assert.That (output, Is.EqualTo (".ctor (Int32 Param1, String Param2, ApplicationAssemblyFinderFilter Param3)"));
    }

    [Test]
    public void GetMemberSignature_Event ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("event ChangedEventHandler ProtectedInternalEvent"));
    }
  }
}