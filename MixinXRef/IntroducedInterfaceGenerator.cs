using System;
using System.Xml.Linq;

namespace MixinXRef
{
  public class IntroducedInterfaceGenerator : IReportGenerator
  {
    public XElement GenerateXml ()
    {
      return new XElement ("IntroducedInterfaces");
    }
  }
}