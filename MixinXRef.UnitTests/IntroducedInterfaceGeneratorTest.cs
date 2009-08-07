using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class IntroducedInterfaceGeneratorTest
  {
    [Test]
    public void GenerateXml_NoInterfaces ()
    {
      var reportGenerator = new IntroducedInterfaceGenerator();
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("IntroducedInterfaces");

      Assert.That (output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}