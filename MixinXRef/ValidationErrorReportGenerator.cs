using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ValidationErrorReportGenerator :IReportGenerator
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
        var topLevelExceptionElement = GenerateExceptionElement (validationException);
        topLevelExceptionElement.Add (new XElement("ValidationLog"));
        validationErrors.Add (topLevelExceptionElement);
      }

      return validationErrors;
    }


    private XElement GenerateExceptionElement (Exception exception)
    {
      if (exception == null)
        return null;

      return new XElement (
          "Exception",
          new XAttribute ("type", exception.GetType()),
          new XElement ("Message", exception.Message),
          new XElement ("StackTrace", exception.StackTrace),
          GenerateExceptionElement (exception.InnerException));
    }
  }
}