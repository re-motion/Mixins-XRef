using System;
using System.IO;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class RemotionReflectorFactoryTest
  {
    private RemotionReflectorFactory _remotionReflectorFactory;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflectorFactory = new RemotionReflectorFactory();
    }

    [Test]
    public void Create_RemotionReflection_1_11_20 ()
    {
      // assumption: assemblyDirectory is correct and RemotionAssembly is named 'Remotion.dll'
      var output = _remotionReflectorFactory.Create (".");

      Assert.That (output, Is.InstanceOfType (typeof (RemotionReflector_1_11_20)));
    }

    [Test]
    [ExpectedException (typeof (FileNotFoundException), ExpectedMessage =
        @"Could not load file or assembly 'C:\Remotion.dll' or one of its dependencies. The system cannot find the file specified.")]
    public void Create_FileNotFound ()
    {
      _remotionReflectorFactory.Create (@"C:\");
    }

    // can not easily test "not recognized case"
  }
}