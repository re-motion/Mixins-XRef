using System;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedTypeReportGenerator : IReportGenerator
  {
    private readonly InvolvedType[] _involvedTypes;
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private readonly IIdentifierGenerator<Type> _attributeIdentifierGenerator;

    public InvolvedTypeReportGenerator (
        InvolvedType[] involvedTypes,
        MixinConfiguration mixinConfiguration,
        IIdentifierGenerator<Assembly> assemblyIdentifierGenerator,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator)
    {
      ArgumentUtility.CheckNotNull ("involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblyIdentifierGenerator", assemblyIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("interfaceIdentifierGenerator", interfaceIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("attributeIdentifierGenerator", attributeIdentifierGenerator);

      _involvedTypes = involvedTypes;
      _mixinConfiguration = mixinConfiguration;
      _assemblyIdentifierGenerator = assemblyIdentifierGenerator;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _interfaceIdentifierGenerator = interfaceIdentifierGenerator;
      _attributeIdentifierGenerator = attributeIdentifierGenerator;
    }

    public XElement GenerateXml ()
    {
      var involvedTypesElement = new XElement ("InvolvedTypes");
      foreach (var involvedType in _involvedTypes)
        involvedTypesElement.Add (CreateInvolvedTypeElement (involvedType));

      return involvedTypesElement;
    }

    private XElement CreateInvolvedTypeElement (InvolvedType involvedType)
    {
      var realType = involvedType.Type;
      return new XElement (
          "InvolvedType",
          new XAttribute ("id", _involvedTypeIdentifierGenerator.GetIdentifier (realType)),
          new XAttribute ("assembly-ref", _assemblyIdentifierGenerator.GetIdentifier (realType.Assembly)),
          new XAttribute ("namespace", realType.Namespace),
          new XAttribute ("name", GetCSharpLikeName(realType)),
          new XAttribute ("base", GetFullNameForBaseType(realType)),
          new XAttribute ("is-target", involvedType.IsTarget),
          new XAttribute ("is-mixin", involvedType.IsMixin),
          new XAttribute ("is-generic-definition", involvedType.Type.IsGenericTypeDefinition),
          new MemberReportGenerator (involvedType.Type).GenerateXml(),
          new InterfaceReferenceReportGenerator (involvedType.Type, _interfaceIdentifierGenerator).GenerateXml (),
          new AttributeReferenceReportGenerator (involvedType.Type, _attributeIdentifierGenerator).GenerateXml (),
          new MixinReferenceReportGenerator (involvedType, _mixinConfiguration, _involvedTypeIdentifierGenerator, _interfaceIdentifierGenerator, _attributeIdentifierGenerator).GenerateXml(),
          new TargetReferenceReportGenerator (involvedType, _involvedTypeIdentifierGenerator).GenerateXml ()
          );
    }

    private string GetCSharpLikeName(Type type) {
      if (!type.ContainsGenericParameters)
        return type.Name;

      var typeName = type.Name.Substring (0, type.Name.IndexOf ('`'));

      StringBuilder result = new StringBuilder (typeName);
      result.Append ("<");
      foreach (Type genericArgument in type.GetGenericArguments())
      {
        result.Append (genericArgument.Name);
        result.Append(", ");
      }
      result.Remove (result.Length - 2, 2);
      result.Append (">");
      return result.ToString();
    }

    private string GetFullNameForBaseType (Type type)
    {
      // for System.Object
      if(type.BaseType == null)
        return "none";

      // for special generic types
      if(type.BaseType.IsGenericType)
        return type.BaseType.GetGenericTypeDefinition().FullName;

      // for standard types
      return type.BaseType.FullName;
    }
  }
}