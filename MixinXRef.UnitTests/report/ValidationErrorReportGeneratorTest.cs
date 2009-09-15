using System;
using System.Xml.Linq;
using MixinXRef.Report;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class ValidationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ValidationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<Exception>();
      var validationException1 = SetUpExceptionWithDummyStackTrace("test validation exception", new DefaultValidationLog());

      errorAggregator.AddException (validationException1);
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();

      var validationExceptionElement = new RecursiveExceptionReportGenerator (validationException1).GenerateXml();
      validationExceptionElement.Add (
          new XElement ("ValidationLog",
                        new XAttribute("number-of-rules-executed", validationException1.ValidationLog.GetNumberOfRulesExecuted()),
                        new XAttribute("number-of-failures", validationException1.ValidationLog.GetNumberOfFailures()),
                        new XAttribute("number-of-unexpected-exceptions", validationException1.ValidationLog.GetNumberOfUnexpectedExceptions()),
                        new XAttribute("number-of-warnings", validationException1.ValidationLog.GetNumberOfWarnings()),
                        new XAttribute("number-of-successes", validationException1.ValidationLog.GetNumberOfSuccesses())
              ));

      var expectedOutput = new XElement ("ValidationErrors", validationExceptionElement);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ValidationException SetUpExceptionWithDummyStackTrace(string exceptionMessage, IValidationLog validationLog)
    {
      try
      {
        throw new ValidationException(exceptionMessage, validationLog);
      }
      catch (ValidationException caughtException)
      {
        return caughtException;
      }
    }
  }
}