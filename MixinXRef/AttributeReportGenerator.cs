using System;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public AttributeReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public XElement GenerateXml ()
    {
      return new XElement ("Attributes");
    }
  }
}