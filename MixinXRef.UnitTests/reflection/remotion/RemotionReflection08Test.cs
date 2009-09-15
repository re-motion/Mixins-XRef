using System;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context.MixedTypes;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using System.Linq;

namespace MixinXRef.UnitTests.Reflection.Remotion
{
  [TestFixture]
  public class RemotionReflection08Test
  {
    private IRemotionReflection _remotionReflection;

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

      var reflectedOutput = _remotionReflection.GetTargetClassDefinition (typeof (TargetClass2), new ReflectedObject (mixinConfiguration));
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetClass2), mixinConfiguration);

      // is only true because target class definition gets cached!
      Assert.That(reflectedOutput.To<TargetClassDefinition>(), Is.SameAs(expectedOutput));

      // TargetClassDefinition has no overriden equals
      var classContext = mixinConfiguration.ClassContexts.First();
      Assert.That(new TargetClassDefinition(classContext), Is.Not.EqualTo(new TargetClassDefinition(classContext)));
    }

    [Test]
    public void BuildConfigurationFromAssemblies()
    {
      var assemblies = new[] { typeof(TargetClass1).Assembly, typeof(object).Assembly };

      var reflectedOuput = _remotionReflection.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(assemblies);

      Assert.That(reflectedOuput.To<MixinConfiguration>().ClassContexts, Is.EqualTo(expectedOutput.ClassContexts));
    }

    [Test]
    public void FindRemotionAssembly_FindRightAssembly ()
    {
      var remotionAssembly = typeof (TargetClassDefinitionUtility).Assembly;
      var assemblies = new[] { typeof (object).Assembly, remotionAssembly };

      var output = _remotionReflection.FindRemotionAssembly (assemblies);

      Assert.That (output, Is.EqualTo (remotionAssembly));
    }

    [Test]
    public void FindRemotionAssembly_FindNone()
    {
      var assemblies = new[] { typeof(object).Assembly };

      var output = _remotionReflection.FindRemotionAssembly(assemblies);

      Assert.That(output, Is.Null);
    }



  }
}