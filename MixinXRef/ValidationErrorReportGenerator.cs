using System;
using System.Xml.Linq;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ValidationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<ValidationException> _errorAggregator;

    public ValidationErrorReportGenerator (ErrorAggregator<ValidationException> errorAggregator)
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
        // TODO: more info about validation log
        topLevelExceptionElement.Add (new XElement ("ValidationLog"));
        validationErrors.Add (topLevelExceptionElement);
      }

      return validationErrors;
    }
  }
}