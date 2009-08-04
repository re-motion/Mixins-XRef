using System;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class TargetClassReportGenerator
  {
    private readonly ITargetClassFinder _targetClassFinder;
    private readonly IdentifierGenerator<Type> _typeIdentifierGenerator;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;

    public TargetClassReportGenerator (
        ITargetClassFinder targetClassFinder, 
        IdentifierGenerator<Type> typeIdentifierGenerator, 
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("targetClassFinder", targetClassFinder);
      ArgumentUtility.CheckNotNull ("typeIdentifierGenerator", typeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);

      _targetClassFinder = targetClassFinder;
      _typeIdentifierGenerator = typeIdentifierGenerator;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var targetClass in _targetClassFinder.FindTargetClasses ())
        involvedTypesElement.Add (CreateInvolvedTypeElement (targetClass));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (Type targetClass)
    {
      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _typeIdentifierGenerator.GetIdentifier (targetClass)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (targetClass.Assembly)),
          new XAttribute ("namespace", targetClass.Namespace),
          new XAttribute ("name", targetClass.Name));
    }
  }
}