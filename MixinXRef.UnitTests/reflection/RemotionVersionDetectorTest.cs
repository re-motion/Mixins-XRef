using System;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

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

      Assert.That(output, Is.InstanceOfType(typeof(RemotionReflection08)));
    }

    [Test]
    public void DetectVersion_Unrecognized ()
    {
      var remotionVersionDetector = new RemotionVersionDetector (".");
      try
      {
        remotionVersionDetector.DetectVersion (typeof (object).Assembly);
        Assert.Fail ("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("The remotion assembly version '2.0.0.0' is not supported."));
      }
    }

    [Test]
    public void DetectVersion_RemotionReflection08 ()
    {
      var remotionVersionDetector = new RemotionVersionDetector (".");
      var assemblyVar = typeof (TargetClassDefinitionUtility).Assembly;
      var output = remotionVersionDetector.DetectVersion (assemblyVar);

      Assert.That (assemblyVar.GetName().Version.ToString(), Is.EqualTo ("1.11.20.13"));
      Assert.That (output, Is.InstanceOfType (typeof (RemotionReflection08)));
    }
  }
}