using System;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class CompositeReportGenerator : IReportGenerator
  {
    private readonly IReportGenerator[] _reportGenerators;

    public CompositeReportGenerator (params IReportGenerator[] reportGenerators)
    {
      ArgumentUtility.CheckNotNull ("reportGenerators", reportGenerators);
      _reportGenerators = reportGenerators;
    }

    public XElement GenerateXml ()
    {
      var mixinXRefReportElement = new XElement ("MixinXRefReport");
      foreach (var reportGenerator in _reportGenerators)
        mixinXRefReportElement.Add (reportGenerator.GenerateXml());

      return mixinXRefReportElement;
    }
  }
}