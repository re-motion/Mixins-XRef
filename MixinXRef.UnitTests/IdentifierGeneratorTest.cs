using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class IdentifierGeneratorTest
  {
    private IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
    }

    [Test]
    public void GetIdentifier ()
    {
      var identifier = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier, Is.EqualTo ("0"));
    }

    [Test]
    public void GetIdentifier_Twice ()
    {
      _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier = _assemblyIdentifierGenerator.GetIdentifier (typeof (object).Assembly);

      Assert.That (identifier, Is.EqualTo ("1"));
    }

    [Test]
    public void GetIdentifier_TwiceOnSameAssembly ()
    {
      var identifier1 = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier2 = _assemblyIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier1, Is.EqualTo (identifier2));
    }

    [Test]
    public void GetIdentifier_OnType ()
    {
      var typeIdentifierGenerator = new IdentifierGenerator<Type>();
      var identifier = typeIdentifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest));

      Assert.That (identifier, Is.EqualTo ("0"));
    }
  }
}