using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ReflectedObjectTest
  {
    [Test]
    public void To_CorrespondingType ()
    {
      var reflectedObject = new ReflectedObject("string");
      String output = reflectedObject.To<String>();

      Assert.That (output, Is.EqualTo ("string"));
    }

    [Test]
    public void To_Exception()
    {
      var reflectedObject = new ReflectedObject("string");
      try
      {
        reflectedObject.To<IDisposable>();
        Assert.Fail ("expected exception not thrown");
      } catch(InvalidCastException invalidCastException)
      {
        Assert.That (invalidCastException.Message, Is.EqualTo ("Cannot convert from System.String to System.IDisposable"));
      }
    }

  }
}