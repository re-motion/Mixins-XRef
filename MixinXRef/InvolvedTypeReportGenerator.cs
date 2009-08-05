using System;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedTypeReportGenerator
  {
    private readonly IInvolvedTypeFinder _involvedTypeFinder;
    private readonly IdentifierGenerator<Type> _typeIdentifierGenerator;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

    public InvolvedTypeReportGenerator (
        IInvolvedTypeFinder involvedTypeFinder, 
        IdentifierGenerator<Type> typeIdentifierGenerator, 
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypeFinder", involvedTypeFinder);
      ArgumentUtility.CheckNotNull ("typeIdentifierGenerator", typeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);

      _involvedTypeFinder = involvedTypeFinder;
      _typeIdentifierGenerator = typeIdentifierGenerator;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var involvedType in _involvedTypeFinder.FindInvolvedTypes ())
        involvedTypesElement.Add (CreateInvolvedTypeElement(involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (Type involvedType)
    {
      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _typeIdentifierGenerator.GetIdentifier (involvedType)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (involvedType.Assembly)),
          new XAttribute ("namespace", involvedType.Namespace),
          new XAttribute ("name", involvedType.Name));
    }
  }
}