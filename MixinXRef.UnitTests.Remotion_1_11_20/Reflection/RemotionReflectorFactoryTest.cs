using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Remotion_1_11_20.Reflection
{
  [TestFixture]
  public class RemotionReflectorFactoryTest
  {
    [Test]
    public void Create_RemotionReflection_1_11_20 ()
    {
      var output = new RemotionReflectorFactory().Create (".");

      Assert.That (output, Is.InstanceOfType (typeof (RemotionReflector_1_11_20)));
    }
  }
}