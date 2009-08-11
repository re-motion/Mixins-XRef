using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ConfigurationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<ConfigurationException> _errorAggregator;

    public ConfigurationErrorReportGenerator (ErrorAggregator<ConfigurationException> errorAggregator)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);

      _errorAggregator = errorAggregator;
    }


    public XElement GenerateXml ()
    {
      return new XElement (
          "ConfigurationErrors",
          from exception in _errorAggregator.Exceptions
          select GenerateExceptionElement (exception)
          );
    }


    private XElement GenerateExceptionElement (Exception exception)
    {
      if (exception == null)
        return null;

      var result = new XElement (
          "Exception",
          new XAttribute ("type", exception.GetType()),
          new XElement ("Message", exception.Message),
          new XElement ("StackTrace", exception.StackTrace),
          GenerateExceptionElement (exception.InnerException));

      return result;
    }
  }
}