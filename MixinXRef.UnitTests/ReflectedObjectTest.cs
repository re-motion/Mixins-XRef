using System;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

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
    public void CallMethod_ExistingMethod_WithReflectedObject()
    {
      var reflectedObject = new ReflectedObject("stringContent");
      var output = reflectedObject.CallMethod("IndexOf", new ReflectedObject('t'));

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

    [Test]
    public void GetProperty_ExistingProperty()
    {
      var reflectedObject = new ReflectedObject("string");
      var output = reflectedObject.GetProperty("Length");

      Assert.That(output.To<int>(), Is.EqualTo(6));
    }

    [Test]
    public void GetProperty_NonExistingProperty()
    {
      var reflectedObject = new ReflectedObject("string");

      try
      {
        reflectedObject.GetProperty("nonExistingProperty");
        Assert.Fail("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That(missingMethodException.Message, Is.EqualTo("Method 'System.String.nonExistingProperty' not found."));
      }
    }

    [Test]
    public void IEnumerableFunctionality_OnEnumerableWrappedObject ()
    {
      var reflectedObject = new ReflectedObject("string");
      var output = new StringBuilder (6);
      
      foreach (var reflectedCharacter in reflectedObject)
      {
        output.Append (reflectedCharacter.To<char>());
      }
      Assert.That(output.ToString(), Is.EqualTo("string"));
    }

    [Test]
    public void IEnumerableFunctionality_OnNonEnumerableWrappedObject()
    {
      var reflectedObject = new ReflectedObject(42);

      try
      {
        reflectedObject.GetEnumerator().MoveNext();
        Assert.Fail("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That(notSupportedException.Message, Is.EqualTo("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable ()
    {
      var reflectedObject = new ReflectedObject("string");
      var output = new StringBuilder (6);

      foreach(var character in reflectedObject.AsEnumerable<char>())
      {
        output.Append (character);
      }
      
      Assert.That(output.ToString(), Is.EqualTo("string"));
    }

    [Test]
    public void AsEnumerable_NonEnumerable()
    {
      var reflectedObject = new ReflectedObject(42);
      
      try
      {
        reflectedObject.AsEnumerable<object>().GetEnumerator().MoveNext();
        Assert.Fail("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That(notSupportedException.Message, Is.EqualTo("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable_EnumerableButWrongType()
    {
      var reflectedObject = new ReflectedObject("string");

      try
      {
        // 'char' is convertible to 'int'!
        reflectedObject.AsEnumerable<float>().GetEnumerator().MoveNext();
        Assert.Fail("expected exception not thrown");
      }
      catch (InvalidCastException notSupportedException)
      {
        Assert.That(notSupportedException.Message, Is.EqualTo("Invalid cast from 'Char' to 'Single'."));
      }
    }

    [Test]
    public void Create ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (int).Assembly, "System.Int32");
      Assert.That(reflectedObject.To<int>(), Is.EqualTo(0));
    }
  }
}