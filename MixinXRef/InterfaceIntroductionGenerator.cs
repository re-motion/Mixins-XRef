using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InterfaceIntroductionGenerator : IReportGenerator
  {
    private readonly Type _targetType;
    private readonly Type _mixinType;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IdentifierGenerator<Type> _interfaceIdentifierGenerator;

    public InterfaceIntroductionGenerator (Type targetType, Type mixinType, MixinConfiguration mixinConfiguration, IdentifierGenerator<Type> interfaceIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);

      _targetType = targetType;
      _mixinType = mixinType;
      _mixinConfiguration = mixinConfiguration;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      if (_targetType.IsGenericTypeDefinition)
        return null;

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (_targetType, _mixinConfiguration);

      return new XElement ("InterfaceIntroductions",
        from introducedInterface in targetClassDefinition.Mixins[_mixinType].InterfaceIntroductions
        select GenerateInterfaceReferanceElement(introducedInterface.InterfaceType));
    }

    private XElement GenerateInterfaceReferanceElement (Type introducedInterface)
    {
      return new XElement (
          "Interface",
          new XAttribute ("ref", _interfaceIdentifierGenerator.GetIdentifier(introducedInterface))
          );
    }
  }
}