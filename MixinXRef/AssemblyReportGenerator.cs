using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AssemblyReportGenerator
  {
    private readonly IdentifierGenerator<Assembly> _identifierGenerator;

    public AssemblyReportGenerator (IdentifierGenerator<Assembly> identifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("identifierGenerator", identifierGenerator);
      _identifierGenerator = identifierGenerator;
    }

    public XElement GenerateXml (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var assembliesElement = new XElement ("Assemblies");

      foreach (var assembly in assemblies)
        assembliesElement.Add (GenerateAssemblyElement (assembly));

      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", _identifierGenerator.GetIdentifier (assembly)), 
          new XAttribute ("full-name", assembly.FullName), 
          new XAttribute ("code-base", assembly.CodeBase));
    }
  }
}