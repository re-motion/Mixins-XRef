using System;
using System.Xml.Linq;

namespace MixinXRef
{
  public interface IAssemblyReportGenerator
  {
    XElement GenerateXml ();
  }
}