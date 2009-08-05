using System;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public InvolvedTypeReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      _context = context;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var involvedType in _context.InvolvedTypeFinder.FindInvolvedTypes ())
        involvedTypesElement.Add (CreateInvolvedTypeElement(involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (InvolvedType involvedType)
    {
      var realType = involvedType.Type;
      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _context.InvolvedTypeIdentifierGenerator.GetIdentifier (realType)),
          new XAttribute ("assembly-ref", _context.AssemblyIdentifierGenerator.GetIdentifier (realType.Assembly)),
          new XAttribute ("namespace", realType.Namespace),
          new XAttribute ("name", realType.Name),
          new XAttribute ("is-target", involvedType.IsTarget),
          new XAttribute ("is-mixin", involvedType.IsMixin));
    }
  }
}