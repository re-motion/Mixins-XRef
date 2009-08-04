using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportGenerator
  {
    public XElement GenerateXml (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      return new XElement ("MixinXRefReport", new XElement ("Assemblies"));
    }
  }
}