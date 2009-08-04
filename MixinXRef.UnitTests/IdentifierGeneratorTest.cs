using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class IdentifierGeneratorTest
  {
    [Test]
    public void GetIdentifier ()
    {
      var identifierGenerator = new IdentifierGenerator();
      var identifier = identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier, Is.EqualTo ("0"));
    }

    [Test]
    public void GetIdentifier_Twice ()
    {
      var identifierGenerator = new IdentifierGenerator ();
      
      identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier = identifierGenerator.GetIdentifier (typeof (object).Assembly);

      Assert.That (identifier, Is.EqualTo ("1"));
    }

    [Test]
    public void GetIdentifier_TwiceOnSameAssembly ()
    {
      var identifierGenerator = new IdentifierGenerator ();

      var identifier1 = identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);
      var identifier2 = identifierGenerator.GetIdentifier (typeof (IdentifierGeneratorTest).Assembly);

      Assert.That (identifier1, Is.EqualTo (identifier2));
    }
  }
}