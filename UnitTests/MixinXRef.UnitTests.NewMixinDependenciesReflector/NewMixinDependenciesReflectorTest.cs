using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.NewMixinDependenciesReflector.TestDomain;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests.NewMixinDependenciesReflector
{
  [TestFixture]
  public class NewMixinDependenciesReflectorTest
  {
    private IRemotionReflector _remotionReflector;
    private TargetClassDefinition _targetClassDefinition;
    private MixinDefinition _mixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.NewMixinDependenciesReflector ();

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      _targetClassDefinition = TargetClassDefinitionFactory.CreateTargetClassDefinition (mixinConfiguration.ClassContexts.First ());
      _mixinDefinition = _targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));
    }

    [Test]
    public void GetNextCallDependencies ()
    {
      var nextCallDependencies = _mixinDefinition.NextCallDependencies;
      var output = _remotionReflector.GetNextCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (nextCallDependencies));
    }

    [Test]
    public void GetTargetCallDependencies ()
    {
      var targetCallDependencies = _mixinDefinition.TargetCallDependencies;
      var output = _remotionReflector.GetTargetCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (targetCallDependencies));
    }

  }
}
