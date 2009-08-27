using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;

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
          select new RecursiveExceptionReportGenerator (exception).GenerateXml()
          );
    }
  }
}