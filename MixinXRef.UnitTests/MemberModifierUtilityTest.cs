using System;
using System.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberModifierUtilityTest
  {
    private MemberModifierUtility _memberModifierUtility;

    [SetUp]
    public void SetUp ()
    {
      _memberModifierUtility = new MemberModifierUtility();
    }

    [Test]
    public void IsOverriddenMember_Method ()
    {
      var baseMethodInfo = typeof (BaseClassWithProperty).GetMethod ("DoSomething");
      var subMethodInfo = typeof (ClassWithProperty).GetMethod ("DoSomething");

      Assert.That (_memberModifierUtility.IsOverriddenMember (baseMethodInfo), Is.EqualTo (false));
      Assert.That (_memberModifierUtility.IsOverriddenMember (subMethodInfo), Is.EqualTo (true));
    }

    [Test]
    public void IsOverridenMember_Property ()
    {
      var basePropertyInfo = typeof (BaseClassWithProperty).GetProperty ("PropertyName");
      var subPropertyInfo = typeof (ClassWithProperty).GetProperty ("PropertyName");

      Assert.That (_memberModifierUtility.IsOverriddenMember (basePropertyInfo), Is.EqualTo (false));
      Assert.That (_memberModifierUtility.IsOverriddenMember (subPropertyInfo), Is.EqualTo (true));
    }

    [Test]
    public void GetMemberModifiers_PublicMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedProperty ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedInternalEvent ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected internal"));
    }

    [Test]
    public void GetMemberModifiers_InternalField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("InternalField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("internal"));
    }

    [Test]
    public void GetMemberModifiers_PrivateField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("_privateField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("private"));
    }

    [Test]
    public void GetMemberModifiers_PublicVirtualMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public virtual"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractMethod ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractAndOverriddenMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract override"));
    }

    [Test]
    public void GetMemberModifiers_PubliOverriddenAndSealedMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public sealed override"));
    }

    [Test]
    public void GetMemberModifiers_NestedType ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("NestedClass")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo (new TypeModifierUtility().GetTypeModifiers(typeof(MemberModifierTestClass.NestedClass))));
    }

    [Test]
    public void GetMemberModifiers_InterfaceImplementation ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("Dispose")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }

    [Test]
    public void GetMemberModifiers_ReadonlyField ()
    {
      var memberInfo = typeof (MemberModifierTestClass).GetMember ("_readonlyField")[0];

      var output = _memberModifierUtility.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public readonly"));
    }
  }
}