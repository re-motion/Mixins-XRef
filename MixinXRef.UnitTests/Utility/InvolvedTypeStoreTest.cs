using System;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class InvolvedTypeStoreTest
  {
    private InvolvedTypeStore _involvedTypeStore;

    [SetUp]
    public void SetUp ()
    {
      _involvedTypeStore = new InvolvedTypeStore();
    }

    [Test]
    public void GetOrCreateValue_EmptyStore ()
    {
      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof (object));

      var expectedInvolvedType = new InvolvedType (typeof (object));
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }

    [Test]
    public void GetOrCreateValue_NonEmptyStore ()
    {
      _involvedTypeStore.GetOrCreateValue (typeof (object));

      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof (object));

      var expectedInvolvedType = new InvolvedType (typeof (object));
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }

    [Test]
    public void ToArray_EmptyStore ()
    {
      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (new InvolvedType[0]));
    }

    [Test]
    public void ToArray_NonEmptyStore ()
    {
      var involvedType1 = _involvedTypeStore.GetOrCreateValue (typeof (object));
      var involvedType2 = _involvedTypeStore.GetOrCreateValue (typeof (string));
      var expectedType = new[] { involvedType1, involvedType2 };

      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (expectedType));
    }
  }
}