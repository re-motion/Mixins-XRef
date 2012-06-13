using System;
using System.Diagnostics;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class ArgumentUtilityTest
  {
    [Test]
    public void CheckNotNull_NotNull ()
    {
      ArgumentUtility.CheckNotNull ("name", "value");
    }

    [Test]
    public void CheckNotNull_IsNull ()
    {
      try
      {
        object obj = null;
        ArgumentUtility.CheckNotNull ("name", obj);
        Assert.Fail ("expected exception not thrown");
      }
      catch (ArgumentNullException argumentNullException)
      {
        Assert.That (argumentNullException.Message, Is.EqualTo ("Value cannot be null.\r\nParameter name: name"));
      }
    }

    [Test]
    public void CheckNotNull_ReturnsSame ()
    {
      var original = new object();
      var returned = ArgumentUtility.CheckNotNull ("name", original);

      Assert.That (returned, Is.SameAs (original));
    }
  }
}