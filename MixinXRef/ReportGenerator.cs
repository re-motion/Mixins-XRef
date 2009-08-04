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
      var identifierGenerator = new IdentifierGenerator<Assembly>();
      return new XElement ("MixinXRefReport", GenerateAssembliesElement (assemblies, identifierGenerator));
    }

    private XElement GenerateAssembliesElement (IEnumerable<Assembly> assemblies, IdentifierGenerator<Assembly> identifierGenerator)
    {
      var element = new XElement ("Assemblies");

      foreach (var assembly in assemblies)
        element.Add (GenerateAssemblyElement (assembly, identifierGenerator));

      return element;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, IdentifierGenerator<Assembly> identifierGenerator)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", identifierGenerator.GetIdentifier (assembly)), 
          new XAttribute ("fullName", assembly.FullName), 
          new XAttribute ("codeBase", assembly.CodeBase));
    }
  }
}