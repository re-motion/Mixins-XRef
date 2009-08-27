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
    public void To_InvalidCast()
    {
      var reflectedObject = new ReflectedObject("string");
      try
      {
        reflectedObject.To<IDisposable>();
        Assert.Fail ("expected exception not thrown");
      } catch(InvalidCastException invalidCastException)
      {
        Assert.That (invalidCastException.Message, Is.EqualTo ("Invalid cast from 'System.String' to 'System.IDisposable'."));
      }
    }

    [Test]
    public void CallMethod_ExistingMethod()
    {
      var reflectedObject = new ReflectedObject("stringContent");
      var output = reflectedObject.CallMethod("IndexOf", 't');

      Assert.That(output.To<int>(), Is.EqualTo(1));
    }

    [Test]
    public void CallMethod_NonExistingMethod()
    {
      var reflectedObject = new ReflectedObject("stringContent");
      try
      {
        reflectedObject.CallMethod("nonExistingMethod");
        Assert.Fail("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That(missingMethodException.Message, Is.EqualTo("Method 'System.String.nonExistingMethod' not found."));
      }
    }

  }
}