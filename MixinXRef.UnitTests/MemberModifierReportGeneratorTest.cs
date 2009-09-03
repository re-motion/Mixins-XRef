using System;
using System.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberModifierReportGeneratorTest
  {
    private MemberModifierReportGenerator _reportGenerator;

    [SetUp]
    public void SetUp ()
    {
      _reportGenerator = new MemberModifierReportGenerator();
    }

    [Test]
    public void IsOverriddenMember_Method ()
    {
      var baseMethodInfo = typeof (BaseClassWithProperty).GetMethod ("DoSomething");
      var subMethodInfo = typeof (ClassWithProperty).GetMethod ("DoSomething");

      Assert.That (_reportGenerator.IsOverriddenMember (baseMethodInfo), Is.EqualTo (false));
      Assert.That (_reportGenerator.IsOverriddenMember (subMethodInfo), Is.EqualTo (true));
    }

    [Test]
    public void IsOverridenMember_Property ()
    {
      var basePropertyInfo = typeof (BaseClassWithProperty).GetProperty ("PropertyName");
      var subPropertyInfo = typeof (ClassWithProperty).GetProperty ("PropertyName");

      Assert.That (_reportGenerator.IsOverriddenMember (basePropertyInfo), Is.EqualTo (false));
      Assert.That (_reportGenerator.IsOverriddenMember (subPropertyInfo), Is.EqualTo (true));
    }

    [Test]
    public void GetMemberModifiers_PublicMethod ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("PublicMethod")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedProperty ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("ProtectedProperty", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected"));
    }

    [Test]
    public void GetMemberModifiers_ProtectedInternalEvent ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("ProtectedInternalEvent", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("protected internal"));
    }

    [Test]
    public void GetMemberModifiers_InternalField ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("InternalField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("internal"));
    }

    [Test]
    public void GetMemberModifiers_PrivateField ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("_privateField", BindingFlags.Instance | BindingFlags.NonPublic)[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("private"));
    }

    [Test]
    public void GetMemberModifiers_PublicVirtualMethod ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public virtual"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractMethod ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract"));
    }

    [Test]
    public void GetMemberModifiers_PublicAbstractAndOverriddenMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicAbstractMethod")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public abstract override"));
    }

    [Test]
    public void GetMemberModifiers_PubliOverriddenAndSealedMethod ()
    {
      var memberInfo = typeof (SubModifierTestClass).GetMember ("PublicVirtualMethod")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public sealed override"));
    }

    [Test]
    public void GetMemberModifiers_NestedType ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("NestedClass")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("todo nested"));
    }

    [Test]
    public void GetMemberModifiers_Test1 ()
    {
      var memberInfo = typeof (ModifierTestClass).GetMember ("Dispose")[0];

      var output = _reportGenerator.GetMemberModifiers (memberInfo);
      Assert.That (output, Is.EqualTo ("public"));
    }
  }
}