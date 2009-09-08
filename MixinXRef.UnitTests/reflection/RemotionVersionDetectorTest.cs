using System;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class RemotionVersionDetectorTest
  {
    [Test]
    public void Constructor ()
    {
      // assumption: assemblyDirectory is correct and RemotionAssembly is named 'Remotion.dll'
      var remotionVersionDetector = new RemotionVersionDetector (".");
      var output = remotionVersionDetector.RemotionReflection;

      Assert.That (output, Is.InstanceOfType (typeof (RemotionReflection08)));
    }

    [Test]
    public void DetectVersion_Old_TargetClassDefinitonUtility()
    {
      
    }
  }
}