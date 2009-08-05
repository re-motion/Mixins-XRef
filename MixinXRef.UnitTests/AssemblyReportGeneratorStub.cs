using System;
using System.Xml.Linq;

namespace MixinXRef.UnitTests
{
  public class AssemblyReportGeneratorStub : IAssemblyReportGenerator
  {
    private readonly string _stubXml;

    public AssemblyReportGeneratorStub (string stubXml)
    {
      _stubXml = stubXml;
    }

    public XElement GenerateXml ()
    {
      return new XElement (_stubXml);
    }
  }
}