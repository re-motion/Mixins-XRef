using System;
using System.Xml.Linq;

namespace MixinXRef
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<Exception> _errorAggregator;

    public ValidationErrorReportGenerator(ErrorAggregator<Exception> errorAggregator)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);
      _errorAggregator = errorAggregator;
    }

    public XElement GenerateXml ()
    {
      var validationErrors = new XElement ("ValidationErrors");

      foreach (var Exception in _errorAggregator.Exceptions)
      {
        var topLevelExceptionElement = new RecursiveExceptionReportGenerator (Exception).GenerateXml();
        var validationLog = new ReflectedObject (Exception).GetProperty ("ValidationLog");

        topLevelExceptionElement.Add (
            new XElement (
                "ValidationLog",
                new XAttribute("number-of-rules-executed", validationLog.CallMethod("GetNumberOfRulesExecuted")),
                new XAttribute("number-of-failures", validationLog.CallMethod("GetNumberOfFailures")),
                new XAttribute("number-of-unexpected-exceptions", validationLog.CallMethod("GetNumberOfUnexpectedExceptions")),
                new XAttribute("number-of-warnings", validationLog.CallMethod("GetNumberOfWarnings")),
                new XAttribute("number-of-successes", validationLog.CallMethod("GetNumberOfSuccesses"))
                )
            );
        validationErrors.Add (topLevelExceptionElement);
      }

      return validationErrors;
    }
  }
}