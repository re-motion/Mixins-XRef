using System;
using System.Xml.Linq;

namespace MixinXRef.UnitTests
{
  public class InvolvedTypeReportGeneratorStub : IInvolvedTypeReportGenerator
  {
    private readonly string _stubXml;

    public InvolvedTypeReportGeneratorStub (string stubXml)
    {
      _stubXml = stubXml;
    }

    public XElement GenerateXml ()
    {
      return new XElement (_stubXml);
    }
  }
}