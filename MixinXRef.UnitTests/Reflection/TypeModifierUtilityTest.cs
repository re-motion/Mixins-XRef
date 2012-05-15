using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
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
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass).GetNestedType ("PrivateClass", BindingFlags.Instance | BindingFlags.NonPublic)), Is.EqualTo ("private"));
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TopLevelInternalClass)), Is.EqualTo ("internal"));
    }

    [Test]
    public void GetTypeModifiers_Sealed ()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (PublicSealedClass)), Is.EqualTo ("public sealed"));
      // struct is sealed by default
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (TypeModifierTestClass.PublicStruct)), Is.EqualTo ("public sealed"));
    }

    [Test]
    public void GetTypeModifiers_Abstract ()
    {
      Assert.That (_typeModifierUtility.GetTypeModifiers (typeof (PublicAbstractClass)), Is.EqualTo ("public abstract"));      
    }
  }
}