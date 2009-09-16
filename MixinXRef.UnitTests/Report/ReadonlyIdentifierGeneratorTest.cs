using System;
using System.Collections.Generic;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ReadonlyIdentifierGeneratorTest
  {
    [Test]
    public void GetIdentifier_NonExistingItem ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "dummy-value");

      var output = readonlyIdentifierGenerator.GetIdentifier ("key-1");

      Assert.That (output, Is.EqualTo ("dummy-value"));
    }

    [Test]
    public void GetIdentifier_ForExistingItem ()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOutput = identifierGenerator.GetIdentifier("value-1");

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter");

      var output = readonlyIdentifierGenerator.GetIdentifier ("value-1");

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetIdentifier2_NonExistingItem()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter EITHER");

      var output = readonlyIdentifierGenerator.GetIdentifier("key-1", "default value");

      Assert.That(output, Is.EqualTo("default value"));
    }

    [Test]
    public void GetIdentifier2_ForExistingItem()
    {
      var identifierGenerator = new IdentifierGenerator<string>();

      var expectedOutput = identifierGenerator.GetIdentifier("value-1");

      var readonlyIdentifierGenerator = new ReadonlyIdentifierGenerator<string>(identifierGenerator, "does not matter");

      var output = readonlyIdentifierGenerator.GetIdentifier("value-1", "does not matter EITHER");

      Assert.That(output, Is.EqualTo(expectedOutput));
    }
  }
}