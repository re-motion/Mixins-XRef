using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeReportGenerator : IReportGenerator
  {
    
    private readonly InvolvedType[] _involvedTypes;
    private readonly IdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IdentifierGenerator<Type> _attributeIdentifierGenerator;

    public AttributeReportGenerator (
        InvolvedType[] involvedTypes,
        IdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement (
          "Attributes",
          from attribute in allAttributes
          where attribute.Assembly != typeof(IInitializableMixin).Assembly
          select GenerateAttributeElement (attribute));
    }

    private HashSet<Type> GetAllAttributes ()
    {
      var allAttributes = new HashSet<Type>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var attribute in involvedType.Type.GetCustomAttributes (true))
          allAttributes.Add (attribute.GetType());
      }
      return allAttributes;
    }

    private XElement GenerateAttributeElement (Type attribute)
    {
      return new XElement (
          "Attribute",
          new XAttribute ("id", _attributeIdentifierGenerator.GetIdentifier (attribute)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (attribute.Assembly)),
          new XAttribute ("namespace", attribute.Namespace),
          new XAttribute ("name", attribute.Name)
          );
    }
  }
}