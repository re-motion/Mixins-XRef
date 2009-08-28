using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Mixins;

namespace MixinXRef
{
  public class InterfaceReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public InterfaceReferenceReportGenerator (Type type, IIdentifierGenerator<Type> interfaceIdentifierGenerator, IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _type = type;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "Interfaces",
          from implementedInterface in _type.GetInterfaces()
          where !_remotionReflection.IsInfrastructureType(implementedInterface)
          select GenerateInterfaceReference (implementedInterface)
          );
    }

    private XElement GenerateInterfaceReference (Type implementedInterface)
    {
      return new XElement ("Interface", new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (implementedInterface)));
    }
  }
}