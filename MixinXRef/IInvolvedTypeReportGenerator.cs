using System;
using System.Xml.Linq;

namespace MixinXRef
{
  public interface IInvolvedTypeReportGenerator
  {
    XElement GenerateXml ();
  }
}