using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ReadonlyIdentifierGeneratorTest
  {
    [Test]
    public void GetIdentifier_NonExistingItem ()
    {
      Dictionary<string, string> identifierDictionary = new Dictionary<string, string> ();

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string> (identifierDictionary, "dummy-value");

      var output = readonlyIdentifierGenerator.GetIdentifier ("key-1");

      Assert.That (output, Is.EqualTo ("dummy-value"));
    }

    [Test]
    public void GetIdentifier_ForExistingItem ()
    {
      Dictionary<string, string> identifierDictionary = new Dictionary<string, string> ();
      identifierDictionary.Add ("key-1", "value-1");

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string> (identifierDictionary, "dummy-value");

      var output = readonlyIdentifierGenerator.GetIdentifier ("key-1");

      Assert.That (output, Is.EqualTo ("value-1"));
    }
  }
}