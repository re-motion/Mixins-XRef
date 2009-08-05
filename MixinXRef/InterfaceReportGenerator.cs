using System;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public InterfaceReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public XElement GenerateXml ()
    {
      return new XElement ("Interfaces");
    }
  }
}