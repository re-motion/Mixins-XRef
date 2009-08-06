using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AssemblyReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public AssemblyReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    public XElement GenerateXml ()
    {
      var assembliesElement = new XElement ("Assemblies");
      var allInvolvedTypes = _context.InvolvedTypes;

      foreach (var assembly in _context.Assemblies)
      {
        InvolvedType[] involvedTypesForAssembly = Array.FindAll (allInvolvedTypes, involvedType => involvedType.Type.Assembly == assembly);
        assembliesElement.Add (GenerateAssemblyElement (assembly, involvedTypesForAssembly));
      }
      return assembliesElement;
    }

    private XElement GenerateAssemblyElement (Assembly assembly, InvolvedType[] involvedTypesForAssembly)
    {
      return new XElement (
          "Assembly",
          new XAttribute ("id", _context.AssemblyIdentifierGenerator.GetIdentifier (assembly)),
          new XAttribute ("full-name", assembly.FullName),
          new XAttribute ("code-base", assembly.CodeBase),
          from involvedType in involvedTypesForAssembly
          select
              new XElement (
              "InvolvedType",
              new XAttribute ("ref", _context.InvolvedTypeIdentifierGenerator.GetIdentifier (involvedType.Type))
              )
          );
    }
  }
}