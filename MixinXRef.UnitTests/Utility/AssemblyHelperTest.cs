using System;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class AssemblyHelperTest
  {
    [Test]
    public void LoadFileOrNull_ExistingAssembly ()
    {
      var result = AssemblyHelper.LoadFileOrNull (".", "Remotion.dll");

      Assert.That (result, Is.Not.Null);
      Assert.That (result.GetName().Name, Is.EqualTo("Remotion"));
    }

    [Test]
    public void LoadFileOrNull_NonExistingAssembly ()
    {
      var result = AssemblyHelper.LoadFileOrNull (".", "NonExistingAssembly.dll");

      Assert.That (result, Is.Null);
    }
  }
}