using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class InterfaceIntroductionReportGenerator : IReportGenerator
  {
    // UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition>
    private readonly ReflectedObject _interfaceIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceIntroductionReportGenerator (
        ReflectedObject interfaceIntroductionDefinitions,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("interfaceIntroductionDefinitions", interfaceIntroductionDefinitions);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _interfaceIntroductionDefinitions = interfaceIntroductionDefinitions;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "InterfaceIntroductions",
          from introducedInterface in _interfaceIntroductionDefinitions
          select GenerateInterfaceReferanceElement (introducedInterface.GetProperty("InterfaceType").To<Type>()));
    }

    private XElement GenerateInterfaceReferanceElement (Type introducedInterface)
    {
      return new XElement (
          "Interface",
          new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (introducedInterface))
          );
    }
  }
}