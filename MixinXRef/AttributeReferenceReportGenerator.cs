using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeReferenceReportGenerator (Type type, IdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _type = type;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "Attributes",
          from attribute in CustomAttributeData.GetCustomAttributes (_type)
          where attribute.Constructor.DeclaringType.Assembly != typeof (IInitializableMixin).Assembly
          select GenerateAttributeReference (attribute.Constructor.DeclaringType)
          );
    }

    private XElement GenerateAttributeReference (Type attribute)
    {
      return new XElement ("Attribute", new XAttribute ("ref", _attributeIdentifierGenerator.GetIdentifier (attribute)));
    }
  }
}