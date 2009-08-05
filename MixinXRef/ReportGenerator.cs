using System;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportGenerator
  {
    private readonly IAssemblyReportGenerator _assemblyReportGenerator;
    private readonly IInvolvedTypeReportGenerator _involvedTypeReportGenerator;

    public ReportGenerator (IAssemblyReportGenerator assemblyReportGenerator, IInvolvedTypeReportGenerator involvedTypeReportGenerator)
    {
      ArgumentUtility.CheckNotNull ("assemblyReportGenerator", assemblyReportGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeReportGenerator", involvedTypeReportGenerator);

      _assemblyReportGenerator = assemblyReportGenerator;
      _involvedTypeReportGenerator = involvedTypeReportGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "MixinXRefReport",
          _assemblyReportGenerator.GenerateXml (),
          _involvedTypeReportGenerator.GenerateXml());
    }
  }
}