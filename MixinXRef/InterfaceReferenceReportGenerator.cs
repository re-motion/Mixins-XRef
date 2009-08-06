using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InterfaceReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceReferenceReportGenerator (Type type, IdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _type = type;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "Interfaces",
          from implementedInterface in _type.GetInterfaces()
          where implementedInterface.Assembly != typeof (IInitializableMixin).Assembly
          select GenerateInterfaceReference (implementedInterface)
          );
    }

    private XElement GenerateInterfaceReference (Type implementedInterface)
    {
      return new XElement ("Interface", new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (implementedInterface)));
    }
  }
}