using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroTypes ()
    {
      var reportGenerator = new MemberReportGenerator (new Type[0]);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("Members");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}