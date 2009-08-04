using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AssemblyReportGenerator
  {
    public XElement GenerateXml (HashSet<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      var identifierGenerator = new IdentifierGenerator<Assembly>();

      var assembliesElement = new XElement ("Assemblies");

      foreach (var assembly in (IEnumerable<Assembly>) assemblies)
        assembliesElement.Add (GenerateAssemblyElement (assembly, identifierGenerator));

      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, IdentifierGenerator<Assembly> identifierGenerator)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", identifierGenerator.GetIdentifier (assembly)), 
          new XAttribute ("full-name", assembly.FullName), 
          new XAttribute ("code-base", assembly.CodeBase));
    }
  }
}