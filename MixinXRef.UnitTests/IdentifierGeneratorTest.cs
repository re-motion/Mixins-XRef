using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class IdentifierGeneratorTest
  {
    private IdentifierGenerator _identifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _identifierGenerator = new IdentifierGenerator();
    }

    [Test]
    public void GetIdentifier ()
    {
      var identifier = _identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier, Is.EqualTo ("0"));
    }

    [Test]
    public void GetIdentifier_Twice ()
    {
      _identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier = _identifierGenerator.GetIdentifier (typeof (object).Assembly);

      Assert.That (identifier, Is.EqualTo ("1"));
    }

    [Test]
    public void GetIdentifier_TwiceOnSameAssembly ()
    {
      var identifier1 = _identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier2 = _identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier1, Is.EqualTo (identifier2));
    }
  }
}