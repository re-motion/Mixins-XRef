using System;
using System.Text;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class ReflectedObjectTest
  {
    [Test]
    public void To_CorrespondingType ()
    {
      var reflectedObject = new ReflectedObject ("string");
      String output = reflectedObject.To<String>();

      Assert.That (output, Is.EqualTo ("string"));
    }

    [Test]
    public void To_InvalidCast ()
    {
      var reflectedObject = new ReflectedObject ("string");
      try
      {
        reflectedObject.To<IDisposable>();
        Assert.Fail ("expected exception not thrown");
      }
      catch (InvalidCastException invalidCastException)
      {
        Assert.That (invalidCastException.Message, Is.EqualTo ("Unable to cast object of type 'System.String' to type 'System.IDisposable'."));
      }
    }

    [Test]
    public void CallMethod_ExistingMethod ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      var output = reflectedObject.CallMethod ("IndexOf", 't');

      Assert.That (output.To<int>(), Is.EqualTo (1));
    }

    [Test]
    public void CallMethod_ExistingMethod_Void ()
    {
      // TargetDoSomething has method: public void DoSomething()
      var reflectedObject = new ReflectedObject (new TargetDoSomething());
      var output = reflectedObject.CallMethod ("DoSomething");

      Assert.That (output, Is.Null);
    }

    [Test]
    public void CallMethod_ExistingMethod_WithReflectedObject ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      var output = reflectedObject.CallMethod ("IndexOf", new ReflectedObject ('t'));

      Assert.That (output.To<int>(), Is.EqualTo (1));
    }

    [Test]
    public void CallMethod_NonExistingMethod ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      try
      {
        reflectedObject.CallMethod ("nonExistingMethod");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Method 'System.String.nonExistingMethod' not found."));
      }
    }

    [Test]
    public void GetProperty_ExistingProperty ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = reflectedObject.GetProperty ("Length");

      Assert.That (output.To<int>(), Is.EqualTo (6));
    }

    [Test]
    public void GetProperty_NonExistingProperty ()
    {
      var reflectedObject = new ReflectedObject ("string");

      try
      {
        reflectedObject.GetProperty ("nonExistingProperty");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Method 'System.String.nonExistingProperty' not found."));
      }
    }

    [Test]
    public void IEnumerableFunctionality_OnEnumerableWrappedObject ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = new StringBuilder (6);

      foreach (var reflectedCharacter in reflectedObject)
        output.Append (reflectedCharacter.To<char>());
      Assert.That (output.ToString(), Is.EqualTo ("string"));
    }

    [Test]
    public void IEnumerableFunctionality_OnNonEnumerableWrappedObject ()
    {
      var reflectedObject = new ReflectedObject (42);

      try
      {
        reflectedObject.GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = new StringBuilder (6);

      foreach (var character in reflectedObject.AsEnumerable<char>())
        output.Append (character);

      Assert.That (output.ToString(), Is.EqualTo ("string"));
    }

    [Test]
    public void AsEnumerable_NonEnumerable ()
    {
      var reflectedObject = new ReflectedObject (42);

      try
      {
        reflectedObject.AsEnumerable<object>().GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable_EnumerableButWrongType ()
    {
      var reflectedObject = new ReflectedObject ("string");

      try
      {
        // 'char' is convertible to 'int'!
        reflectedObject.AsEnumerable<float>().GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (InvalidCastException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("Specified cast is not valid."));
      }
    }

    [Test]
    public void Create_DefaultConstructor ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (int).Assembly, "System.Int32");
      Assert.That (reflectedObject.To<int>(), Is.EqualTo (0));
    }

    [Test]
    public void Create_ConstructorWithArguments ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (string).Assembly, "System.String", 'x', 5);
      var expectedOutput = new String ('x', 5);
      Assert.That (reflectedObject.To<string>(), Is.EqualTo (expectedOutput));
    }

    [Test]
    public void Create_ConstructorWithInvalidArguments ()
    {
      try
      {
        ReflectedObject.Create (typeof (string).Assembly, "System.String", "string constructor is not overloaded with string");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Constructor on type 'System.String' not found."));
      }
    }

    [Test]
    public void Create_ConstructorWithWrappedArguments ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (string).Assembly, "System.String", 'x', new ReflectedObject (5));
      var expectedOutput = new String ('x', 5);
      Assert.That (reflectedObject.To<string>(), Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetForeignType ()
    {
      var type = ReflectedObject.GetForeignType (typeof (IInitializableMixin).Assembly, "Remotion.Mixins.IInitializableMixin");
      Assert.That (type.GetMethod ("Initialize"), Is.Not.Null);
    }

    [Test]
    public void GetForeignType_InvalidType ()
    {
      try
      {
        ReflectedObject.GetForeignType (typeof (int).Assembly, "Does.Not.Exist");
        Assert.Fail ("expected exception not thrown");
      }
      catch (TypeLoadException typeLoadException)
      {
        var message = typeLoadException.Message;
        Assert.That (message.Substring (0, message.IndexOf (',')), Is.EqualTo ("Could not load type 'Does.Not.Exist' from assembly 'mscorlib"));
        // only check first part of the message because of full assembly name (includes version and other components, which could change)
      }
    }

    [Test]
    public void ToString_Test ()
    {
      const string content = "toString() for string";
      var reflectedObject = new ReflectedObject (content);

      Assert.That (reflectedObject.ToString(), Is.EqualTo (content));
    }
  }
}