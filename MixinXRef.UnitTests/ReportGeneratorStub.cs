using System;
using System.Xml.Linq;
using MixinXRef.Report;

namespace MixinXRef.UnitTests
{
  public class ReportGeneratorStub : IReportGenerator
  {
    private readonly string _stubXml;

    public ReportGeneratorStub (string stubXml)
    {
      _stubXml = stubXml;
    }

    public XElement GenerateXml ()
    {
      return new XElement (_stubXml);
    }
  }
}