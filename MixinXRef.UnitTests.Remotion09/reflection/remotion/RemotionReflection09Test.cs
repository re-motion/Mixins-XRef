using System;
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.UnitTests.Remotion09.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.Remotion09.Reflection.Remotion
{
  [TestFixture]
  public class RemotionReflection09Test
  {
    private IRemotionReflection _remotionReflection;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflection = new RemotionReflection09 (typeof (TargetClassDefinitionFactory).Assembly);
    }

    [Test]
    public void IsNonApplicationAssembly_False ()
    {
      var assembly = typeof (IDisposable).Assembly;
      var output = _remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.False);
    }

    //[Test]
    //public void IsNonApplicationAssembly_True()
    //{
    //  // SafeContext is type in Remotion.Mixins.Persistent.Signed, which is NonApplicationAssembly
    //  var assembly = typeof(SafeContext).Assembly;
    //  var output = _remotionReflection.IsNonApplicationAssembly(assembly);

    //  Assert.That(output, Is.True);
    //}

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
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var reflectedOutput = _remotionReflection.GetTargetClassDefinition (typeof (TargetClass1), new ReflectedObject (mixinConfiguration));
      var expectedOutput = TargetClassDefinitionFactory.CreateTargetClassDefinition (mixinConfiguration.ClassContexts.First());

      Assert.That (reflectedOutput.To<TargetClassDefinition>(), Is.InstanceOfType (typeof (TargetClassDefinition)));
      Assert.That (reflectedOutput.To<TargetClassDefinition>().FullName, Is.EqualTo (expectedOutput.FullName));
    }

    [Test]
    public void BuildConfigurationFromAssemblies ()
    {
      var assemblies = new[] { typeof (TargetClass1).Assembly, typeof (object).Assembly };

      var reflectedOuput = _remotionReflection.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (reflectedOuput.To<MixinConfiguration>().ClassContexts, Is.EqualTo (expectedOutput.ClassContexts));
    }

    [Test]
    public void FindRemotionAssembly_FindRightAssembly ()
    {
      var remotionAssembly = typeof (TargetClassDefinitionFactory).Assembly;
      var assemblies = new[] { typeof (object).Assembly, remotionAssembly };

      var output = _remotionReflection.FindRemotionAssembly (assemblies);

      Assert.That (output, Is.EqualTo (remotionAssembly));
    }

    [Test]
    public void FindRemotionAssembly_FindNone ()
    {
      var assemblies = new[] { typeof (object).Assembly };

      var output = _remotionReflection.FindRemotionAssembly (assemblies);

      Assert.That (output, Is.Null);
    }
  }
}