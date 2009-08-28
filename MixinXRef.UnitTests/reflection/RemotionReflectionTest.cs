using System;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context.MixedTypes;
using Remotion.Mixins;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class RemotionReflectionTest
  {
    [Test]
    public void IsNonApplicationAssembly_False ()
    {
      var remotionReflection = new RemotionReflection();

      var assembly = typeof (IDisposable).Assembly;
      var output = remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.False);
    }

    [Test]
    public void IsNonApplicationAssembly_True ()
    {
      var remotionReflection = new RemotionReflection();

      // SafeContext is type in Remotion.Mixins.Persistent.Signed, which is NonApplicationAssembly
      var assembly = typeof (SafeContext).Assembly;
      var output = remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }

    [Test]
    public void IsConfigurationException ()
    {
      var remotionReflection = new RemotionReflection();

      var configurationException = new ConfigurationException ("configurationException");

      var outputTrue = remotionReflection.IsConfigurationException (configurationException);
      var outputFalse = remotionReflection.IsConfigurationException (new Exception());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsValidationException()
    {
      var remotionReflection = new RemotionReflection();

      var validationException = new ValidationException (new DefaultValidationLog());

      var outputTrue = remotionReflection.IsValidationException(validationException);
      var outputFalse = remotionReflection.IsValidationException(new Exception());

      Assert.That(outputTrue, Is.True);
      Assert.That(outputFalse, Is.False);
    }
  }
}