using System;
using System.Xml.Linq;

namespace MixinXRef
{
  public interface IReportGenerator
  {
    XElement GenerateXml ();
  }
}