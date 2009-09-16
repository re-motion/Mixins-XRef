using System;
using System.Reflection;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class IdentifierGeneratorTest
  {
    private IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

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
    public void GetIdentifier2_ForExistingValue ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOuput = identifierGenerator.GetIdentifier ("test-value");

      var output = identifierGenerator.GetIdentifier ("test-value", "does not matter");

      Assert.That (output, Is.EqualTo (expectedOuput));
    }

    [Test]
    public void GetIdentifier2_ForNonExistingValue ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var output = identifierGenerator.GetIdentifier ("test-value", "default value if not present");

      Assert.That (output, Is.EqualTo ("default value if not present"));
    }
  }
}