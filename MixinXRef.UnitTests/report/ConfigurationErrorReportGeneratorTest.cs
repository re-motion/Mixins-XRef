using System;
using System.Xml.Linq;
using MixinXRef.Report;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ConfigurationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ConfigurationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();

      var innerException1 = SetUpExceptionWithDummyStackTrace ("inner exception", null);
      var Exception1 = SetUpExceptionWithDummyStackTrace ("test configuration exception 1", innerException1);
      var Exception2 = SetUpExceptionWithDummyStackTrace ("test configuration excpetion 2", null);

      errorAggregator.AddException (Exception1);
      errorAggregator.AddException (Exception2);
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "ConfigurationErrors",
          new RecursiveExceptionReportGenerator (Exception1).GenerateXml(),
          new RecursiveExceptionReportGenerator (Exception2).GenerateXml()
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private Exception SetUpExceptionWithDummyStackTrace (string exceptionMessage, Exception innerException)
    {
      try
      {
        throw new Exception (exceptionMessage, innerException);
      }
      catch (Exception caughtException)
      {
        return caughtException;
      }
    }
  }
}