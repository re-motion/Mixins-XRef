using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Collections;
using Remotion.Mixins;

namespace MixinXRef
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private readonly IRemotionReflection _remotionReflection;

    public AttributeReportGenerator (
        InvolvedType[] involvedTypes,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
      IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _involvedTypes = involvedTypes;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
      _remotionReflection = remotionReflection;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement (
          "Attributes",
          from attribute in allAttributes.Keys
          select GenerateAttributeElement (attribute, allAttributes));
    }

    private MultiDictionary<Type, Type> GetAllAttributes ()
    {
      var allAttributes = new MultiDictionary<Type, Type>();

      foreach (var involvedType in _involvedTypes)
      {
        foreach (var attribute in involvedType.Type.GetCustomAttributes (false))
        {
          if ((!_remotionReflection.IsInfrastructureType(attribute.GetType()))
              && !(allAttributes[attribute.GetType()].Contains (involvedType.Type)))
            allAttributes.Add (attribute.GetType(), involvedType.Type);
        }
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