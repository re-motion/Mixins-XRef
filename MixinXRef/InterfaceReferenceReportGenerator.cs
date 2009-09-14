using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class InterfaceReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public InterfaceReferenceReportGenerator (
        InvolvedType involvedType, IIdentifierGenerator<Type> interfaceIdentifierGenerator, IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _involvedType = involvedType;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "Interfaces",
          from implementedInterface in GetAllInterfaces()
          where !_remotionReflection.IsInfrastructureType (implementedInterface)
          select GenerateInterfaceReference (implementedInterface)
          );
    }

    private XElement GenerateInterfaceReference (Type implementedInterface)
    {
      return new XElement ("Interface", new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (implementedInterface)));
    }

    private HashSet<Type> GetAllInterfaces ()
    {
      var allInterfaces = new HashSet<Type>();

      foreach (var iface in _involvedType.Type.GetInterfaces())
        allInterfaces.Add (iface);

      if (_involvedType.IsTarget)
      {
        foreach (var completeInterface in _involvedType.ClassContext.GetProperty ("CompleteInterfaces"))
          allInterfaces.Add (completeInterface.To<Type>());
      }

      return allInterfaces;
    }
  }
}