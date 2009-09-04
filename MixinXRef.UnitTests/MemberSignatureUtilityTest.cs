using System;
using System.Reflection;
using MixinXRef.Formatting;
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
      _memberSignatureUtility = new MemberSignatureUtility(new OutputFormatter());
    }

    [Test]
    public void GetMemberSignature_MethodNoParams ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMethod ("Dispose");
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("Void Dispose ()"));
    }

    [Test]
    public void GetMemberSignature_MethodWithParams ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMethod ("MethodWithParams");
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("long MethodWithParams (int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam)"));
    }

    [Test]
    public void GetMemberSignature_Property ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("string ProtectedProperty"));
    }

    [Test]
    public void GetMemberSignature_Constructor ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember (".ctor", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("MemberSignatureTestClass (int Param1, string Param2, ApplicationAssemblyFinderFilter Param3)"));
      // Assert.That (output, Is.EqualTo (".ctor (Int32 Param1, String Param2, ApplicationAssemblyFinderFilter Param3)"));
    }

    [Test]
    public void GetMemberSignature_Event ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("event ChangedEventHandler ProtectedInternalEvent"));
    }

    [Test]
    public void GetMemberSignature_Field ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember ("_dictionary", BindingFlags.Instance | BindingFlags.NonPublic)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("MultiDictionary<string, int> _dictionary"));
    }
    
    [Test]
    public void GetMemberSignature_NestedClassWithInterfaceAndInheritance ()
    {
      var memberInfo = typeof (MemberSignatureTestClass).GetMember ("NestedClassWithInterfaceAndInheritance", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
      var output = _memberSignatureUtility.GetMemberSignatur (memberInfo);

      Assert.That (output, Is.EqualTo ("class NestedClassWithInterfaceAndInheritance : GenericTarget<string, int>, IDisposable"));
    }

  }
}