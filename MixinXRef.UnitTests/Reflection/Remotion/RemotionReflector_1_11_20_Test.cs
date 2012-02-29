using System;
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.UnitTests.Explore;
using MixinXRef.UnitTests.NonApplicationAssembly;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.Reflection.Remotion
{
  [TestFixture]
  public class RemotionReflector_1_11_20_Test
  {
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new RemotionReflector_1_11_20 (".");
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
      // SafeContext is type in Remotion.Mixins.Persistent.Signed, which is a "NonApplicationAssembly"
      var assembly = typeof (ClassForNonApplicationAssembly).Assembly;
      var output = _remotionReflector.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }

    [Test]
    public void IsConfigurationException ()
    {
      var configurationException = new ConfigurationException ("configurationException");

      var outputTrue = _remotionReflector.IsConfigurationException (configurationException);
      var outputFalse = _remotionReflector.IsConfigurationException (new Exception());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsValidationException ()
    {
      var validationException = new ValidationException (new DefaultValidationLog());

      var outputTrue = _remotionReflector.IsValidationException (validationException);
      var outputFalse = _remotionReflector.IsValidationException (new Exception());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsInfrastructurType ()
    {
      var outputTrue = _remotionReflector.IsInfrastructureType (typeof (IInitializableMixin));
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
      var outputTrue3 = _remotionReflector.IsInheritedFromMixin (typeof (MemberOverrideWithInheritanceTest.CustomMixin));
      var outputFalse = _remotionReflector.IsInheritedFromMixin ((typeof (object)));

      Assert.That (outputTrue1, Is.True);
      Assert.That (outputTrue2, Is.True);
      Assert.That (outputTrue3, Is.True);
      Assert.That (outputFalse, Is.False);
    }


    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var targetType = typeof (TargetClass2);
      var classContextForTargetType = new ReflectedObject(mixinConfiguration.ClassContexts.GetWithInheritance (targetType));
      var reflectedOutput = _remotionReflector.GetTargetClassDefinition (targetType, new ReflectedObject (mixinConfiguration), classContextForTargetType);
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration);

      // is only true because target class definition gets cached!
      Assert.That (reflectedOutput.To<TargetClassDefinition>(), Is.SameAs (expectedOutput));

      // TargetClassDefinition has no overridden equals
      var classContext = mixinConfiguration.ClassContexts.First();
      Assert.That (new TargetClassDefinition (classContext), Is.Not.EqualTo (new TargetClassDefinition (classContext)));
    }

    [Test]
    public void BuildConfigurationFromAssemblies ()
    {
      var assemblies = new[] { typeof (TargetClass1).Assembly, typeof (object).Assembly };

      var reflectedOuput = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (reflectedOuput.To<MixinConfiguration>().ClassContexts, Is.EqualTo (expectedOutput.ClassContexts));
    }

    [Test]
    public void GetValidationLogFromValidationException ()
    {
      var validationLog = new DefaultValidationLog();
      var validationException = new ValidationException (validationLog);

      var reflectedValidationLog = _remotionReflector.GetValidationLogFromValidationException (validationException);
      var result = reflectedValidationLog.To<IValidationLog>();

      Assert.That (result, Is.SameAs (validationLog));
    }

    [Test]
    public void FindRemotionAssembly_FindRightAssembly ()
    {
      var remotionAssembly = typeof (TargetClassDefinitionUtility).Assembly;
      var assemblies = new[] { typeof (object).Assembly, remotionAssembly };

      var output = _remotionReflector.FindRemotionAssembly (assemblies);

      Assert.That (output, Is.EqualTo (remotionAssembly));
    }

    [Test]
    public void FindRemotionAssembly_FindNone ()
    {
      var assemblies = new[] { typeof (object).Assembly };

      var output = _remotionReflector.FindRemotionAssembly (assemblies);

      Assert.That (output, Is.Null);
    }
  }
}