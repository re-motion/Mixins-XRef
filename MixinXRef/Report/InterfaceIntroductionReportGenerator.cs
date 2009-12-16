using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Report
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
          select GenerateInterfaceReferenceElement (introducedInterface.GetProperty ("InterfaceType").To<Type>()));
    }

    private XElement GenerateInterfaceReferenceElement (Type introducedInterface)
    {
      /*
      MixinDefinition ab;
      ab.InterfaceIntroductions[0].IntroducedMethods[0].Visibility;
      ab.InterfaceIntroductions[0].IntroducedMethods[0].Name;
      ab.InterfaceIntroductions[0].IntroducedProperties;
      ab.InterfaceIntroductions[0].IntroducedEvents
      */
      return new XElement (
          "Interface",
          new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier (introducedInterface))
          //, GenerateMemberIntroductions
          );
    }
  }
}