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
      validationExceptionElement.Add (new XElement ("ValidationLog"));

      var expectedOutput = new XElement ("ValidationErrors", validationExceptionElement);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}