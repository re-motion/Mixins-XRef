using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceReportGenerator (
        InvolvedType[] involvedTypes,
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }


    public XElement GenerateXml ()
    {
      var allInterfaces = GetAllInterfaces();

      return new XElement (
          "Interfaces",
          from usedInterface in allInterfaces
          where usedInterface.Assembly != typeof (IInitializableMixin).Assembly
          select GenerateInterfaceElement (usedInterface)
          );
    }

    private HashSet<Type> GetAllInterfaces ()
    {
      var allInterfaces = new HashSet<Type>();

      foreach (var involvedType in _involvedTypes)
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
          new XAttribute ("id", _interfaceIdentifierGenerator.GetIdentifier (usedInterface)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (usedInterface.Assembly)),
          new XAttribute ("namespace", usedInterface.Namespace),
          new XAttribute ("name", usedInterface.Name),
          new MemberReportGenerator (usedInterface).GenerateXml(),
          new XElement (
              "ImplementedBy",
              new XElement (
                  "InvolvedType",
                  new XAttribute ("ref", "0"))
              )
          );
    }
  }
}