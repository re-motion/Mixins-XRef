using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class InterfaceReferenceReportGenerator : IReportGenerator
  {
    private readonly InvolvedType _involvedType;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflector _remotionReflector;

    public InterfaceReferenceReportGenerator (
        InvolvedType involvedType, IIdentifierGenerator<Type> interfaceIdentifierGenerator, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("involvedType", involvedType);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _involvedType = involvedType;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflector = remotionReflector;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "ImplementedInterfaces",
          from implementedInterface in GetAllInterfaces()
          where !_remotionReflector.IsInfrastructureType (implementedInterface)
          select GenerateInterfaceReference (implementedInterface)
          );
    }

    private XElement GenerateInterfaceReference (Type implementedInterface)
    {
      return new XElement ("ImplementedInterface", new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (implementedInterface)));
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