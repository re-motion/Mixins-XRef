using System;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class RecursiveExceptionReportGenerator : IReportGenerator
  {
    private readonly Exception _exception;

    public RecursiveExceptionReportGenerator (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      _exception = exception;
    }

    public XElement GenerateXml ()
    {
      return GenerateExceptionElement (_exception);
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