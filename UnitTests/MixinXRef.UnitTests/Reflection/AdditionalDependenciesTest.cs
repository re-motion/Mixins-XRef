using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class AdditionalDependenciesTest
  {

    [Test]
    public void AdditionalDependencies ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ().ForClass<TargetClass> ()
          .AddMixin<Mixin1> ()
          .AddMixin<Mixin2> ()
          .AddMixin<Mixin3> ()
          .WithDependencies<Mixin1, Mixin2>()
          .BuildConfiguration ();

      var explicitDependencies = mixinConfiguration.ClassContexts.Single().Mixins.Last().ExplicitDependencies;

      Assert.That (explicitDependencies.Count, Is.EqualTo(2));
      Assert.That (explicitDependencies.First (), Is.EqualTo (typeof(Mixin1)));
      Assert.That (explicitDependencies.Last (), Is.EqualTo (typeof (Mixin2)));
      }

    #region TestDomain for AdditionalDependenciesTest

    public class TargetClass
    {
    }

    public class Mixin1{}

    public class Mixin2{}

    public class Mixin3 { }

    #endregion
  }
}