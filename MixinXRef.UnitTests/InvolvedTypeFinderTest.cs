using System;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeFinderTest
  {
    [Test]
    public void FindInvolvedTypes_EmptyConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration();
      var involvedTypeFinder = new InvolvedTypeFinder (mixinConfiguration);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      Assert.That(involvedTypes, Is.Empty);
    }

    [Test]
    public void FindInvolvedTypes_WithOneTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      var involvedTypeFinder = new InvolvedTypeFinder (mixinConfiguration);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var expectedType2 = new InvolvedType (typeof (Mixin1), false, true);
      Assert.That (involvedTypes, Is.EqualTo (new[] { expectedType1, expectedType2 }));
    }

    [Test]
    public void FindInvolvedTypes_WithMoreTargets ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .ForClass<TargetClass2>().AddMixin<Mixin2> ()
          .BuildConfiguration ();
      var involvedTypeFinder = new InvolvedTypeFinder (mixinConfiguration);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var expectedType2 = new InvolvedType (typeof (Mixin1), false, true);
      var expectedType3 = new InvolvedType (typeof (TargetClass2), true, false);
      var expectedType4 = new InvolvedType (typeof (Mixin2), false, true);
      
      Assert.That (involvedTypes, Is.EquivalentTo (new[] { expectedType1, expectedType2, expectedType3, expectedType4 }));
    }

    [Test]
    public void FindInvolvedTypes_WithMixedMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .ForClass<Mixin1> ().AddMixin<Mixin2> ()
          .BuildConfiguration ();
      var involvedTypeFinder = new InvolvedTypeFinder (mixinConfiguration);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes ();

      var expectedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var expectedType2 = new InvolvedType (typeof (Mixin1), true, true);
      var expectedType3 = new InvolvedType (typeof (Mixin2), false, true);
      
      Assert.That (involvedTypes, Is.EquivalentTo (new[] { expectedType1, expectedType2, expectedType3}));
    }
  }
}