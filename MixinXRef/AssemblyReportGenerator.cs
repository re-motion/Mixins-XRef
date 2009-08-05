using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AssemblyReportGenerator : IReportGenerator
  {
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly IInvolvedTypeFinder _involvedTypeFinder;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public AssemblyReportGenerator (
      IEnumerable<Assembly> assemblies,
      IInvolvedTypeFinder involvedTypeFinder, 
      IdentifierGenerator<Assembly> assemblyIdentifierGenerator, 
      IdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _assemblies = assemblies;
      _involvedTypeFinder = involvedTypeFinder;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");
      var allInvolvedTypes = _involvedTypeFinder.FindInvolvedTypes();

      foreach (var assembly in _assemblies)
      {
        IInvolvedType[] involvedTypesForAssembly = Array.FindAll (allInvolvedTypes, involvedType => involvedType.Type.Assembly == assembly);
        assembliesElement.Add (GenerateAssemblyElement (assembly, involvedTypesForAssembly));
      }
      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, IInvolvedType[] involvedTypesForAssembly)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", _assemblyIdentifierGenerator.GetIdentifier (assembly)), 
          new XAttribute ("full-name", assembly.FullName), 
          new XAttribute ("code-base", assembly.CodeBase),
          from involvedType in involvedTypesForAssembly
          select 
              new XElement(
                  "InvolvedTypeRef",
                  new XAttribute("id", _involvedTypeIdentifierGenerator.GetIdentifier(involvedType.Type))
              )
          );
    }
  }
}