using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AssemblyReportGenerator : IAssemblyReportGenerator
  {
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly IdentifierGenerator<Assembly> _identifierGenerator;

    public AssemblyReportGenerator (IEnumerable<Assembly> assemblies, IdentifierGenerator<Assembly> identifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("identifierGenerator", identifierGenerator);

      _assemblies = assemblies;
      _identifierGenerator = identifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");

      foreach (var assembly in _assemblies)
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