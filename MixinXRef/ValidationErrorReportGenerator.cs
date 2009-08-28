using System;
using System.Xml.Linq;
using Remotion.Mixins.Validation;


namespace MixinXRef
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<ValidationException> _errorAggregator;

    public ValidationErrorReportGenerator(ErrorAggregator<ValidationException> errorAggregator)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);
      _errorAggregator = errorAggregator;
    }

    public XElement GenerateXml ()
    {
      var validationErrors = new XElement ("ValidationErrors");

      foreach (var validationException in _errorAggregator.Exceptions)
      {
        var topLevelExceptionElement = new RecursiveExceptionReportGenerator (validationException).GenerateXml();
        var wrappedValidationException = new ReflectedObject (validationException);

        topLevelExceptionElement.Add (
            new XElement (
                "ValidationLog",
                new XAttribute("number-of-rules-executed", wrappedValidationException.GetProperty("ValidationLog").CallMethod("GetNumberOfRulesExecuted").To<int>()),
                new XAttribute ("number-of-failures", validationException.ValidationLog.GetNumberOfFailures()),
                new XAttribute ("number-of-unexpected-exceptions", validationException.ValidationLog.GetNumberOfUnexpectedExceptions()),
                new XAttribute ("number-of-warnings", validationException.ValidationLog.GetNumberOfWarnings()),
                new XAttribute ("number-of-successes", validationException.ValidationLog.GetNumberOfSuccesses())
                )
            );
        validationErrors.Add (topLevelExceptionElement);
      }

      return validationErrors;
    }
  }
}