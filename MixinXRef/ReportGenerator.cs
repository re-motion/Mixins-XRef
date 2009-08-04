using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportGenerator
  {
    public XElement GenerateXml (HashSet<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var assemblyReportGenerator = new AssemblyReportGenerator();
      return new XElement (
          "MixinXRefReport",
          assemblyReportGenerator.GenerateXml (assemblies));
    }
  }
}