using System;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.DefaultReflector.TestDomain;
using MixinXRef.UnitTests.NonApplicationAssembly;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.DefaultReflector
{
  [TestFixture]
  public class DefaultReflectorTest
  {
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.DefaultReflector (".");
    }

    [Test]
    public void IsRelevantAssemblyForConfiguration ()
    {
      var assembly1 = typeof (IDisposable).Assembly;
      var assembly2 = typeof (DefaultReflectorTest).Assembly;

      Assert.IsFalse (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly1));
      Assert.IsTrue (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly2));
    }

    [Test]
    public void IsNonApplicationAssembly_False ()
    {
      var assembly = typeof (IDisposable).Assembly;
      var output = _remotionReflector.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.False);
    }

    [Test]
    public void IsNonApplicationAssembly_True ()
    {
      var assembly = typeof (ClassForNonApplicationAssembly).Assembly;
      var output = _remotionReflector.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }

    [Test]
    public void IsConfigurationException ()
    {
      var configurationException = new ConfigurationException ("configurationException");

      var outputTrue = _remotionReflector.IsConfigurationException (configurationException);
      var outputFalse = _remotionReflector.IsConfigurationException (new Exception ());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsValidationException ()
    {
      var validationLogData = new DefaultValidationLog ();
      var validationException = new ValidationException (validationLogData);

      var outputTrue = _remotionReflector.IsValidationException (validationException);
      var outputFalse = _remotionReflector.IsValidationException (new Exception ());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsInfrastructurType ()
    {
      var initializableMixinType = typeof (IInitializableMixin);
      var outputTrue = _remotionReflector.IsInfrastructureType (initializableMixinType);
      var outputFalse = _remotionReflector.IsInfrastructureType (typeof (IDisposable));

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsInheritedFromMixin ()
    {
      var outputTrue1 = _remotionReflector.IsInheritedFromMixin (typeof (Mixin<>));
      // Mixin<,> inherits from Mixin<>
      var outputTrue2 = _remotionReflector.IsInheritedFromMixin (typeof (Mixin<,>));
      // MemberOverrideWithInheritanceTest.CustomMixin inherits from Mixin<>
      var outputTrue3 = _remotionReflector.IsInheritedFromMixin (typeof (CompleteInterfacesTestClass.MyMixin));
      var outputFalse = _remotionReflector.IsInheritedFromMixin (typeof (object));

      Assert.That (outputTrue1, Is.True);
      Assert.That (outputTrue2, Is.True);
      Assert.That (outputTrue3, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      var reflectedOutput = _remotionReflector.GetTargetClassDefinition (typeof (TargetClass1), new ReflectedObject (mixinConfiguration), new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass1))));
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetClass1), mixinConfiguration);

      Assert.That (reflectedOutput.To<TargetClassDefinition> (), Is.InstanceOf (typeof (TargetClassDefinition)));
      Assert.That (reflectedOutput.To<TargetClassDefinition> ().FullName, Is.EqualTo (expectedOutput.FullName));
    }

    [Test]
    public void BuildConfigurationFromAssemblies ()
    {
      var assemblies = new[] { typeof (TargetClass1).Assembly, typeof (object).Assembly };

      var reflectedOuput = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (reflectedOuput.To<MixinConfiguration> ().ClassContexts, Is.EqualTo (expectedOutput.ClassContexts));
    }

    [Test]
    public void GetValidationLogFromValidationException ()
    {
      var validationLogData = new DefaultValidationLog ();
      var validationException = new ValidationException (validationLogData);

      var reflectedValidationLog = _remotionReflector.GetValidationLogFromValidationException (validationException);
      var result = reflectedValidationLog.To<DefaultValidationLog> ();

      Assert.That (result, Is.SameAs (validationLogData));
    }
  }
}
