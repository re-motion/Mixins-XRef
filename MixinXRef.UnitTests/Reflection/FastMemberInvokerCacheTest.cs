using System;
using System.Reflection;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class FastMemberInvokerCacheTest
  {
    private FastMemberInvokerCache _cache;

    [SetUp]
    public void SetUp()
    {
      _cache = new FastMemberInvokerCache ();
    }

    [Test]
    public void CacheKey_Equals_EqualKeys()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_DeclaringType ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (int), "Foo", new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_MemberName ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Bar", new[] { typeof (int), typeof (string) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_Equals_NonEqualKeys_ArgumentTypes ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (string), typeof (int) });

      Assert.That (key1, Is.Not.EqualTo (key2));
    }

    [Test]
    public void CacheKey_GetHashCode_EqualKeys ()
    {
      var key1 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });
      var key2 = new FastMemberInvokerCache.CacheKey (typeof (string), "Foo", new[] { typeof (int), typeof (string) });

      Assert.That (key1.GetHashCode(), Is.EqualTo (key2.GetHashCode()));
    }

    [Test]
    public void GetFastMethodInvoker()
    {
      var instance = "stringContent";
      var invoker = _cache.GetOrCreateFastMethodInvoker (
          instance.GetType (),
          "IsNullOrEmpty",
          new[] { typeof (string) }, 
          BindingFlags.Public | BindingFlags.Static);

      var output = invoker (null, new object[] { instance });

      Assert.That (output, Is.EqualTo (false));
    }

    [Test]
    public void GetFastMethodInvoker_Twice ()
    {
      var invoker1 = _cache.GetOrCreateFastMethodInvoker (
          typeof (string),
          "IsNullOrEmpty",
          new[] { typeof (string) },
          BindingFlags.Public | BindingFlags.Static);
      var invoker2 = _cache.GetOrCreateFastMethodInvoker (
          typeof (string),
          "IsNullOrEmpty",
          new[] { typeof (string) },
          BindingFlags.Public | BindingFlags.Static);

      Assert.That (invoker2, Is.SameAs (invoker1));
    }

    [Test]
    public void GetFastPropertyInvoker ()
    {
      var instance = "stringContent";
      var invoker = _cache.GetOrCreateFastPropertyInvoker (
          instance.GetType (),
          "Length",
          Type.EmptyTypes,
          BindingFlags.Public | BindingFlags.Instance);

      var output = invoker (instance, new object[0]);

      Assert.That (output, Is.EqualTo (13));
    }

    [Test]
    public void GetFastPropertyInvoker_Twice ()
    {
      var invoker1 = _cache.GetOrCreateFastPropertyInvoker (
          typeof (string),
          "Length",
          Type.EmptyTypes,
          BindingFlags.Public | BindingFlags.Instance);
      var invoker2 = _cache.GetOrCreateFastPropertyInvoker (
          typeof (string),
          "Length",
          Type.EmptyTypes,
          BindingFlags.Public | BindingFlags.Instance);

      Assert.That (invoker2, Is.SameAs (invoker1));
    }
  }
}