using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeReportGenerator (
        InvolvedType[] involvedTypes,
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement (
          "Attributes",
          from attribute in allAttributes.Keys
          where attribute.Assembly != typeof (IInitializableMixin).Assembly
          select GenerateAttributeElement (attribute, allAttributes));
    }

    private MultiDictionary<Type, Type> GetAllAttributes ()
    {
      var allAttributes = new MultiDictionary<Type, Type>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var attribute in involvedType.Type.GetCustomAttributes (true))
          allAttributes.Add (attribute.GetType(), involvedType.Type);
      }
      return allAttributes;
    }

    private XElement GenerateAttributeElement (Type attribute, MultiDictionary<Type, Type> allAttributes)
    {
      return new XElement (
          "Attribute",
          new XAttribute ("id", _attributeIdentifierGenerator.GetIdentifier (attribute)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (attribute.Assembly)),
          new XAttribute ("namespace", attribute.Namespace),
          new XAttribute ("name", attribute.Name),
          new XElement (
              "AppliedTo",
              from appliedToType in allAttributes[attribute]
              select
                  new XElement (
                  "InvolvedType",
                  new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (appliedToType)))
              )
          );
    }
  }
}