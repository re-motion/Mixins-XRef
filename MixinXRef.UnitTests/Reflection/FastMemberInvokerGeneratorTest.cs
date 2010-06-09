using System;
using System.Reflection;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class FastMemberInvokerGeneratorTest
  {
    private FastMemberInvokerGenerator _generator;

    [SetUp]
    public void SetUp()
    {
      _generator = new FastMemberInvokerGenerator ();
    }

    [Test]
    public void GetFastMethodInvoker_ForStaticMethod ()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "IsNullOrEmpty",
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Static);

      var output = invoker (null, new object[] { instance });

      Assert.That (output, Is.EqualTo (false));
    }

    [Test]
    public void GetFastMethodInvoker_ForInstanceMethod_WithoutOverloads ()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType (),
          "GetHashCode",
          Type.EmptyTypes, BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[0]);

      Assert.That (output, Is.EqualTo (instance.GetHashCode()));
    }

    [Test]
    public void GetFastMethodInvoker_ForInstanceMethod_WithOverloads()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastMethodInvoker (
          instance.GetType(),
          "IndexOf",
          new[] { typeof (char) }, BindingFlags.Public | BindingFlags.Instance);
      
      var output = invoker(instance, new object[] { 't' });

      Assert.That (output, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Method 'Foo' not found on type 'System.String'.\r\nParameter name: memberName")]
    public void GetFastMethodInvoker_ForNonExistingMethod ()
    {
      var instance = "stringContent";
      _generator.GetFastMethodInvoker (
          instance.GetType (),
          "Foo",
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Static);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Overload of method 'GetHashCode' not found on type 'System.String'.\r\nParameter name: memberName")]
    public void GetFastMethodInvoker_ForExistingMethod_WithInvalidSignature ()
    {
      var instance = "stringContent";
      _generator.GetFastMethodInvoker (
          instance.GetType (),
          "GetHashCode",
          new[] { typeof (string) }, BindingFlags.Public | BindingFlags.Instance);
    }

    [Test]
    public void GetFastPropertyInvoker_ForInstanceProperty ()
    {
      var instance = "stringContent";
      var invoker = _generator.GetFastPropertyInvoker (
          instance.GetType (),
          "Length",
          Type.EmptyTypes, BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[0]);

      Assert.That (output, Is.EqualTo (13));
    }
  }
}