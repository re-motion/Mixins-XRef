using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.OldMixinDependenciesReflector.TestDomain;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests.OldMixinDependenciesReflector
{
  [TestFixture]
  public class OldMixinDependenciesReflector
  {
    private IRemotionReflector _remotionReflector;
    private TargetClassDefinition _targetClassDefinition;
    private MixinDefinition _mixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.OldMixinDependenciesReflector ();

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      _targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetClass1), mixinConfiguration);
      _mixinDefinition = _targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));
    }

    [Test]
    public void GetNextCallDependencies ()
    {
      var nextCallDependencies = _mixinDefinition.BaseDependencies;
      var output = _remotionReflector.GetNextCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (nextCallDependencies));
    }

    [Test]
    public void GetTargetCallDependencies ()
    {
      var targetCallDependencies = _mixinDefinition.ThisDependencies;
      var output = _remotionReflector.GetTargetCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (targetCallDependencies));
    }
  }
}
