using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class AttributeReferenceReportGenerator : IReportGenerator
  {
    private readonly Type _type;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public AttributeReferenceReportGenerator (
        Type type, IIdentifierGenerator<Type> attributeIdentifierGenerator, IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _type = type;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "Attributes",
          from attribute in CustomAttributeData.GetCustomAttributes (_type)
          where !_remotionReflection.IsInfrastructureType (attribute.Constructor.DeclaringType)
          select GenerateAttributeReference (attribute)
          );
    }

    private XElement GenerateAttributeReference (CustomAttributeData attribute)
    {
      var attributeElement = new XElement (
          "Attribute", new XAttribute ("ref", _attributeIdentifierGenerator.GetIdentifier (attribute.Constructor.DeclaringType)));

      for (int i = 0; i < attribute.ConstructorArguments.Count; i++)
      {
        var constructorArgument = attribute.ConstructorArguments[i];
        var parameterName = attribute.Constructor.GetParameters()[i].Name;
        attributeElement.Add (GenerateParameterElement ("constructor", constructorArgument.ArgumentType, parameterName, constructorArgument.Value));
      }

      foreach (var namedArgument in attribute.NamedArguments)
      {
        attributeElement.Add (
            GenerateParameterElement ("named", namedArgument.TypedValue.ArgumentType, namedArgument.MemberInfo.Name, namedArgument.TypedValue.Value));
      }

      return attributeElement;
    }

    private XElement GenerateParameterElement (string kind, Type type, string name, object value)
    {
      var demultiplexedValue = RecursiveToString (type, value);

      return new XElement (
          "Argument",
          new XAttribute ("kind", kind),
          new XAttribute ("type", type.Name),
          new XAttribute ("name", name),
          new XAttribute ("value", demultiplexedValue));
    }

    private string RecursiveToString (Type argumentType, object argumentValue)
    {
      if (!argumentType.IsArray)
        return argumentValue.ToString();

      var valueCollection = (ReadOnlyCollection<CustomAttributeTypedArgument>) argumentValue;

      StringBuilder concatenatedValues = new StringBuilder ("{");
      for (int i = 0; i < valueCollection.Count; i++)
      {
        if (i != 0)
          concatenatedValues.Append (", ");
        concatenatedValues.Append (RecursiveToString (valueCollection[i].ArgumentType, valueCollection[i].Value));
      }
      concatenatedValues.Append ("}");

      return concatenatedValues.ToString();
    }
  }
}