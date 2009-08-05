// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
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
      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof(object));
      
      var expectedInvolvedType = new InvolvedType (typeof (object), false, false);
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }

    [Test]
    public void GetOrCreateValue_NonEmptyStore ()
    {
      var involvedTypeSetup = _involvedTypeStore.GetOrCreateValue (typeof (object));
      involvedTypeSetup.IsMixin = true;

      var involvedType = _involvedTypeStore.GetOrCreateValue (typeof (object));

      var expectedInvolvedType = new InvolvedType (typeof (object), false, true);
      Assert.That (involvedType, Is.EqualTo (expectedInvolvedType));
    }


  }
}