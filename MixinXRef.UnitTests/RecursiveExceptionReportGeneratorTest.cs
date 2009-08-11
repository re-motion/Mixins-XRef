using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class RecursiveExceptionReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ForExceptionWithoutInnerException ()
    {
      var exception1 = new Exception ("plain exception");
      var reportGenerator = new RecursiveExceptionReportGenerator (exception1);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Exception",
          new XAttribute ("type", exception1.GetType()),
          new XElement ("Message", exception1.Message),
          new XElement ("StackTrace", exception1.StackTrace)
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForExceptionWithInnerException ()
    {
      var innerException = new Exception ("inner exception");
      var outerException = new Exception ("exception with inner exception", innerException);
      var reportGenerator = new RecursiveExceptionReportGenerator (outerException);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Exception",
          new XAttribute ("type", outerException.GetType()),
          new XElement ("Message", outerException.Message),
          new XElement ("StackTrace", outerException.StackTrace),
          new XElement (
              "Exception",
              new XAttribute ("type", innerException.GetType()),
              new XElement ("Message", innerException.Message),
              new XElement ("StackTrace", innerException.StackTrace))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}