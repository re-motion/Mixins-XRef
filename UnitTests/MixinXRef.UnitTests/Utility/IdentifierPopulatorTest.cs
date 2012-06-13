using System;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class IdentifierPopulatorTest
  {
    [Test]
    public void GetReadonlyIdentifierGenerator ()
    {
      var testStrings = new[] { "test-value-1", "test-value-2" };

      var identifierGenerator = new IdentifierGenerator<string>();
      identifierGenerator.GetIdentifier (testStrings[0]);
      identifierGenerator.GetIdentifier (testStrings[1]);

      var expectedOutput = identifierGenerator.GetReadonlyIdentiferGenerator ("default-value");

      var output = new IdentifierPopulator<string>(testStrings).GetReadonlyIdentifierGenerator("default-value");

      Assert.That (output.GetIdentifier (testStrings[0]), Is.EqualTo (expectedOutput.GetIdentifier (testStrings[0])));
      Assert.That (output.GetIdentifier (testStrings[1]), Is.EqualTo (expectedOutput.GetIdentifier (testStrings[1])));
      Assert.That (output.GetIdentifier ("not-present!"), Is.EqualTo (expectedOutput.GetIdentifier ("not-present!")));
    }
  }
}