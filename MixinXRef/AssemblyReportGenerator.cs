using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MixinXRef
{
  public class AssemblyReportGenerator : IReportGenerator
  {
    private readonly Assembly[] _assemblies;
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public AssemblyReportGenerator (
        Assembly[] assemblies,
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _assemblies = assemblies;
      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");

      foreach (var assembly in _assemblies)
      {
        InvolvedType[] involvedTypesForAssembly = Array.FindAll (_involvedTypes, involvedType => involvedType.Type.Assembly == assembly);
        assembliesElement.Add (GenerateAssemblyElement (assembly, involvedTypesForAssembly));
      }
      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, InvolvedType[] involvedTypesForAssembly)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", _assemblyIdentifierGenerator.GetIdentifier (assembly)),
          new XAttribute ("name", assembly.GetName().Name),
          new XAttribute ("version", assembly.GetName().Version),
          new XAttribute ("location", assembly.Location),
          from involvedType in involvedTypesForAssembly
          select
              new XElement (
              "InvolvedType",
              new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (involvedType.Type))
              )
          );
    }
  }
}