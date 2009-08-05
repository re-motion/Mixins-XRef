using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public InterfaceReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public XElement GenerateXml ()
    {
      var allInterfaces = GetAllInterfaces();


      return new XElement (
          "Interfaces",
          from usedInterface in allInterfaces
          select
              GenerateInterfaceElement (usedInterface)
          );
    }

    private HashSet<Type> GetAllInterfaces ()
    {
      var allInterfaces = new HashSet<Type>();

      foreach (var involvedType in _context.InvolvedTypeFinder.FindInvolvedTypes())
      {
        foreach (var usedInterface in involvedType.Type.GetInterfaces())
          allInterfaces.Add (usedInterface);
      }

      return allInterfaces;
    }

    private XElement GenerateInterfaceElement (Type usedInterface)
    {
      return new XElement (
          "Interface",
          new XAttribute ("id", _context.InterfaceIdentifierGenerator.GetIdentifier (usedInterface)),
          new XAttribute ("assembly-ref", _context.AssemblyIdentifierGenerator.GetIdentifier (usedInterface.Assembly)),
          new XAttribute ("namespace", usedInterface.Namespace),
          new XAttribute ("name", usedInterface.Name),
          new MemberReportGenerator(usedInterface).GenerateXml()
          );
    }
  }
}