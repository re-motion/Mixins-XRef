using System;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context.MixedTypes;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class RemotionReflectionTest
  {
    private RemotionReflection _remotionReflection;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflection = ProgramTest.GetRemotionReflection();
    }

    [Test]
    public void IsNonApplicationAssembly_False ()
    {
      var assembly = typeof (IDisposable).Assembly;
      var output = _remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.False);
    }

    [Test]
    public void IsNonApplicationAssembly_True ()
    {
      // SafeContext is type in Remotion.Mixins.Persistent.Signed, which is NonApplicationAssembly
      var assembly = typeof (SafeContext).Assembly;
      var output = _remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }

    [Test]
    public void IsConfigurationException ()
    {
      var configurationException = new ConfigurationException ("configurationException");

      var outputTrue = _remotionReflection.IsConfigurationException (configurationException);
      var outputFalse = _remotionReflection.IsConfigurationException (new Exception());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsValidationException ()
    {
      var validationException = new ValidationException (new DefaultValidationLog());

      var outputTrue = _remotionReflection.IsValidationException (validationException);
      var outputFalse = _remotionReflection.IsValidationException (new Exception());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsInfrastructurType ()
    {
      var outputTrue = _remotionReflection.IsInfrastructureType (typeof (IInitializableMixin));
      var outputFalse = _remotionReflection.IsInfrastructureType (typeof (IDisposable));

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var output = _remotionReflection.GetTargetClassDefinition (typeof (TargetClass2), new ReflectedObject (mixinConfiguration));
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetClass2), mixinConfiguration);

      Assert.That (output.To<TargetClassDefinition>(), Is.EqualTo (expectedOutput));
    }
  }
}