using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ValidationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<ValidationException>();
      var reportGenerator = new ValidationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ValidationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<ValidationException>();
      var validationException1 = new ValidationException ("test validation exception", new DefaultValidationLog());

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
  }
}