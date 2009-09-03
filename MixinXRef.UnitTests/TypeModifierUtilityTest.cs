using System;
using System.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class TypeModifierUtilityTest
  {
    private TypeModifierUtility _typeModifierUtility;

    [SetUp]
    public void SetUp ()
    {
      _typeModifierUtility = new TypeModifierUtility();
    }

    [Test]
    public void GetTypeModifiers_Visibility()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers(typeof(TypeModifierTestClass)), Is.EqualTo ("public"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass).GetNestedType ("ProtectedClass", BindingFlags.Instance | BindingFlags.NonPublic)), Is.EqualTo ("protected"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.ProtectedInternalClass)), Is.EqualTo ("protected internal"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.InternalClass)), Is.EqualTo ("internal"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass).GetNestedType ("PrivateStruct", BindingFlags.Instance | BindingFlags.NonPublic)), Is.EqualTo ("private"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TopLevelInternalClass)), Is.EqualTo ("internal"));
    }
  }
}