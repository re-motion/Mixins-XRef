using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class InterfaceIntroductionReportGenerator : IReportGenerator
  {
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _interfaceIntroductionDefinitions;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceIntroductionReportGenerator (
        UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> interfaceIntroductionDefinitions,
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
          select GenerateInterfaceReferanceElement (introducedInterface.InterfaceType));
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