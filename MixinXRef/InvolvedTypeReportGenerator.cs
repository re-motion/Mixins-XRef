using System;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _involvedTypeIdentifierGenerator;

    public InvolvedTypeReportGenerator (
        InvolvedType[] involvedTypes,
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IdentifierGenerator<Type> involvedTypeIdentifierGenerator)
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
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var involvedType in _involvedTypes)
        involvedTypesElement.Add (CreateInvolvedTypeElement (involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (InvolvedType involvedType)
    {
      var realType = involvedType.Type;
      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _involvedTypeIdentifierGenerator.GetIdentifier (realType)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (realType.Assembly)),
          new XAttribute ("namespace", realType.Namespace),
          new XAttribute ("name", realType.Name),
          new XAttribute ("is-target", involvedType.IsTarget),
          new XAttribute ("is-mixin", involvedType.IsMixin));
    }
  }
}