using System;
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.NonApplicationAssembly;
using MixinXRef.UnitTests.TargetClassDefinitionFactoryReflector.TestDomain;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests.TargetClassDefinitionFactoryReflector
{
  [TestFixture]
  public class TargetClassDefinitionFactoryReflectorTest
  {
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.TargetClassDefinitionFactoryReflector().Initialize (".");
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      var reflectedOutput = _remotionReflector.GetTargetClassDefinition (typeof (TargetClass1), new ReflectedObject (mixinConfiguration), new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass1))));
      var expectedOutput = TargetClassDefinitionFactory.CreateTargetClassDefinition (mixinConfiguration.ClassContexts.First ());

      Assert.That (reflectedOutput.To<TargetClassDefinition> (), Is.InstanceOf (typeof (TargetClassDefinition)));
      Assert.That (reflectedOutput.To<TargetClassDefinition> ().FullName, Is.EqualTo (expectedOutput.FullName));
    }

    [Test]
    public void IsNonApplicationAssembly ()
    {
      var assembly1 = typeof (IDisposable).Assembly;
      var assembly2 = typeof (ClassForNonApplicationAssembly).Assembly;

      Assert.IsFalse (_remotionReflector.IsNonApplicationAssembly (assembly1));
      Assert.IsTrue (_remotionReflector.IsNonApplicationAssembly (assembly2));
    }

  }
}
