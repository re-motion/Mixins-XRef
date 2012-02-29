using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Remotion_1_13_141.Reflection
{
  [TestFixture]
  public class RemotionReflectorFactoryTest
  {
    [Test]
    public void Create_RemotionReflection_1_13_141 ()
    {
      var output = new RemotionReflectorFactory().Create (".");

      Assert.That (output, Is.InstanceOfType (typeof (RemotionReflector_1_13_141)));
    }
  }
}