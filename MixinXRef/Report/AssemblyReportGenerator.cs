using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class AssemblyReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public AssemblyReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");

      var assemblies = _involvedTypes.GroupBy(i => i.Type.Assembly);
      foreach (var assembly in assemblies)
        assembliesElement.Add(GenerateAssemblyElement(assembly.Key, assembly));

      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, IEnumerable<InvolvedType> involvedTypesForAssembly)
    {
      return new XElement (
        "Assembly",
        new XAttribute ("id", _assemblyIdentifierGenerator.GetIdentifier (assembly)),
        new XAttribute ("name", assembly.GetName ().Name),
        new XAttribute ("version", assembly.GetName ().Version),
        new XAttribute ("location", GetShortAssemblyLocation (assembly)),
        new XAttribute ("culture", assembly.GetName ().CultureInfo),
        new XAttribute ("publicKeyToken", Convert.ToBase64String (assembly.GetName ().GetPublicKeyToken ())),
        from involvedType in involvedTypesForAssembly
        select
          new XElement (
          "InvolvedType-Reference",
          new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (involvedType.Type))
          )
        );
    }

    public string GetShortAssemblyLocation (Assembly assembly)
    {
      return assembly.GlobalAssemblyCache ? assembly.Location : "./" + Path.GetFileName (assembly.Location);
    }
  }
}