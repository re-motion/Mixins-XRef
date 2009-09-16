using System;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
        ArgumentUtility.CheckNotNull ("name", null);
        Assert.Fail ("expected exception not thrown");
      }
      catch (ArgumentNullException argumentNullException)
      {
        Assert.That (argumentNullException.Message, Is.EqualTo ("Value cannot be null.\r\nParameter name: name"));
      }
    }
  }
}