using System;
using System.Xml.Linq;

namespace MixinXRef.Report
{
  public interface IReportGenerator
  {
    XElement GenerateXml ();
  }
}