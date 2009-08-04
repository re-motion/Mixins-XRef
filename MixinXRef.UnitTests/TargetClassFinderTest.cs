using System;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class TargetClassFinderTest
  {
    [Test]
    public void FindInvolvedTypes_EmptyConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration();
      var targetClassFinder = new TargetClassFinder (mixinConfiguration);

      var targetClasses = targetClassFinder.FindTargetClasses();

      Assert.That (targetClasses, Is.Empty);
    }

    [Test]
    public void FindInvolvedTypes_WithOneTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      var targetClassFinder = new TargetClassFinder (mixinConfiguration);

      var targetClasses = targetClassFinder.FindTargetClasses();

      Assert.That (targetClasses, Is.EqualTo (new[] { typeof (TargetClass1) } ));
    }

    [Test]
    public void FindInvolvedTypes_WithMoreTargets ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .ForClass<TargetClass2>().AddMixin<Mixin2> ()
          .BuildConfiguration ();
      var targetClassFinder = new TargetClassFinder (mixinConfiguration);

      var targetClasses = targetClassFinder.FindTargetClasses ();

      Assert.That (targetClasses, Is.EquivalentTo (new[] { typeof (TargetClass1), typeof (TargetClass2) }));
    }
  }
}