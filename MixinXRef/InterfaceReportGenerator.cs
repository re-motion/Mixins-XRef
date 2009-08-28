using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Collections;

namespace MixinXRef
{
  public class InterfaceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public InterfaceReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }


    public XElement GenerateXml ()
    {
      var allInterfaces = GetAllInterfaces();

      return new XElement (
          "Interfaces",
          from usedInterface in allInterfaces.Keys
          where !_remotionReflection.IsInfrastructureType (usedInterface)
          select GenerateInterfaceElement (usedInterface, allInterfaces)
          );
    }

    private MultiDictionary<Type, Type> GetAllInterfaces ()
    {
      var allInterfaces = new MultiDictionary<Type, Type>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var usedInterface in involvedType.Type.GetInterfaces())
          allInterfaces.Add (usedInterface, involvedType.Type);
      }

      return allInterfaces;
    }

    private XElement GenerateInterfaceElement (Type usedInterface, MultiDictionary<Type, Type> allInterfaces)
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
              from implementingType in allInterfaces[usedInterface]
              select
                  new XElement (
                  "InvolvedType",
                  new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (implementingType)))
              )
          );
    }
  }
}