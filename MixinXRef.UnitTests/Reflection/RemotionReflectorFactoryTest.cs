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
    public void Create_FileNotFound ()
    {
      try
      {
        _remotionReflectorFactory.Create ("..");
        Assert.Fail ("expected exception not thrown");
      }
      catch (FileNotFoundException fileNotFoundException)
      {
        Assert.That (fileNotFoundException.Message, Is.EqualTo ("The system cannot find the file specified. (Exception from HRESULT: 0x80070002)"));
      }
    }

    // can not easily test "not recognized case"
  }
}