using System;
using System.Linq;
using System.Xml.Linq;

namespace MixinXRef
{
  public class ConfigurationErrorReportGenerator : IReportGenerator
  {
    private readonly ErrorAggregator<Exception> _errorAggregator;

    public ConfigurationErrorReportGenerator(ErrorAggregator<Exception> errorAggregator)
    {
      ArgumentUtility.CheckNotNull ("errorAggregator", errorAggregator);

      _errorAggregator = errorAggregator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "ConfigurationErrors",
          from exception in _errorAggregator.Exceptions
          select new RecursiveExceptionReportGenerator (exception).GenerateXml()
          );
    }
  }
}