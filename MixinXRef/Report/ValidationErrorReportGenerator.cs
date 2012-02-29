using System;
using System.Xml.Linq;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<Exception> _errorAggregator;
    private readonly IRemotionReflector _remotionReflector;

    public ValidationErrorReportGenerator(ErrorAggregator<Exception> errorAggregator, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _errorAggregator = errorAggregator;
      _remotionReflector = remotionReflector;
    }

    public XElement GenerateXml ()
    {
      var validationErrors = new XElement ("ValidationErrors");

      foreach (var exception in _errorAggregator.Exceptions)
      {
        var topLevelExceptionElement = new RecursiveExceptionReportGenerator (exception).GenerateXml();
        var validationLog = _remotionReflector.GetValidationLogFromValidationException(exception);

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