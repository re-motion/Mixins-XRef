using System;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
      var assembly = typeof(Remotion.Context.MixedTypes.SafeContext).Assembly;
      var output = remotionReflection.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }
  }
}