using System;
using System.Linq;
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
    public void ToSortedArray_EmptyStore ()
    {
      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (new InvolvedType[0]));
    }

    [Test]
    public void ToSortedArray_NonEmptyStore ()
    {
      var involvedType1 = _involvedTypeStore.GetOrCreateValue (typeof (object));
      var involvedType2 = _involvedTypeStore.GetOrCreateValue (typeof (string));

      var expectedTypes = new[] { involvedType1, involvedType2 };
      Assert.That (_involvedTypeStore.ToArray(), Is.EqualTo (expectedTypes));
    }

    [Test]
    public void ToSortedArray_OrderByFullName ()
    {
      var involvedType1 = _involvedTypeStore.GetOrCreateValue (typeof (System.String));      // 3
      var involvedType2 = _involvedTypeStore.GetOrCreateValue (typeof (System.Object));      // 2
      var involvedType3 = _involvedTypeStore.GetOrCreateValue (typeof (System.IDisposable)); // 1

      var expectedTypesInOrder = new[] { involvedType3, involvedType2, involvedType1 };
      Assert.That (_involvedTypeStore.ToArray (), Is.EqualTo (expectedTypesInOrder));
    }
  }
}